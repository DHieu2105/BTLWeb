using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BTL_Web.ViewModels;

public class GiaoVienFormViewModel
{
    [Required(ErrorMessage = "Mã giáo viên là bắt buộc")]
    [StringLength(10)]
    [Display(Name = "Mã giáo viên")]
    public string MaGv { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Tên giáo viên")]
    public string? Ten { get; set; }

    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? Sdt { get; set; }

    [StringLength(50)]
    [Display(Name = "Chuyên môn")]
    public string? ChuyenMon { get; set; }

    [StringLength(10)]
    [Display(Name = "Khóa học")]
    public string? MaKhoaHoc { get; set; }

    [StringLength(10)]
    [Display(Name = "Trung tâm")]
    public string? MaTrungTam { get; set; }

    [StringLength(10)]
    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    public IEnumerable<SelectListItem> KhoaHocOptions { get; set; } = [];

    public IEnumerable<SelectListItem> TrungTamOptions { get; set; } = [];
}
