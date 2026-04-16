using BTL_Web.Constants;
using BTL_Web.Models;
using BTL_Web.Services;
using BTL_Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers;

[Authorize(Roles = AppRoles.Admin + "," + AppRoles.NhanVien)]
public class NhanVienController : Controller
{
    private readonly TtamContext _context;

    public NhanVienController(TtamContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var nhanViens = await _context.NhanViens
            .Include(x => x.MaTrungTamNavigation)
            .Include(x => x.TaiKhoans)
            .ToListAsync();

        nhanViens = nhanViens
            .OrderBy(x => GetMaNvPrefix(x.MaNv))
            .ThenBy(x => GetMaNvNumber(x.MaNv))
            .ThenBy(x => x.MaNv)
            .ToList();

        return View(nhanViens);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var nhanVien = await _context.NhanViens
            .Include(x => x.MaTrungTamNavigation)
            .Include(x => x.TaiKhoans)
            .FirstOrDefaultAsync(x => x.MaNv == id);

        if (nhanVien == null)
        {
            return NotFound();
        }

        return View(nhanVien);
    }

    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create()
    {
        var model = new NhanVienFormViewModel();
        await LoadSelectListsAsync(model.MaTrungTam, model.SelectedUsername, null);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(NhanVienFormViewModel model)
    {
        model.NewUsername = string.IsNullOrWhiteSpace(model.NewUsername) ? null : model.NewUsername.Trim();
        model.NewPassword = string.IsNullOrWhiteSpace(model.NewPassword) ? null : model.NewPassword.Trim();

        var hasNewAccountInput = !string.IsNullOrWhiteSpace(model.NewUsername) || !string.IsNullOrWhiteSpace(model.NewPassword);

        if (hasNewAccountInput && !string.IsNullOrWhiteSpace(model.SelectedUsername))
        {
            ModelState.AddModelError(nameof(model.SelectedUsername), "Chỉ được chọn tài khoản có sẵn hoặc tạo tài khoản mới");
        }

        if (hasNewAccountInput)
        {
            if (string.IsNullOrWhiteSpace(model.NewUsername))
            {
                ModelState.AddModelError(nameof(model.NewUsername), "Vui lòng nhập tên đăng nhập mới");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Vui lòng nhập mật khẩu mới");
            }
            else if (model.NewPassword.Length < 8)
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Mật khẩu mới phải ít nhất 8 ký tự");
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync(model.MaTrungTam, model.SelectedUsername, null);
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.NewUsername))
        {
            var usernameExists = await _context.TaiKhoans.AnyAsync(x => x.Username == model.NewUsername);
            if (usernameExists)
            {
                ModelState.AddModelError(nameof(model.NewUsername), "Tên đăng nhập đã tồn tại");
                await LoadSelectListsAsync(model.MaTrungTam, model.SelectedUsername, null);
                return View(model);
            }
        }

        var existed = await _context.NhanViens.AnyAsync(x => x.MaNv == model.MaNv);
        if (existed)
        {
            ModelState.AddModelError(nameof(model.MaNv), "Mã nhân viên đã tồn tại");
            await LoadSelectListsAsync(model.MaTrungTam, model.SelectedUsername, null);
            return View(model);
        }

        var nhanVien = new NhanVien
        {
            MaNv = model.MaNv.Trim(),
            HoVaTen = model.HoVaTen.Trim(),
            ChucVu = string.IsNullOrWhiteSpace(model.ChucVu) ? null : model.ChucVu.Trim(),
            MaTrungTam = string.IsNullOrWhiteSpace(model.MaTrungTam) ? null : model.MaTrungTam,
            GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh
        };

        _context.NhanViens.Add(nhanVien);

        if (!string.IsNullOrWhiteSpace(model.NewUsername) && !string.IsNullOrWhiteSpace(model.NewPassword))
        {
            _context.TaiKhoans.Add(new TaiKhoan
            {
                Username = model.NewUsername,
                Password = PasswordHasher.Hash(model.NewPassword),
                Role = AppRoles.NhanVien,
                MaNv = nhanVien.MaNv
            });
        }
        else
        {
            await UpdateAccountAssignmentAsync(model.SelectedUsername, nhanVien.MaNv);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Thêm nhân viên thành công";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var nhanVien = await _context.NhanViens
            .Include(x => x.TaiKhoans)
            .FirstOrDefaultAsync(x => x.MaNv == id);

        if (nhanVien == null)
        {
            return NotFound();
        }

        var model = new NhanVienFormViewModel
        {
            MaNv = nhanVien.MaNv,
            HoVaTen = nhanVien.HoVaTen ?? string.Empty,
            ChucVu = nhanVien.ChucVu,
            MaTrungTam = nhanVien.MaTrungTam,
            GioiTinh = nhanVien.GioiTinh,
            SelectedUsername = nhanVien.TaiKhoans.FirstOrDefault()?.Username
        };

        await LoadSelectListsAsync(model.MaTrungTam, model.SelectedUsername, nhanVien.MaNv);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(string id, NhanVienFormViewModel model)
    {
        if (id != model.MaNv)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync(model.MaTrungTam, model.SelectedUsername, model.MaNv);
            return View(model);
        }

        var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(x => x.MaNv == id);
        if (nhanVien == null)
        {
            return NotFound();
        }

        nhanVien.HoVaTen = model.HoVaTen.Trim();
        nhanVien.ChucVu = string.IsNullOrWhiteSpace(model.ChucVu) ? null : model.ChucVu.Trim();
        nhanVien.MaTrungTam = string.IsNullOrWhiteSpace(model.MaTrungTam) ? null : model.MaTrungTam;
        nhanVien.GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh;

        await UpdateAccountAssignmentAsync(model.SelectedUsername, nhanVien.MaNv);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật nhân viên thành công";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var nhanVien = await _context.NhanViens
            .Include(x => x.MaTrungTamNavigation)
            .Include(x => x.TaiKhoans)
            .FirstOrDefaultAsync(x => x.MaNv == id);

        if (nhanVien == null)
        {
            return NotFound();
        }

        return View(nhanVien);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(x => x.MaNv == id);
        if (nhanVien == null)
        {
            return NotFound();
        }

        var accounts = await _context.TaiKhoans.Where(x => x.MaNv == id).ToListAsync();
        foreach (var account in accounts)
        {
            account.MaNv = null;
        }

        _context.NhanViens.Remove(nhanVien);

        try
        {
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa nhân viên thành công";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa nhân viên do dữ liệu đang được sử dụng";
            return RedirectToAction(nameof(Delete), new { id });
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectListsAsync(string? selectedTrungTam, string? selectedUsername, string? maNv)
    {
        var trungTamItems = await _context.TrungTams
            .OrderBy(x => x.MaTrungTam)
            .Select(x => new SelectListItem
            {
                Value = x.MaTrungTam,
                Text = $"{x.MaTrungTam} - {x.TenTrungTam}"
            })
            .ToListAsync();

        var accountItems = await _context.TaiKhoans
            .Where(x => x.MaNv == null || x.MaNv == maNv)
            .OrderBy(x => x.Username)
            .Select(x => new SelectListItem
            {
                Value = x.Username,
                Text = string.IsNullOrWhiteSpace(x.Role) ? x.Username : $"{x.Username} ({x.Role})"
            })
            .ToListAsync();

        ViewBag.TrungTamList = new SelectList(trungTamItems, "Value", "Text", selectedTrungTam);
        ViewBag.AccountList = new SelectList(accountItems, "Value", "Text", selectedUsername);
    }

    private async Task UpdateAccountAssignmentAsync(string? username, string maNv)
    {
        var oldAccounts = await _context.TaiKhoans.Where(x => x.MaNv == maNv).ToListAsync();
        foreach (var oldAccount in oldAccounts)
        {
            oldAccount.MaNv = null;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        var account = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.Username == username);
        if (account == null)
        {
            return;
        }

        account.MaNv = maNv;
        if (string.IsNullOrWhiteSpace(account.Role))
        {
            account.Role = AppRoles.NhanVien;
        }
    }

    private static string GetMaNvPrefix(string? maNv)
    {
        if (string.IsNullOrWhiteSpace(maNv))
        {
            return string.Empty;
        }

        var index = 0;
        while (index < maNv.Length && !char.IsDigit(maNv[index]))
        {
            index++;
        }

        return maNv[..index];
    }

    private static int GetMaNvNumber(string? maNv)
    {
        if (string.IsNullOrWhiteSpace(maNv))
        {
            return int.MaxValue;
        }

        var index = 0;
        while (index < maNv.Length && !char.IsDigit(maNv[index]))
        {
            index++;
        }

        if (index >= maNv.Length)
        {
            return int.MaxValue;
        }

        var start = index;
        while (index < maNv.Length && char.IsDigit(maNv[index]))
        {
            index++;
        }

        return int.TryParse(maNv[start..index], out var number) ? number : int.MaxValue;
    }
}
