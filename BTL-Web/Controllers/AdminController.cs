using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BTL_Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly TtanContext _db;
        public AdminController(TtanContext db) => _db = db;

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _db.HocViens.CountAsync();
            ViewBag.TotalTeachers = await _db.GiaoViens.CountAsync();
            ViewBag.TotalClasses = await _db.LopHocs.CountAsync();
            ViewBag.TotalStaff = await _db.NhanViens.CountAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HocVien()
            => View(await _db.HocViens.Include(h => h.DangKis).ThenInclude(d => d.MaKhoaHocNavigation).ToListAsync());

        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> GiaoVien(string? keyword, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _db.GiaoViens
                .Include(g => g.LopHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(g =>
                    g.MaGv.Contains(keyword) ||
                    (g.Ten != null && g.Ten.Contains(keyword)) ||
                    (g.ChuyenMon != null && g.ChuyenMon.Contains(keyword)) ||
                    (g.Sdt != null && g.Sdt.Contains(keyword)));
            }

            var totalItems = await query.CountAsync();
            var data = await query
                .OrderBy(g => g.MaGv)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NhanVien()
            => View(await _db.NhanViens.Include(n => n.MaTrungTamNavigation).ToListAsync());

        [Authorize(Roles = "Admin,NhanVien,HocVien")]
        public async Task<IActionResult> KhoaHoc(string? keyword, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _db.KhoaHocs
                .Include(k => k.MaTrungTamNavigation)
                .Include(k => k.DangKis)
                .Include(k => k.LopHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(k =>
                    k.MaKhoaHoc.Contains(keyword) ||
                    (k.TenKhoaHoc != null && k.TenKhoaHoc.Contains(keyword)));
            }

            var totalItems = await query.CountAsync();
            var data = await query
                .OrderBy(k => k.MaKhoaHoc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TrungTams = await _db.TrungTams.OrderBy(t => t.MaTrungTam).ToListAsync();

            return View(data);
        }

        [Authorize(Roles = "GiaoVien")]
        public async Task<IActionResult> MyTeacherProfile()
        {
            var maGv = User.FindFirst("MaGV")?.Value;
            if (string.IsNullOrWhiteSpace(maGv))
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                maGv = await _db.TaiKhoans
                    .Where(t => t.Username == username)
                    .Select(t => t.MaGv)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrWhiteSpace(maGv))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(TeacherDetail), new { id = maGv });
        }

        [Authorize(Roles = "GiaoVien")]
        public async Task<IActionResult> MyTeachingCourses(string? keyword, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var maGv = User.FindFirst("MaGV")?.Value;
            if (string.IsNullOrWhiteSpace(maGv))
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                maGv = await _db.TaiKhoans
                    .Where(t => t.Username == username)
                    .Select(t => t.MaGv)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrWhiteSpace(maGv))
            {
                return View("KhoaHoc", new List<KhoaHoc>());
            }

            var courseIds = await _db.LopHocs
                .Where(l => l.MaGv == maGv && l.MaKhoaHoc != null)
                .Select(l => l.MaKhoaHoc!)
                .Distinct()
                .ToListAsync();

            var query = _db.KhoaHocs
                .Include(k => k.MaTrungTamNavigation)
                .Include(k => k.DangKis)
                .Include(k => k.LopHocs)
                .Where(k => courseIds.Contains(k.MaKhoaHoc))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(k =>
                    k.MaKhoaHoc.Contains(keyword) ||
                    (k.TenKhoaHoc != null && k.TenKhoaHoc.Contains(keyword)));
            }

            var totalItems = await query.CountAsync();
            var data = await query
                .OrderBy(k => k.MaKhoaHoc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TrungTams = await _db.TrungTams.OrderBy(t => t.MaTrungTam).ToListAsync();

            return View("KhoaHoc", data);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PhongHoc()
        {
            ViewBag.TrungTams = await _db.TrungTams.ToListAsync();
            return View(await _db.PhongHocs.Include(p => p.MaTrungTamNavigation).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThietBi()
            => View(await _db.ThietBis.Include(t => t.MaPhongs).ToListAsync());

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TrungTam()
            => View(await _db.TrungTams.ToListAsync());

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LichHoc()
        {
            ViewBag.LopHocs = await _db.LopHocs.ToListAsync();
            ViewBag.PhongHocs = await _db.PhongHocs.ToListAsync();
            return View(await _db.LichHocs
                .Include(l => l.MaLopNavigation)
                .Include(l => l.MaPhongNavigation)
                .OrderBy(l => l.NgayHoc)
                .ThenBy(l => l.GioBatDau)
                .ToListAsync());
        }

            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TaiKhoan()
            => View(await _db.TaiKhoans.ToListAsync());

            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> KetQua()
        {
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            ViewBag.LopHocs = await _db.LopHocs.ToListAsync();
            return View(await _db.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .ToListAsync());
        }

            [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CourseRegistration()
        {
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            ViewBag.KhoaHocs = await _db.KhoaHocs.ToListAsync();
            ViewBag.LopHocs = await _db.LopHocs.Include(l => l.MaKhoaHocNavigation).ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TeacherStudentAssignment()
        {
            ViewBag.GiaoViens = await _db.GiaoViens.ToListAsync();
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StaffTeacherAssignment()
        {
            ViewBag.NhanViens = await _db.NhanViens.ToListAsync();
            ViewBag.GiaoViens = await _db.GiaoViens.ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RoomEquipmentAssignment()
        {
            ViewBag.PhongHocs = await _db.PhongHocs.ToListAsync();
            ViewBag.ThietBis = await _db.ThietBis.ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StudentDetail(string id)
        {
            var sv = await _db.HocViens
                .Include(h => h.DangKis).ThenInclude(d => d.MaKhoaHocNavigation)
                .Include(h => h.KetQuas).ThenInclude(k => k.MaKhoaHocNavigation)
                .Include(h => h.MaLops).ThenInclude(l => l.MaGvNavigation)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);
            return View(sv);
        }

        [Authorize(Roles = "Admin,NhanVien,GiaoVien")]
        public async Task<IActionResult> TeacherDetail(string id)
        {
            if (User.IsInRole("GiaoVien"))
            {
                var ownMaGv = User.FindFirst("MaGV")?.Value;
                if (!string.Equals(ownMaGv, id, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }

            var gv = await _db.GiaoViens
                .Include(g => g.LopHocs).ThenInclude(l => l.MaKhoaHocNavigation)
                .Include(g => g.LopHocs).ThenInclude(l => l.MaHocViens)
                .FirstOrDefaultAsync(g => g.MaGv == id);
            return View(gv);
        }

        [Authorize(Roles = "Admin,NhanVien,HocVien,GiaoVien")]
        public async Task<IActionResult> CourseDetail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (User.IsInRole("GiaoVien"))
            {
                var ownMaGv = User.FindFirst("MaGV")?.Value;
                if (string.IsNullOrWhiteSpace(ownMaGv))
                {
                    return Forbid();
                }

                var isAssigned = await _db.LopHocs.AnyAsync(l => l.MaGv == ownMaGv && l.MaKhoaHoc == id)
                                 || await _db.GiaoViens.AnyAsync(g => g.MaGv == ownMaGv && g.MaKhoaHoc == id);

                if (!isAssigned)
                {
                    return Forbid();
                }
            }

            var course = await _db.KhoaHocs
                .Include(k => k.MaTrungTamNavigation)
                .Include(k => k.DangKis)
                .Include(k => k.LopHocs)
                    .ThenInclude(l => l.MaGvNavigation)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            return course == null ? NotFound() : View(course);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddHocVien(HocVien m)
        {
            m.MaHocVien = "HV" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.HocViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("HocVien");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddGiaoVien(GiaoVien m)
        {
            m.MaGv = "GV" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.GiaoViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("GiaoVien");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGiaoVien(GiaoVien m)
        {
            if (string.IsNullOrWhiteSpace(m.MaGv))
            {
                TempData["Error"] = "Thiếu mã giáo viên để cập nhật.";
                return RedirectToAction("GiaoVien");
            }

            var entity = await _db.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == m.MaGv);
            if (entity == null)
            {
                TempData["Error"] = "Không tìm thấy giáo viên cần sửa.";
                return RedirectToAction("GiaoVien");
            }

            entity.Ten = m.Ten?.Trim();
            entity.Sdt = m.Sdt?.Trim();
            entity.ChuyenMon = m.ChuyenMon?.Trim();
            entity.GioiTinh = m.GioiTinh?.Trim();

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật giáo viên thành công.";
            return RedirectToAction("GiaoVien");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNhanVien(NhanVien m)
        {
            m.MaNv = "NV" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.NhanViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("NhanVien");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddKhoaHoc(KhoaHoc m)
        {
            m.MaKhoaHoc = "KH" + DateTime.Now.Ticks.ToString().Substring(10);
            m.MaTrungTam = string.IsNullOrWhiteSpace(m.MaTrungTam) ? null : m.MaTrungTam;
            _db.KhoaHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("KhoaHoc");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateKhoaHoc(KhoaHoc m)
        {
            if (string.IsNullOrWhiteSpace(m.MaKhoaHoc))
            {
                TempData["Error"] = "Thiếu mã khóa học để cập nhật.";
                return RedirectToAction("KhoaHoc");
            }

            var entity = await _db.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == m.MaKhoaHoc);
            if (entity == null)
            {
                TempData["Error"] = "Không tìm thấy khóa học cần sửa.";
                return RedirectToAction("KhoaHoc");
            }

            entity.TenKhoaHoc = m.TenKhoaHoc?.Trim();
            entity.ThoiLuong = m.ThoiLuong;
            entity.HocPhi = m.HocPhi;
            entity.MaTrungTam = string.IsNullOrWhiteSpace(m.MaTrungTam) ? null : m.MaTrungTam;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật khóa học thành công.";
            return RedirectToAction("KhoaHoc");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLopHoc(LopHoc m)
        {
            m.MaLop = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.LopHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("LopHoc");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPhongHoc(PhongHoc m)
        {
            m.MaPhong = "PH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.PhongHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("PhongHoc");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddThietBi(ThietBi m)
        {
            m.MaThietBi = "TB" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.ThietBis.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("ThietBi");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTrungTam(TrungTam m)
        {
            m.MaTrungTam = "TT" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.TrungTams.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("TrungTam");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLichHoc(LichHoc m)
        {
            var roomId = await _db.LopHocs
                .Where(l => l.MaLop == m.MaLop)
                .Select(l => l.MaPhong)
                .FirstOrDefaultAsync();

            m.MaLichHoc = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            m.MaPhong = roomId;
            _db.LichHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("LichHoc");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddKetQua(KetQua m)
        {
            _db.KetQuas.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("KetQua");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTaiKhoan(TaiKhoan m)
        {
            _db.TaiKhoans.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("TaiKhoan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveStudentFromClass(string maLop, string maHocVien)
        {
            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == maLop);
            var sv = lop?.MaHocViens.FirstOrDefault(h => h.MaHocVien == maHocVien);
            if (sv != null) { lop!.MaHocViens.Remove(sv); await _db.SaveChangesAsync(); }
            return RedirectToAction("StudentsInClass", new { id = maLop });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHocVien(string id)
        {
            var e = await _db.HocViens.FindAsync(id);
            if (e != null) { _db.HocViens.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("HocVien");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGiaoVien(string id)
        {
            var e = await _db.GiaoViens.FindAsync(id);
            if (e != null) { _db.GiaoViens.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("GiaoVien");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteKhoaHoc(string id)
        {
            var e = await _db.KhoaHocs.FindAsync(id);
            if (e != null) { _db.KhoaHocs.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("KhoaHoc");
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhongHoc(string id)
        {
            var e = await _db.PhongHocs.FindAsync(id);
            if (e != null) { _db.PhongHocs.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("PhongHoc");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteThietBi(string id)
        {
            var e = await _db.ThietBis.FindAsync(id);
            if (e != null) { _db.ThietBis.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("ThietBi");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrungTam(string id)
        {
            var e = await _db.TrungTams.FindAsync(id);
            if (e != null) { _db.TrungTams.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("TrungTam");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLichHoc(string id)
        {
            var e = await _db.LichHocs.FindAsync(id);
            if (e != null) { _db.LichHocs.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("LichHoc");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTaiKhoan(string id)
        {
            var e = await _db.TaiKhoans.FindAsync(id);
            if (e != null) { _db.TaiKhoans.Remove(e); await _db.SaveChangesAsync(); }
            return RedirectToAction("TaiKhoan");
        }
    }
}
