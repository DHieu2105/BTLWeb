using System.ComponentModel.DataAnnotations;
using BTL_Web.Models;
using BTL_Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers.Api;

[ApiController]
[Route("api/courses")]
[Authorize(Roles = "Admin,NhanVien")]
public class CoursesApiController : ControllerBase
{
    private readonly TtanContext _db;

    public CoursesApiController(TtanContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        (page, pageSize) = ApiQueryHelpers.NormalizePaging(page, pageSize);

        var query = _db.KhoaHocs
            .Include(k => k.MaTrungTamNavigation)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var search = $"%{keyword.Trim()}%";
            query = query.Where(k =>
                EF.Functions.Like(k.MaKhoaHoc, search) ||
                (k.TenKhoaHoc != null && EF.Functions.Like(k.TenKhoaHoc, search)));
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(k => k.MaKhoaHoc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(k => new CourseResponse(
                k.MaKhoaHoc,
                k.TenKhoaHoc,
                k.ThoiLuong,
                k.HocPhi,
                k.MaTrungTamNavigation != null ? k.MaTrungTamNavigation.TenTrungTam : null))
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
        var item = await _db.KhoaHocs
            .Include(k => k.MaTrungTamNavigation)
            .AsNoTracking()
            .Where(k => k.MaKhoaHoc == id)
            .Select(k => new CourseResponse(
                k.MaKhoaHoc,
                k.TenKhoaHoc,
                k.ThoiLuong,
                k.HocPhi,
                k.MaTrungTamNavigation != null ? k.MaTrungTamNavigation.TenTrungTam : null))
            .FirstOrDefaultAsync();

        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CourseUpsertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var course = new KhoaHoc
        {
            MaKhoaHoc = await EntityIdGenerator.GenerateNextIdAsync(
                _db.KhoaHocs.AsNoTracking().Select(k => k.MaKhoaHoc),
                candidate => _db.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == candidate),
                "KH"),
            TenKhoaHoc = request.TenKhoaHoc.Trim(),
            ThoiLuong = request.ThoiLuong,
            HocPhi = request.HocPhi,
            MaTrungTam = request.MaTrungTam?.Trim()
        };

        _db.KhoaHocs.Add(course);
        await _db.SaveChangesAsync();

        var trungTam = course.MaTrungTam != null
            ? await _db.TrungTams.FirstOrDefaultAsync(t => t.MaTrungTam == course.MaTrungTam)
            : null;

        return CreatedAtAction(nameof(GetById), new { id = course.MaKhoaHoc }, new CourseResponse(
            course.MaKhoaHoc,
            course.TenKhoaHoc,
            course.ThoiLuong,
            course.HocPhi,
            trungTam?.TenTrungTam));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CourseUpsertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var course = await _db.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        if (course == null)
        {
            return NotFound();
        }

        course.TenKhoaHoc = request.TenKhoaHoc.Trim();
        course.ThoiLuong = request.ThoiLuong;
        course.HocPhi = request.HocPhi;
        course.MaTrungTam = request.MaTrungTam?.Trim();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var course = await _db.KhoaHocs.FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        if (course == null)
        {
            return NotFound();
        }

        _db.KhoaHocs.Remove(course);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    public record CourseResponse(string MaKhoaHoc, string? TenKhoaHoc, int? ThoiLuong, int? HocPhi, string? CenterName);

    public class CourseUpsertRequest
    {
        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(200)]
        public string TenKhoaHoc { get; set; } = string.Empty;

        public int? ThoiLuong { get; set; }

        public int? HocPhi { get; set; }

        [StringLength(50)]
        public string? MaTrungTam { get; set; }
    }
}
