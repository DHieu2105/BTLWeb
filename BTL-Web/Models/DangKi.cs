using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class DangKi
{
    public string MaKhoaHoc { get; set; } = null!;

    public string MaHocVien { get; set; } = null!;

    public DateOnly? NgayDangKi { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
