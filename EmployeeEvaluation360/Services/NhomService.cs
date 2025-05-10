using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class NhomService : INhomService
	{
		private readonly ApplicationDBContext _context;
		public NhomService(ApplicationDBContext context)
		{
			_context = context;
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
			var nhom = await _context.NHOM.FindAsync(addDto.MaNhom);
			if (nhom == null)
			{
				return null;
			}

			var listData = new List<Nhom_NguoiDung>();

			foreach (var maNguoiDung in addDto.MaNguoiDung)
			{
				var chucVu = await _context.NGUOIDUNG_CHUCVU
					.Where(x => x.MaNguoiDung == maNguoiDung && x.TrangThai == "Active")
					.Select(x => x.ChucVu.TenChucVu)
					.FirstOrDefaultAsync();

				if (chucVu == null)
				{
					continue;
				}

				var nhomNguoiDung = new Nhom_NguoiDung
				{
					MaNhom = addDto.MaNhom,
					MaNguoiDung = maNguoiDung,
					VaiTro = chucVu,
					TrangThai = "Active"
				};

				var existingNhomNguoiDung = await _context.NHOM_NGUOIDUNG
					.FirstOrDefaultAsync(x => x.MaNhom == addDto.MaNhom && x.MaNguoiDung == maNguoiDung);

				if (existingNhomNguoiDung != null)
				{
					return null;
				}
				listData.Add(nhomNguoiDung);
				await _context.NHOM_NGUOIDUNG.AddAsync(nhomNguoiDung);
			}

			await _context.SaveChangesAsync();
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
