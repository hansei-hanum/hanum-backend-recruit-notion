
using System.ComponentModel.DataAnnotations;

namespace Hanum.Recruit.Models.DTO.Requests;

public class WriteApplicationRequest {
    /// <summary>
    /// 부서 ID
    /// </summary>
    public required string DepartmentId { get; set; }
    /// <summary>
    /// 자기소개
    /// </summary>
    [Required(ErrorMessage = "자기소개를 입력해주세요.")]
    [StringLength(
        maximumLength: 500,
        MinimumLength = 10,
        ErrorMessage = "자기소개는 최소 10자 이상 500자 이하여야 합니다.")]
    public required string Introduction { get; set; }

    /// <summary>
    /// 지원동기
    /// </summary>
    [Required(ErrorMessage = "지원동기를 입력해주세요.")]
    [StringLength(
        maximumLength: 500,
        MinimumLength = 10,
        ErrorMessage = "지원동기는 최소 10자 이상 500자 이하여야 합니다.")]
    public required string Motivation { get; set; }

    /// <summary>
    /// 포부
    /// </summary>
    [Required(ErrorMessage = "포부를 입력해주세요.")]
    [StringLength(
        maximumLength: 500,
        MinimumLength = 10,
        ErrorMessage = "포부는 최소 10자 이상 500자 이하여야 합니다.")]
    public required string Aspiration { get; set; }
}