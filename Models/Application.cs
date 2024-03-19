
namespace Hanum.Recruit.Models;

/// <summary>
/// 지원서
/// </summary>
public class Application {
    /// <summary>
    /// 지원서 ID
    /// </summary>
    public required string Id { get; set; }
    /// <summary>
    /// 사용자 ID
    /// </summary>
    public required ulong UserId { get; set; }
    /// <summary>
    /// 부서
    /// </summary>
    public required Department Department { get; set; }
    /// <summary>
    /// 자기소개
    /// </summary>
    public required string Introduction { get; set; }
    /// <summary>
    /// 지원동기
    /// </summary>
    public required string Motivation { get; set; }
    /// <summary>
    /// 포부
    /// </summary>
    public required string Aspiration { get; set; }
    /// <summary>
    /// 제출 여부
    /// </summary>
    public required bool IsSubmitted { get; set; }
    /// <summary>
    /// 작성날짜
    /// </summary>
    public required DateTime CreatedAt { get; set; }
    /// <summary>
    /// 수정날짜
    /// </summary>
    public required DateTime UpdatedAt { get; set; }
}