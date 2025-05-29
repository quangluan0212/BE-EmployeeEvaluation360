using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class KetQuaDanhGiaService : IKetQuaDanhGiaService
	{
		private readonly ApplicationDBContext _context;
		public KetQuaDanhGiaService(ApplicationDBContext context)
		{
			_context = context;
		}
		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetLatestPaged(int page = 1, int pageSize = 10, string? search = null)
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

			var latestResults = _context.KETQUA_DANHGIA
				.Include(x => x.NguoiDung)
				.GroupBy(x => x.MaNguoiDung)
				.Select(g => new { MaNguoiDung = g.Key, MaxThoiGianTinh = g.Max(x => x.ThoiGianTinh) });

			query = query
				.Where(x => latestResults
					.Any(lr => lr.MaNguoiDung == x.KetQua.MaNguoiDung && lr.MaxThoiGianTinh == x.KetQua.ThoiGianTinh));

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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty

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

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetLatest()
		{
			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.AsQueryable();

			var latestResults = _context.KETQUA_DANHGIA
				.Include(x => x.NguoiDung)
				.GroupBy(x => x.MaNguoiDung)
				.Select(g => new { MaNguoiDung = g.Key, MaxThoiGianTinh = g.Max(x => x.ThoiGianTinh) });

			query = query
				.Where(x => latestResults
					.Any(lr => lr.MaNguoiDung == x.KetQua.MaNguoiDung && lr.MaxThoiGianTinh == x.KetQua.ThoiGianTinh));

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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty

				})
				.ToListAsync();

			return results;
		}


		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetGoodCurrentPaged(int page = 1,int pageSize = 10,string? search = null)
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

			var latestResults = _context.KETQUA_DANHGIA
				.GroupBy(x => x.MaNguoiDung)
				.Select(g => new { MaNguoiDung = g.Key, MaxThoiGianTinh = g.Max(x => x.ThoiGianTinh) });

			query = query
				.Where(x => latestResults
					.Any(lr => lr.MaNguoiDung == x.KetQua.MaNguoiDung && lr.MaxThoiGianTinh == x.KetQua.ThoiGianTinh));

			query = query.Where(x => x.KetQua.DiemTongKet >= 90);

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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty
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

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetGoodCurrent()
		{

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.AsQueryable();

			var latestResults = _context.KETQUA_DANHGIA
				.GroupBy(x => x.MaNguoiDung)
				.Select(g => new { MaNguoiDung = g.Key, MaxThoiGianTinh = g.Max(x => x.ThoiGianTinh) });

			query = query
				.Where(x => latestResults
					.Any(lr => lr.MaNguoiDung == x.KetQua.MaNguoiDung && lr.MaxThoiGianTinh == x.KetQua.ThoiGianTinh));

			query = query.Where(x => x.KetQua.DiemTongKet >= 90);		

		
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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty
				})
				.ToListAsync();

			return results;
		}


		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetBadCurrentPaged(int page = 1,int pageSize = 10,string? search = null)
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

			var latestResults = _context.KETQUA_DANHGIA
				.GroupBy(x => x.MaNguoiDung)
				.Select(g => new { MaNguoiDung = g.Key, MaxThoiGianTinh = g.Max(x => x.ThoiGianTinh) });

			query = query
				.Where(x => latestResults
					.Any(lr => lr.MaNguoiDung == x.KetQua.MaNguoiDung && lr.MaxThoiGianTinh == x.KetQua.ThoiGianTinh));

			query = query.Where(x => x.KetQua.DiemTongKet <= 40);

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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty
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
		public async Task<List<KetQua_DanhGiaChiTietDto>> GetBadCurrent()
		{
			

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.AsQueryable();

			var latestResults = _context.KETQUA_DANHGIA
				.GroupBy(x => x.MaNguoiDung)
				.Select(g => new { MaNguoiDung = g.Key, MaxThoiGianTinh = g.Max(x => x.ThoiGianTinh) });

			query = query
				.Where(x => latestResults
					.Any(lr => lr.MaNguoiDung == x.KetQua.MaNguoiDung && lr.MaxThoiGianTinh == x.KetQua.ThoiGianTinh));

			query = query.Where(x => x.KetQua.DiemTongKet <= 40);


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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty
				})
				.ToListAsync();

			return results;
		}


		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetAllPaged(int page = 1,int pageSize = 10,string? search = null)
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
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty
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

		public async Task<List<KetQua_DanhGiaChiTietDto>> GetAll()
		{

			var query = _context.KETQUA_DANHGIA
				.Include(kq => kq.DotDanhGia)
				.Join(_context.NGUOIDUNG,
					kq => kq.MaNguoiDung,
					nd => nd.MaNguoiDung,
					(kq, nd) => new { KetQua = kq, NguoiDung = nd })
				.AsQueryable();


			var results = await query
				.Select(x => new KetQua_DanhGiaChiTietDto
				{
					MaKetQuaDanhGia = x.KetQua.MaKetQua,
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh,
					MaDotDanhGia = x.KetQua.MaDotDanhGia,
					TenDotDanhGia = x.KetQua.DotDanhGia.TenDot ?? string.Empty
				})
				.ToListAsync();

			return results;
		}
	}
}
