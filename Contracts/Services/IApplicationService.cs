
using Hanum.Recruit.Models;
using Hanum.Recruit.Models.DTO.Requests;

namespace Hanum.Recruit.Contracts.Services;

/// <summary>
/// 지원서 서비스
/// </summary>
public interface IApplicationService {
    /// <summary>
    /// 사용자가 지원서를 제출했는지 확인
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <returns>제출 여부</returns>
    public Task<bool> IsUserSubmittedAsync(ulong userId);
    /// <summary>
    /// 지원서 제출
    /// </summary>
    /// <param name="applicationId">지원서 ID</param>
    /// <returns>성공 여부</returns>
    public Task<bool> IsSubmittedAsync(string applicationId);
    /// <summary>
    /// 지원서 저장
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="application">지원서</param>
    /// <param name="applicationId">기존 지원서 ID</param>
    /// <param name="isSubmit">제출 여부</param>
    /// <returns>지원서 ID</returns>
    public Task<string> WriteAsync(ulong userId, WriteApplicationRequest application, string? applicationId = null, bool isSubmit = false);
    /// <summary>
    /// 내 지원서 조회
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <returns>지원서</returns>
    public Task<IEnumerable<Application>> GetApplicationsAsync(ulong userId);
    /// <summary>
    /// 지원서 조회
    /// </summary>
    /// <param name="applicationId">지원서 ID</param>
    /// <param name="userId">사용자 ID</param>
    /// <returns>지원서</returns>
    public Task<Application?> GetApplicationAsync(string applicationId, ulong? userId = null);
}