namespace BTL_Web.ViewModels;

public class TeacherCourseViewModel
{
    public string MaKhoaHoc { get; set; } = string.Empty;

    public string TenKhoaHoc { get; set; } = string.Empty;

    public int? ThoiLuong { get; set; }

    public int? HocPhi { get; set; }

    public int ClassCount { get; set; }

    public int StudentCount { get; set; }
}
