using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class TrungTam
{
    public string MaTrungTam { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? TenTrungTam { get; set; }

    public virtual ICollection<GiaoVien> GiaoViens { get; set; } = new List<GiaoVien>();

    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<PhongHoc> PhongHocs { get; set; } = new List<PhongHoc>();
}
