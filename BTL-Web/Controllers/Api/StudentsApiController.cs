using System.ComponentModel.DataAnnotations;
using BTL_Web.Models;
using BTL_Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers.Api;

[ApiController]
[Route("api/students")]
[Authorize(Roles = "Admin,NhanVien")]
public class StudentsApiController : ControllerBase
{
    private readonly TtanContext _db;

    public StudentsApiController(TtanContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        (page, pageSize) = ApiQueryHelpers.NormalizePaging(page, pageSize);

        var query = _db.HocViens
            .Include(h => h.DangKis)
                .ThenInclude(d => d.MaKhoaHocNavigation)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var search = $"%{keyword.Trim()}%";
            query = query.Where(h =>
                EF.Functions.Like(h.MaHocVien, search) ||
                (h.HoVaTen != null && EF.Functions.Like(h.HoVaTen, search)));
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(h => h.MaHocVien)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new StudentResponse(
                h.MaHocVien,
                h.HoVaTen,
                h.Sdt,
                h.GioiTinh,
                h.DangKis
                    .OrderBy(d => d.NgayDangKi)
                    .Select(d => d.MaKhoaHocNavigation != null ? d.MaKhoaHocNavigation.TenKhoaHoc : null)
                    .FirstOrDefault()))
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
        var item = await _db.HocViens
            .AsNoTracking()
            .Where(h => h.MaHocVien == id)
            .Select(h => new StudentResponse(h.MaHocVien, h.HoVaTen, h.Sdt, h.GioiTinh, null))
            .FirstOrDefaultAsync();

        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentUpsertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var student = new HocVien
        {
            MaHocVien = await EntityIdGenerator.GenerateNextIdAsync(
                _db.HocViens.AsNoTracking().Select(h => h.MaHocVien),
                candidate => _db.HocViens.AnyAsync(h => h.MaHocVien == candidate),
                "HV"),
            HoVaTen = request.HoVaTen.Trim(),
            Sdt = request.Sdt?.Trim(),
            GioiTinh = request.GioiTinh?.Trim(),
            NgayDangKi = DateOnly.FromDateTime(DateTime.Now)
        };

        _db.HocViens.Add(student);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = student.MaHocVien }, new StudentResponse(
            student.MaHocVien,
            student.HoVaTen,
            student.Sdt,
            student.GioiTinh,
            null));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] StudentUpsertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var student = await _db.HocViens.FirstOrDefaultAsync(h => h.MaHocVien == id);
        if (student == null)
        {
            return NotFound();
        }

        student.HoVaTen = request.HoVaTen.Trim();
        student.Sdt = request.Sdt?.Trim();
        student.GioiTinh = request.GioiTinh?.Trim();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var student = await _db.HocViens.FirstOrDefaultAsync(h => h.MaHocVien == id);
        if (student == null)
        {
            return NotFound();
        }

        _db.HocViens.Remove(student);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    public record StudentResponse(string MaHocVien, string? HoVaTen, string? Sdt, string? GioiTinh, string? CourseName);

    public class StudentUpsertRequest
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100)]
        public string HoVaTen { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Sdt { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }
    }
}
