using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class ThietBi
{
    public string MaThietBi { get; set; } = null!;

    public int? SoLuong { get; set; }

    public string? TenThietBi { get; set; }

    public virtual ICollection<PhongHoc> MaPhongs { get; set; } = new List<PhongHoc>();
}
