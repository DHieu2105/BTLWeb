using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BTL_Web.Services;
using BTL_Web.ViewModels;

namespace BTL_Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly TtanContext _db;
        private readonly IWebHostEnvironment _environment;

        public AdminController(TtanContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _db.HocViens.CountAsync();
            ViewBag.TotalTeachers = await _db.GiaoViens.CountAsync();
            ViewBag.TotalClasses = await _db.LopHocs.CountAsync();
            ViewBag.TotalStaff = await _db.NhanViens.CountAsync();
            return View();
        }

        public async Task<IActionResult> HocVien(int page = 1, int pageSize = 10, string searchStudent = "")
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _db.HocViens
                .Include(h => h.DangKis)
                    .ThenInclude(d => d.MaKhoaHocNavigation)
                .AsNoTracking();

            // Filter by student name or ID
            if (!string.IsNullOrWhiteSpace(searchStudent))
            {
                var search = searchStudent.Trim().ToLower();
                query = query.Where(h => h.MaHocVien.ToLower().Contains(search) || 
                                         (h.HoVaTen != null && h.HoVaTen.ToLower().Contains(search)));
            }

            query = query.OrderBy(h => h.MaHocVien);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchStudent = searchStudent;

            return View(items);
        }

        public async Task<IActionResult> GiaoVien()
            => View(await _db.GiaoViens.Include(g => g.LopHocs).ToListAsync());

        public async Task<IActionResult> NhanVien()
            => View(await _db.NhanViens.Include(n => n.MaTrungTamNavigation).ToListAsync());

        public async Task<IActionResult> KhoaHoc()
            => View(await _db.KhoaHocs.AsNoTracking().ToListAsync());

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
        {
            ViewBag.PhongHocs = await _db.PhongHocs.AsNoTracking().OrderBy(p => p.TenPhong).ToListAsync();
            return View(await _db.ThietBis.Include(t => t.MaPhongs).ToListAsync());
        }

        public async Task<IActionResult> TrungTam()
            => View(await _db.TrungTams.ToListAsync());

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

        public async Task<IActionResult> TaiKhoan(int page = 1, int pageSize = 10, string? keyword = null, string? role = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _db.TaiKhoans
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var search = keyword.Trim().ToLower();
                query = query.Where(t =>
                    t.Username.ToLower().Contains(search) ||
                    (t.Role != null && t.Role.ToLower().Contains(search)) ||
                    (t.MaNv != null && t.MaNv.ToLower().Contains(search)) ||
                    (t.MaGv != null && t.MaGv.ToLower().Contains(search)) ||
                    (t.MaHocVien != null && t.MaHocVien.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(t => t.Role == role);
            }

            query = query.OrderBy(t => t.Username);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword ?? string.Empty;
            ViewBag.RoleFilter = role ?? string.Empty;
            ViewBag.AdminCount = await _db.TaiKhoans.CountAsync(x => x.Role == "Admin");
            ViewBag.StaffCount = await _db.TaiKhoans.CountAsync(x => x.Role == "NhanVien");
            ViewBag.TeacherCount = await _db.TaiKhoans.CountAsync(x => x.Role == "GiaoVien");
            ViewBag.StudentCount = await _db.TaiKhoans.CountAsync(x => x.Role == "HocVien");

            return View(items);
        }

        public async Task<IActionResult> RevenueReport()
        {
            var rows = await _db.DangKis
                .Include(d => d.MaKhoaHocNavigation)
                .AsNoTracking()
                .GroupBy(d => new
                {
                    d.MaKhoaHoc,
                    TenKhoaHoc = d.MaKhoaHocNavigation.TenKhoaHoc,
                    HocPhi = d.MaKhoaHocNavigation.HocPhi ?? 0
                })
                .Select(g => new StaffRevenueReportRowViewModel
                {
                    MaKhoaHoc = g.Key.MaKhoaHoc,
                    TenKhoaHoc = g.Key.TenKhoaHoc,
                    HocPhi = g.Key.HocPhi,
                    SoDangKy = g.Count(),
                    SoHocVien = g.Select(x => x.MaHocVien).Distinct().Count(),
                    TongDoanhThu = g.Count() * g.Key.HocPhi
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ThenBy(x => x.TenKhoaHoc)
                .ToListAsync();

            return View(rows);
        }

        public IActionResult ReportCenter()
        {
            return View();
        }

        public async Task<IActionResult> KetQua(int page = 1, int pageSize = 10, int? minScore = null, int? maxScore = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _db.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .AsNoTracking();

            // Filter by score range
            if (minScore.HasValue)
            {
                query = query.Where(k => (k.DiemTong ?? 0) >= minScore.Value);
            }
            if (maxScore.HasValue)
            {
                query = query.Where(k => (k.DiemTong ?? 0) <= maxScore.Value);
            }

            query = query.OrderByDescending(k => k.DiemTong)
                .ThenBy(k => k.MaHocVien);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.MinScore = minScore;
            ViewBag.MaxScore = maxScore;

            return View(items);
        }

        public async Task<IActionResult> StudentsInClass(string id)
        {
            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id);
            ViewBag.AllStudents = await _db.HocViens.ToListAsync();
            ViewBag.AllClasses = await _db.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .OrderBy(l => l.TenLop)
                .ToListAsync();
            return View(lop);
        }

        public async Task<IActionResult> CourseRegistration()
        {
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            ViewBag.KhoaHocs = await _db.KhoaHocs.ToListAsync();
            ViewBag.LopHocs = await _db.LopHocs.Include(l => l.MaKhoaHocNavigation).ToListAsync();
            ViewBag.DangKis = await _db.DangKis
                .Include(d => d.MaHocVienNavigation)
                .Include(d => d.MaKhoaHocNavigation)
                .OrderByDescending(d => d.NgayDangKi)
                .ToListAsync();
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

            ViewBag.StudentImageUrl = GetStudentImageUrl(id);
            return View(sv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadStudentImage(string id, IFormFile? image)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Mã học viên không hợp lệ.";
                return RedirectToAction(nameof(HocVien));
            }

            var studentExists = await _db.HocViens.AnyAsync(h => h.MaHocVien == id);
            if (!studentExists)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction(nameof(HocVien));
            }

            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ảnh để tải lên.";
                return RedirectToAction(nameof(StudentDetail), new { id });
            }

            if (image.Length > 2 * 1024 * 1024)
            {
                TempData["Error"] = "Ảnh vượt quá dung lượng 2MB.";
                return RedirectToAction(nameof(StudentDetail), new { id });
            }

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(extension))
            {
                TempData["Error"] = "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG hoặc WEBP.";
                return RedirectToAction(nameof(StudentDetail), new { id });
            }

            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsDir);

            foreach (var ext in allowed)
            {
                var oldPath = Path.Combine(uploadsDir, $"{id}{ext}");
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            var fileName = $"{id}{extension}";
            var filePath = Path.Combine(uploadsDir, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            HttpContext.Session.SetString("LastUploadedStudentId", id);
            Response.Cookies.Append(
                "LastUploadedStudentImage",
                $"/uploads/students/{fileName}",
                new CookieOptions { HttpOnly = true, IsEssential = true, Expires = DateTimeOffset.UtcNow.AddDays(7) });

            TempData["Success"] = "Tải ảnh học viên thành công.";
            return RedirectToAction(nameof(StudentDetail), new { id });
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHocVien(HocVien m)
        {
            if (string.IsNullOrWhiteSpace(m.HoVaTen))
            {
                TempData["Error"] = "Student name is required.";
                return RedirectToAction(nameof(HocVien));
            }

            m.MaHocVien = await GenerateNextHocVienIdAsync();
            m.NgayDangKi = DateOnly.FromDateTime(DateTime.Now);
            _db.HocViens.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("HocVien");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHocVien(HocVien model)
        {
            if (string.IsNullOrWhiteSpace(model.MaHocVien) || string.IsNullOrWhiteSpace(model.HoVaTen))
            {
                TempData["Error"] = "Student information is invalid.";
                return RedirectToAction(nameof(HocVien));
            }

            var existing = await _db.HocViens.FindAsync(model.MaHocVien);
            if (existing == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction(nameof(HocVien));
            }

            existing.HoVaTen = model.HoVaTen;
            existing.Sdt = model.Sdt;
            existing.GioiTinh = model.GioiTinh;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Student updated successfully.";
            return RedirectToAction(nameof(HocVien));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddThietBi(ThietBi m, string? maPhong)
        {
            m.MaThietBi = "TB" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.ThietBis.Add(m);

            if (!string.IsNullOrWhiteSpace(maPhong))
            {
                var room = await _db.PhongHocs.FindAsync(maPhong);
                if (room != null)
                {
                    m.MaPhongs.Add(room);
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("ThietBi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateThietBi(string maThietBi, string? tenThietBi, int? soLuong, string? maPhong)
        {
            if (string.IsNullOrWhiteSpace(maThietBi))
            {
                return RedirectToAction("ThietBi");
            }

            var existing = await _db.ThietBis
                .Include(t => t.MaPhongs)
                .FirstOrDefaultAsync(t => t.MaThietBi == maThietBi);

            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy thiết bị cần cập nhật.";
                return RedirectToAction("ThietBi");
            }

            existing.TenThietBi = tenThietBi;
            existing.SoLuong = soLuong;

            existing.MaPhongs.Clear();
            if (!string.IsNullOrWhiteSpace(maPhong))
            {
                var room = await _db.PhongHocs.FindAsync(maPhong);
                if (room != null)
                {
                    existing.MaPhongs.Add(room);
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thiết bị thành công.";
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLichHoc(LichHoc m)
        {
            var existing = await _db.LichHocs.FindAsync(m.MaLichHoc);
            if (existing == null)
            {
                return RedirectToAction("LichHoc");
            }

            var roomId = await _db.LopHocs
                .Where(l => l.MaLop == m.MaLop)
                .Select(l => l.MaPhong)
                .FirstOrDefaultAsync();

            existing.MaLop = m.MaLop;
            existing.MaPhong = roomId;
            existing.NgayHoc = m.NgayHoc;
            existing.GioBatDau = m.GioBatDau;
            existing.GioKetThuc = m.GioKetThuc;

            await _db.SaveChangesAsync();
            return RedirectToAction("LichHoc");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddKetQua(KetQua m)
        {
            TempData["Error"] = "Theo nghiệp vụ, chỉ Giáo viên mới có quyền nhập/sửa điểm.";
            return RedirectToAction("KetQua");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTaiKhoan(TaiKhoan m)
        {
            if (!string.IsNullOrWhiteSpace(m.Password))
            {
                m.Password = PasswordHasher.Hash(m.Password);
            }

            _db.TaiKhoans.Add(m);
            await _db.SaveChangesAsync();
            return RedirectToAction("TaiKhoan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTaiKhoan(string Username, string? Password, string? Role)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return RedirectToAction("TaiKhoan");
            }

            var existing = await _db.TaiKhoans.FindAsync(Username);
            if (existing == null)
            {
                return RedirectToAction("TaiKhoan");
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                existing.Password = PasswordHasher.Hash(Password);
            }

            if (!string.IsNullOrWhiteSpace(Role))
            {
                existing.Role = Role;
            }

            _db.TaiKhoans.Update(existing);
            await _db.SaveChangesAsync();
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
        public async Task<IActionResult> UpdateRegistration(
            string oldMaHocVien,
            string oldMaKhoaHoc,
            string newMaHocVien,
            string newMaKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(oldMaHocVien) || string.IsNullOrWhiteSpace(oldMaKhoaHoc) ||
                string.IsNullOrWhiteSpace(newMaHocVien) || string.IsNullOrWhiteSpace(newMaKhoaHoc))
            {
                TempData["Error"] = "Registration information is invalid.";
                return RedirectToAction(nameof(CourseRegistration));
            }

            var existing = await _db.DangKis.FindAsync(oldMaKhoaHoc, oldMaHocVien);
            if (existing == null)
            {
                TempData["Error"] = "Registration not found.";
                return RedirectToAction(nameof(CourseRegistration));
            }

            var targetExists = await _db.DangKis.AnyAsync(d => d.MaHocVien == newMaHocVien && d.MaKhoaHoc == newMaKhoaHoc);
            if (targetExists && (oldMaHocVien != newMaHocVien || oldMaKhoaHoc != newMaKhoaHoc))
            {
                TempData["Error"] = "The target registration already exists.";
                return RedirectToAction(nameof(CourseRegistration));
            }

            if (oldMaHocVien == newMaHocVien && oldMaKhoaHoc == newMaKhoaHoc)
            {
                TempData["Success"] = "Registration updated successfully.";
                return RedirectToAction(nameof(CourseRegistration));
            }

            var ngayDangKi = existing.NgayDangKi;
            _db.DangKis.Remove(existing);
            _db.DangKis.Add(new DangKi
            {
                MaHocVien = newMaHocVien,
                MaKhoaHoc = newMaKhoaHoc,
                NgayDangKi = ngayDangKi ?? DateOnly.FromDateTime(DateTime.Now)
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = "Registration updated successfully.";
            return RedirectToAction(nameof(CourseRegistration));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRegistration(string maHocVien, string maKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(maHocVien) || string.IsNullOrWhiteSpace(maKhoaHoc))
            {
                return RedirectToAction(nameof(CourseRegistration));
            }

            var existing = await _db.DangKis.FindAsync(maKhoaHoc, maHocVien);
            if (existing != null)
            {
                _db.DangKis.Remove(existing);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Registration deleted successfully.";
            }

            return RedirectToAction(nameof(CourseRegistration));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudentToClass(string maLop, string maHocVien)
        {
            if (string.IsNullOrWhiteSpace(maLop) || string.IsNullOrWhiteSpace(maHocVien))
            {
                return RedirectToAction("LopHoc");
            }

            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == maLop);
            var sv = await _db.HocViens.FindAsync(maHocVien);
            if (lop != null && sv != null && !lop.MaHocViens.Contains(sv))
            {
                lop.MaHocViens.Add(sv);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã thêm học viên vào lớp.";
            }
            return RedirectToAction("StudentsInClass", new { id = maLop });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudentFromClass(string maLop, string maHocVien)
        {
            if (string.IsNullOrWhiteSpace(maLop) || string.IsNullOrWhiteSpace(maHocVien))
            {
                return RedirectToAction("LopHoc");
            }

            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == maLop);
            var sv = lop?.MaHocViens.FirstOrDefault(h => h.MaHocVien == maHocVien);
            if (sv != null)
            {
                lop!.MaHocViens.Remove(sv);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xóa học viên khỏi lớp.";
            }
            return RedirectToAction("StudentsInClass", new { id = maLop });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStudentClass(string oldMaLop, string newMaLop, string maHocVien)
        {
            if (string.IsNullOrWhiteSpace(oldMaLop) || string.IsNullOrWhiteSpace(newMaLop) || string.IsNullOrWhiteSpace(maHocVien))
            {
                TempData["Error"] = "Thông tin chuyển lớp không hợp lệ.";
                return RedirectToAction("StudentsInClass", new { id = oldMaLop });
            }

            if (oldMaLop == newMaLop)
            {
                TempData["Success"] = "Học viên đã ở lớp được chọn.";
                return RedirectToAction("StudentsInClass", new { id = oldMaLop });
            }

            var oldClass = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .FirstOrDefaultAsync(l => l.MaLop == oldMaLop);
            var newClass = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .FirstOrDefaultAsync(l => l.MaLop == newMaLop);

            if (oldClass == null || newClass == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học.";
                return RedirectToAction("StudentsInClass", new { id = oldMaLop });
            }

            var student = oldClass.MaHocViens.FirstOrDefault(h => h.MaHocVien == maHocVien);
            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy học viên trong lớp hiện tại.";
                return RedirectToAction("StudentsInClass", new { id = oldMaLop });
            }

            if (newClass.MaHocViens.Any(h => h.MaHocVien == maHocVien))
            {
                TempData["Error"] = "Học viên đã có trong lớp đích.";
                return RedirectToAction("StudentsInClass", new { id = oldMaLop });
            }

            oldClass.MaHocViens.Remove(student);
            newClass.MaHocViens.Add(student);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đã chuyển học viên sang lớp mới.";
            return RedirectToAction("StudentsInClass", new { id = newMaLop });
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
            var hocVien = await _db.HocViens
                .Include(h => h.TaiKhoans)
                .Include(h => h.DangKis)
                .Include(h => h.KetQuas)
                .Include(h => h.MaGvs)
                .Include(h => h.MaLops)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);
            
            if (hocVien == null)
                return RedirectToAction("HocVien");

            // Delete relationship with teachers (GiaoVien)
            hocVien.MaGvs.Clear();
            
            // Delete relationship with classes (LopHoc)
            hocVien.MaLops.Clear();
            
            // Delete accounts (foreign key constraint)
            _db.TaiKhoans.RemoveRange(hocVien.TaiKhoans);
            
            // Delete registrations
            _db.DangKis.RemoveRange(hocVien.DangKis);
            
            // Delete results
            _db.KetQuas.RemoveRange(hocVien.KetQuas);
            
            // Delete the student
            _db.HocViens.Remove(hocVien);
            
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa học viên.";
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

        private async Task<string> GenerateNextHocVienIdAsync()
        {
            var ids = await _db.HocViens
                .AsNoTracking()
                .Select(h => h.MaHocVien)
                .Where(id => id != null)
                .ToListAsync();

            var maxNumeric = 0;
            foreach (var id in ids)
            {
                if (string.IsNullOrWhiteSpace(id) || !id.StartsWith("HV", StringComparison.OrdinalIgnoreCase) || id.Length <= 2)
                {
                    continue;
                }

                if (int.TryParse(id.Substring(2), out var num) && num > maxNumeric)
                {
                    maxNumeric = num;
                }
            }

            var next = maxNumeric + 1;
            var candidate = $"HV{next:D6}";

            while (await _db.HocViens.AnyAsync(h => h.MaHocVien == candidate))
            {
                next++;
                candidate = $"HV{next:D6}";
            }

            return candidate;
        }

        private string? GetStudentImageUrl(string? studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                return null;
            }

            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "students");
            if (!Directory.Exists(uploadDir))
            {
                return null;
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            foreach (var ext in allowed)
            {
                var fileName = $"{studentId}{ext}";
                var fullPath = Path.Combine(uploadDir, fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    return $"/uploads/students/{fileName}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                }
            }

            return null;
        }
    }
}
