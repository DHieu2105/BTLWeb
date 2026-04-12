using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class PhongHoc
{
    public string MaPhong { get; set; } = null!;

    public string? TenPhong { get; set; }

    public string? MaTrungTam { get; set; }

    public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public virtual TrungTam? MaTrungTamNavigation { get; set; }

    public virtual ICollection<ThietBi> MaThietBis { get; set; } = new List<ThietBi>();
}
