using System.ComponentModel.DataAnnotations;

namespace BTL_Web.ViewModels;

public class NhanVienFormViewModel
{
    [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
    [Display(Name = "Mã nhân viên")]
    public string MaNv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [Display(Name = "Họ và tên")]
    public string HoVaTen { get; set; } = string.Empty;

    [Display(Name = "Chức vụ")]
    public string? ChucVu { get; set; }

    [Display(Name = "Mã trung tâm")]
    public string? MaTrungTam { get; set; }

    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [Display(Name = "Tài khoản")]
    public string? SelectedUsername { get; set; }

    [Display(Name = "Tên đăng nhập mới")]
    public string? NewUsername { get; set; }

    [Display(Name = "Mật khẩu mới")]
    public string? NewPassword { get; set; }
}
