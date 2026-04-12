using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string? HoVaTen { get; set; }

    public string? ChucVu { get; set; }

    public string? MaTrungTam { get; set; }

    public string? GioiTinh { get; set; }

    public virtual ICollection<HocVien> HocViens { get; set; } = new List<HocVien>();

    public virtual TrungTam? MaTrungTamNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<GiaoVien> MaGvs { get; set; } = new List<GiaoVien>();
}
