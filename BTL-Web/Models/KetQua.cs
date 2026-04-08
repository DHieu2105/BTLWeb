using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class KetQua
{
    public string MaHocVien { get; set; } = null!;

    public string MaKhoaHoc { get; set; } = null!;

    public int? DiemListening { get; set; }

    public int? DiemReading { get; set; }

    public int? DiemTong { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
