using BTL_Web.Models;
using BTL_Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
public class GiaoVienController : Controller
{
    private readonly TtanContext _context;

    public GiaoVienController(TtanContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var query = _context.GiaoViens
            .Include(g => g.MaKhoaHocNavigation)
            .Include(g => g.MaTrungTamNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(g =>
                g.MaGv.Contains(keyword) ||
                (g.Ten != null && g.Ten.Contains(keyword)) ||
                (g.Sdt != null && g.Sdt.Contains(keyword)));
        }

        ViewBag.Keyword = keyword;
        var data = await query.OrderBy(g => g.MaGv).ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var giaoVien = await _context.GiaoViens
            .Include(g => g.MaKhoaHocNavigation)
            .Include(g => g.MaTrungTamNavigation)
            .FirstOrDefaultAsync(g => g.MaGv == id);

        return giaoVien == null ? NotFound() : View(giaoVien);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new GiaoVienFormViewModel();
        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GiaoVienFormViewModel vm)
    {
        if (await _context.GiaoViens.AnyAsync(g => g.MaGv == vm.MaGv))
        {
            ModelState.AddModelError(nameof(vm.MaGv), "Mã giáo viên đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        var entity = new GiaoVien
        {
            MaGv = vm.MaGv.Trim(),
            Ten = vm.Ten?.Trim(),
            Sdt = vm.Sdt?.Trim(),
            ChuyenMon = vm.ChuyenMon?.Trim(),
            MaKhoaHoc = string.IsNullOrWhiteSpace(vm.MaKhoaHoc) ? null : vm.MaKhoaHoc,
            MaTrungTam = string.IsNullOrWhiteSpace(vm.MaTrungTam) ? null : vm.MaTrungTam,
            GioiTinh = vm.GioiTinh?.Trim()
        };

        _context.GiaoViens.Add(entity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm giáo viên thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var entity = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new GiaoVienFormViewModel
        {
            MaGv = entity.MaGv,
            Ten = entity.Ten,
            Sdt = entity.Sdt,
            ChuyenMon = entity.ChuyenMon,
            MaKhoaHoc = entity.MaKhoaHoc,
            MaTrungTam = entity.MaTrungTam,
            GioiTinh = entity.GioiTinh
        };

        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, GiaoVienFormViewModel vm)
    {
        if (id != vm.MaGv)
        {
            return NotFound();
        }

        var entity = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == id);
        if (entity == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        entity.Ten = vm.Ten?.Trim();
        entity.Sdt = vm.Sdt?.Trim();
        entity.ChuyenMon = vm.ChuyenMon?.Trim();
        entity.MaKhoaHoc = string.IsNullOrWhiteSpace(vm.MaKhoaHoc) ? null : vm.MaKhoaHoc;
        entity.MaTrungTam = string.IsNullOrWhiteSpace(vm.MaTrungTam) ? null : vm.MaTrungTam;
        entity.GioiTinh = vm.GioiTinh?.Trim();

        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật giáo viên thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var giaoVien = await _context.GiaoViens
            .Include(g => g.MaKhoaHocNavigation)
            .Include(g => g.MaTrungTamNavigation)
            .FirstOrDefaultAsync(g => g.MaGv == id);

        return giaoVien == null ? NotFound() : View(giaoVien);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var entity = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == id);
        if (entity == null)
        {
            return NotFound();
        }

        var hasRelatedLopHoc = await _context.LopHocs.AnyAsync(l => l.MaGv == id);
        var hasRelatedTaiKhoan = await _context.TaiKhoans.AnyAsync(t => t.MaGv == id);

        if (hasRelatedLopHoc || hasRelatedTaiKhoan)
        {
            TempData["Error"] = "Không thể xóa giáo viên vì còn dữ liệu liên quan (lớp học hoặc tài khoản).";
            return RedirectToAction(nameof(Index));
        }

        _context.GiaoViens.Remove(entity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa giáo viên thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(GiaoVienFormViewModel vm)
    {
        vm.KhoaHocOptions = await _context.KhoaHocs
            .OrderBy(k => k.MaKhoaHoc)
            .Select(k => new SelectListItem
            {
                Value = k.MaKhoaHoc,
                Text = $"{k.MaKhoaHoc} - {k.TenKhoaHoc}"
            })
            .ToListAsync();

        vm.TrungTamOptions = await _context.TrungTams
            .OrderBy(t => t.MaTrungTam)
            .Select(t => new SelectListItem
            {
                Value = t.MaTrungTam,
                Text = $"{t.MaTrungTam} - {t.TenTrungTam}"
            })
            .ToListAsync();
    }
}
