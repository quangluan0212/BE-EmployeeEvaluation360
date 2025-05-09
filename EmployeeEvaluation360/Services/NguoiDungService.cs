using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class NguoiDungService : INguoiDungService
	{
		private readonly ApplicationDBContext _context;

		public NguoiDungService(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<PagedResult<DanhSachNguoiDungDto>> GetNguoiDungWithRolePagedAsync(int page, int pageSize, string? s)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.NGUOIDUNG
				.Include(nd => nd.NguoiDungChucVus)
					.ThenInclude(ndcv => ndcv.ChucVu)
				.AsNoTracking();


			if (!string.IsNullOrEmpty(s))
			{
				string keyword = s.Trim().ToLower();
				query = query.Where(n =>
					n.HoTen.ToLower().Contains(keyword));					
			}
			query = query.OrderBy(n => n.MaNguoiDung);

			var totalCount = await query.CountAsync();
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			var users = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var dtoItems = users.Select(user => new DanhSachNguoiDungDto
			{
				MaNguoiDung = user.MaNguoiDung,
				HoTen = user.HoTen,
				Email = user.Email,
				DienThoai = user.DienThoai,
				TrangThai = user.TrangThai,
				ChucVu = user.NguoiDungChucVus
									.Where(c => c.TrangThai == "Active")
									.Select(c => c.ChucVu.TenChucVu)
									.ToList()
			}).ToList();

			return new PagedResult<DanhSachNguoiDungDto>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount,
				Items = dtoItems
			};
		}

		public async Task<NguoiDung> AdminUpdateNguoiDungAsync(string maNguoiDung,AdminUpdateNguoiDungDto updateDto)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return null;
			}
			if (nguoiDung.Email != updateDto.Email && await _context.NGUOIDUNG.AnyAsync(n => n.Email == updateDto.Email))
			{
				return null;
			}

			nguoiDung.HoTen = updateDto.HoTen;
			nguoiDung.Email = updateDto.Email;
			nguoiDung.DienThoai = updateDto.DienThoai;
			nguoiDung.MatKhau = await HashPasswordAsync(updateDto.MatKhau);

			try
			{
				await _context.SaveChangesAsync();
				return nguoiDung;
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await NguoiDungExistsAsync(maNguoiDung))
				{
					return null;
				}
				throw;
			}
		}

		public async Task<PagedResult<NguoiDung>> GetNguoiDungPagedAsync(int page, int pageSize)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.NGUOIDUNG
				.AsNoTracking()
				.OrderBy(n => n.MaNguoiDung);

			var totalCount = await query.CountAsync();
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			var items = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<NguoiDung>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount,
				Items = items
			};
		}


		public async Task<NguoiDung> GetNguoiDungByIdAsync(string maNguoiDung)
		{
			return await _context.NGUOIDUNG
			   .Include(nd => nd.NguoiDungChucVus)
				   .ThenInclude(ndcv => ndcv.ChucVu)
			   .FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
		}

		public async Task<NguoiDung> GetNguoiDungByEmailAsync(string email)
		{
			return await _context.NGUOIDUNG
				.AsNoTracking()
				.FirstOrDefaultAsync(n => n.Email == email);
		}

		public async Task<IEnumerable<NguoiDung>> GetActiveNguoiDungAsync()
		{
			return await _context.NGUOIDUNG
				.Where(n => n.TrangThai == "Active")
				.OrderBy(n => n.HoTen)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<NguoiDung> CreateNguoiDungAsync(NguoiDung nguoiDung, int ?maChucVu)
		{
			if (await EmailExistsAsync(nguoiDung.Email))
			{
				return null;
			}

			string password = nguoiDung.MatKhau;

			nguoiDung.MaNguoiDung = await GenerateMaNguoiDungAsync(nguoiDung.NgayVaoCongTy);
			nguoiDung.MatKhau = await HashPasswordAsync(password);
			nguoiDung.TrangThai = "Active";

			_context.NGUOIDUNG.Add(nguoiDung);

			await _context.SaveChangesAsync();

			if(maChucVu.HasValue)
			{
				_context.NGUOIDUNG_CHUCVU.Add(new NguoiDung_ChucVu
				{
					MaNguoiDung = nguoiDung.MaNguoiDung,
					MaChucVu = maChucVu.Value,
					CapBac = 0,
					TrangThai = "Active"
				});
				await _context.SaveChangesAsync();
			}
			return nguoiDung;
		}

		public async Task<NguoiDung> UpdateNguoiDungAsync(string maNguoiDung, UpdateNguoiDungDto updateDto)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return null;
			}

			if (nguoiDung.Email != updateDto.Email && await _context.NGUOIDUNG.AnyAsync(n => n.Email == updateDto.Email))
			{
				return null;
			}

			nguoiDung.HoTen = updateDto.HoTen;
			nguoiDung.Email = updateDto.Email;
			nguoiDung.DienThoai = updateDto.DienThoai;

			try
			{
				await _context.SaveChangesAsync();
				return nguoiDung;
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await NguoiDungExistsAsync(maNguoiDung))
				{
					return null;
				}
				throw;
			}
		}

		public async Task<bool> ChangePasswordAsync(string maNguoiDung, string currentPassword, string newPassword)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return false;
			}

			if (!await ValidatePasswordAsync(maNguoiDung, currentPassword))
			{
				return false;
			}

			nguoiDung.MatKhau = await HashPasswordAsync(newPassword);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ResetPasswordAsync(string maNguoiDung, string newPassword)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return false;
			}

			nguoiDung.MatKhau = await HashPasswordAsync(newPassword);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteNguoiDungAsync(string maNguoiDung)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return false;
			}

			// Kiểm tra xem người dùng có liên kết với các bảng khác không
			bool hasDanhGia = await _context.DANHGIA.AnyAsync(d => d.NguoiDanhGia == maNguoiDung);
			bool hasKetQuaDanhGia = await _context.KETQUA_DANHGIA.AnyAsync(k => k.MaNguoiDung == maNguoiDung);

			if (hasDanhGia || hasKetQuaDanhGia)
			{
				// Có ràng buộc dữ liệu, chỉ cập nhật trạng thái thành "Inactive"
				nguoiDung.TrangThai = "Inactive";
				await _context.SaveChangesAsync();
				return true;
			}

			// Xóa các bản ghi liên quan trong bảng NGUOIDUNG_CHUCVU
			var nguoiDungChucVus = await _context.NGUOIDUNG_CHUCVU
				.Where(nc => nc.MaNguoiDung == maNguoiDung)
				.ToListAsync();
			_context.NGUOIDUNG_CHUCVU.RemoveRange(nguoiDungChucVus);

			// Xóa các bản ghi liên quan trong bảng NHOM_NGUOIDUNG
			var nhomNguoiDungs = await _context.NHOM_NGUOIDUNG
				.Where(nn => nn.MaNguoiDung == maNguoiDung)
				.ToListAsync();
			_context.NHOM_NGUOIDUNG.RemoveRange(nhomNguoiDungs);

			// Xóa người dùng
			_context.NGUOIDUNG.Remove(nguoiDung);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> SetStatusAsync(string maNguoiDung, string trangThai)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return false;
			}

			nguoiDung.TrangThai = trangThai;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<ChucVu>> GetChucVuByNguoiDungAsync(string maNguoiDung)
		{
			return await _context.NGUOIDUNG_CHUCVU
				.Where(nc => nc.MaNguoiDung == maNguoiDung && nc.TrangThai == "Active")
				.Include(nc => nc.ChucVu)
				.Select(nc => nc.ChucVu)
				.OrderBy(c => c.TenChucVu)
				.ToListAsync();
		}

		public async Task<bool> AddChucVuAsync(string maNguoiDung, int maChucVu, int capBac)
		{
			// Kiểm tra người dùng và chức vụ tồn tại
			if (!await NguoiDungExistsAsync(maNguoiDung) || !await _context.CHUCVU.AnyAsync(c => c.MaChucVu == maChucVu))
			{
				return false;
			}

			// Kiểm tra xem đã tồn tại bản ghi này chưa
			var exists = await _context.NGUOIDUNG_CHUCVU
				.AnyAsync(nc => nc.MaNguoiDung == maNguoiDung && nc.MaChucVu == maChucVu);

			if (exists)
			{
				// Cập nhật bản ghi hiện có
				var nguoiDungChucVu = await _context.NGUOIDUNG_CHUCVU
					.FirstOrDefaultAsync(nc => nc.MaNguoiDung == maNguoiDung && nc.MaChucVu == maChucVu);

				nguoiDungChucVu.CapBac = capBac;
				nguoiDungChucVu.TrangThai = "Active";
			}
			else
			{
				// Tạo bản ghi mới
				_context.NGUOIDUNG_CHUCVU.Add(new NguoiDung_ChucVu
				{
					MaNguoiDung = maNguoiDung,
					MaChucVu = maChucVu,
					CapBac = capBac,
					TrangThai = "Active"
				});
			}

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RemoveChucVuAsync(string maNguoiDung, int maChucVu)
		{
			var nguoiDungChucVu = await _context.NGUOIDUNG_CHUCVU
				.FirstOrDefaultAsync(nc => nc.MaNguoiDung == maNguoiDung && nc.MaChucVu == maChucVu);

			if (nguoiDungChucVu == null)
			{
				return false;
			}

			nguoiDungChucVu.TrangThai = "Inactive";
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<Nhom>> GetNhomByNguoiDungAsync(string maNguoiDung)
		{
			return await _context.NHOM_NGUOIDUNG
				.Where(nn => nn.MaNguoiDung == maNguoiDung && nn.TrangThai == "Active")
				.Include(nn => nn.Nhom)
				.Select(nn => nn.Nhom)
				.Distinct()
				.OrderBy(n => n.TenNhom)
				.ToListAsync();
		}

		public async Task<bool> NguoiDungExistsAsync(string maNguoiDung)
		{
			return await _context.NGUOIDUNG.AnyAsync(n => n.MaNguoiDung == maNguoiDung);
		}

		public async Task<bool> EmailExistsAsync(string email)
		{
			return await _context.NGUOIDUNG.AnyAsync(n => n.Email == email);
		}

		public async Task<bool> ValidatePasswordAsync(string maNguoiDung, string password)
		{
			var nguoiDung = await _context.NGUOIDUNG.FindAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return false;
			}

			return BCrypt.Net.BCrypt.Verify(password, nguoiDung.MatKhau);
		}

		public async Task<string> GenerateMaNguoiDungAsync(DateTime ngayVaoCongTy)
		{
			string year = ngayVaoCongTy.Year.ToString();
			string prefix = $"{year}NV";
			int nextNumber = 1;

			var lastUser = await _context.NGUOIDUNG
				.Where(n => n.MaNguoiDung.StartsWith(prefix))
				.OrderByDescending(n => n.MaNguoiDung)
				.FirstOrDefaultAsync();

			if (lastUser != null)
			{
				string numberPart = lastUser.MaNguoiDung.Substring(prefix.Length);
				if (int.TryParse(numberPart, out int lastNumber))
				{
					nextNumber = lastNumber + 1;
				}
			}

			return $"{prefix}{nextNumber:D4}";
		}

		private async Task<string> HashPasswordAsync(string password)
		{
			return await Task.Run(() =>
			{
				return BCrypt.Net.BCrypt.HashPassword(password, 10).ToString();
			});
		}


	}
}
