using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Services;

public interface IAuthService
{
    Task<TaiKhoan?> AuthenticateAsync(string username, string password);
    Task<TaiKhoan?> GetAccountByUsernameAsync(string username);
}

public class AuthService : IAuthService
{
    private readonly TtanContext _context;

    public AuthService(TtanContext context)
    {
        _context = context;
    }

    public async Task<TaiKhoan?> AuthenticateAsync(string username, string password)
    {
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        if (account == null)
            return null;

        // So sánh plaintext
        if (account.Password != password)
            return null;

        return account;
    }

    public async Task<TaiKhoan?> GetAccountByUsernameAsync(string username)
    {
        return await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
    }
}
