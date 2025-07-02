using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace EmployeeEvaluation360.Services
{
	public class DanhGiaService : IDanhGiaService
	{
		private readonly ApplicationDBContext _context;
		public DanhGiaService(ApplicationDBContext applicationDBContext)
		{
			_context = applicationDBContext;
		}

		public async Task<PagedResult<DanhGiaDto>> LeaderGetAllDanhGiaAsync(string leaderId, int page, int pageSize, string? search, int? MaNhom)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			try
			{
				// Lấy danh sách nhóm mà leader quản lý
				var leaderGroupsQuery = _context.NHOM_NGUOIDUNG
					.Where(nnd => nnd.MaNguoiDung == leaderId && nnd.VaiTro == "Leader" && nnd.TrangThai == "Active");

				if (MaNhom.HasValue)
				{
					leaderGroupsQuery = leaderGroupsQuery.Where(nnd => nnd.MaNhom == MaNhom.Value);
				}

				var leaderGroups = await leaderGroupsQuery
					.Select(nnd => nnd.MaNhom)
					.ToListAsync();

				if (!leaderGroups.Any())
				{
					return new PagedResult<DanhGiaDto>
					{
						CurrentPage = page,
						PageSize = pageSize,
						TotalCount = 0,
						TotalPages = 0,
						Items = new List<DanhGiaDto>()
					};
				}

				// Lấy danh sách MaNhomNguoiDung trong các nhóm, trừ leader
				var groupMembers = await _context.NHOM_NGUOIDUNG
					.Where(nnd => leaderGroups.Contains(nnd.MaNhom) && nnd.VaiTro != "Leader" && nnd.TrangThai == "Active" && nnd.MaNguoiDung != leaderId)
					.Select(nnd => nnd.MaNhomNguoiDung)
					.ToListAsync();

				if (!groupMembers.Any())
				{
					return new PagedResult<DanhGiaDto>
					{
						CurrentPage = page,
						PageSize = pageSize,
						TotalCount = 0,
						TotalPages = 0,
						Items = new List<DanhGiaDto>()
					};
				}

				// Truy vấn đánh giá
				var query = _context.DANHGIA
					.Where(dg => groupMembers.Contains(dg.NguoiDuocDanhGia) && dg.TrangThai == "Đã đánh giá")
					.Include(dg => dg.NguoiDanhGiaObj)
					.Include(dg => dg.NguoiDuocDanhGiaObj)
						.ThenInclude(nnd => nnd.NguoiDung)
					.Include(dg => dg.DotDanhGia).AsQueryable();

				// Tìm kiếm theo HoTen hoặc TenDot
				if (!string.IsNullOrEmpty(search))
				{
					query = query.Where(dg => (dg.NguoiDanhGiaObj != null && dg.NguoiDanhGiaObj.HoTen != null && dg.NguoiDanhGiaObj.HoTen.Contains(search)) ||
											  (dg.NguoiDuocDanhGiaObj != null && dg.NguoiDuocDanhGiaObj.NguoiDung != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen.Contains(search)) ||
											  (dg.DotDanhGia != null && dg.DotDanhGia.TenDot != null && dg.DotDanhGia.TenDot.Contains(search)));
				}

				// Phân trang
				var totalCount = await query.CountAsync();
				var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
				var danhGiaList = await query
					.OrderBy(dg => dg.NguoiDuocDanhGiaObj.NguoiDung != null ? dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen : string.Empty)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.Select(dg => new DanhGiaDto
					{
						MaDanhGia = dg.MaDanhGia,
						MaNguoiDanhGia = dg.NguoiDanhGia ?? string.Empty,
						NguoiDanhGia = dg.NguoiDanhGiaObj != null && dg.NguoiDanhGiaObj.HoTen != null ? dg.NguoiDanhGiaObj.HoTen : string.Empty,
						maNguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj != null && dg.NguoiDuocDanhGiaObj.MaNguoiDung != null ? dg.NguoiDuocDanhGiaObj.MaNguoiDung : string.Empty,
						NguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj != null && dg.NguoiDuocDanhGiaObj.NguoiDung != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen != null ? dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen : string.Empty,
						MaDotDanhGia = dg.MaDotDanhGia,
						TenDotDanhGia = dg.DotDanhGia != null && dg.DotDanhGia.TenDot != null ? dg.DotDanhGia.TenDot : string.Empty,
						Diem = dg.Diem
					})
					.ToListAsync();

				if (!danhGiaList.Any())
				{
					return new PagedResult<DanhGiaDto>
					{
						CurrentPage = page,
						PageSize = pageSize,
						TotalCount = 0,
						TotalPages = 0,
						Items = new List<DanhGiaDto>()
					};
				}

				return new PagedResult<DanhGiaDto>
				{
					Items = danhGiaList,
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = totalCount,
					TotalPages = totalPages
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy danh sách đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return new PagedResult<DanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<DanhGiaDto>()
				};
			}
		}

		public async Task<int> GetDanhGiaChuaDanhGiaAsyncByMaNguoiDungAsync(string maNguoiDung)
		{
			var currentDotDanhGia = await _context.DOT_DANHGIA
				.Where(ddg => ddg.TrangThai == "Active")
				.FirstOrDefaultAsync();
			if (currentDotDanhGia == null)
				return 0;
			var danhGiaChuaDanhGia = await _context.DANHGIA
				.Where(dg => dg.NguoiDanhGia == maNguoiDung
							 && dg.MaDotDanhGia == currentDotDanhGia.MaDotDanhGia
							 && dg.TrangThai == "Chưa đánh giá")
				.Select(dg => dg.MaDanhGia)
				.ToListAsync();
			return danhGiaChuaDanhGia.Count();
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

		public async Task<List<NguoiThieuDanhGiaDto>> GetDanhSachNguoiThieuDanhGiaAsync(int? maDotDanhGia = null)
		{
			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0) // Không tìm thấy DotDanhGia phù hợp
			{
				new List<NguoiThieuDanhGiaDto>();
			}
			try
			{
				// Truy vấn người dùng có đánh giá chưa hoàn thành
				var query = _context.DANHGIA
					.Where(dg => dg.MaDotDanhGia == selectedMaDotDanhGia
								 && dg.TrangThai == "Chưa đánh giá"
								 && dg.NguoiDanhGiaObj != null
								 && dg.NguoiDanhGiaObj.TrangThai == "Active")
					.Select(dg => new
					{
						nd = dg.NguoiDanhGiaObj!,
						chucVu = dg.NguoiDanhGiaObj.NguoiDungChucVus
							.Where(c => c.TrangThai == "Active")
							.Select(c => c.ChucVu.TenChucVu)
							.FirstOrDefault()
					})
					.GroupBy(x => new { x.nd.MaNguoiDung, x.nd.HoTen, TenChucVu = x.chucVu ?? "" })//Nhom theo MaNguoiDung, HoTen và TenChucVu
					.Select(g => new
					{
						g.Key.MaNguoiDung,
						g.Key.HoTen,
						g.Key.TenChucVu,
						SoDanhGiaConThieu = g.Count()
					});

				var results = await query
					.OrderBy(x => x.MaNguoiDung)
					.Select(x => new NguoiThieuDanhGiaDto
					{
						MaNguoiDung = x.MaNguoiDung,
						HoTen = x.HoTen ?? string.Empty,
						TenChucVu = x.TenChucVu ?? string.Empty,
						SoDanhGiaConThieu = x.SoDanhGiaConThieu
					})
					.ToListAsync();

				return results;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy danh sách người thiếu đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return new List<NguoiThieuDanhGiaDto>();
			}
		}

		public async Task<PagedResult<NguoiThieuDanhGiaDto>> GetDanhSachNguoiThieuDanhGiaPagedAsync(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			// Xác định MaDotDanhGia sẽ sử dụng
			int selectedMaDotDanhGia = maDotDanhGia ?? await GetLatestMaDotDanhGia();

			if (selectedMaDotDanhGia == 0) // Không tìm thấy DotDanhGia phù hợp
			{
				return new PagedResult<NguoiThieuDanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<NguoiThieuDanhGiaDto>()
				};
			}

			try
			{
				// Truy vấn người dùng có đánh giá chưa hoàn thành
				var query = _context.DANHGIA
					.Where(dg => dg.MaDotDanhGia == selectedMaDotDanhGia
								 && dg.TrangThai == "Chưa đánh giá"
								 && dg.NguoiDanhGiaObj != null
								 && dg.NguoiDanhGiaObj.TrangThai == "Active")
					.Select(dg => new
					{
						nd = dg.NguoiDanhGiaObj!,
						chucVu = dg.NguoiDanhGiaObj.NguoiDungChucVus
							.Where(c => c.TrangThai == "Active")
							.Select(c => c.ChucVu.TenChucVu)
							.FirstOrDefault()
					})
					.GroupBy(x => new { x.nd.MaNguoiDung, x.nd.HoTen, TenChucVu = x.chucVu ?? "" })
					.Select(g => new
					{
						g.Key.MaNguoiDung,
						g.Key.HoTen,
						g.Key.TenChucVu,
						SoDanhGiaConThieu = g.Count()
					});


				// Áp dụng tìm kiếm theo HoTen nếu có
				if (!string.IsNullOrEmpty(search))
				{
					query = query.Where(x => EF.Functions.Like(x.HoTen, $"%{search}%"));
				}

				var totalRecords = await query.CountAsync();
				var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
				page = page > totalPages && totalPages > 0 ? totalPages : page;

				var results = await query
					.OrderBy(x => x.HoTen) // Sắp xếp theo HoTen để dễ đọc
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.Select(x => new NguoiThieuDanhGiaDto
					{
						MaNguoiDung = x.MaNguoiDung,
						HoTen = x.HoTen ?? string.Empty,
						TenChucVu = x.TenChucVu ?? string.Empty,
						SoDanhGiaConThieu = x.SoDanhGiaConThieu
					})
					.ToListAsync();

				return new PagedResult<NguoiThieuDanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = totalRecords,
					TotalPages = totalPages,
					Items = results
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy danh sách người thiếu đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return new PagedResult<NguoiThieuDanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<NguoiThieuDanhGiaDto>()
				};
			}
		}

		public async Task<PagedResult<DanhGiaDto>> NhanVienGetAllDanhGiaCheoAsync(int page, int pageSize, string? search)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			try
			{
				var query = _context.DANHGIA.Where(dg => (dg.HeSo == 1 || dg.HeSo == 3) && dg.TrangThai == "Đã đánh giá").AsQueryable();

				var totalCount = await query.CountAsync();
				var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

				if (!string.IsNullOrEmpty(search))
				{
					query = query.Where(dg => (dg.NguoiDanhGiaObj != null && dg.NguoiDanhGiaObj.HoTen != null && dg.NguoiDanhGiaObj.HoTen.Contains(search)) ||
											  (dg.NguoiDuocDanhGiaObj != null && dg.NguoiDuocDanhGiaObj.NguoiDung != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen.Contains(search)));
				}

				var danhGiaList = await query
					.Include(dg => dg.NguoiDanhGiaObj)
					.Include(dg => dg.NguoiDuocDanhGiaObj)
						.ThenInclude(nddg => nddg.NguoiDung)
					.Include(dg => dg.DotDanhGia)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (!danhGiaList.Any())
				{
					return new PagedResult<DanhGiaDto>
					{
						CurrentPage = page,
						PageSize = pageSize,
						TotalCount = 0,
						TotalPages = 0,
						Items = new List<DanhGiaDto>()
					};
				}

				var listDanhGiaDto = danhGiaList.Select(dg => new DanhGiaDto
				{
					MaDanhGia = dg.MaDanhGia,
					MaNguoiDanhGia = dg.NguoiDanhGia,
					NguoiDanhGia = dg.NguoiDanhGiaObj?.HoTen ?? string.Empty,
					maNguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj?.MaNguoiDung ?? string.Empty,
					NguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj?.NguoiDung?.HoTen ?? string.Empty,
					MaDotDanhGia = dg.MaDotDanhGia,
					TenDotDanhGia = dg.DotDanhGia?.TenDot ?? string.Empty,
					Diem = dg.Diem
				}).ToList();

				return new PagedResult<DanhGiaDto>
				{
					Items = listDanhGiaDto,
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = totalCount,
					TotalPages = totalPages
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy danh sách đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return new PagedResult<DanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<DanhGiaDto>()
				};
			}
		}

		public async Task<PagedResult<DanhGiaDto>> AdminGetAllTuDanhGiaAsync(int page, int pageSize, string? search)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			try
			{
				var query = _context.DANHGIA.Where(dg => (dg.HeSo == 2 && dg.TrangThai == "Đã đánh giá")).AsQueryable();

				var totalCount = await query.CountAsync();
				var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

				if (!string.IsNullOrEmpty(search))
				{
					query = query.Where(dg => (dg.NguoiDanhGiaObj != null && dg.NguoiDanhGiaObj.HoTen != null && dg.NguoiDanhGiaObj.HoTen.Contains(search)) ||
											  (dg.NguoiDuocDanhGiaObj != null && dg.NguoiDuocDanhGiaObj.NguoiDung != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen.Contains(search)));
				}

				var danhGiaList = await query
					.Include(dg => dg.NguoiDanhGiaObj)
					.Include(dg => dg.NguoiDuocDanhGiaObj)
						.ThenInclude(nddg => nddg.NguoiDung)
					.Include(dg => dg.DotDanhGia)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (!danhGiaList.Any())
				{
					return new PagedResult<DanhGiaDto>
					{
						CurrentPage = page,
						PageSize = pageSize,
						TotalCount = 0,
						TotalPages = 0,
						Items = new List<DanhGiaDto>()
					};
				}

				var listDanhGiaDto = danhGiaList.Select(dg => new DanhGiaDto
				{
					MaDanhGia = dg.MaDanhGia,
					MaNguoiDanhGia = dg.NguoiDanhGia,
					NguoiDanhGia = dg.NguoiDanhGiaObj?.HoTen ?? string.Empty,
					maNguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj?.MaNguoiDung ?? string.Empty,
					NguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj?.NguoiDung?.HoTen ?? string.Empty,
					MaDotDanhGia = dg.MaDotDanhGia,
					TenDotDanhGia = dg.DotDanhGia?.TenDot ?? string.Empty,
					Diem = dg.Diem
				}).ToList();

				return new PagedResult<DanhGiaDto>
				{
					Items = listDanhGiaDto,
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = totalCount,
					TotalPages = totalPages
				};
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				Console.WriteLine($"Lỗi khi lấy danh sách đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return new PagedResult<DanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<DanhGiaDto>()
				};
			}
		}

		public async Task<PagedResult<DanhGiaDto>> AdminGetAllDanhGiaAsync(int page, int pageSize, string? search)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			try
			{
				var query = _context.DANHGIA.Where(dg => dg.TrangThai == "Đã đánh giá").AsQueryable();
				// Lấy tổng số lượng đánh giá
				var totalCount = await query.CountAsync();
				var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Tính tổng số trang

				if (!string.IsNullOrEmpty(search))
				{
					query = query.Where(dg => (dg.NguoiDanhGiaObj != null && dg.NguoiDanhGiaObj.HoTen != null && dg.NguoiDanhGiaObj.HoTen.Contains(search)) ||
											  (dg.NguoiDuocDanhGiaObj != null && dg.NguoiDuocDanhGiaObj.NguoiDung != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen != null && dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen.Contains(search)));
				}				

				var danhGiaList = await query
					.Include(dg => dg.NguoiDanhGiaObj)
					.Include(dg => dg.NguoiDuocDanhGiaObj)
						.ThenInclude(nddg => nddg.NguoiDung)
					.Include(dg => dg.DotDanhGia)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (!danhGiaList.Any())
				{
					return new PagedResult<DanhGiaDto>
					{
						CurrentPage = page,
						PageSize = pageSize,
						TotalCount = 0,
						TotalPages = 0,
						Items = new List<DanhGiaDto>()
					};
				}

				var listDanhGiaDto = danhGiaList.Select(dg => new DanhGiaDto
				{
					MaDanhGia = dg.MaDanhGia,
					MaNguoiDanhGia = dg.NguoiDanhGia,
					NguoiDanhGia = dg.NguoiDanhGiaObj?.HoTen ?? string.Empty,
					maNguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj?.MaNguoiDung ?? string.Empty,
					NguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj?.NguoiDung?.HoTen ?? string.Empty,
					MaDotDanhGia = dg.MaDotDanhGia,
					TenDotDanhGia = dg.DotDanhGia?.TenDot ?? string.Empty,
					Diem = dg.Diem
				}).ToList();

				return new PagedResult<DanhGiaDto>
				{
					Items = listDanhGiaDto,
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = totalCount,
					TotalPages = totalPages
				};
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				Console.WriteLine($"Lỗi khi lấy danh sách đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return new PagedResult<DanhGiaDto>
				{
					CurrentPage = page,
					PageSize = pageSize,
					TotalCount = 0,
					TotalPages = 0,
					Items = new List<DanhGiaDto>()
				};
			}
		}

		public async Task<MauDanhGiaCauHoiTraLoiDto> GetCauTraLoiTheoMaDanhGiaAsync(int maDanhGia)
		{
			var danhGia = await _context.DANHGIA_CAUHOI
				.Where(x => x.MaDanhGia == maDanhGia)
				.Include(x => x.CauHoi)
				.Select(x => new CauTraloiDto
				
				{
					MaCauHoi = x.MaCauHoi,
					NoiDung = x.CauHoi.NoiDung,
					TraLoi = x.TraLoi
				})
				.ToListAsync();

			if (danhGia == null || !danhGia.Any())
				throw new Exception("Không tìm thấy câu trả lời cho mã đánh giá.");

			return new MauDanhGiaCauHoiTraLoiDto
			{
				MaDanhGia = maDanhGia,
				DanhSachCauTraLoi = danhGia
			};
		}

		public async Task<bool> SubmitDanhGia(DanhGiaTraLoiDto giaTraLoiDto)
		{
			// Kiểm tra xem đánh giá có tồn tại không
			var danhGia = await _context.DANHGIA
				.FirstOrDefaultAsync(dg => dg.MaDanhGia == giaTraLoiDto.MaDanhGia);
			// Nếu không tìm thấy đánh giá, ném ngoại lệ
			if (danhGia == null)
				throw new InvalidOperationException("Không tìm thấy đánh giá.");
			
			if (giaTraLoiDto.CauHoiTraLoi == null || !giaTraLoiDto.CauHoiTraLoi.Any())
				throw new InvalidOperationException("Danh sách câu trả lời không được để trống.");
			// Kiểm tra xem người dùng đã đánh giá hay chưa
			var cauHoiCu = await _context.DANHGIA_CAUHOI
				.Where(ch => ch.MaDanhGia == giaTraLoiDto.MaDanhGia)
				.ToListAsync();

			if (cauHoiCu.Any())
			{
				_context.DANHGIA_CAUHOI.RemoveRange(cauHoiCu);
			}
			// Thêm các câu trả lời mới vào cơ sở dữ liệu
			var danhSachTraLoi = giaTraLoiDto.CauHoiTraLoi.Select(cauTraLoi => new DanhGia_CauHoi
			{
				MaDanhGia = giaTraLoiDto.MaDanhGia,
				MaCauHoi = cauTraLoi.MaCauHoi,
				TraLoi = cauTraLoi.TraLoi
			}).ToList();

			await _context.DANHGIA_CAUHOI.AddRangeAsync(danhSachTraLoi);

			// Cập nhật trạng thái đánh giá và tính điểm
			int tongDiem = danhSachTraLoi.Sum(t => t.TraLoi);
			danhGia.Diem = tongDiem;
			danhGia.TrangThai = "Đã đánh giá";
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<AdminGetDanhGiaDto>> AdminGetListDanhGiaAsync(string maNguoiDung)
		{
			// Kiểm tra đợt đánh giá hiện tại
			var currentDotDanhGia = await _context.DOT_DANHGIA
				.Where(ddg => ddg.TrangThai == "Active")
				.FirstOrDefaultAsync();
			if (currentDotDanhGia == null)
				throw new InvalidOperationException("Không tìm thấy đợt đánh giá hiện tại.");
			// Lấy danh sách đánh giá của người dùng trong đợt đánh giá hiện tại
			var danhGias = await _context.DANHGIA
				.Where(dg => dg.NguoiDanhGia == maNguoiDung && dg.MaDotDanhGia == currentDotDanhGia.MaDotDanhGia)
				.Include(dg => dg.NguoiDanhGiaObj)
				.Include(dg => dg.NguoiDuocDanhGiaObj)
					.ThenInclude(nd => nd.NguoiDung)
				.ToListAsync();
			// Chọn dữ liệu cần thiết và chuyển đổi sang DTO
			var result = danhGias.Select(dg => new AdminGetDanhGiaDto
			{
				MaDanhGia = dg.MaDanhGia,
				MaNguoiDanhGia = dg.NguoiDanhGiaObj.MaNguoiDung,
				MaNhomNguoiDung = dg.NguoiDuocDanhGia,
				HotTen = dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen,
				TrangThai = dg.TrangThai
			}).ToList();

			return result;
		}

		public async Task<List<NhanVienGetDanhGiaDto>> NhanVienGetDanhGiaAsync(string maNguoiDung)
		{
			var currentDotDanhGia = await _context.DOT_DANHGIA
			.Where(ddg => ddg.TrangThai == "Active")
			.FirstOrDefaultAsync();
			if (currentDotDanhGia == null)
				throw new InvalidOperationException("Không tìm thấy đợt đánh giá hiện tại.");

			var danhGias = await _context.DANHGIA
				.Where(dg => dg.NguoiDanhGia == maNguoiDung && dg.MaDotDanhGia == currentDotDanhGia.MaDotDanhGia)
				.Include(dg => dg.NguoiDuocDanhGiaObj)
					.ThenInclude(nd => nd.NguoiDung)
				.Include(dg => dg.NguoiDuocDanhGiaObj)
					.ThenInclude(nd => nd.Nhom)
				.ToListAsync();

			var grouped = danhGias
				.GroupBy(dg => dg.NguoiDuocDanhGiaObj.Nhom.TenNhom)
				.Select(group => new NhanVienGetDanhGiaDto
				{
					TenNhom = group.Key,
					thanhViens = group.Select(dg => new ThanhVienDanhGiaCheoDto
					{
						MaDanhGia = dg.MaDanhGia,
						MaNguoiDuocDanhGia = dg.NguoiDuocDanhGiaObj.MaNguoiDung,
						HoTen = dg.NguoiDuocDanhGiaObj.NguoiDung.HoTen,
						TrangThai = dg.TrangThai,
						TenChucVu = _context.NGUOIDUNG_CHUCVU
							.Where(x => x.MaNguoiDung == dg.NguoiDuocDanhGiaObj.MaNguoiDung && x.TrangThai == "Active")
							.OrderByDescending(x => x.MaChucVu)
							.Select(x => x.ChucVu.TenChucVu)
							.FirstOrDefault() ?? "Chưa rõ",		
					}).ToList()
				}).ToList();

			return grouped;
		}


		// Lấy mẫu đánh giá câu hỏi theo mã đánh giá
		public async Task<MauDanhGiaCauHoiDto> GetFormDanhGiaCauHoiAsync(int maDanhGia)
		{
			// Kiểm trả xem mã đánh giá có hợp lệ không
			var danhGia = await _context.DANHGIA
				.FirstOrDefaultAsync(d => d.MaDanhGia == maDanhGia);

			if (danhGia == null)
				throw new InvalidOperationException("Không tìm thấy đánh giá.");

			string nguoiDanhGia = danhGia.NguoiDanhGia;
			int maNguoiDuocDanhGia = danhGia.NguoiDuocDanhGia;
			int maDotDanhGia = danhGia.MaDotDanhGia;

			int maNhomNguoiDung = await _context.NHOM_NGUOIDUNG
				.Where(nd => nd.MaNhomNguoiDung == maNguoiDuocDanhGia)
				.Select(nd => nd.MaNhomNguoiDung)
				.FirstOrDefaultAsync();

			if (maNhomNguoiDung == 0)
				throw new InvalidOperationException("Không tìm thấy nhóm người dùng của người được đánh giá.");

			string loaiDanhGia = await XacDinhLoaiDanhGiaAsync(nguoiDanhGia, maNhomNguoiDung);

			var chiTiet = await _context.CHITIET_DOTDANHGIA
				.Include(ct => ct.DotDanhGia)
				.FirstOrDefaultAsync(ct =>
					ct.MaDotDanhGia == maDotDanhGia &&
					ct.LoaiNguoiDuocDanhGia == loaiDanhGia);

			if (chiTiet == null)
				throw new InvalidOperationException("Không tìm thấy mẫu đánh giá phù hợp.");

			int maMau = chiTiet.MaMau;
			string tenDot = chiTiet.DotDanhGia.TenDot;

			//Lấy danh sách câu hỏi theo mẫu
			var cauHoiList = await _context.CAUHOI
				.Where(c => c.MaMau == maMau)
				.Select(c => new CauHoiDto
				{
					MaCauHoi = c.MaCauHoi,
					NoiDung = c.NoiDung,
					DiemToiDa = c.DiemToiDa
				})
				.ToListAsync();

			//Trả về DTO
			return new MauDanhGiaCauHoiDto
			{
				MaDanhGia = maDanhGia,
				MaMauDanhGia = maMau,
				TenDotDanhGia = tenDot,
				DanhSachCauHoi = cauHoiList
			};
		}

		public async Task<string> XacDinhLoaiDanhGiaAsync(string nguoiDanhGia, int maNhomNguoiDung)
		{
			// Lấy người được đánh giá từ nhóm người dùng
			var nguoiDuocDanhGia = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNhomNguoiDung == maNhomNguoiDung)
				.Select(x => x.MaNguoiDung)
				.FirstOrDefaultAsync();

			if (nguoiDuocDanhGia == null)
				throw new Exception("Không tìm thấy người được đánh giá.");
			// Nếu người đánh giá và người được đánh giá là cùng một người
			if (nguoiDanhGia == nguoiDuocDanhGia)
			{
				// Kiểm tra xem người đánh giá có vai trò Leader trong nhóm của họ không
				var isLeaderSelf = await _context.NHOM_NGUOIDUNG
					.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.VaiTro == "Leader");
				return isLeaderSelf ? "LEADER" : "NHANVIEN";
			}
			// Kiểm tra xem người đánh giá có vai trò Admin không
			var isAdmin = await _context.NGUOIDUNG_CHUCVU
				.Join(_context.CHUCVU,
					  ndcv => ndcv.MaChucVu,
					  cv => cv.MaChucVu,
					  (ndcv, cv) => new { ndcv.MaNguoiDung, cv.TenChucVu })
				.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.TenChucVu == "Admin");
			// Kiểm tra xem người được đánh giá có vai trò Leader trong nhóm của họ không
			var leaderGroups = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNguoiDung == nguoiDanhGia && x.VaiTro == "Leader")
				.Select(x => x.MaNhom)
				.ToListAsync();
			// Lấy danh sách nhóm của người được đánh giá
			var targetGroups = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNguoiDung == nguoiDuocDanhGia)
				.Select(x => x.MaNhom)
				.ToListAsync();
			// Kiểm tra xem người đánh giá có cùng nhóm với người được đánh giá không
			bool sameTeam = leaderGroups.Intersect(targetGroups).Any()
							|| await _context.NHOM_NGUOIDUNG
								.Where(x => x.MaNguoiDung == nguoiDanhGia)
								.Select(x => x.MaNhom)
								.Intersect(targetGroups)
								.AnyAsync();
			// Xác đinh người được đánh giá có vai trò Leader không
			var isLeaderTarget = await _context.NHOM_NGUOIDUNG
				.AnyAsync(x => x.MaNguoiDung == nguoiDuocDanhGia && x.VaiTro == "Leader");

			if (isAdmin && isLeaderTarget)
			{
				return "ADMIN-LEADER";
			}

			if (!isAdmin && sameTeam && isLeaderTarget)
			{
				return "NHANVIEN-LEADER";
			}

			if (sameTeam)
			{
				return "TEAM";
			}

			return "TEAM";
		}

	}
}
