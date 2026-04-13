using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTL_Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace BTL_Web.Controllers
{
    // Cho phép Admin, GiaoVien và NhanVien có thể thêm/sửa điểm (nhất quán với các module khác)
    [Authorize(Roles = "Admin,GiaoVien,NhanVien")]
    public class KetQuasController : Controller
    {
        private readonly TtanContext _context;

        public KetQuasController(TtanContext context)
        {
            _context = context;
        }

        // GET: KetQuas
        public async Task<IActionResult> Index(string searchString, string filterKhoaHoc)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentKhoaHocFilter"] = filterKhoaHoc;

            var ttanContext = _context.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .AsQueryable();

            // Tìm kiếm theo tên hoặc mã học viên
            if (!string.IsNullOrEmpty(searchString))
            {
                ttanContext = ttanContext.Where(k => 
                    k.MaHocVien.Contains(searchString) || 
                    (k.MaHocVienNavigation != null && k.MaHocVienNavigation.HoVaTen != null && k.MaHocVienNavigation.HoVaTen.Contains(searchString)));
            }

            // Lọc theo khóa học
            if (!string.IsNullOrEmpty(filterKhoaHoc))
            {
                ttanContext = ttanContext.Where(k => k.MaKhoaHoc == filterKhoaHoc);
            }

            // Dropdown cho lọc khóa học
            ViewData["KhoaHocList"] = new SelectList(await _context.KhoaHocs.ToListAsync(), "MaKhoaHoc", "MaKhoaHoc");

            return View(await ttanContext.OrderBy(k => k.MaHocVien).ToListAsync());
        }

        // GET: KetQuas/Details/5
        public async Task<IActionResult> Details(string maHocVien, string maKhoaHoc)
        {
            if (string.IsNullOrEmpty(maHocVien) || string.IsNullOrEmpty(maKhoaHoc) || _context.KetQuas == null)
            {
                return NotFound();
            }


            var ketQua = await _context.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(m => m.MaHocVien == maHocVien && m.MaKhoaHoc == maKhoaHoc);
            if (ketQua == null)
            {
                return NotFound();
            }

            return View(ketQua);
        }

        // GET: KetQuas/Create
        public IActionResult Create()
        {
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien");
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AddKetQuaPartial", new KetQua());
            }
            return View();
        }

        // POST: KetQuas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHocVien,MaKhoaHoc,DiemListening,DiemReading")] KetQua ketQua)
        {
            // Validate score range (0-100)
            if (ketQua.DiemListening.HasValue && (ketQua.DiemListening < 0 || ketQua.DiemListening > 100))
            {
                ModelState.AddModelError("DiemListening", "Listening score must be between 0 and 100");
            }
            if (ketQua.DiemReading.HasValue && (ketQua.DiemReading < 0 || ketQua.DiemReading > 100))
            {
                ModelState.AddModelError("DiemReading", "Reading score must be between 0 and 100");
            }

            if (ModelState.IsValid)
            {
                // KHÔNG tính DiemTong ở đây - để DB tính toán thông qua computed column
                _context.Add(ketQua);
                await _context.SaveChangesAsync();
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", ketQua.MaHocVien);
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc", ketQua.MaKhoaHoc);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AddKetQuaPartial", ketQua);
            }
            return View(ketQua);
        }

        // GET: KetQuas/Edit/5
        public async Task<IActionResult> Edit(string maHocVien, string maKhoaHoc)
        {
            if (string.IsNullOrEmpty(maHocVien) || string.IsNullOrEmpty(maKhoaHoc) || _context.KetQuas == null)
            {
                return NotFound();
            }


            // Tìm bản ghi bằng cả 2 khóa
            var ketQua = await _context.KetQuas
                .FirstOrDefaultAsync(m => m.MaHocVien == maHocVien && m.MaKhoaHoc == maKhoaHoc);
            if (ketQua == null)
            {
                return NotFound();
            }
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", ketQua.MaHocVien);
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc", ketQua.MaKhoaHoc);
            return View(ketQua);
        }

        // POST: KetQuas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string maHocVien, string maKhoaHoc, [Bind("MaHocVien,MaKhoaHoc,DiemListening,DiemReading")] KetQua ketQua)
        {
            // Kiểm tra xem khóa trong form có khớp với khóa trên URL không
            if (maHocVien != ketQua.MaHocVien || maKhoaHoc != ketQua.MaKhoaHoc)
            {
                return NotFound();
            }

            // Validate score range (0-100)
            if (ketQua.DiemListening.HasValue && (ketQua.DiemListening < 0 || ketQua.DiemListening > 100))
            {
                ModelState.AddModelError("DiemListening", "Listening score must be between 0 and 100");
            }
            if (ketQua.DiemReading.HasValue && (ketQua.DiemReading < 0 || ketQua.DiemReading > 100))
            {
                ModelState.AddModelError("DiemReading", "Reading score must be between 0 and 100");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem bản ghi có tồn tại không
                    var existingRecord = await _context.KetQuas
                        .AsNoTracking()
                        .FirstOrDefaultAsync(k => k.MaHocVien == maHocVien && k.MaKhoaHoc == maKhoaHoc);

                    if (existingRecord == null)
                    {
                        return NotFound();
                    }

                    // Tạo object mới để update (tránh tracking conflict)
                    var updatedRecord = new KetQua
                    {
                        MaHocVien = maHocVien,
                        MaKhoaHoc = maKhoaHoc,
                        DiemListening = ketQua.DiemListening,
                        DiemReading = ketQua.DiemReading
                        // DiemTong sẽ tính tự động từ DB computed column
                    };

                    _context.Update(updatedRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KetQuaExists(maHocVien, maKhoaHoc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            // Nếu ModelState không valid hoặc có error, reload form với data
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", ketQua.MaHocVien);
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc", ketQua.MaKhoaHoc);
            return View(ketQua);
        }

        // GET: KetQuas/Delete/5
        public async Task<IActionResult> Delete(string maHocVien, string maKhoaHoc)
        {
            if (string.IsNullOrEmpty(maHocVien) || string.IsNullOrEmpty(maKhoaHoc) || _context.KetQuas == null)
            {
                return NotFound();
            }

            var ketQua = await _context.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(m => m.MaHocVien == maHocVien && m.MaKhoaHoc == maKhoaHoc);
            if (ketQua == null)
            {
                return NotFound();
            }

            return View(ketQua);
        }

        // POST: KetQuas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string maHocVien, string maKhoaHoc)
        {
            var ketQua = await _context.KetQuas.FindAsync(maHocVien, maKhoaHoc);
            if (ketQua != null)
            {
                _context.KetQuas.Remove(ketQua);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KetQuaExists(string maHocVien, string maKhoaHoc)
        {
            return _context.KetQuas.Any(e => e.MaHocVien == maHocVien && e.MaKhoaHoc == maKhoaHoc);
        }
    }
}
