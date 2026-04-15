using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BTL_Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly TtanContext _db;
        public AdminController(TtanContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _db.HocViens.CountAsync();
            ViewBag.TotalTeachers = await _db.GiaoViens.CountAsync();
            ViewBag.TotalClasses = await _db.LopHocs.CountAsync();
            ViewBag.TotalStaff = await _db.NhanViens.CountAsync();
            return View();
        }

        public async Task<IActionResult> HocVien()
            => View(await _db.HocViens.Include(h => h.DangKis).ThenInclude(d => d.MaKhoaHocNavigation).ToListAsync());

        public async Task<IActionResult> GiaoVien()
            => View(await _db.GiaoViens.Include(g => g.LopHocs).ToListAsync());

        public async Task<IActionResult> NhanVien()
            => View(await _db.NhanViens.Include(n => n.MaTrungTamNavigation).ToListAsync());

        public async Task<IActionResult> KhoaHoc()
            => View(await _db.KhoaHocs.Include(k => k.DangKis).Include(k => k.LopHocs).ToListAsync());

        public async Task<IActionResult> LopHoc(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            ViewBag.KhoaHocs = await _db.KhoaHocs.ToListAsync();
            ViewBag.GiaoViens = await _db.GiaoViens.ToListAsync();
            ViewBag.PhongHocs = await _db.PhongHocs.ToListAsync();

            var query = _db.LopHocs
            .Include(l => l.MaGvNavigation)
            .Include(l => l.MaKhoaHocNavigation)
            .Include(l => l.MaPhongNavigation)
            .Include(l => l.MaHocViens)
            .AsNoTracking();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var items = await query
            .OrderBy(l => l.MaLop)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var vm = new LopHocPage
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(vm);
        }

        public async Task<IActionResult> PhongHoc()
        {
            ViewBag.TrungTams = await _db.TrungTams.ToListAsync();
            return View(await _db.PhongHocs.Include(p => p.MaTrungTamNavigation).ToListAsync());
        }

        public async Task<IActionResult> ThietBi()
            => View(await _db.ThietBis.Include(t => t.MaPhongs).ToListAsync());

        public async Task<IActionResult> TrungTam()
            => View(await _db.TrungTams.ToListAsync());

        public async Task<IActionResult> LichHoc()
        {
            ViewBag.LopHocs = await _db.LopHocs.ToListAsync();
            ViewBag.PhongHocs = await _db.PhongHocs.ToListAsync();
            return View(await _db.LichHocs
                .Include(l => l.MaPhongNavigation)
                .ToListAsync());
        }

        public async Task<IActionResult> TaiKhoan()
            => View(await _db.TaiKhoans.ToListAsync());

        public async Task<IActionResult> KetQua()
        {
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            ViewBag.LopHocs = await _db.LopHocs.ToListAsync();
            return View(await _db.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .ToListAsync());
        }

        public async Task<IActionResult> StudentsInClass(string id)
        {
            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id);
            ViewBag.AllStudents = await _db.HocViens.ToListAsync();
            return View(lop);
        }

        public async Task<IActionResult> CourseRegistration()
        {
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            ViewBag.KhoaHocs = await _db.KhoaHocs.ToListAsync();
            ViewBag.LopHocs = await _db.LopHocs.Include(l => l.MaKhoaHocNavigation).ToListAsync();
            return View();
        }

        public async Task<IActionResult> TeacherStudentAssignment()
        {
            ViewBag.GiaoViens = await _db.GiaoViens.ToListAsync();
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            return View();
        }

        public async Task<IActionResult> StaffTeacherAssignment()
        {
            ViewBag.NhanViens = await _db.NhanViens.ToListAsync();
            ViewBag.GiaoViens = await _db.GiaoViens.ToListAsync();
            return View();
        }

        public async Task<IActionResult> RoomEquipmentAssignment()
        {
            ViewBag.PhongHocs = await _db.PhongHocs.ToListAsync();
            ViewBag.ThietBis = await _db.ThietBis.ToListAsync();
            return View();
        }

        public async Task<IActionResult> StudentDetail(string id)
        {
            var sv = await _db.HocViens
                .Include(h => h.DangKis).ThenInclude(d => d.MaKhoaHocNavigation)
                .Include(h => h.KetQuas).ThenInclude(k => k.MaKhoaHocNavigation)
                .Include(h => h.MaLops).ThenInclude(l => l.MaGvNavigation)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);
            return View(sv);
        }

        public async Task<IActionResult> TeacherDetail(string id)
        {
            var gv = await _db.GiaoViens
                .Include(g => g.LopHocs).ThenInclude(l => l.MaKhoaHocNavigation)
                .Include(g => g.LopHocs).ThenInclude(l => l.MaHocViens)
                .FirstOrDefaultAsync(g => g.MaGv == id);
            return View(gv);
        }

        public async Task<IActionResult> RoomDetail(string id)
        {
            var room = await _db.PhongHocs
                .Include(p => p.MaTrungTamNavigation)
                .Include(p => p.LichHocs)
                .Include(p => p.LopHocs)
                .ThenInclude(l => l.MaKhoaHocNavigation)
                .Include(p => p.MaThietBis)
                .FirstOrDefaultAsync(p => p.MaPhong == id);
            return View(room);
        }

        // ── POST actions ──────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> AddHocVien(HocVien m)
        {
            m.MaHocVien = "HV" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.HocViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("HocVien");
        }

        [HttpPost]
        public async Task<IActionResult> AddGiaoVien(GiaoVien m)
        {
            m.MaGv = "GV" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.GiaoViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("GiaoVien");
        }

        [HttpPost]
        public async Task<IActionResult> AddNhanVien(NhanVien m)
        {
            m.MaNv = "NV" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.NhanViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("NhanVien");
        }

        [HttpPost]
        public async Task<IActionResult> AddKhoaHoc(KhoaHoc m)
        {
            m.MaKhoaHoc = "KH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.KhoaHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("KhoaHoc");
        }

        [HttpPost]
        public async Task<IActionResult> AddLopHoc(LopHoc m)
        {
            m.MaLop = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.LopHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("LopHoc");
        }

        [HttpPost]
        public async Task<IActionResult> AddPhongHoc(PhongHoc m)
        {
            m.MaPhong = "PH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.PhongHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("PhongHoc");
        }

        [HttpPost]
        public async Task<IActionResult> AddThietBi(ThietBi m)
        {
            m.MaThietBi = "TB" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.ThietBis.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("ThietBi");
        }

        [HttpPost]
        public async Task<IActionResult> AddTrungTam(TrungTam m)
        {
            m.MaTrungTam = "TT" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.TrungTams.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("TrungTam");
        }

        [HttpPost]
        public async Task<IActionResult> AddLichHoc(LichHoc m)
        {
            m.MaLichHoc = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.LichHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("LichHoc");
        }

        [HttpPost]
        public async Task<IActionResult> AddKetQua(KetQua m)
        {
            _db.KetQuas.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("KetQua");
        }

        [HttpPost]
        public async Task<IActionResult> AddTaiKhoan(TaiKhoan m)
        {
            _db.TaiKhoans.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("TaiKhoan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCourse(string MaHocVien, string MaKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(MaHocVien) || string.IsNullOrWhiteSpace(MaKhoaHoc))
            {
                return RedirectToAction("CourseRegistration");
            }

            var exists = await _db.DangKis.AnyAsync(d => d.MaHocVien == MaHocVien && d.MaKhoaHoc == MaKhoaHoc);
            if (!exists)
            {
                _db.DangKis.Add(new DangKi
                {
                    MaHocVien = MaHocVien,
                    MaKhoaHoc = MaKhoaHoc,
                    NgayDangKi = DateOnly.FromDateTime(DateTime.Now)
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("CourseRegistration");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacherStudent(string MaHocVien, string MaGv)
        {
            if (string.IsNullOrWhiteSpace(MaHocVien) || string.IsNullOrWhiteSpace(MaGv))
            {
                return RedirectToAction("TeacherStudentAssignment");
            }

            var giaoVien = await _db.GiaoViens
                .Include(g => g.MaHocViens)
                .FirstOrDefaultAsync(g => g.MaGv == MaGv);
            var hocVien = await _db.HocViens.FindAsync(MaHocVien);

            if (giaoVien != null && hocVien != null && !giaoVien.MaHocViens.Any(h => h.MaHocVien == MaHocVien))
            {
                giaoVien.MaHocViens.Add(hocVien);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("TeacherStudentAssignment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStaffTeacher(string MaNv, string MaGv)
        {
            if (string.IsNullOrWhiteSpace(MaNv) || string.IsNullOrWhiteSpace(MaGv))
            {
                return RedirectToAction("StaffTeacherAssignment");
            }

            var nhanVien = await _db.NhanViens
                .Include(n => n.MaGvs)
                .FirstOrDefaultAsync(n => n.MaNv == MaNv);
            var giaoVien = await _db.GiaoViens.FindAsync(MaGv);

            if (nhanVien != null && giaoVien != null && !nhanVien.MaGvs.Any(g => g.MaGv == MaGv))
            {
                nhanVien.MaGvs.Add(giaoVien);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("StaffTeacherAssignment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEquipmentToRoom(string MaThietBi, string MaPhong)
        {
            if (string.IsNullOrWhiteSpace(MaThietBi) || string.IsNullOrWhiteSpace(MaPhong))
            {
                return RedirectToAction("RoomEquipmentAssignment");
            }

            var thietBi = await _db.ThietBis
                .Include(t => t.MaPhongs)
                .FirstOrDefaultAsync(t => t.MaThietBi == MaThietBi);
            var phongHoc = await _db.PhongHocs.FindAsync(MaPhong);

            if (thietBi != null && phongHoc != null && !thietBi.MaPhongs.Any(p => p.MaPhong == MaPhong))
            {
                thietBi.MaPhongs.Add(phongHoc);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("RoomEquipmentAssignment");
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentToClass(string maLop, string maHocVien)
        {
            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == maLop);
            var sv = await _db.HocViens.FindAsync(maHocVien);
            if (lop != null && sv != null && !lop.MaHocViens.Contains(sv))
            {
                lop.MaHocViens.Add(sv);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("StudentsInClass", new { id = maLop });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStudentFromClass(string maLop, string maHocVien)
        {
            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == maLop);
            var sv = lop?.MaHocViens.FirstOrDefault(h => h.MaHocVien == maHocVien);
            if (sv != null) { lop!.MaHocViens.Remove(sv); await _db.SaveChangesAsync(); }
            return RedirectToAction("StudentsInClass", new { id = maLop });
        }

        private bool LopHocExists(string id)
        {
            return _db.LopHocs.Any(e => e.MaLop == id);
        }

        private void PopulateSelectLists(string? maGv = null, string? maPhong = null, string? maKhoaHoc = null)
        {
            ViewData["MaGv"] = new SelectList(_db.GiaoViens.AsNoTracking(), "MaGv", "Ten", maGv);
            ViewData["MaPhong"] = new SelectList(_db.PhongHocs.AsNoTracking(), "MaPhong", "MaPhong", maPhong);
            ViewData["MaKhoaHoc"] = new SelectList(_db.KhoaHocs.AsNoTracking(), "MaKhoaHoc", "TenKhoaHoc", maKhoaHoc);
        }
        // Backward-compatible route for old links: /Admin/EditClass/{id}
        [HttpGet]
        public IActionResult EditClass(string? id)
        {
            return RedirectToAction(nameof(EditLopHoc), new { id });
        }

        // GET: LopHoc/Edit/5
        public async Task<IActionResult> EditLopHoc(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Class id is invalid.";
                return RedirectToAction(nameof(LopHoc));
            }

            id = id.Trim();

            var lopHoc = await _db.LopHocs
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.MaLop.Trim() == id);
            if (lopHoc == null)
            {
                TempData["Error"] = $"Class with id '{id}' was not found.";
                return RedirectToAction(nameof(LopHoc));
            }

            PopulateSelectLists(lopHoc.MaGv, lopHoc.MaPhong, lopHoc.MaKhoaHoc);
            return View(lopHoc);
        }

        // POST: LopHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLopHoc(string id, [Bind("MaLop,MaGv,MaPhong,MaKhoaHoc,TenLop")] LopHoc lopHoc)
        {
            if (id != lopHoc.MaLop)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(lopHoc);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocExists(lopHoc.MaLop))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(LopHoc));
            }

            PopulateSelectLists(lopHoc.MaGv, lopHoc.MaPhong, lopHoc.MaKhoaHoc);
            return View(lopHoc);
        }

        public async Task<IActionResult> DeleteHocVien(string id)
        {
            var e = await _db.HocViens.FindAsync(id);
            if (e != null) { _db.HocViens.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("HocVien");
        }
        public async Task<IActionResult> DeleteGiaoVien(string id)
        {
            var e = await _db.GiaoViens.FindAsync(id);
            if (e != null) { _db.GiaoViens.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("GiaoVien");
        }
        public async Task<IActionResult> DeleteKhoaHoc(string id)
        {
            var e = await _db.KhoaHocs.FindAsync(id);
            if (e != null) { _db.KhoaHocs.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("KhoaHoc");
        }
        public async Task<IActionResult> DeleteLopHoc(string id)
        {
            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .FirstOrDefaultAsync(l => l.MaLop == id);
            if (lop == null) return RedirectToAction("LopHoc");

            if (lop.MaHocViens.Any())
            {
                TempData["Error"] = $"The class with {id} cannot be deleted because there are still students participating.";
                return RedirectToAction("LopHoc");
            }

            _db.LopHocs.Remove(lop);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"The class with the {id} has been deleted.";
            return RedirectToAction("LopHoc");
        }
        public async Task<IActionResult> DeletePhongHoc(string id)
        {
            var e = await _db.PhongHocs.FindAsync(id);
            if (e != null) { _db.PhongHocs.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("PhongHoc");
        }
        public async Task<IActionResult> DeleteThietBi(string id)
        {
            var e = await _db.ThietBis.FindAsync(id);
            if (e != null) { _db.ThietBis.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("ThietBi");
        }
        public async Task<IActionResult> DeleteTrungTam(string id)
        {
            var e = await _db.TrungTams.FindAsync(id);
            if (e != null) { _db.TrungTams.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("TrungTam");
        }
        public async Task<IActionResult> DeleteLichHoc(string id)
        {
            var e = await _db.LichHocs.FindAsync(id);
            if (e != null) { _db.LichHocs.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("LichHoc");
        }
        public async Task<IActionResult> DeleteTaiKhoan(string id)
        {
            var e = await _db.TaiKhoans.FindAsync(id);
            if (e != null) { _db.TaiKhoans.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("TaiKhoan");
        }
    }
}
