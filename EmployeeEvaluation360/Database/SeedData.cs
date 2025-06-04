using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using EmployeeEvaluation360.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Database
{
	public class SeedData
	{
		private readonly ApplicationDBContext _context;

		public SeedData(ApplicationDBContext context)
		{
			_context = context;
		}

		public void EnsureAdminUser()
		{
			var adminUser = _context.NGUOIDUNG.FirstOrDefault();
			if (adminUser == null)
			{
				string password = "abc123@@";
				string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 10).ToString();

				var user = new NguoiDung
				{
					MaNguoiDung = GenerateMaNguoiDung(DateTime.Now),
					HoTen = "Lê Quang Luân",
					Email = "quangluan0212@gmail.com",
					DienThoai = "0334586712",
					MatKhau = hashedPassword,
					NgayVaoCongTy = DateTime.Now,
					TrangThai = "Active"
				};

				_context.NGUOIDUNG.Add(user);				
				_context.SaveChanges();

				var adminRole = new NguoiDung_ChucVu
				{
					MaNguoiDung = user.MaNguoiDung,
					MaChucVu = 1,
					CapBac = 0,
					TrangThai = "Active"
				};
				_context.NGUOIDUNG_CHUCVU.Add(adminRole);
				_context.SaveChanges();
			}
		}
		private string GenerateMaNguoiDung(DateTime ngayVaoCongTy)
		{
			string year = ngayVaoCongTy.Year.ToString();
			string prefix = $"{year}NV000";
			string MaNguoiDung = $"{prefix}{"1"}";
			return MaNguoiDung;
		}
	}
}
