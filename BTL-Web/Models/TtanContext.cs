using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Models;

public partial class TtanContext : DbContext
{
    public TtanContext()
    {
    }

    public TtanContext(DbContextOptions<TtanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DangKi> DangKis { get; set; }

    public virtual DbSet<GiaoVien> GiaoViens { get; set; }

    public virtual DbSet<HocVien> HocViens { get; set; }

    public virtual DbSet<KetQua> KetQuas { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<LichHoc> LichHocs { get; set; }

    public virtual DbSet<LopHoc> LopHocs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhongHoc> PhongHocs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThietBi> ThietBis { get; set; }

    public virtual DbSet<TrungTam> TrungTams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-9CKDHNV;Initial Catalog=TTAN;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DangKi>(entity =>
        {
            entity.HasKey(e => new { e.MaKhoaHoc, e.MaHocVien }).HasName("PK__DangKi__FE754F7ECEF4B953");

            entity.ToTable("DangKi");

            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaHocVien)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.DangKis)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKi__MaHocVie__656C112C");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.DangKis)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKi__MaKhoaHo__6477ECF3");
        });

        modelBuilder.Entity<GiaoVien>(entity =>
        {
            entity.HasKey(e => e.MaGv).HasName("PK__GiaoVien__2725AEF309AD95F4");

            entity.ToTable("GiaoVien");

            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.ChuyenMon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaTrungTam)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Ten)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.GiaoViens)
                .HasForeignKey(d => d.MaKhoaHoc)
                .HasConstraintName("FK__GiaoVien__MaKhoa__5BE2A6F2");

            entity.HasOne(d => d.MaTrungTamNavigation).WithMany(p => p.GiaoViens)
                .HasForeignKey(d => d.MaTrungTam)
                .HasConstraintName("FK__GiaoVien__MaTrun__5CD6CB2B");

            entity.HasMany(d => d.MaHocViens).WithMany(p => p.MaGvs)
                .UsingEntity<Dictionary<string, object>>(
                    "GiangDay",
                    r => r.HasOne<HocVien>().WithMany()
                        .HasForeignKey("MaHocVien")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GiangDay__MaHocV__6D0D32F4"),
                    l => l.HasOne<GiaoVien>().WithMany()
                        .HasForeignKey("MaGv")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GiangDay__MaGV__6C190EBB"),
                    j =>
                    {
                        j.HasKey("MaGv", "MaHocVien").HasName("PK__GiangDay__91A01E15EB37CBA4");
                        j.ToTable("GiangDay");
                        j.IndexerProperty<string>("MaGv")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("MaGV");
                        j.IndexerProperty<string>("MaHocVien")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<HocVien>(entity =>
        {
            entity.HasKey(e => e.MaHocVien).HasName("PK__HocVien__685B0E6A251E8644");

            entity.ToTable("HocVien");

            entity.Property(e => e.MaHocVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.HoVaTen)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HocViens)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__HocVien__MaNV__5812160E");
        });

        modelBuilder.Entity<KetQua>(entity =>
        {
            entity.HasKey(e => new { e.MaHocVien, e.MaKhoaHoc }).HasName("PK__KetQua__FCD401939F591DE8");

            entity.ToTable("KetQua");

            entity.Property(e => e.MaHocVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DiemTong).HasComputedColumnSql("([DiemListening]+[DiemReading])", false);

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.KetQuas)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KetQua__MaHocVie__7C4F7684");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.KetQuas)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KetQua__MaKhoaHo__7D439ABD");
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaKhoaHoc).HasName("PK__KhoaHoc__48F0FF9833D36681");

            entity.ToTable("KhoaHoc");

            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaTrungTam)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenKhoaHoc)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTrungTamNavigation).WithMany(p => p.KhoaHocs)
                .HasForeignKey(d => d.MaTrungTam)
                .HasConstraintName("FK__KhoaHoc__MaTrung__5441852A");
        });

        modelBuilder.Entity<LichHoc>(entity =>
        {
            entity.HasKey(e => e.MaLichHoc).HasName("PK__LichHoc__150EBC21B5236A5A");

            entity.ToTable("LichHoc");

            entity.Property(e => e.MaLichHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaPhong)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.LichHocs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__LichHoc__MaPhong__778AC167");
        });

        modelBuilder.Entity<LopHoc>(entity =>
        {
            entity.HasKey(e => e.MaLop).HasName("PK__LopHoc__3B98D273EBD5909A");

            entity.ToTable("LopHoc");

            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaPhong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenLop)
                .HasMaxLength(40)
                .IsUnicode(false);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK__LopHoc__MaGV__5FB337D6");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.MaKhoaHoc)
                .HasConstraintName("FK__LopHoc__MaKhoaHo__619B8048");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__LopHoc__MaPhong__60A75C0F");

            entity.HasMany(d => d.MaHocViens).WithMany(p => p.MaLops)
                .UsingEntity<Dictionary<string, object>>(
                    "ThamGium",
                    r => r.HasOne<HocVien>().WithMany()
                        .HasForeignKey("MaHocVien")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ThamGia__MaHocVi__70DDC3D8"),
                    l => l.HasOne<LopHoc>().WithMany()
                        .HasForeignKey("MaLop")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ThamGia__MaLop__6FE99F9F"),
                    j =>
                    {
                        j.HasKey("MaLop", "MaHocVien").HasName("PK__ThamGia__8D1D6295254B26F4");
                        j.ToTable("ThamGia");
                        j.IndexerProperty<string>("MaLop")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("MaHocVien")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70ACF98D531");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.ChucVu)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.HoVaTen)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaTrungTam)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTrungTamNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaTrungTam)
                .HasConstraintName("FK__NhanVien__MaTrun__5165187F");

            entity.HasMany(d => d.MaGvs).WithMany(p => p.MaNvs)
                .UsingEntity<Dictionary<string, object>>(
                    "PhanCong",
                    r => r.HasOne<GiaoVien>().WithMany()
                        .HasForeignKey("MaGv")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PhanCong__MaGV__693CA210"),
                    l => l.HasOne<NhanVien>().WithMany()
                        .HasForeignKey("MaNv")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PhanCong__MaNV__68487DD7"),
                    j =>
                    {
                        j.HasKey("MaNv", "MaGv").HasName("PK__PhanCong__05578DE5B8948E66");
                        j.ToTable("PhanCong");
                        j.IndexerProperty<string>("MaNv")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("MaNV");
                        j.IndexerProperty<string>("MaGv")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("MaGV");
                    });
        });

        modelBuilder.Entity<PhongHoc>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongHoc__20BD5E5B310B184E");

            entity.ToTable("PhongHoc");

            entity.Property(e => e.MaPhong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaTrungTam)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenPhong)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTrungTamNavigation).WithMany(p => p.PhongHocs)
                .HasForeignKey(d => d.MaTrungTam)
                .HasConstraintName("FK__PhongHoc__MaTrun__4BAC3F29");

            entity.HasMany(d => d.MaThietBis).WithMany(p => p.MaPhongs)
                .UsingEntity<Dictionary<string, object>>(
                    "SuDung",
                    r => r.HasOne<ThietBi>().WithMany()
                        .HasForeignKey("MaThietBi")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SuDung__MaThietB__74AE54BC"),
                    l => l.HasOne<PhongHoc>().WithMany()
                        .HasForeignKey("MaPhong")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SuDung__MaPhong__73BA3083"),
                    j =>
                    {
                        j.HasKey("MaPhong", "MaThietBi").HasName("PK__SuDung__48139944B9A9350A");
                        j.ToTable("SuDung");
                        j.IndexerProperty<string>("MaPhong")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("MaThietBi")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__TaiKhoan__536C85E5449E6CA0");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaHocVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK__TaiKhoan__MaGV__01142BA1");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaHocVien)
                .HasConstraintName("FK__TaiKhoan__MaHocV__02084FDA");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__TaiKhoan__MaNV__00200768");
        });

        modelBuilder.Entity<ThietBi>(entity =>
        {
            entity.HasKey(e => e.MaThietBi).HasName("PK__ThietBi__8AEC71F7C398EEFC");

            entity.ToTable("ThietBi");

            entity.Property(e => e.MaThietBi)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenThietBi)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TrungTam>(entity =>
        {
            entity.HasKey(e => e.MaTrungTam).HasName("PK__TrungTam__54A2B84F543220C8");

            entity.ToTable("TrungTam");

            entity.Property(e => e.MaTrungTam)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DiaChi)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenTrungTam)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
