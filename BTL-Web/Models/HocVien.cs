using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class HocVien
{
    public string MaHocVien { get; set; } = null!;

    public string? HoVaTen { get; set; }

    public string? Sdt { get; set; }

    public DateOnly? NgayDangKi { get; set; }

    public string? MaNv { get; set; }

    public string? GioiTinh { get; set; }

    public virtual ICollection<DangKi> DangKis { get; set; } = new List<DangKi>();

    public virtual ICollection<KetQua> KetQuas { get; set; } = new List<KetQua>();

    public virtual NhanVien? MaNvNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<GiaoVien> MaGvs { get; set; } = new List<GiaoVien>();

    public virtual ICollection<LopHoc> MaLops { get; set; } = new List<LopHoc>();
}
