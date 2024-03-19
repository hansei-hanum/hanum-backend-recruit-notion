
using Hanum.Recruit.Models;

namespace Hanum.Recruit.Contracts.Services;

/// <summary>
/// 부서 서비스
/// </summary>
public interface IDepartmentService {
    /// <summary>
    /// 부서 목록 조회
    /// </summary>
    public Task<IEnumerable<Department>> GetDepartmentsAsync();
    /// <summary>
    /// 부서 조회
    /// </summary>
    /// <param name="name">부서 이름</param>
    /// <returns>부서</returns>
    public Task<Department?> GetDepartmentByNameAsync(string name);
    /// <summary>
    /// 부서 조회
    /// </summary>
    /// <param name="id">부서 ID</param>
    /// <returns>부서</returns>
    public Task<Department?> GetDepartmentAsync(string id);
}