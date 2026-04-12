using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Thêm dòng này

namespace BTL_Web.Models;

public partial class KetQua
{
    [Display(Name = "Mã Học Viên")]
    public string MaHocVien { get; set; } = null!;

    [Display(Name = "Mã Khóa Học")]
    public string MaKhoaHoc { get; set; } = null!;

    [Display(Name = "Điểm Listening")]
    // Remove restrictive 0-100 range because scores may use broader exam scales (e.g. TOEIC/IELTS totals)
    // Keep as nullable int to match existing database values
    public int? DiemListening { get; set; }

    [Display(Name = "Điểm Reading")]
    public int? DiemReading { get; set; }

    [Display(Name = "Tổng Điểm")]
    public int? DiemTong { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;
    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}