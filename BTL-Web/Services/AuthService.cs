using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Services;

public interface IAuthService
{
    Task<TaiKhoan?> AuthenticateAsync(string username, string password);
    Task<TaiKhoan?> GetAccountByUsernameAsync(string username);
    Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
}

public class AuthService : IAuthService
{
    private readonly TtamContext _context;

    public AuthService(TtamContext context)
    {
        _context = context;
    }

    public async Task<TaiKhoan?> AuthenticateAsync(string username, string password)
    {
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        if (account == null)
            return null;

        if (!PasswordHasher.Verify(password, account.Password))
            return null;

        return account;
    }

    public async Task<TaiKhoan?> GetAccountByUsernameAsync(string username)
    {
        return await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
    {
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        if (account == null)
            return false;

        if (!PasswordHasher.Verify(oldPassword, account.Password))
            return false;

        account.Password = PasswordHasher.Hash(newPassword);
        _context.TaiKhoans.Update(account);
        await _context.SaveChangesAsync();

        return true;
    }
}
