using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class ChucVuService : IChucVuService
	{
		private readonly ApplicationDBContext _context;

		public ChucVuService(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ChucVu>> GetAllChucVuAsync()
		{
			return await _context.CHUCVU
				.OrderBy(c => c.TenChucVu)
				.ToListAsync();
		}

		public async Task<ChucVu> GetChucVuByIdAsync(int maChucVu)
		{
			return await _context.CHUCVU
				.FirstOrDefaultAsync(c => c.MaChucVu == maChucVu);
		}

		public async Task<IEnumerable<ChucVu>> GetActiveChucVuAsync()
		{
			return await _context.CHUCVU
				.Where(c => c.TrangThai == "Active")
				.OrderBy(c => c.TenChucVu)
				.ToListAsync();
		}

		public async Task<ChucVu> CreateChucVuAsync(ChucVu chucVu)
		{
			_context.CHUCVU.Add(chucVu);
			await _context.SaveChangesAsync();
			return chucVu;
		}

		public async Task<ChucVu> UpdateChucVuAsync(int maChucVu, ChucVu chucVu)
		{
			if (maChucVu != chucVu.MaChucVu)
				return null;

			var existingChucVu = await _context.CHUCVU.FindAsync(maChucVu);
			if (existingChucVu == null)
				return null;

			existingChucVu.TenChucVu = chucVu.TenChucVu;
			existingChucVu.TrangThai = chucVu.TrangThai;

			try
			{
				await _context.SaveChangesAsync();
				return existingChucVu;
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await ChucVuExistsAsync(maChucVu))
					return null;
				throw;
			}
		}

		public async Task<bool> DeleteChucVuAsync(int maChucVu)
		{
			var chucVu = await _context.CHUCVU.FindAsync(maChucVu);
			if (chucVu == null)
				return false;

			// Kiểm tra xem có người dùng nào đang sử dụng chức vụ này không
			bool hasRelatedNguoiDung = await _context.NGUOIDUNG_CHUCVU
				.AnyAsync(nc => nc.MaChucVu == maChucVu && nc.TrangThai == "Active");

			if (hasRelatedNguoiDung)
				return false;

			_context.CHUCVU.Remove(chucVu);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> SetStatusAsync(int maChucVu, string trangThai)
		{
			var chucVu = await _context.CHUCVU.FindAsync(maChucVu);
			if (chucVu == null)
				return false;

			chucVu.TrangThai = trangThai;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<NguoiDung>> GetNguoiDungByChucVuAsync(int maChucVu)
		{
			return await _context.NGUOIDUNG_CHUCVU
				.Where(nc => nc.MaChucVu == maChucVu)
				.Select(nc => nc.NguoiDung)
				.Distinct()
				.ToListAsync();
		}

		public async Task<bool> ChucVuExistsAsync(int maChucVu)
		{
			return await _context.CHUCVU.AnyAsync(c => c.MaChucVu == maChucVu);
		}
	}
}
