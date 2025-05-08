using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Database
{
	public class ApplicationDBContext : DbContext
	{
		public ApplicationDBContext() : base()
		{
		}
		public ApplicationDBContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChiTiet_DotDanhGia>(entity =>
			{
				entity.HasKey(e => new { e.MaDotDanhGia, e.MaMau });
			});

			modelBuilder.Entity<NguoiDung_ChucVu>(entity =>
			{
				entity.HasKey(e => new { e.MaNguoiDung, e.MaChucVu });
			});

			modelBuilder.Entity<DanhGia_CauHoi>(entity =>
			{
				entity.HasKey(e => new { e.MaDanhGia, e.MaCauHoi });
			});

			modelBuilder.Entity<Nhom>(entity =>
			{
				entity.HasOne(n => n.DuAn)
					.WithMany(d => d.Nhoms)
					.HasForeignKey(n => n.MaDuAn)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<Nhom_NguoiDung>(entity =>
			{
				entity.HasOne(nn => nn.Nhom)
					.WithMany(n => n.NhomNguoiDungs)
					.HasForeignKey(nn => nn.MaNhom)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(nn => nn.NguoiDung)
					.WithMany(nd => nd.NhomNguoiDungs)
					.HasForeignKey(nn => nn.MaNguoiDung)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<DanhGia>(entity =>
			{
				entity.HasOne(dg => dg.NguoiDanhGiaObj)
					.WithMany(nd => nd.DanhGias)
					.HasForeignKey(dg => dg.NguoiDanhGia)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(dg => dg.NguoiDuocDanhGiaObj)
					.WithMany(nn => nn.DanhGias)
					.HasForeignKey(dg => dg.NguoiDuocDanhGia)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(dg => dg.DotDanhGia)
					.WithMany(dd => dd.DanhGias)
					.HasForeignKey(dg => dg.MaDotDanhGia)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<CauHoi>(entity =>
			{
				entity.HasOne(ch => ch.MauDanhGia)
					.WithMany(md => md.CauHois)
					.HasForeignKey(ch => ch.MaMau)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<KetQua_DanhGia>(entity =>
			{
				entity.HasOne(kq => kq.NguoiDung)
					.WithMany(nd => nd.KetQuaDanhGias)
					.HasForeignKey(kq => kq.MaNguoiDung)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<DanhGia_CauHoi>(entity =>
			{
				entity.HasOne(dgc => dgc.DanhGia)
					.WithMany(dg => dg.DanhGiaCauHois)
					.HasForeignKey(dgc => dgc.MaDanhGia)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(dgc => dgc.CauHoi)
					.WithMany(ch => ch.DanhGiaCauHois)
					.HasForeignKey(dgc => dgc.MaCauHoi)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<ChucVu>(entity =>
			{
				entity.HasMany(c => c.NguoiDungChucVus)
					.WithOne(nc => nc.ChucVu)
					.HasForeignKey(nc => nc.MaChucVu)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<DotDanhGia>(entity =>
			{
				entity.HasMany(dd => dd.ChiTietDotDanhGias)
					.WithOne(ct => ct.DotDanhGia)
					.HasForeignKey(ct => ct.MaDotDanhGia)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasMany(dd => dd.DanhGias)
					.WithOne(dg => dg.DotDanhGia)
					.HasForeignKey(dg => dg.MaDotDanhGia)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<DuAn>(entity =>
			{
				entity.HasMany(d => d.Nhoms)
					.WithOne(n => n.DuAn)
					.HasForeignKey(n => n.MaDuAn)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<MauDanhGia>(entity =>
			{
				entity.HasMany(md => md.ChiTietDotDanhGias)
					.WithOne(ct => ct.MauDanhGia)
					.HasForeignKey(ct => ct.MaMau)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasMany(md => md.CauHois)
					.WithOne(ch => ch.MauDanhGia)
					.HasForeignKey(ch => ch.MaMau)
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<NguoiDung>()
				.HasIndex(nd => nd.Email)
				.IsUnique();
		}

		public DbSet<NguoiDung> NGUOIDUNG { get; set; }
		public DbSet<NguoiDung_ChucVu> NGUOIDUNG_CHUCVU { get; set; }
		public DbSet<DanhGia> DANHGIA { get; set; }
		public DbSet<MauDanhGia> MAUDANHGIA { get; set; }
		public DbSet<DotDanhGia> DOT_DANHGIA { get; set; }
		public DbSet<ChiTiet_DotDanhGia> CHITIET_DOTDANHGIA { get; set; }
		public DbSet<DanhGia_CauHoi> DANHGIA_CAUHOI { get; set; }
		public DbSet<CauHoi> CAUHOI { get; set; }
		public DbSet<ChucVu> CHUCVU { get; set; }
		public DbSet<KetQua_DanhGia> KETQUA_DANHGIA { get; set; }
		public DbSet<Nhom> NHOM { get; set; }
		public DbSet<Nhom_NguoiDung> NHOM_NGUOIDUNG { get; set; }
		public DbSet<DuAn> DUAN { set; get; }
	}
}
