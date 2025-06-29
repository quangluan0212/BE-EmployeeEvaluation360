﻿using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
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

		public async Task<int> GetCapBacChucVu(string maNguoiDung, int maChucVu)
		{
			if (string.IsNullOrEmpty(maNguoiDung) || maChucVu <= 0)
			{
				return 0; // Invalid input
			}
			var nguoiDungChucVu = await _context.NGUOIDUNG_CHUCVU
				.FirstOrDefaultAsync(nc => nc.MaNguoiDung == maNguoiDung && nc.MaChucVu == maChucVu && nc.TrangThai == "Active");
			return nguoiDungChucVu?.CapBac ?? 0;
		}

		public async Task<CapNhatChucVuDto> CapNhatChucVuChoNguoiDung(CapNhatChucVuDto capNhatChucVu)
		{
			if (capNhatChucVu == null || string.IsNullOrEmpty(capNhatChucVu.MaNguoiDung) || capNhatChucVu.MaChucVu <= 0)
			{
				throw new ArgumentException("Invalid input data");
			}

			// tim kiếm người dùng theo mã
			var nguoiDung = await _context.NGUOIDUNG
				.Include(nd => nd.NguoiDungChucVus)
				.FirstOrDefaultAsync(nd => nd.MaNguoiDung == capNhatChucVu.MaNguoiDung);

			if (nguoiDung == null)
			{
				throw new Exception("User not found");
			}

			// tim kiếm chức vụ theo mã
			var currentActiveChucVu = nguoiDung.NguoiDungChucVus
				.FirstOrDefault(cv => cv.TrangThai == "Active");

			// kiem tra xem chức vụ hiện tại có tồn tại không
			if (currentActiveChucVu != null && currentActiveChucVu.MaChucVu == capNhatChucVu.MaChucVu)
			{
				// cap nhật cấp bậc của chức vụ hiện tại
				currentActiveChucVu.CapBac = capNhatChucVu.CapBac;
			}
			else
			{
				// chuyển trạng thái chức vụ hiện tại sang không hoạt động
				if (currentActiveChucVu != null)
				{
					currentActiveChucVu.TrangThai = "Inactive";
				}

				// tam thêm chức vụ mới
				var newChucVu = new NguoiDung_ChucVu
				{
					MaNguoiDung = capNhatChucVu.MaNguoiDung,
					MaChucVu = capNhatChucVu.MaChucVu,
					CapBac = 0,
					TrangThai = "Active"
				};

				_context.NGUOIDUNG_CHUCVU.Add(newChucVu);
			}

			// Save changes to database
			await _context.SaveChangesAsync();

			return capNhatChucVu;
		}



		public async Task<PagedResult<ChucVuDto>> GetAllChucVuPagedAsync(int page, int pageSize, string? s)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;
			var query = _context.CHUCVU.AsQueryable();

			if (!string.IsNullOrEmpty(s))
			{
				query = query.Where(c => c.TenChucVu.Contains(s));
			}

			var totalCount = query.Count();
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			var chucVuList = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var chucVuDtoList = chucVuList.Select(c => new ChucVuDto
			{
				maChucVu = c.MaChucVu,
				tenChucVu = c.TenChucVu,
				trangThai = c.TrangThai,
			}).ToList();

			return new PagedResult<ChucVuDto>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount,
				Items = chucVuDtoList,
			};
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
			if (string.IsNullOrEmpty(chucVu.TenChucVu))
				return null;
			if (await _context.CHUCVU.AnyAsync(c => c.TenChucVu == chucVu.TenChucVu))
				return null;
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
