
using System.Net.Http.Headers;
using Hanum.Core.Authentication;
using Hanum.Core.Models.DTO.Responses;
using Hanum.Recruit.Contracts.Services;
using Hanum.Recruit.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hanum.Recruit.Controller;

[ApiController]
[Route("departments")]
[HanumCommomAuthorize]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase {
    /// <summary>
    /// 부서 목록 조회
    /// </summary>
    /// <returns>부서 목록</returns>
    [HttpGet]
    public async Task<APIResponse<APIPaginationData<Department>>> GetDepartmentsAsync() {
        var items = await departmentService.GetDepartmentsAsync();
        return APIResponse<APIPaginationData<Department>>.FromData(new() {
            Items = items,
            Total = items.Count(),
            Limit = 0
        });
    }
}