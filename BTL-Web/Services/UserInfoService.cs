using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Services;

public interface IUserInfoService
{
    Task<dynamic?> GetUserInfoAsync(string username, string role);
    Task<string?> GetUserFullNameAsync(string username, string role);
}

public class UserInfoService : IUserInfoService
{
    private readonly TtanContext _context;

    public UserInfoService(TtanContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy thông tin chi tiết của người dùng dựa vào role
    /// </summary>
    public async Task<dynamic?> GetUserInfoAsync(string username, string role)
    {
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        if (account == null)
            return null;

        dynamic? result = null;

        if (role == "Admin")
        {
            result = new { account.Username, account.Role, Type = "Admin" };
        }
        else if (role == "NhanVien")
        {
            if (!string.IsNullOrEmpty(account.MaNv))
            {
                result = await _context.NhanViens
                    .Where(x => x.MaNv == account.MaNv)
                    .Select(x => new
                    {
                        account.Username,
                        x.MaNv,
                        x.HoVaTen,
                        x.ChucVu,
                        x.GioiTinh,
                        x.MaTrungTam,
                        account.Role,
                        Type = "NhanVien"
                    })
                    .FirstOrDefaultAsync();
            }
        }
        else if (role == "GiaoVien")
        {
            if (!string.IsNullOrEmpty(account.MaGv))
            {
                result = await _context.GiaoViens
                    .Where(x => x.MaGv == account.MaGv)
                    .Select(x => new
                    {
                        account.Username,
                        x.MaGv,
                        x.Ten,
                        x.Sdt,
                        x.ChuyenMon,
                        x.MaKhoaHoc,
                        x.MaTrungTam,
                        x.GioiTinh,
                        account.Role,
                        Type = "GiaoVien"
                    })
                    .FirstOrDefaultAsync();
            }
        }
        else if (role == "HocVien")
        {
            if (!string.IsNullOrEmpty(account.MaHocVien))
            {
                result = await _context.HocViens
                    .Where(x => x.MaHocVien == account.MaHocVien)
                    .Select(x => new
                    {
                        account.Username,
                        x.MaHocVien,
                        x.HoVaTen,
                        x.Sdt,
                        x.NgayDangKi,
                        x.GioiTinh,
                        x.MaNv,
                        account.Role,
                        Type = "HocVien"
                    })
                    .FirstOrDefaultAsync();
            }
        }

        // Nếu không tìm thấy dữ liệu chi tiết, trả về thông tin tài khoản cơ bản
        if (result == null && role != "Admin")
        {
            return new { account.Username, account.Role, Type = role };
        }

        return result;
    }

    /// <summary>
    /// Lấy tên đầy đủ của người dùng
    /// </summary>
    public async Task<string?> GetUserFullNameAsync(string username, string role)
    {
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        if (account == null)
            return null;

        return role switch
        {
            "Admin" => "Admin",

            "NhanVien" => string.IsNullOrEmpty(account.MaNv) ? null : await _context.NhanViens
                .Where(x => x.MaNv == account.MaNv)
                .Select(x => x.HoVaTen)
                .FirstOrDefaultAsync(),

            "GiaoVien" => string.IsNullOrEmpty(account.MaGv) ? null : await _context.GiaoViens
                .Where(x => x.MaGv == account.MaGv)
                .Select(x => x.Ten)
                .FirstOrDefaultAsync(),

            "HocVien" => string.IsNullOrEmpty(account.MaHocVien) ? null : await _context.HocViens
                .Where(x => x.MaHocVien == account.MaHocVien)
                .Select(x => x.HoVaTen)
                .FirstOrDefaultAsync(),

            _ => null
        };
    }
}
