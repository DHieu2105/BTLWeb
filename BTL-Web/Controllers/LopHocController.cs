using BTL_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers;

public class LopHocController : Controller
{
    private readonly TtanContext _context;

    public LopHocController(TtanContext context)
    {
        _context = context;
    }
    
    //Get : Lophoc
    public async Task<IActionResult> Index()
    {
        var lopHocs = _context.LopHocs
            .Include(l => l.MaGvNavigation)
            .Include(l => l.MaKhoaHocNavigation)
            .Include(l => l.MaPhongNavigation);

        return View(await lopHocs.ToListAsync());
    }
    
    //GET: LopHoc/Details
    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var lopHoc = await _context.LopHocs
            .Include(l => l.MaGvNavigation)
            .Include(l => l.MaKhoaHocNavigation)
            .Include(l => l.MaPhongNavigation)
            .FirstOrDefaultAsync(m => m.MaLop == id);

        if (lopHoc == null)
        {
            return NotFound();
        }

        return View(lopHoc);
    }
    
    //GET : LopHoc/Create
    public IActionResult Create()
    {
        PopulateSelectLists();
        return View();
    }
    
    //Post: LopHoc/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaLop,MaGv,MaPhong,MaKhoaHoc,TenLop")] LopHoc lopHoc){
        if (ModelState.IsValid)
        {
            _context.Add(lopHoc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateSelectLists(lopHoc.MaGv, lopHoc.MaPhong, lopHoc.MaKhoaHoc);
        return View(lopHoc);
    }
        
    // GET: LopHoc/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var lopHoc = await _context.LopHocs.FindAsync(id);
        if (lopHoc == null)
        {
            return NotFound();
        }

        PopulateSelectLists(lopHoc.MaGv, lopHoc.MaPhong, lopHoc.MaKhoaHoc);
        return View(lopHoc);
    }

    // POST: LopHoc/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("MaLop,MaGv,MaPhong,MaKhoaHoc,TenLop")] LopHoc lopHoc)
    {
        if (id != lopHoc.MaLop)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(lopHoc);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LopHocExists(lopHoc.MaLop))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        PopulateSelectLists(lopHoc.MaGv, lopHoc.MaPhong, lopHoc.MaKhoaHoc);
        return View(lopHoc);
    }

    // GET: LopHoc/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var lopHoc = await _context.LopHocs
            .Include(l => l.MaGvNavigation)
            .Include(l => l.MaKhoaHocNavigation)
            .Include(l => l.MaPhongNavigation)
            .FirstOrDefaultAsync(m => m.MaLop == id);

        if (lopHoc == null)
        {
            return NotFound();
        }

        return View(lopHoc);
    }

    // POST: LopHoc/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var lopHoc = await _context.LopHocs.FindAsync(id);
        if (lopHoc != null)
        {
            _context.LopHocs.Remove(lopHoc);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool LopHocExists(string id)
    {
        return _context.LopHocs.Any(e => e.MaLop == id);
    }

    private void PopulateSelectLists(string? maGv = null, string? maPhong = null, string? maKhoaHoc = null)
    {
        ViewData["MaGv"] = new SelectList(_context.GiaoViens.AsNoTracking(), "MaGv", "Ten", maGv);
        ViewData["MaPhong"] = new SelectList(_context.PhongHocs.AsNoTracking(), "MaPhong", "MaPhong", maPhong);
        ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs.AsNoTracking(), "MaKhoaHoc", "TenKhoaHoc", maKhoaHoc);
    }
}