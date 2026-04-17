using BTL_Web.Models;
using BTL_Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
public class KhoaHocController : Controller
{
    private readonly TtanContext _context;

    public KhoaHocController(TtanContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var query = _context.KhoaHocs
            .Include(k => k.MaTrungTamNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(k =>
                k.MaKhoaHoc.Contains(keyword) ||
                (k.TenKhoaHoc != null && k.TenKhoaHoc.Contains(keyword)));
        }

        ViewBag.Keyword = keyword;
        var data = await query.OrderBy(k => k.MaKhoaHoc).ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var khoaHoc = await _context.KhoaHocs
            .Include(k => k.MaTrungTamNavigation)
            .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

        return khoaHoc == null ? NotFound() : View(khoaHoc);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new KhoaHocFormViewModel();
        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhoaHocFormViewModel vm)
    {
        if (await _context.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == vm.MaKhoaHoc))
        {
            ModelState.AddModelError(nameof(vm.MaKhoaHoc), "Mã khóa học đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        var entity = new KhoaHoc
        {
            MaKhoaHoc = vm.MaKhoaHoc.Trim(),
            TenKhoaHoc = vm.TenKhoaHoc?.Trim(),
            ThoiLuong = vm.ThoiLuong,
            HocPhi = vm.HocPhi,
            MaTrungTam = string.IsNullOrWhiteSpace(vm.MaTrungTam) ? null : vm.MaTrungTam
        };

        _context.KhoaHocs.Add(entity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm khóa học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var entity = await _context.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new KhoaHocFormViewModel
        {
            MaKhoaHoc = entity.MaKhoaHoc,
            TenKhoaHoc = entity.TenKhoaHoc,
            ThoiLuong = entity.ThoiLuong,
            HocPhi = entity.HocPhi,
            MaTrungTam = entity.MaTrungTam
        };

        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, KhoaHocFormViewModel vm)
    {
        if (id != vm.MaKhoaHoc)
        {
            return NotFound();
        }

        var entity = await _context.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        if (entity == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        entity.TenKhoaHoc = vm.TenKhoaHoc?.Trim();
        entity.ThoiLuong = vm.ThoiLuong;
        entity.HocPhi = vm.HocPhi;
        entity.MaTrungTam = string.IsNullOrWhiteSpace(vm.MaTrungTam) ? null : vm.MaTrungTam;

        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật khóa học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var khoaHoc = await _context.KhoaHocs
            .Include(k => k.MaTrungTamNavigation)
            .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

        return khoaHoc == null ? NotFound() : View(khoaHoc);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var entity = await _context.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        if (entity == null)
        {
            return NotFound();
        }

        var hasRelatedGiaoVien = await _context.GiaoViens.AnyAsync(g => g.MaKhoaHoc == id);
        var hasRelatedLopHoc = await _context.LopHocs.AnyAsync(l => l.MaKhoaHoc == id);

        if (hasRelatedGiaoVien || hasRelatedLopHoc)
        {
            TempData["Error"] = "Không thể xóa khóa học vì còn dữ liệu liên quan (giáo viên hoặc lớp học).";
            return RedirectToAction(nameof(Index));
        }

        _context.KhoaHocs.Remove(entity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa khóa học thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(KhoaHocFormViewModel vm)
    {
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
