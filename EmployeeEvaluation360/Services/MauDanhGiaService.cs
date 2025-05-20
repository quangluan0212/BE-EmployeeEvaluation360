using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class MauDanhGiaService : IMauDanhGiaService
	{
		private readonly ApplicationDBContext _context;
		public MauDanhGiaService(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<List<MauDanhGiaDto>> GetMauDanhGiaByMaDotDanhGia(int maDotDanhGia)
		{
			var listMauDanhGia = await _context.CHITIET_DOTDANHGIA
				.Include(x => x.MauDanhGia)
				.Where(x => x.MaDotDanhGia == maDotDanhGia)
				.ToListAsync();
			if (listMauDanhGia == null || !listMauDanhGia.Any())
			{
				return null;
			}


			var listMauDanhGiaDto = listMauDanhGia.Select(x => new MauDanhGiaDto
			{
				MaMauDanhGia = x.MaMau,
				TenMauDanhGia = x.MauDanhGia.TenMau,
				LoaiDanhGia = x.MauDanhGia.LoaiDanhGia,
				TrangThai = x.MauDanhGia.TrangThai
			}).ToList();

			return listMauDanhGiaDto;

		}

		public async Task<List<MauDanhGiaDto>> GetAllMauDanhGiaActive()
		{
			var mauDanhGias = await _context.MAUDANHGIA.Where(x => x.TrangThai == "Active").ToListAsync();
			return mauDanhGias.Select(x => new MauDanhGiaDto
			{
				MaMauDanhGia = x.MaMau,
				TenMauDanhGia = x.TenMau,
				LoaiDanhGia = x.LoaiDanhGia,
				TrangThai = x.TrangThai
			}).ToList();
		}
		public async Task<PagedResult<MauDanhGiaDto>> GetAllAsync(int page, int pageSize, string? s)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.MAUDANHGIA.AsQueryable();

			if (!string.IsNullOrEmpty(s))
			{
				query = query.Where(x => x.TenMau.Contains(s));
			}

			var totalRecords = await query.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			var mauDanhGias = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var mauDanhGiaDtos = mauDanhGias.Select(x => new MauDanhGiaDto
			{
				MaMauDanhGia = x.MaMau,
				TenMauDanhGia = x.TenMau,
				LoaiDanhGia = x.LoaiDanhGia,
				TrangThai = x.TrangThai
			}).ToList();

			return new PagedResult<MauDanhGiaDto>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalRecords,
				Items = mauDanhGiaDtos
			};
		}

		public async Task<MauDanhGiaDto> GetMauDanhGiaById(int maMau)
		{
			var mauDanhGia = await _context.MAUDANHGIA.FindAsync(maMau);
			if (mauDanhGia == null)
			{
				return null;
			}
			return new MauDanhGiaDto
			{
				MaMauDanhGia = mauDanhGia.MaMau,
				TenMauDanhGia = mauDanhGia.TenMau,
				TrangThai = mauDanhGia.TrangThai
			};
		}

		public async Task<CreateMauDanhGiaDto> CreateMauDanhGia (CreateMauDanhGiaDto createMauDanhGiaDto)
		{
			var mauDanhGia = new MauDanhGia
			{
				TenMau = createMauDanhGiaDto.TenMau,
				TrangThai = "Active",
				LoaiDanhGia = createMauDanhGiaDto.LoaiDanhGia
			};
			await _context.MAUDANHGIA.AddAsync(mauDanhGia);
			await _context.SaveChangesAsync();
			foreach (var cauHoi in createMauDanhGiaDto.DanhSachCauHoi)
			{
				var mauDanhGiaCauHoi = new CauHoi
				{
					MaMau = mauDanhGia.MaMau,
					NoiDung = cauHoi.NoiDung,
					DiemToiDa = cauHoi.DiemToiDa,
				};
				await _context.CAUHOI.AddAsync(mauDanhGiaCauHoi);
			}
			await _context.SaveChangesAsync();

			return createMauDanhGiaDto;
		}
	}
	
}
