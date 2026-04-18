using System.ComponentModel.DataAnnotations;
using BTL_Web.Models;
using BTL_Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers.Api;

[ApiController]
[Route("api/teachers")]
[Authorize(Roles = "Admin,NhanVien")]
public class TeachersApiController : ControllerBase
{
    private readonly TtanContext _db;

    public TeachersApiController(TtanContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        (page, pageSize) = ApiQueryHelpers.NormalizePaging(page, pageSize);

        var query = _db.GiaoViens
            .Include(g => g.MaKhoaHocNavigation)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var search = $"%{keyword.Trim()}%";
            query = query.Where(g =>
                EF.Functions.Like(g.MaGv, search) ||
                (g.Ten != null && EF.Functions.Like(g.Ten, search)) ||
                (g.ChuyenMon != null && EF.Functions.Like(g.ChuyenMon, search)));
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(g => g.MaGv)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new TeacherResponse(
                g.MaGv,
                g.Ten,
                g.Sdt,
                g.ChuyenMon,
                g.GioiTinh,
                g.MaKhoaHocNavigation != null ? g.MaKhoaHocNavigation.TenKhoaHoc : null))
            .ToListAsync();

        return Ok(new
        {
            page,
            pageSize,
            totalItems,
            totalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            items
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _db.GiaoViens
            .Include(g => g.MaKhoaHocNavigation)
            .AsNoTracking()
            .Where(g => g.MaGv == id)
            .Select(g => new TeacherResponse(g.MaGv, g.Ten, g.Sdt, g.ChuyenMon, g.GioiTinh, g.MaKhoaHocNavigation != null ? g.MaKhoaHocNavigation.TenKhoaHoc : null))
            .FirstOrDefaultAsync();

        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TeacherUpsertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var teacher = new GiaoVien
        {
            MaGv = await EntityIdGenerator.GenerateNextIdAsync(
                _db.GiaoViens.AsNoTracking().Select(g => g.MaGv),
                candidate => _db.GiaoViens.AnyAsync(g => g.MaGv == candidate),
                "GV"),
            Ten = request.Ten.Trim(),
            Sdt = request.Sdt?.Trim(),
            ChuyenMon = request.ChuyenMon?.Trim(),
            GioiTinh = request.GioiTinh?.Trim(),
            MaKhoaHoc = request.MaKhoaHoc?.Trim()
        };

        _db.GiaoViens.Add(teacher);
        await _db.SaveChangesAsync();

        var khoaHoc = teacher.MaKhoaHoc != null 
            ? await _db.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == teacher.MaKhoaHoc)
            : null;

        return CreatedAtAction(nameof(GetById), new { id = teacher.MaGv }, new TeacherResponse(
            teacher.MaGv,
            teacher.Ten,
            teacher.Sdt,
            teacher.ChuyenMon,
            teacher.GioiTinh,
            khoaHoc?.TenKhoaHoc));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TeacherUpsertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var teacher = await _db.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == id);
        if (teacher == null)
        {
            return NotFound();
        }

        teacher.Ten = request.Ten.Trim();
        teacher.Sdt = request.Sdt?.Trim();
        teacher.ChuyenMon = request.ChuyenMon?.Trim();
        teacher.GioiTinh = request.GioiTinh?.Trim();
        teacher.MaKhoaHoc = request.MaKhoaHoc?.Trim();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var teacher = await _db.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == id);
        if (teacher == null)
        {
            return NotFound();
        }

        _db.GiaoViens.Remove(teacher);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    public record TeacherResponse(string MaGv, string? Ten, string? Sdt, string? ChuyenMon, string? GioiTinh, string? CourseName);

    public class TeacherUpsertRequest
    {
        [Required(ErrorMessage = "Tên giáo viên là bắt buộc")]
        [StringLength(100)]
        public string Ten { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Sdt { get; set; }

        [StringLength(100)]
        public string? ChuyenMon { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        [StringLength(50)]
        public string? MaKhoaHoc { get; set; }
    }
}
