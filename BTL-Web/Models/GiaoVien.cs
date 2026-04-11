using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class GiaoVien
{
    public string MaGv { get; set; } = null!;

    public string? Ten { get; set; }

    public string? Sdt { get; set; }

    public string? ChuyenMon { get; set; }

    public string? MaKhoaHoc { get; set; }

    public string? MaTrungTam { get; set; }

    public string? GioiTinh { get; set; }

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }

    public virtual TrungTam? MaTrungTamNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<HocVien> MaHocViens { get; set; } = new List<HocVien>();

    public virtual ICollection<NhanVien> MaNvs { get; set; } = new List<NhanVien>();
}
