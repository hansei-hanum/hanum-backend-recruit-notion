
using Hanum.Core.Authentication;
using Hanum.Core.Helpers;
using Hanum.Core.Models;
using Hanum.Core.Models.DTO.Responses;
using Hanum.Recruit.Contracts.Services;
using Hanum.Recruit.Models;
using Hanum.Recruit.Models.DTO.Requests;
using Hanum.Recruit.Models.DTO.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Hanum.Recruit.Controller;

[ApiController]
[Route("users/@me/applications")]
[HanumCommomAuthorize]
public class ApplicationController(IApplicationService applicationService) : ControllerBase {
    /// <summary>
    /// 내 지원서 목록 조회
    /// </summary>
    /// <returns>부서 목록</returns>
    [HttpGet]
    public async Task<APIResponse<ApplicationsDetail>> GetMyApplicationsAsync() {
        var items = await applicationService.GetApplicationsAsync(this.GetHanumUserClaim());
        return APIResponse<ApplicationsDetail>.FromData(new() {
            Items = items,
            Total = items.Count(),
            Limit = 0,
            IsSubmitted = items.Any(x => x.IsSubmitted)
        });
    }

    /// <summary>
    /// 지원서 조회
    /// </summary>
    /// <param name="applicationId">지원서 ID</param>
    /// <returns>지원서</returns>
    [HttpGet("{applicationId}")]
    public async Task<APIResponse<Application>> GetApplicationAsync(string applicationId) {
        var application = await applicationService.GetApplicationAsync(applicationId, this.GetHanumUserClaim());

        if (application == null)
            return APIResponse<Application>.FromError(HanumStatusCode.ApplicationNotFound);

        return APIResponse<Application>.FromData(application);
    }

    /// <summary>
    /// 지원서 작성
    /// </summary>
    /// <param name="request">지원서 작성 요청</param>
    /// <param name="isSubmit">최종 제출 여부</param>
    /// <returns>지원서 ID</returns>
    [HttpPost]
    public async Task<APIResponse<string>> WriteApplicationAsync(WriteApplicationRequest request, [FromQuery] bool isSubmit = false) {
        try {
            return APIResponse<string>.FromData(await applicationService.WriteAsync(this.GetHanumUserClaim(), request, isSubmit: isSubmit));
        } catch (ArgumentException ex) when (ex.ParamName == "userId") {
            return APIResponse<string>.FromError(HanumStatusCode.UserNotFound);
        } catch (ArgumentException ex) when (ex.ParamName == "request") {
            return APIResponse<string>.FromError(HanumStatusCode.DepartmentNotFound);
        } catch (ArgumentException ex) when (ex.ParamName == "isSubmit") {
            return APIResponse<string>.FromError(HanumStatusCode.ApplicationAlreadySubmitted);
        } catch (ArgumentException ex) when (ex.ParamName == "applicationId") {
            return APIResponse<string>.FromError(HanumStatusCode.ApplicationNotFound);
        } catch (InvalidOperationException) {
            return APIResponse<string>.FromError(HanumStatusCode.ApplicationAlreadySubmitted);
        }
    }

    /// <summary>
    /// 지원서 수정
    /// </summary>
    /// <param name="applicationId">지원서 ID</param>
    /// <param name="request">지원서 수정 요청</param>
    /// <param name="isSubmit">최종 제출 여부</param>
    /// <returns>지원서 ID</returns>
    [HttpPatch("{applicationId}")]
    public async Task<APIResponse<string>> UpdateApplicationAsync(string applicationId, WriteApplicationRequest request, [FromQuery] bool isSubmit = false) {
        try {
            return APIResponse<string>.FromData(await applicationService.WriteAsync(this.GetHanumUserClaim(), request, applicationId: applicationId, isSubmit: isSubmit));
        } catch (ArgumentException ex) when (ex.ParamName == "userId") {
            return APIResponse<string>.FromError(HanumStatusCode.UserNotFound);
        } catch (ArgumentException ex) when (ex.ParamName == "application") {
            return APIResponse<string>.FromError(HanumStatusCode.DepartmentNotFound);
        } catch (ArgumentException ex) when (ex.ParamName == "isSubmit") {
            return APIResponse<string>.FromError(HanumStatusCode.ApplicationAlreadySubmitted);
        } catch (ArgumentException ex) when (ex.ParamName == "applicationId") {
            return APIResponse<string>.FromError(HanumStatusCode.ApplicationNotFound);
        } catch (InvalidOperationException) {
            return APIResponse<string>.FromError(HanumStatusCode.ApplicationAlreadySubmitted);
        }
    }
}