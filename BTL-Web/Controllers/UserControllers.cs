using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

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

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _db.HocViens.CountAsync();
            ViewBag.TotalClasses = await _db.LopHocs.CountAsync();
            return View();
        }

        public async Task<IActionResult> StudentManagement()
            => View(await _db.HocViens.Include(h => h.DangKis).ThenInclude(d => d.MaKhoaHocNavigation).ToListAsync());

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
            ViewBag.LopHocs = await _db.LopHocs.ToListAsync();
            ViewBag.PhongHocs = await _db.PhongHocs.ToListAsync();
            return View(await _db.LichHocs.Include(l => l.MaPhongNavigation).ToListAsync());
        }

        public async Task<IActionResult> ClassDetail(string id)
        {
            var lop = await _db.LopHocs
                .Include(l => l.MaHocViens)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaPhongNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id);
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSchedule(LichHoc m)
        {
            m.MaLichHoc = "LH" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.LichHocs.Add(m); await _db.SaveChangesAsync();
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
            return RedirectToAction("StudentManagement");
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentToClassSubmit(string MaLop, string MaHocVien)
        {
            var lop = await _db.LopHocs.Include(l => l.MaHocViens).FirstOrDefaultAsync(l => l.MaLop == MaLop);
            var sv = await _db.HocViens.FindAsync(MaHocVien);
            if (lop != null && sv != null && !lop.MaHocViens.Contains(sv))
            {
                lop.MaHocViens.Add(sv);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("ClassManagement");
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

        public async Task<IActionResult> Schedule()
        {
            var maGv = GetMaGv();
            var lops = await _db.LopHocs
                .Where(l => l.MaGv == maGv)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaPhongNavigation)
                .ToListAsync();
            return View(lops);
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
            var hv = await _db.HocViens
                .Include(h => h.MaLops).ThenInclude(l => l.MaPhongNavigation)
                .Include(h => h.MaLops).ThenInclude(l => l.MaGvNavigation)
                .Include(h => h.MaLops).ThenInclude(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(h => h.MaHocVien == maHv);
            return View(hv?.MaLops ?? new List<LopHoc>());
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
