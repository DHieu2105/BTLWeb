using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace BTL_Web.Models;

public partial class KetQua
{
    [Display(Name = "Mã học viên")]
    public string MaHocVien { get; set; } = null!;

    [Display(Name = "Mã khóa học")]
    public string MaKhoaHoc { get; set; } = null!;

    [Display(Name = "Điểm Nghe")]
    [Range(0, 495, ErrorMessage = "Điểm Listening phải từ 0 đến 495")] // Quy tắc TOEIC
    public int? DiemListening { get; set; }

    [Display(Name = "Điểm Đọc")]
    [Range(0, 495, ErrorMessage = "Điểm Reading phải từ 0 đến 495")] // Quy tắc TOEIC
    public int? DiemReading { get; set; }

    [Display(Name = "Tổng điểm")]
    [Range(0, 990, ErrorMessage = "Tổng điểm không hợp lệ")]
    public int? DiemTong { get; set; }

    // Các phần quan hệ này Minh giữ nguyên nhé
    public virtual HocVien MaHocVienNavigation { get; set; } = null!;
    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}