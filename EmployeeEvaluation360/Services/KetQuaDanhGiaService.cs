using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeEvaluation360.Services
{
	public class KetQuaDanhGiaService : IKetQuaDanhGiaService
	{
		private readonly ApplicationDBContext _context;
		public KetQuaDanhGiaService(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<List<KetQua_DanhGiaDto>> getKetQuaDanhGiaByMaNguoiDung(string maNguoiDung)
		{
			if (maNguoiDung.IsNullOrEmpty())
			{
				return new List<KetQua_DanhGiaDto>();
			}
			var user = await _context.NGUOIDUNG.FindAsync(maNguoiDung);

			if (user == null)
			{
				return new List<KetQua_DanhGiaDto>();
			}
			var listKetQuaDanhGia = _context.KETQUA_DANHGIA
					.Include(qkdg => qkdg.DotDanhGia)
					.Where(x => x.MaNguoiDung == maNguoiDung)
					.AsQueryable();

			if (!listKetQuaDanhGia.Any())
			{
				return new List<KetQua_DanhGiaDto>();
			}
			
			var result = await listKetQuaDanhGia.OrderByDescending(x => x.ThoiGianTinh)
				.Select(x => new KetQua_DanhGiaDto
				{
					MaNguoiDung = x.MaNguoiDung,
					DiemTongKet = x.DiemTongKet,
					ThoiGianTinh =x.ThoiGianTinh,
					MaDotDanhGia = x.MaDotDanhGia,
					TenDotDanhGia = x.DotDanhGia.TenDot ?? string.Empty
				}).ToListAsync();

			return  result;
		}
			

		private async Task<int> GetLatestMaDotDanhGia()
		{
			// Tìm DotDanhGia gần nhất dựa trên ThoiGianKetThuc
			var latestDotDanhGia = await _context.DOT_DANHGIA
				.Where(dd => dd.TrangThai == "Inactive")
				.OrderByDescending(dd => dd.ThoiGianKetThuc)
				.Select(dd => dd.MaDotDanhGia)
				.FirstOrDefaultAsync();

			return latestDotDanhGia;
		}

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetLatestPaged(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0) // Không tìm thấy DotDanhGia phù hợp
			{
				return new PagedResult<KetQua_DanhGiaChiTietDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<KetQua_DanhGiaChiTietDto>()
				};
			}

			// Truy vấn KETQUA_DANHGIA cho MaDotDanhGia đã chọn
			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.Where(x => x.KetQua.MaDotDanhGia == selectedMaDotDanhGia)
				.AsQueryable();

			// Áp dụng tìm kiếm theo HoTen nếu có
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => EF.Functions.Like(x.NguoiDung.HoTen, $"%{search}%"));
			}

			var totalRecords = await query.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			page = page > totalPages && totalPages > 0 ? totalPages : page;

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return new PagedResult<KetQua_DanhGiaChiTietDto>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalCount = totalRecords,
				TotalPages = totalPages,
				Items = results
			};
		}

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetLatest(int? maDotDanhGia = null)
		{
			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0)
			{
				return new List<KetQua_DanhGiaChiTietDto>();
			}

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.Where(x => x.KetQua.MaDotDanhGia == selectedMaDotDanhGia)
				.AsQueryable();

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return results;
		}

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetGoodCurrentPaged(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0)
			{
				return new PagedResult<KetQua_DanhGiaChiTietDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<KetQua_DanhGiaChiTietDto>()
				};
			}

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.Where(x => x.KetQua.MaDotDanhGia == selectedMaDotDanhGia)
				.Where(x => x.KetQua.DiemTongKet >= 90)
				.AsQueryable();

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => EF.Functions.Like(x.NguoiDung.HoTen, $"%{search}%"));
			}

			var totalRecords = await query.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			page = page > totalPages && totalPages > 0 ? totalPages : page;

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return new PagedResult<KetQua_DanhGiaChiTietDto>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalCount = totalRecords,
				TotalPages = totalPages,
				Items = results
			};
		}

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetGoodCurrent(int? maDotDanhGia = null)
		{
			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0)
			{
				return new List<KetQua_DanhGiaChiTietDto>();
			}

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.Where(x => x.KetQua.MaDotDanhGia == selectedMaDotDanhGia)
				.Where(x => x.KetQua.DiemTongKet >= 90)
				.AsQueryable();

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return results;
		}

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetBadCurrentPaged(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0)
			{
				return new PagedResult<KetQua_DanhGiaChiTietDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<KetQua_DanhGiaChiTietDto>()
				};
			}

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.Where(x => x.KetQua.MaDotDanhGia == selectedMaDotDanhGia)
				.Where(x => x.KetQua.DiemTongKet <= 40)
				.AsQueryable();

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => EF.Functions.Like(x.NguoiDung.HoTen, $"%{search}%"));
			}

			var totalRecords = await query.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			page = page > totalPages && totalPages > 0 ? totalPages : page;

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return new PagedResult<KetQua_DanhGiaChiTietDto>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalCount = totalRecords,
				TotalPages = totalPages,
				Items = results
			};
		}

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetBadCurrent(int? maDotDanhGia = null)
		{
			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0)
			{
				return new List<KetQua_DanhGiaChiTietDto>();
			}

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.Where(x => x.KetQua.MaDotDanhGia == selectedMaDotDanhGia)
				.Where(x => x.KetQua.DiemTongKet <= 40)
				.AsQueryable();

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return results;
		}

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetAllPaged(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.AsQueryable();

			if (maDotDanhGia.HasValue)
			{
				query = query.Where(x => x.KetQua.MaDotDanhGia == maDotDanhGia.Value);
			}

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => EF.Functions.Like(x.NguoiDung.HoTen, $"%{search}%"));
			}

			var totalRecords = await query.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			page = page > totalPages && totalPages > 0 ? totalPages : page;

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return new PagedResult<KetQua_DanhGiaChiTietDto>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalCount = totalRecords,
				TotalPages = totalPages,
				Items = results
			};
		}

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetAll(int? maDotDanhGia = null)
		{
			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.AsQueryable();

			if (maDotDanhGia.HasValue)
			{
				query = query.Where(x => x.KetQua.MaDotDanhGia == maDotDanhGia.Value);
			}

			var results = await query
				.OrderByDescending(x => x.KetQua.ThoiGianTinh)
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia != null ? x.KetQua.DotDanhGia.TenDot ?? string.Empty : string.Empty
				})
				.ToListAsync();

			return results;
		}
	}
}