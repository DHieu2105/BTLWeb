namespace BTL_Web.Constants;

/// <summary>
/// Định nghĩa các vai trò trong hệ thống
/// </summary>
public static class AppRoles
{
    public const string Admin = "Admin";
    public const string GiaoVien = "GiaoVien";
    public const string HocVien = "HocVien";
    public const string NhanVien = "NhanVien";

    public static readonly List<string> AllRoles = new()
    {
        Admin,
        GiaoVien,
        HocVien,
        NhanVien
    };

    public static string GetRoleDisplayName(string role) => role switch
    {
        Admin => "Quản Trị Viên",
        GiaoVien => "Giáo Viên",
        HocVien => "Học Viên",
        NhanVien => "Nhân Viên",
        _ => "Người Dùng"
    };
}
