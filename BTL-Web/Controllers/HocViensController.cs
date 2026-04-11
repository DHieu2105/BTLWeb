using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTL_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList; // Thêm using cho PagedList
using X.PagedList.EF;

namespace BTL_Web.Controllers
{
    public class HocViensController : Controller
    {
        private readonly TtanContext _context;

        public HocViensController(TtanContext context)
        {
            _context = context;
        }

        // GET: HocViens
        // Thêm tham số searchString để nhận từ khóa từ người dùng
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            ViewData["CurrentFilter"] = searchString;

            var hocViens = from h in _context.HocViens select h;

            if (!String.IsNullOrEmpty(searchString))
            {
                hocViens = hocViens.Where(s => s.HoVaTen.Contains(searchString) || s.MaHocVien.Contains(searchString));
            }

            // 1. Thiết lập phân trang
            int pageSize = 10; // Mỗi trang hiện 5 học viên (có thể đổi thành 10)
            int pageNumber = (page ?? 1); // Nếu không có số trang thì mặc định là trang 1

            // 2. Bắt buộc phải sắp xếp (OrderBy) trước khi phân trang
            hocViens = hocViens.OrderBy(h => h.MaHocVien);

            // 3. Sử dụng ToPagedListAsync thay vì ToListAsync
            // Bỏ await và đổi ToPagedListAsync thành ToPagedList
            // Giữ await và dùng ToPagedListAsync
            return View(await hocViens.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: HocViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHocVien == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // GET: HocViens/Create
        public IActionResult Create()
        {
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            return View();
        }

        // POST: HocViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHocVien,HoVaTen,Sdt,NgayDangKi,MaNv,GioiTinh")] HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hocVien.MaNv);
            return View(hocVien);
        }

        // GET: HocViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hocVien.MaNv);
            return View(hocVien);
        }

        // POST: HocViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHocVien,HoVaTen,Sdt,NgayDangKi,MaNv,GioiTinh")] HocVien hocVien)
        {
            if (id != hocVien.MaHocVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocVienExists(hocVien.MaHocVien))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hocVien.MaNv);
            return View(hocVien);
        }

        // GET: HocViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHocVien == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // POST: HocViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien != null)
            {
                _context.HocViens.Remove(hocVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocVienExists(string id)
        {
            return _context.HocViens.Any(e => e.MaHocVien == id);
        }
    }
}


