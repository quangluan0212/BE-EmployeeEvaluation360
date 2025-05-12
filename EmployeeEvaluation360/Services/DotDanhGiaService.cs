using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class DotDanhGiaService : IDotDanhGiaService
	{
		private readonly ApplicationDBContext _context;

		public DotDanhGiaService(ApplicationDBContext context)
		{
			_context = context;
		}
		public async Task<DotDanhGiaDto> GetDotDanhGiaActivesAsync()
		{
			var listData = await _context.DOT_DANHGIA.Where(x=>x.TrangThai=="Active").ToListAsync();
			if (!listData.Any())
			{
				return null;
			}
			var lisdDataDto = listData.Select(x => new DotDanhGiaDto 
			{
				MaDotDanhGia = x.MaDotDanhGia,
				TenDot = x.TenDot,
				ThoiGianBatDau = x.ThoiGianBatDau,
				ThoiGianKetThuc = x.ThoiGianKetThuc,
				TrangThai = x.TrangThai,
			}).ToList();
			return lisdDataDto.FirstOrDefault();
		}
	}
}
