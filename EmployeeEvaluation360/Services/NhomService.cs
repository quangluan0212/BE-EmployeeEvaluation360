using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class NhomService : INhomService
	{
		private readonly ApplicationDBContext _context;
		private readonly IMailService _mailService;
		public NhomService(ApplicationDBContext context, IMailService mailService)
		{
			_context = context;
			_mailService = mailService;
		}

		public async Task<List<SimpleNhomDto>> GetDanhSachNhomByLeader(string maNguoiDung)
		{
			var leader = await _context.NGUOIDUNG
				.Include(x => x.NguoiDungChucVus)
				.FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung && x.NguoiDungChucVus.Any(c => c.ChucVu.TenChucVu == "Leader" && c.TrangThai == "Active"));

			if (leader == null)
			{
				return new List<SimpleNhomDto>();
			}
			var nhoms = await _context.NHOM_NGUOIDUNG
				.Include(x => x.Nhom)
				.Where(x => x.MaNguoiDung == maNguoiDung && x.TrangThai == "Active" && x.VaiTro == "Leader")
				.Select(x => new SimpleNhomDto
				{
					MaNhom = x.Nhom.MaNhom,
					TenNhom = x.Nhom.TenNhom,
				})
				.ToListAsync();
			return nhoms;
		}

		public async Task<bool> XoaThanhVien(int maNhom, string maNguoiDung)
		{
			var nhomNguoiDung = await _context.NHOM_NGUOIDUNG
				.FirstOrDefaultAsync(x => x.MaNhom == maNhom && x.MaNguoiDung == maNguoiDung);
			if (nhomNguoiDung == null)
			{
				return false;
			}
			var result = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNhom == maNhom && x.MaNguoiDung == maNguoiDung)
				.ExecuteDeleteAsync();
			if (result <= 0)
			{
				return false;
			}
			return true;
		}
		public async Task<List<NguoiDungKhongNhomDto>> LayDanhSachNguoiDungKhongCoTrongNhomAsync(int maNhom)
		{
			try
			{
				var usersNotInGroup = await _context.NGUOIDUNG
					.Where(nd => nd.TrangThai == "Active" &&
								 !nd.NguoiDungChucVus.Any(r => r.ChucVu.TenChucVu == "Admin") &&
								 !nd.NhomNguoiDungs.Any(nnd => nnd.MaNhom == maNhom))
					.Select(nd => new NguoiDungKhongNhomDto
					{
						MaNguoiDung = nd.MaNguoiDung,
						HoTen = nd.HoTen ?? string.Empty,
						SoDuAn = nd.NhomNguoiDungs != null ? nd.NhomNguoiDungs.Count : 0,
						ChucVu = nd.NguoiDungChucVus
							.Select(c => c.ChucVu.TenChucVu)
							.Where(cv => cv != null)
							.FirstOrDefault() ?? string.Empty
					})
					.ToListAsync();

				return usersNotInGroup;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy danh sách người dùng không thuộc nhóm {maNhom}: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw;
			}
		}
		public Task<bool> DeleteNhomAsync(int maNhom)
		{
			var nhom = _context.NHOM.FirstOrDefault(x => x.MaNhom == maNhom);
			if (nhom == null)
			{
				return Task.FromResult(false);
			}
			var linkedToGroup = _context.NHOM_NGUOIDUNG.Any(x => x.MaNhom == maNhom);
			if (linkedToGroup)
			{
				nhom.TrangThai = "Deleted";
				_context.NHOM.Update(nhom);
			}
			else
			{
				_context.NHOM.Remove(nhom);
			}
			_context.SaveChanges();
			return Task.FromResult(true);
		}

		public async Task<PagedResult<ListNhomDuAnDto>> GetAllNhomPagedAsync(int page, int pageSize, string? search)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;

			var query = _context.NHOM
			.Include(n => n.DuAn)
			.AsQueryable();

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(n => n.TenNhom.Contains(search) || n.DuAn.TenDuAn.Contains(search));
			}

			var totalCount = query.Count();
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

			var items = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var ListDto = items.Select(n => new ListNhomDuAnDto
			{
				MaNhom = n.MaNhom,
				TenNhom = n.TenNhom,
				TenDuAn = n.DuAn.TenDuAn,
				TrangThai = n.TrangThai
			}).ToList();


			return new PagedResult<ListNhomDuAnDto>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount,
				Items = ListDto
			};
		}

		public async Task<Nhom> GetNhomByIdAsync(int maNhom)
		{
			var Nhom = await _context.NHOM
				.Include(n => n.DuAn)
				.FirstOrDefaultAsync(n => n.MaNhom == maNhom);
			if (Nhom == null)
			{
				return null;
			}
			return Nhom;
		}

		public async Task<Nhom> ThemNhom(CreateNhomDto creatNhom)
		{
			var ExistingNhom = await _context.NHOM
				.FirstOrDefaultAsync(x => x.TenNhom == creatNhom.TenNhom && x.MaDuAn == creatNhom.MaDuAn);

			if (ExistingNhom != null)
			{
				return null;
			}

			try
			{
				var result = await _context.NHOM.AddAsync(creatNhom.CreateToEntity());
				if (result == null)
				{
					throw new Exception("Thêm nhóm không thành công");
				}
				await _context.SaveChangesAsync();
				return creatNhom.CreateToEntity();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new Exception("Lỗi khi thêm nhóm");
			} 
		}

		public async Task<Nhom> UpdateNhomAsync(int maNhom, UpdateNhomDto updateDto)
		{
			var nhomExisting = await _context.NHOM.FindAsync(maNhom);
			if (nhomExisting == null)
			{
				return null;
			}

			var nhomTrung = await _context.NHOM
				.FirstOrDefaultAsync(x =>
					x.MaNhom != maNhom &&
					x.TenNhom == updateDto.TenNhom &&
					x.MaDuAn == updateDto.MaDuAn);

			if (nhomTrung != null)
			{
				return null;
			}

			nhomExisting.TenNhom = updateDto.TenNhom;
			nhomExisting.MaDuAn = updateDto.MaDuAn;
			nhomExisting.TrangThai = updateDto.TrangThai;

			try
			{
				var result = await _context.SaveChangesAsync();
				if (result <= 0)
				{
					throw new Exception("Cập nhật nhóm không thành công");
				}
				return nhomExisting;
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new Exception("Lỗi khi cập nhật nhóm");
			}
		}

		public async Task<List<Nhom_NguoiDung>> ThemNhanVienVaoNhom(ThemNhanVienVaoNhomDto addDto)
		{
			// Kiểm tra nhóm tồn tại
			var nhom = await _context.NHOM.FindAsync(addDto.MaNhom);
			if (nhom == null)
			{
				return null;
			}

			// Tải dữ liệu hàng loạt
			var maNguoiDungList = addDto.MaNguoiDung?.Distinct().ToList() ?? new List<string>();
			if (!maNguoiDungList.Any())
			{
				return new List<Nhom_NguoiDung>();
			}

			// Lấy chức vụ Active của tất cả người dùng
			var chucVus = await _context.NGUOIDUNG_CHUCVU
				.Where(x => maNguoiDungList.Contains(x.MaNguoiDung) && x.TrangThai == "Active")
				.Include(x => x.ChucVu)
				.ToDictionaryAsync(
					x => x.MaNguoiDung,
					x => x.ChucVu?.TenChucVu);

			// Kiểm tra người dùng đã tồn tại trong nhóm
			var existingUsers = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNhom == addDto.MaNhom && maNguoiDungList.Contains(x.MaNguoiDung))
				.Select(x => x.MaNguoiDung)
				.ToListAsync();

			// Lấy thông tin người dùng (Email, HoTen)
			var users = await _context.NGUOIDUNG
				.Where(u => maNguoiDungList.Contains(u.MaNguoiDung))
				.Select(u => new { u.MaNguoiDung, u.Email, u.HoTen })
				.ToDictionaryAsync(
					u => u.MaNguoiDung,
					u => new { u.Email, u.HoTen });

			var listData = new List<Nhom_NguoiDung>();
			var emailTasks = new List<Task>();

			foreach (var maNguoiDung in maNguoiDungList)
			{
				// Kiểm tra chức vụ
				if (!chucVus.TryGetValue(maNguoiDung, out var chucVu) || string.IsNullOrEmpty(chucVu))
				{
					continue;
				}

				// Kiểm tra người dùng đã tồn tại trong nhóm
				if (existingUsers.Contains(maNguoiDung))
				{
					return null;
				}

				// Thêm người dùng vào nhóm
				var nhomNguoiDung = new Nhom_NguoiDung
				{
					MaNhom = addDto.MaNhom,
					MaNguoiDung = maNguoiDung,
					VaiTro = chucVu,
					TrangThai = "Active"
				};

				listData.Add(nhomNguoiDung);

				// Gửi email thông báo
				if (users.TryGetValue(maNguoiDung, out var user) && !string.IsNullOrWhiteSpace(user.Email))
				{
					var emailTask = _mailService.SendMailUserAddedGroup(
						user.Email,
						nhom.TenNhom,
						nhom.MaNhom.ToString(),
						user.HoTen,
						maNguoiDung)
						.ContinueWith(t =>
						{
							Console.WriteLine($"Kết quả gửi email cho {user.Email}: {t.Result}");
						});
					emailTasks.Add(emailTask);
				}
			}

			if (listData.Any())
			{
				// Thêm tất cả bản ghi vào database cùng lúc
				await _context.NHOM_NGUOIDUNG.AddRangeAsync(listData);
				await _context.SaveChangesAsync();
				// Chờ tất cả email được gửi
				await Task.WhenAll(emailTasks);
			}

			return listData;
		}

		public async Task<PagedResult<Nhom_NguoiDungDto>> GetDanhSachThanhVienNhom(int maNhom, int page, int pageSize, string? search)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;
			var query = _context.NHOM_NGUOIDUNG
				.Include(n => n.NguoiDung)
				.Include(n => n.Nhom)
				.AsQueryable();
			if (maNhom > 0)
			{
				query = query.Where(n => n.MaNhom == maNhom);
			}
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(n => n.NguoiDung.HoTen.Contains(search) || n.VaiTro.Contains(search));
			}
			var totalCount = query.Count();
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			var items = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
			var ListDto = items.Select(n => new Nhom_NguoiDungDto
			{
				MaNhom = n.MaNhom,
				MaNguoiDung = n.MaNguoiDung,
				HoTen = n.NguoiDung.HoTen,
				ChucVu = n.VaiTro,
				TrangThai = n.TrangThai
			}).ToList();
			return new PagedResult<Nhom_NguoiDungDto>
			{
				CurrentPage = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				TotalCount = totalCount,
				Items = ListDto
			};
		}

		public async Task<List<NhomDto>> GetAllNhomByMaNguoiDung(string maNguoiDung)
		{
			var nhoms = await _context.NHOM_NGUOIDUNG
				.Where(nd => nd.MaNguoiDung == maNguoiDung && nd.TrangThai == "Active")
				.Include(nd => nd.Nhom)
				.Select(nd => new NhomDto
				{
					MaNhom = nd.Nhom.MaNhom,
					TenNhom = nd.Nhom.TenNhom,
					MaDuAn = nd.Nhom.MaDuAn,
					TrangThai = nd.Nhom.TrangThai
				})
				.ToListAsync();
			return nhoms;
		}

		public async Task<List<NhomVaThanhVienDto>> LayDanhSachNhomVaThanhVienAsync(string maNguoiDung)
		{
			var nhomIds = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNguoiDung == maNguoiDung && x.TrangThai == "Active")
				.Select(x => x.MaNhom)
				.Distinct()
				.ToListAsync();

			var result = new List<NhomVaThanhVienDto>();

			foreach (var maNhom in nhomIds)
			{
				var nhom = await _context.NHOM.FindAsync(maNhom);
				if (nhom == null) continue;

				var thanhVien = await _context.NHOM_NGUOIDUNG
					.Include(x => x.NguoiDung)
					.Where(x => x.MaNhom == maNhom && x.TrangThai == "Active")
					.Select(x => new ThanhVienDto
					{
						MaNhomNguoiDung = x.MaNhomNguoiDung,
						MaNguoiDung = x.MaNguoiDung,
						HoTen = x.NguoiDung.HoTen,
						SoDienThoai = x.NguoiDung.DienThoai,
						Email = x.NguoiDung.Email,
						ChucVu = x.VaiTro
					})
					.ToListAsync();

				result.Add(new NhomVaThanhVienDto
				{
					MaNhom = nhom.MaNhom,
					TenNhom = nhom.TenNhom,
					ThanhVien = thanhVien
				});
			}

			return result;
		}

	}
}
