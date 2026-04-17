using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using BTL_Web.ViewModels;

namespace BTL_Web.Controllers
{
    // ═══════════════════════════════════════════════════════════════
    //  STAFF CONTROLLER
    // ═══════════════════════════════════════════════════════════════
    [Authorize(Roles = "NhanVien")]
    public class StaffController : Controller
    {
        private readonly TtanContext _db;
        public StaffController(TtanContext db) => _db = db;

        private string? GetMaNv() => User.FindFirst("MaNV")?.Value;

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _db.HocViens.CountAsync();
            ViewBag.TotalClasses = await _db.LopHocs.CountAsync();
            return View();
        }

        public async Task<IActionResult> StudentManagement(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _db.HocViens
                .Include(h => h.DangKis)
                    .ThenInclude(d => d.MaKhoaHocNavigation)
                .AsNoTracking()
                .OrderBy(h => h.MaHocVien);

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

            return View(items);
        }

        public async Task<IActionResult> ResultManagement(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _db.KetQuas
                .Include(k => k.MaHocVienNavigation)
                .Include(k => k.MaKhoaHocNavigation)
                .AsNoTracking()
                .OrderByDescending(k => k.DiemTong)
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

            return View(items);
        }

        public async Task<IActionResult> ClassManagement(int page = 1, int pageSize = 10)
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

        public async Task<IActionResult> CourseManagement()
            => View(await _db.KhoaHocs.Include(k => k.LopHocs).ToListAsync());

        public async Task<IActionResult> RoomManagement()
            => View(await _db.PhongHocs.Include(p => p.MaTrungTamNavigation).ToListAsync());

        public async Task<IActionResult> ScheduleManagement()
        {
            ViewBag.LopHocs = await _db.LopHocs
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaKhoaHocNavigation)
                .OrderBy(l => l.MaLop)
                .ToListAsync();
            return View(await _db.LichHocs
                .Include(l => l.MaPhongNavigation)
                .Include(l => l.MaLopNavigation)
                .ToListAsync());
        }

        public async Task<IActionResult> ClassDetail(string id)
        {
            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaPhongNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id);
            ViewBag.AllClasses = await _db.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .OrderBy(l => l.TenLop)
                .ToListAsync();
            return View(lop);
        }

        public async Task<IActionResult> AddStudentToClass()
        {
            ViewBag.LopHocs = await _db.LopHocs.Include(l => l.MaKhoaHocNavigation).ToListAsync();
            ViewBag.HocViens = await _db.HocViens.ToListAsync();
            return View();
        }

        public async Task<IActionResult> RegisterStudent()
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

        [HttpPost]
        public async Task<IActionResult> AddSchedule(LichHoc m)
        {
            var classRoom = await _db.LopHocs
                .Where(l => l.MaLop == m.MaLop)
                .Select(l => l.MaPhong)
                .FirstOrDefaultAsync();

            m.MaLichHoc = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            m.MaPhong = classRoom;
            _db.LichHocs.Add(m); await _db.SaveChangesAsync();
            return RedirectToAction("ScheduleManagement");
        }

        [HttpPost]
        public async Task<IActionResult> EditSchedule(LichHoc m)
        {
            var existing = await _db.LichHocs.FindAsync(m.MaLichHoc);
            if (existing == null)
            {
                return RedirectToAction("ScheduleManagement");
            }

            existing.NgayHoc = m.NgayHoc;
            existing.GioBatDau = m.GioBatDau;
            existing.GioKetThuc = m.GioKetThuc;
            existing.MaLop = m.MaLop;
            existing.MaPhong = await _db.LopHocs
                .Where(l => l.MaLop == m.MaLop)
                .Select(l => l.MaPhong)
                .FirstOrDefaultAsync();

            await _db.SaveChangesAsync();
            return RedirectToAction("ScheduleManagement");
        }

        [HttpPost]
        public async Task<IActionResult> AddClass(LopHoc m)
        {
            m.MaLop = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.LopHocs.Add(m);
            await _db.SaveChangesAsync();
            return RedirectToAction("ClassManagement");
        }

        public async Task<IActionResult> DeleteSchedule(string id)
        {
            var e = await _db.LichHocs.FindAsync(id);
            if (e != null)
            {
                _db.LichHocs.Remove(e);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("ScheduleManagement");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudentSubmit(string MaHocVien, string MaKhoaHoc, string TrangThai)
        {
            if (!string.IsNullOrEmpty(MaHocVien) && !string.IsNullOrEmpty(MaKhoaHoc))
            {
                var exists = await _db.DangKis.AnyAsync(d => d.MaHocVien == MaHocVien && d.MaKhoaHoc == MaKhoaHoc);
                if (!exists)
                {
                    _db.DangKis.Add(new DangKi { MaHocVien = MaHocVien, MaKhoaHoc = MaKhoaHoc, NgayDangKi = DateOnly.FromDateTime(DateTime.Now) });
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(RegisterStudent));
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
                TempData["Error"] = "Thông tin đăng ký không hợp lệ.";
                return RedirectToAction(nameof(RegisterStudent));
            }

            var existing = await _db.DangKis.FindAsync(oldMaKhoaHoc, oldMaHocVien);
            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi đăng ký.";
                return RedirectToAction(nameof(RegisterStudent));
            }

            var targetExists = await _db.DangKis.AnyAsync(d => d.MaHocVien == newMaHocVien && d.MaKhoaHoc == newMaKhoaHoc);
            if (targetExists && (oldMaHocVien != newMaHocVien || oldMaKhoaHoc != newMaKhoaHoc))
            {
                TempData["Error"] = "Bản ghi đăng ký đích đã tồn tại.";
                return RedirectToAction(nameof(RegisterStudent));
            }

            if (oldMaHocVien == newMaHocVien && oldMaKhoaHoc == newMaKhoaHoc)
            {
                TempData["Success"] = "Đã cập nhật đăng ký.";
                return RedirectToAction(nameof(RegisterStudent));
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
            TempData["Success"] = "Đã cập nhật đăng ký.";
            return RedirectToAction(nameof(RegisterStudent));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRegistration(string maHocVien, string maKhoaHoc)
        {
            if (string.IsNullOrWhiteSpace(maHocVien) || string.IsNullOrWhiteSpace(maKhoaHoc))
            {
                return RedirectToAction(nameof(RegisterStudent));
            }

            var existing = await _db.DangKis.FindAsync(maKhoaHoc, maHocVien);
            if (existing != null)
            {
                _db.DangKis.Remove(existing);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xóa đăng ký.";
            }

            return RedirectToAction(nameof(RegisterStudent));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudentToClassSubmit(string MaLop, string MaHocVien)
        {
            if (string.IsNullOrWhiteSpace(MaLop) || string.IsNullOrWhiteSpace(MaHocVien))
            {
                return RedirectToAction(nameof(ClassManagement));
            }

            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == MaLop);
            var sv = await _db.HocViens.FindAsync(MaHocVien);
            if (lop != null && sv != null && !lop.MaHocViens.Contains(sv))
            {
                lop.MaHocViens.Add(sv);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã thêm học viên vào lớp.";
            }
            return RedirectToAction(nameof(ClassDetail), new { id = MaLop });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudentFromClass(string maLop, string maHocVien)
        {
            if (string.IsNullOrWhiteSpace(maLop) || string.IsNullOrWhiteSpace(maHocVien))
            {
                return RedirectToAction(nameof(ClassManagement));
            }

            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .FirstOrDefaultAsync(l => l.MaLop == maLop);
            var sv = lop?.MaHocViens.FirstOrDefault(h => h.MaHocVien == maHocVien);

            if (sv != null)
            {
                lop!.MaHocViens.Remove(sv);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xóa học viên khỏi lớp.";
            }

            return RedirectToAction(nameof(ClassDetail), new { id = maLop });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStudentClass(string oldMaLop, string newMaLop, string maHocVien)
        {
            if (string.IsNullOrWhiteSpace(oldMaLop) || string.IsNullOrWhiteSpace(newMaLop) || string.IsNullOrWhiteSpace(maHocVien))
            {
                TempData["Error"] = "Thông tin chuyển lớp không hợp lệ.";
                return RedirectToAction(nameof(ClassDetail), new { id = oldMaLop });
            }

            if (oldMaLop == newMaLop)
            {
                TempData["Success"] = "Học viên đã ở lớp được chọn.";
                return RedirectToAction(nameof(ClassDetail), new { id = oldMaLop });
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
                return RedirectToAction(nameof(ClassDetail), new { id = oldMaLop });
            }

            var student = oldClass.MaHocViens.FirstOrDefault(h => h.MaHocVien == maHocVien);
            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy học viên trong lớp hiện tại.";
                return RedirectToAction(nameof(ClassDetail), new { id = oldMaLop });
            }

            if (newClass.MaHocViens.Any(h => h.MaHocVien == maHocVien))
            {
                TempData["Error"] = "Học viên đã có trong lớp đích.";
                return RedirectToAction(nameof(ClassDetail), new { id = oldMaLop });
            }

            oldClass.MaHocViens.Remove(student);
            newClass.MaHocViens.Add(student);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đã chuyển học viên sang lớp mới.";
            return RedirectToAction(nameof(ClassDetail), new { id = newMaLop });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(HocVien model)
        {
            if (string.IsNullOrWhiteSpace(model.HoVaTen))
            {
                TempData["Error"] = "Tên học viên không được để trống.";
                return RedirectToAction(nameof(StudentManagement));
            }

            model.MaHocVien = await GenerateNextHocVienIdAsync();
            model.NgayDangKi = DateOnly.FromDateTime(DateTime.Now);
            model.MaNv = GetMaNv();

            _db.HocViens.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã thêm học viên mới.";
            return RedirectToAction(nameof(StudentManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(HocVien model)
        {
            if (string.IsNullOrWhiteSpace(model.MaHocVien) || string.IsNullOrWhiteSpace(model.HoVaTen))
            {
                TempData["Error"] = "Thông tin học viên không hợp lệ.";
                return RedirectToAction(nameof(StudentManagement));
            }

            var existing = await _db.HocViens.FindAsync(model.MaHocVien);
            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy học viên để cập nhật.";
                return RedirectToAction(nameof(StudentManagement));
            }

            existing.HoVaTen = model.HoVaTen;
            existing.Sdt = model.Sdt;
            existing.GioiTinh = model.GioiTinh;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật thông tin học viên.";
            return RedirectToAction(nameof(StudentManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(string maHocVien)
        {
            if (string.IsNullOrWhiteSpace(maHocVien))
            {
                return RedirectToAction(nameof(StudentManagement));
            }

            var student = await _db.HocViens
                .Include(h => h.TaiKhoans)
                .Include(h => h.DangKis)
                .Include(h => h.KetQuas)
                .Include(h => h.MaGvs)
                .Include(h => h.MaLops)
                .FirstOrDefaultAsync(h => h.MaHocVien == maHocVien);
            
            if (student != null)
            {
                // Delete relationship with teachers (GiaoVien)
                student.MaGvs.Clear();
                
                // Delete relationship with classes (LopHoc)
                student.MaLops.Clear();
                
                // Delete accounts (foreign key constraint)
                _db.TaiKhoans.RemoveRange(student.TaiKhoans);
                
                // Delete registrations
                _db.DangKis.RemoveRange(student.DangKis);
                
                // Delete results
                _db.KetQuas.RemoveRange(student.KetQuas);
                
                // Delete the student
                _db.HocViens.Remove(student);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xóa học viên.";
            }

            return RedirectToAction(nameof(StudentManagement));
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
    }

    // ═══════════════════════════════════════════════════════════════
    //  TEACHER CONTROLLER
    // ═══════════════════════════════════════════════════════════════
    [Authorize(Roles = "GiaoVien")]
    public class TeacherController : Controller
    {
        private readonly TtanContext _db;
        public TeacherController(TtanContext db) => _db = db;

        private string? GetMaGv() => User.FindFirst("MaGV")?.Value;

        public async Task<IActionResult> Index()
        {
            var maGv = GetMaGv();
            var classes = await _db.LopHocs
                .Where(l => l.MaGv == maGv)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaHocViens)
                .ToListAsync();
            ViewBag.TotalClasses = classes.Count;
            ViewBag.TotalStudents = classes.Sum(l => l.MaHocViens.Count);
            return View();
        }

        public async Task<IActionResult> MyClasses()
        {
            var maGv = GetMaGv();
            return View(await _db.LopHocs
                .Where(l => l.MaGv == maGv)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaPhongNavigation)
                .Include(l => l.MaHocViens)
                .ToListAsync());
        }

        public async Task<IActionResult> ClassDetail(string id)
        {
            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens).ThenInclude(h => h.KetQuas)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaPhongNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id);
            return View(lop);
        }

        public async Task<IActionResult> StudentList()
        {
            var maGv = GetMaGv();
            var students = await _db.HocViens
                .Where(h => h.MaGvs.Any(g => g.MaGv == maGv))
                .Include(h => h.MaLops).ThenInclude(l => l.MaKhoaHocNavigation)
                .ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> ScoreManagement(string? maLop = null)
        {
            var maGv = GetMaGv();

            var teacherClasses = await _db.LopHocs
                .Where(l => l.MaGv == maGv)
                .Include(l => l.MaKhoaHocNavigation)
                .OrderBy(l => l.TenLop)
                .AsNoTracking()
                .ToListAsync();

            var selectedClass = teacherClasses.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(maLop))
            {
                selectedClass = teacherClasses.FirstOrDefault(l => l.MaLop == maLop) ?? selectedClass;
            }

            var vm = new TeacherScoreManagementViewModel
            {
                Classes = teacherClasses,
                SelectedClassId = selectedClass?.MaLop
            };

            if (selectedClass == null)
            {
                return View(vm);
            }

            var studentRows = await _db.HocViens
                .Where(h => h.MaLops.Any(l => l.MaLop == selectedClass.MaLop))
                .Select(h => new TeacherScoreRowViewModel
                {
                    MaHocVien = h.MaHocVien,
                    HoVaTen = h.HoVaTen,
                    MaKhoaHoc = selectedClass.MaKhoaHoc,
                    TenKhoaHoc = selectedClass.MaKhoaHocNavigation != null ? selectedClass.MaKhoaHocNavigation.TenKhoaHoc : selectedClass.MaKhoaHoc,
                    DiemListening = h.KetQuas.Where(k => k.MaKhoaHoc == selectedClass.MaKhoaHoc).Select(k => k.DiemListening).FirstOrDefault(),
                    DiemReading = h.KetQuas.Where(k => k.MaKhoaHoc == selectedClass.MaKhoaHoc).Select(k => k.DiemReading).FirstOrDefault(),
                    DiemTong = h.KetQuas.Where(k => k.MaKhoaHoc == selectedClass.MaKhoaHoc).Select(k => k.DiemTong).FirstOrDefault(),
                    HasScore = h.KetQuas.Any(k => k.MaKhoaHoc == selectedClass.MaKhoaHoc)
                })
                .OrderBy(x => x.HoVaTen)
                .ToListAsync();

            vm.Rows = studentRows;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddScore(string maHocVien, string maKhoaHoc, int diemListening, int diemReading, string? maLop)
        {
            var maGv = GetMaGv();

            if (string.IsNullOrWhiteSpace(maHocVien) || string.IsNullOrWhiteSpace(maKhoaHoc))
            {
                TempData["Error"] = "Thiếu thông tin học viên hoặc khóa học.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            if (diemListening < 0 || diemListening > 495 || diemReading < 0 || diemReading > 495)
            {
                TempData["Error"] = "Điểm hợp lệ trong khoảng 0-495 cho từng kỹ năng Listening/Reading.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            var isTeacherOfStudentCourse = await _db.LopHocs.AnyAsync(l =>
                l.MaGv == maGv &&
                l.MaKhoaHoc == maKhoaHoc &&
                l.MaHocViens.Any(h => h.MaHocVien == maHocVien));

            if (!isTeacherOfStudentCourse)
            {
                TempData["Error"] = "Bạn không có quyền nhập điểm cho học viên/khóa học này.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            var existing = await _db.KetQuas.FindAsync(maHocVien, maKhoaHoc);
            if (existing != null)
            {
                TempData["Error"] = "Bản ghi điểm đã tồn tại. Vui lòng dùng tab Sửa điểm.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            existing = new KetQua
            {
                MaHocVien = maHocVien,
                MaKhoaHoc = maKhoaHoc,
                DiemListening = diemListening,
                DiemReading = diemReading,
                DiemTong = diemListening + diemReading
            };

            _db.KetQuas.Add(existing);

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã nhập điểm cho học viên.";
            return RedirectToAction(nameof(ScoreManagement), new { maLop });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScore(string maHocVien, string maKhoaHoc, int diemListening, int diemReading, string? maLop)
        {
            var maGv = GetMaGv();

            if (string.IsNullOrWhiteSpace(maHocVien) || string.IsNullOrWhiteSpace(maKhoaHoc))
            {
                TempData["Error"] = "Thiếu thông tin học viên hoặc khóa học.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            if (diemListening < 0 || diemListening > 495 || diemReading < 0 || diemReading > 495)
            {
                TempData["Error"] = "Điểm hợp lệ trong khoảng 0-495 cho từng kỹ năng Listening/Reading.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            var isTeacherOfStudentCourse = await _db.LopHocs.AnyAsync(l =>
                l.MaGv == maGv &&
                l.MaKhoaHoc == maKhoaHoc &&
                l.MaHocViens.Any(h => h.MaHocVien == maHocVien));

            if (!isTeacherOfStudentCourse)
            {
                TempData["Error"] = "Bạn không có quyền sửa điểm cho học viên/khóa học này.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            var existing = await _db.KetQuas.FindAsync(maHocVien, maKhoaHoc);
            if (existing == null)
            {
                TempData["Error"] = "Chưa có bản ghi điểm. Vui lòng dùng tab Nhập điểm.";
                return RedirectToAction(nameof(ScoreManagement), new { maLop });
            }

            existing.DiemListening = diemListening;
            existing.DiemReading = diemReading;
            existing.DiemTong = diemListening + diemReading;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật điểm cho học viên.";
            return RedirectToAction(nameof(ScoreManagement), new { maLop });
        }

        public async Task<IActionResult> Schedule()
        {
            var maGv = GetMaGv();
            var sessions = await _db.LichHocs
                .Where(lh => lh.MaLopNavigation != null && lh.MaLopNavigation.MaGv == maGv)
                .Include(lh => lh.MaLopNavigation)
                    .ThenInclude(l => l!.MaKhoaHocNavigation)
                .Include(lh => lh.MaPhongNavigation)
                .OrderBy(lh => lh.NgayHoc)
                .ThenBy(lh => lh.GioBatDau)
                .ToListAsync();
            return View(sessions);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  STUDENT CONTROLLER
    // ═══════════════════════════════════════════════════════════════
    [Authorize(Roles = "HocVien")]
    public class StudentController : Controller
    {
        private readonly TtanContext _db;
        public StudentController(TtanContext db) => _db = db;

        private string? GetMaHocVien() => User.FindFirst("MaHocVien")?.Value;

        public async Task<IActionResult> Index()
        {
            var maHv = GetMaHocVien();
            ViewBag.MaHocVien = maHv;
            return View();
        }

        public async Task<IActionResult> MyCourses()
        {
            var maHv = GetMaHocVien();
            return View(await _db.DangKis
                .Where(d => d.MaHocVien == maHv)
                .Include(d => d.MaKhoaHocNavigation)
                .ToListAsync());
        }

        public async Task<IActionResult> MyClasses()
        {
            var maHv = GetMaHocVien();
            var hv = await _db.HocViens
                .Include(h => h.MaLops).ThenInclude(l => l.MaGvNavigation)
                .Include(h => h.MaLops).ThenInclude(l => l.MaKhoaHocNavigation)
                .Include(h => h.MaLops).ThenInclude(l => l.MaPhongNavigation)
                .FirstOrDefaultAsync(h => h.MaHocVien == maHv);
            return View(hv?.MaLops ?? new List<LopHoc>());
        }

        public async Task<IActionResult> Schedule()
        {
            var maHv = GetMaHocVien();
            var sessions = await _db.LichHocs
                .Where(lh => lh.MaLopNavigation != null && lh.MaLopNavigation.MaHocViens.Any(h => h.MaHocVien == maHv))
                .Include(lh => lh.MaLopNavigation)
                    .ThenInclude(l => l!.MaGvNavigation)
                .Include(lh => lh.MaLopNavigation)
                    .ThenInclude(l => l!.MaKhoaHocNavigation)
                .Include(lh => lh.MaPhongNavigation)
                .OrderBy(lh => lh.NgayHoc)
                .ThenBy(lh => lh.GioBatDau)
                .ToListAsync();
            return View(sessions);
        }

        public async Task<IActionResult> Results()
        {
            var maHv = GetMaHocVien();
            return View(await _db.KetQuas
                .Where(k => k.MaHocVien == maHv)
                .Include(k => k.MaKhoaHocNavigation)
                .ToListAsync());
        }

        public async Task<IActionResult> ExportResults()
        {
            var maHv = GetMaHocVien();
            if (string.IsNullOrWhiteSpace(maHv))
            {
                return RedirectToAction("Results");
            }

            var results = await _db.KetQuas
                .Where(k => k.MaHocVien == maHv)
                .Include(k => k.MaKhoaHocNavigation)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("CourseCode,CourseName,Listening,Reading,Total");
            foreach (var r in results)
            {
                var courseCode = r.MaKhoaHoc ?? string.Empty;
                var courseName = (r.MaKhoaHocNavigation?.TenKhoaHoc ?? string.Empty).Replace(",", " ");
                var listening = r.DiemListening?.ToString() ?? string.Empty;
                var reading = r.DiemReading?.ToString() ?? string.Empty;
                var total = r.DiemTong?.ToString() ?? string.Empty;
                csv.AppendLine($"{courseCode},{courseName},{listening},{reading},{total}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"student-results-{maHv}.csv";
            return File(bytes, "text/csv", fileName);
        }
    }
}
