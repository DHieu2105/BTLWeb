using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class KhoaHoc
{
    public string MaKhoaHoc { get; set; } = null!;

    public string? TenKhoaHoc { get; set; }

    public int? ThoiLuong { get; set; }

    public int? HocPhi { get; set; }

    public string? MaTrungTam { get; set; }

    public virtual ICollection<DangKi> DangKis { get; set; } = new List<DangKi>();

    public virtual ICollection<GiaoVien> GiaoViens { get; set; } = new List<GiaoVien>();

    public virtual ICollection<KetQua> KetQuas { get; set; } = new List<KetQua>();

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public virtual TrungTam? MaTrungTamNavigation { get; set; }
}
