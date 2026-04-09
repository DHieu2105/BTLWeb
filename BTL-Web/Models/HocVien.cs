using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Thêm dòng này

namespace BTL_Web.Models;

public partial class HocVien
{
    [Required(ErrorMessage = "Vui lòng nhập mã học viên.")] // Thêm dòng này để hiện tiếng Việt
    [Display(Name = "Mã học viên")]
    public string MaHocVien { get; set; } = null!;
    [Required(ErrorMessage = "Vui lòng nhập họ và tên học viên.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ và tên")]
    public string? HoVaTen { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng (phải có 10 số, bắt đầu bằng 03, 05, 07, 08 hoặc 09).")]
    [Display(Name = "Số điện thoại")]
    public string? Sdt { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày đăng ký.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày đăng ký")]
    public DateOnly? NgayDangKi { get; set; }

    [Display(Name = "Mã nhân viên quản lý")]
    public string? MaNv { get; set; }

    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    // Các thuộc tính quan hệ (Navigation Properties) - Giữ nguyên
    public virtual ICollection<DangKi> DangKis { get; set; } = new List<DangKi>();
    public virtual ICollection<KetQua> KetQuas { get; set; } = new List<KetQua>();
    public virtual NhanVien? MaNvNavigation { get; set; }
    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    public virtual ICollection<GiaoVien> MaGvs { get; set; } = new List<GiaoVien>();
    public virtual ICollection<LopHoc> MaLops { get; set; } = new List<LopHoc>();
}