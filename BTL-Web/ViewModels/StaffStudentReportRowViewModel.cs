namespace BTL_Web.ViewModels;

public class StaffStudentReportRowViewModel
{
    public string MaHocVien { get; set; } = string.Empty;

    public string? HoVaTen { get; set; }

    public int SoKhoaHocDaDangKy { get; set; }

    public int TongHocPhi { get; set; }

    public DateOnly? NgayDangKyGanNhat { get; set; }
}
