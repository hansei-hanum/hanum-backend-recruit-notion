using Hanum.Core.Services;
using Hanum.Recruit.Contracts.Services;
using Hanum.Recruit.Helpers;
using Hanum.Recruit.Models;
using Hanum.Recruit.Models.DTO.Requests;
using Notion.Client;

namespace Hanum.Recruit.Services;

/// <summary>
/// 지원서 서비스
/// </summary>
public class ApplicationService(
    IConfiguration configuration,
    IDepartmentService departmentService,
    IHanumUserService userService,
    INotionClient notionClient) : IApplicationService {
    private readonly string databaseId = configuration.GetValue<string>("Recruit:Application:DatabaseId")!;

    public async Task<bool> IsUserSubmittedAsync(ulong userId) {
        var user = await userService.GetUserAsync(userId)
            ?? throw new ArgumentException("사용자를 찾을 수 없습니다.", nameof(userId));

        if (user.Verification?.IsStudent != true)
            throw new ArgumentException("학생 인증이 되지 않은 사용자는 지원서를 작성할 수 없습니다.", nameof(userId));

        return (await notionClient.Databases.QueryAsync(
            databaseId,
            new DatabasesQueryParameters {
                Filter = new NumberFilter(
                    "사용자ID",
                    equal: userId
                ),
            }
        )).Results.Any(p => p.Properties["제출여부"] is CheckboxPropertyValue checkbox && checkbox.Checkbox);
    }

    public async Task<bool> IsSubmittedAsync(string applicationId) {
        return (await notionClient.Pages.RetrieveAsync(applicationId)).Properties["제출여부"] is CheckboxPropertyValue checkbox && checkbox.Checkbox;
    }

    public async Task<string> WriteAsync(ulong userId, WriteApplicationRequest request, string? applicationId = null, bool isSubmit = false) {
        var user = await userService.GetUserAsync(userId)
            ?? throw new ArgumentException("사용자를 찾을 수 없습니다.", nameof(userId));

        if (user.Verification?.IsStudent != true)
            throw new ArgumentException("학생 인증이 되지 않은 사용자는 지원서를 작성할 수 없습니다.", nameof(userId));

        var department = await departmentService.GetDepartmentAsync(request.DepartmentId)
            ?? throw new ArgumentException("부서를 찾을 수 없습니다.", nameof(request));

        if (isSubmit && await IsUserSubmittedAsync(userId))
            throw new ArgumentException("이미 제출한 지원서가 있습니다.", nameof(isSubmit));

        var studentId = $"{user.Verification.GetStudentId()}{user.Name}";

        Dictionary<string, PropertyValue> properties = new() {
            ["지원서"] = new TitlePropertyValue {
                Title = [$"{studentId}의 {department.Name} 지원서".ToRichText()]
            },
            ["지원부서"] = new SelectPropertyValue {
                Select = new SelectOption {
                    Id = department.Id
                }
            },
            ["지원자"] = new RichTextPropertyValue {
                RichText = [studentId.ToRichText()]
            },
            ["전화번호"] = new PhoneNumberPropertyValue {
                PhoneNumber = user.PhoneNumber
            },
            ["제출여부"] = new CheckboxPropertyValue {
                Checkbox = isSubmit
            },
            ["자기소개"] = new RichTextPropertyValue {
                RichText = [request.Introduction.ToRichText()]
            },
            ["지원동기"] = new RichTextPropertyValue {
                RichText = [request.Motivation.ToRichText()]
            },
            ["포부"] = new RichTextPropertyValue {
                RichText = [request.Aspiration.ToRichText()]
            },
            ["사용자ID"] = new NumberPropertyValue {
                Number = userId
            }
        };

        IEnumerable<IBlock> CreateContext(string title, string content) {
            yield return new HeadingTwoBlock {
                Heading_2 = new HeadingTwoBlock.Info {
                    RichText = [title.ToRichText()]
                }
            };
            yield return content.ToParagraphBlock();
        }

        List<IBlock> children = [
            new CalloutBlock {
                Callout = new CalloutBlock.Info {
                    Icon = new EmojiObject {
                        Emoji = "📝"
                    },
                    Color = Color.GrayBackground,
                    RichText = [
                        (
                            $"지원부서: {department.Name}\n" +
                            $"지원자: {studentId}\n" +
                            $"전화번호: {user.PhoneNumber}\n" +
                            $"제출여부: {(isSubmit ? "제출" : "임시저장")}\n" +
                            $"작성날짜: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                        ).ToRichText()
                    ],
                }
            },
            ..CreateContext("자기소개", request.Introduction),
            ..CreateContext("지원동기", request.Motivation),
            ..CreateContext("포부", request.Aspiration),
        ];

        if (applicationId != null) {
            if (await IsSubmittedAsync(applicationId))
                throw new InvalidOperationException("이미 제출한 지원서는 수정할 수 없습니다.");

            // 기존 지원서 제거
            try {
                var page = await notionClient.Pages.UpdateAsync(
                    applicationId,
                    new PagesUpdateParameters {
                        Archived = true,
                    }
                );

                if (page.Properties["사용자ID"] is NumberPropertyValue number && number.Number != userId)
                    throw new ArgumentException("지원서를 찾을 수 없습니다.", nameof(applicationId));
            } catch (NotionApiException) {
                throw new ArgumentException("지원서를 찾을 수 없습니다.", nameof(applicationId));
            }
        }

        // 지원서 생성
        return (await notionClient.Pages.CreateAsync(
            new PagesCreateParameters {
                Parent = new DatabaseParentInput {
                    DatabaseId = databaseId
                },
                Properties = properties,
                Children = children
            }
        )).Id;
    }

    private static Application PageToApplication(Page page) =>
        new() {
            Id = page.Id,
            UserId = (ulong)((NumberPropertyValue)page.Properties["사용자ID"]).Number!,
            Department = new Department {
                Id = ((SelectPropertyValue)page.Properties["지원부서"]).Select.Id,
                Name = ((SelectPropertyValue)page.Properties["지원부서"]).Select.Name
            },
            Introduction = ((RichTextPropertyValue)page.Properties["자기소개"]).RichText.FirstOrDefault()?.PlainText ?? "",
            Motivation = ((RichTextPropertyValue)page.Properties["지원동기"]).RichText.FirstOrDefault()?.PlainText ?? "",
            Aspiration = ((RichTextPropertyValue)page.Properties["포부"]).RichText.FirstOrDefault()?.PlainText ?? "",
            IsSubmitted = page.Properties["제출여부"] is CheckboxPropertyValue checkbox && checkbox.Checkbox,
            CreatedAt = page.CreatedTime,
            UpdatedAt = page.LastEditedTime
        };

    public async Task<IEnumerable<Application>> GetApplicationsAsync(ulong userId) {
        var user = await userService.GetUserAsync(userId)
            ?? throw new ArgumentException("사용자를 찾을 수 없습니다.", nameof(userId));

        if (user.Verification?.IsStudent != true)
            throw new ArgumentException("학생 인증이 되지 않은 사용자는 지원서를 작성할 수 없습니다.", nameof(userId));

        return (await notionClient.Databases.QueryAsync(
                databaseId,
                new DatabasesQueryParameters {
                    Filter = new NumberFilter(
                        "사용자ID",
                        equal: userId
                    ),
                    Sorts = [
                        new() {
                            Property = "작성날짜",
                            Direction = Direction.Descending
                        }
                    ]
                }
            )).Results
            .Select(PageToApplication);
    }

    public async Task<Application?> GetApplicationAsync(string applicationId, ulong? userId = null) {
        try {
            var application = PageToApplication(await notionClient.Pages.RetrieveAsync(applicationId));

            if (userId != null && application.UserId != userId)
                return null;

            return application;
        } catch (Exception) {
            return null;
        }
    }
}