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
		public DbSet<NguoiDung> NGUOIDUNG { get; set; }
		public DbSet<NguoiDung_ChuvVu> NGUOIDUNG_CHUCVU { get; set; }
		public DbSet<MauDanhGia> MAUDANHGIA { get; set; }
		public DbSet<DotDanhGia> DOT_DANHGIA { get; set; }
		public DbSet<ChiTiet_DotDanhGia> CHITIET_DOTDANHGIA { get; set; }
		public DbSet<DanhGia_CauHoi> DANHGIA_CAUHOI { get; set; }
		public DbSet<CauuHoi> CAUHOI { get; set; }
		public DbSet<ChucVu> CHUCVU { get; set; }
		public DbSet<KetQua_DanhGia> KETQUA_DANHGIA { get; set; }
		public DbSet<Nhom> NHOM { get; set; }
		public DbSet<Nhom_NguoiDung> NHOM_NGUOIDUNG { get; set; }
		public DbSet<DuAn> DUAN { set; get; }
	}
}
