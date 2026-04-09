using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BTL_Web.ViewModels;

public class KhoaHocFormViewModel
{
    [Required(ErrorMessage = "Mã khóa học là bắt buộc")]
    [StringLength(10)]
    [Display(Name = "Mã khóa học")]
    public string MaKhoaHoc { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Tên khóa học")]
    public string? TenKhoaHoc { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thời lượng phải lớn hơn 0")]
    [Display(Name = "Thời lượng")]
    public int? ThoiLuong { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Học phí không hợp lệ")]
    [Display(Name = "Học phí")]
    public int? HocPhi { get; set; }

    [StringLength(10)]
    [Display(Name = "Trung tâm")]
    public string? MaTrungTam { get; set; }

    public IEnumerable<SelectListItem> TrungTamOptions { get; set; } = [];
}
