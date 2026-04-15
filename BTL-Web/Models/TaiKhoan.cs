using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class TaiKhoan
{
    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? MaNv { get; set; }

    public string? MaGv { get; set; }

    public string? MaHocVien { get; set; }

    public virtual GiaoVien? MaGvNavigation { get; set; }

    public virtual HocVien? MaHocVienNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
