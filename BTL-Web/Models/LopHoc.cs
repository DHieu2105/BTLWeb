using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class LopHoc
{
    public string MaLop { get; set; } = null!;

    public string? MaGv { get; set; }

    public string? MaPhong { get; set; }

    public string? MaKhoaHoc { get; set; }

    public string? TenLop { get; set; }

    public virtual GiaoVien? MaGvNavigation { get; set; }

    public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }

    public virtual PhongHoc? MaPhongNavigation { get; set; }

    public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

    public virtual ICollection<HocVien> MaHocViens { get; set; } = new List<HocVien>();
}
