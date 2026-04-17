using BTL_Web.Models;

namespace BTL_Web.ViewModels;

public class TeacherScoreManagementViewModel
{
    public string? SelectedClassId { get; set; }

    public List<LopHoc> Classes { get; set; } = new();

    public List<TeacherScoreRowViewModel> Rows { get; set; } = new();
}

public class TeacherScoreRowViewModel
{
    public string MaHocVien { get; set; } = string.Empty;

    public string? HoVaTen { get; set; }

    public string MaKhoaHoc { get; set; } = string.Empty;

    public string? TenKhoaHoc { get; set; }

    public int? DiemListening { get; set; }

    public int? DiemReading { get; set; }

    public int? DiemTong { get; set; }

    public bool HasScore { get; set; }
}
