using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class DuAnService : IDuAnService
	{
		private readonly ApplicationDBContext _context;
		public DuAnService(ApplicationDBContext context) 
		{
			_context = context;
		}

		public Task<bool> DeleteDuAnAsync(int maDuAn)
		{
			throw new NotImplementedException();
		}

		public async Task<DuAn> GetDuAnByIdAsync(int maDuAn)
		{
			return await _context.DUAN.FindAsync(maDuAn);
		}

		public async Task<PagedResult<DuAnDto>> GetAllDuAnPagedAsync(int page, int pageSize)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;
			var query = _context.DUAN.AsQueryable();
			var totalCount = query.Count();
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			var duan = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var items = duan.Select(x => new DuAnDto
			{
				MaDuAn = x.MaDuAn,
				TenDuAn = x.TenDuAn,
				MoTa = x.MoTa,
				TrangThai = x.TrangThai,
			}).ToList();


			return new PagedResult<DuAnDto>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount,
				Items = items,
			};
		}

		public async Task<DuAn> ThemDuAn(CreateDuAnDto createDuAnDto)
		{
			if (createDuAnDto == null)
			{
				throw new ArgumentNullException(nameof(createDuAnDto));
			}
			var result = await _context.DUAN.AddAsync(createDuAnDto.ToEntity());
			if (result == null)
			{
				throw new Exception("Thêm dự án không thành công");
			}
			return createDuAnDto.ToEntity();
		}

		public async Task<DuAn> UpdateDuAnAsync(int maDuAn, UpdateDuAnDto updateDto)
		{
			if (updateDto == null)
			{
				throw new ArgumentNullException(nameof(updateDto));
			}
			var duAn = await _context.DUAN.FindAsync(maDuAn);
			if (duAn == null)
			{
				throw new Exception("Dự án không tồn tại");
			}
			duAn.TenDuAn = updateDto.TenDuAn;
			duAn.MoTa = updateDto.MoTa;
			duAn.TrangThai = updateDto.TrangThai;

			try
			{
				_context.DUAN.Update(duAn);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception("Cập nhật dự án không thành công", ex);
			}

			return duAn;
		}
	}
}
