using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class LichHoc
{
    public string MaLichHoc { get; set; } = null!;

    public DateOnly? NgayHoc { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public string? MaPhong { get; set; }

    public virtual PhongHoc? MaPhongNavigation { get; set; }
}
