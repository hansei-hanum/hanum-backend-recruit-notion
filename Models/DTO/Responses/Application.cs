
using Hanum.Core.Models.DTO.Responses;

namespace Hanum.Recruit.Models.DTO.Responses;

public class ApplicationsDetail : APIPaginationData<Application> {
    /// <summary>
    /// 제출 여부
    /// </summary>
    public required bool IsSubmitted { get; set; }
}
