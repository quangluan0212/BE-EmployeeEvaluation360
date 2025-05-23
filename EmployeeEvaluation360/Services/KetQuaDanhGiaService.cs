﻿using EmployeeEvaluation360.Database;
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
		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetCurrent(int page = 1, int pageSize = 10, string? search = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.KETQUA_DANHGIA
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
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh
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

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetGoodCurrent(int page = 1,int pageSize = 10,string? search = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.KETQUA_DANHGIA
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

			query = query.Where(x => x.KetQua.DiemTongKet > 80);

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
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh
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

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetBadCurrent(int page = 1,int pageSize = 10,string? search = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.KETQUA_DANHGIA
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

			query = query.Where(x => x.KetQua.DiemTongKet < 40);

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
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh
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

		public async Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetAll(int page = 1,int pageSize = 10,string? search = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.KETQUA_DANHGIA
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
					MaNguoiDung = x.KetQua.MaNguoiDung,
					HoTen = x.NguoiDung.HoTen ?? string.Empty,
					DiemTongKet = x.KetQua.DiemTongKet,
					ThoiGianTinh = x.KetQua.ThoiGianTinh
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
	}
}
