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

		public async Task<bool> DeleteMauDanhGia(int maMau)
		{
			var mauDanhGia = await _context.MAUDANHGIA.FindAsync(maMau);
			if (mauDanhGia == null)
			{
				return false;
			}
			// Kiểm tra xem mẫu đánh giá có được sử dụng trong dot đánh giá nào không
			var isUsedInDotDanhGia = await _context.CHITIET_DOTDANHGIA.AnyAsync(d => d.MaMau == maMau);
			if (isUsedInDotDanhGia)
			{
				mauDanhGia.TrangThai = "Inactive";
				_context.MAUDANHGIA.Update(mauDanhGia);
			}
			else
			{
				_context.CAUHOI.RemoveRange(_context.CAUHOI.Where(c => c.MaMau == maMau));
				_context.MAUDANHGIA.Remove(mauDanhGia);
			}
			await _context.SaveChangesAsync();
			return true;
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

		public async Task<GetMauDanhGia> GetMauDanhGiaById(int maMau)
		{
			var mauDanhGia = await _context.MAUDANHGIA.FindAsync(maMau);
			var cauHoiList = await _context.CAUHOI
				.Where(x => x.MaMau == maMau)
				.Select(x => new CauHoiDto
				{
					MaCauHoi = x.MaCauHoi,
					NoiDung = x.NoiDung,
					DiemToiDa = x.DiemToiDa
				}).ToListAsync();
			if (mauDanhGia == null)
			{
				return null;
			}
			return new GetMauDanhGia
			{
				MaMauDanhGia = mauDanhGia.MaMau,
				TenMauDanhGia = mauDanhGia.TenMau,
				DanhSachCauHoi = cauHoiList,
				TrangThai = mauDanhGia.TrangThai,
				LoaiDanhGia = mauDanhGia.LoaiDanhGia
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

		public async Task<UpdateMauDanhGiaDto> UpdateMauDanhGia(int maMau, UpdateMauDanhGiaDto mauDanhGiaDto)
		{
			try
			{
				// Kiểm tra mẫu đánh giá có tồn tại không
				var mauDanhGia = await _context.MAUDANHGIA
					.Include(m => m.CauHois)
					.FirstOrDefaultAsync(m => m.MaMau == maMau);

				if (mauDanhGia == null)
				{
					throw new Exception("Mẫu đánh giá không tồn tại");
				}

				// Kiểm tra tổng điểm tối đa của các câu hỏi
				var totalDiemToiDa = mauDanhGiaDto.DanhSachCauHoi.Sum(c => c.DiemToiDa);
				if (totalDiemToiDa != 100)
				{
					throw new Exception($"Tổng điểm tối đa của các câu hỏi phải bằng 100, hiện tại là {totalDiemToiDa}");
				}

				// Kiểm tra các câu hỏi hiện có đã được sử dụng chưa (chỉ cho phép cập nhật/xóa nếu chưa sử dụng)
				foreach (var cauHoiDto in mauDanhGiaDto.DanhSachCauHoi.Where(c => c.MaCauHoi != 0))
				{
					var cauHoi = await _context.CAUHOI
						.FirstOrDefaultAsync(c => c.MaCauHoi == cauHoiDto.MaCauHoi && c.MaMau == maMau);

					if (cauHoi == null)
					{
						throw new Exception($"Câu hỏi với MaCauHoi {cauHoiDto.MaCauHoi} không thuộc mẫu đánh giá này");
					}

					var isCauHoiUsed = await _context.DANHGIA_CAUHOI
						.AnyAsync(d => d.MaCauHoi == cauHoiDto.MaCauHoi);

					if (isCauHoiUsed)
					{
						throw new Exception($"Câu hỏi với MaCauHoi {cauHoiDto.MaCauHoi} đã được sử dụng trong đánh giá");
					}
				}

				// Cập nhật thông tin mẫu đánh giá
				mauDanhGia.TenMau = mauDanhGiaDto.TenMau;
				mauDanhGia.LoaiDanhGia = mauDanhGiaDto.LoaiDanhGia;

				// Lấy danh sách MaCauHoi từ DTO để so sánh
				var existingCauHoiIds = mauDanhGiaDto.DanhSachCauHoi.Select(c => c.MaCauHoi).ToList();

				// Xóa các câu hỏi không còn trong danh sách DTO và chưa được sử dụng
				var cauHoisToRemove = mauDanhGia.CauHois
					.Where(c => c.MaCauHoi != 0 && !existingCauHoiIds.Contains(c.MaCauHoi))
					.ToList();

				foreach (var cauHoiToRemove in cauHoisToRemove)
				{
					var isUsed = await _context.DANHGIA_CAUHOI
						.AnyAsync(d => d.MaCauHoi == cauHoiToRemove.MaCauHoi);
					if (!isUsed)
					{
						_context.CAUHOI.Remove(cauHoiToRemove);
					}
					else
					{
						throw new Exception($"Câu hỏi với MaCauHoi {cauHoiToRemove.MaCauHoi} đã được sử dụng, không thể xóa");
					}
				}

				// Chỉ cập nhật hoặc thêm câu hỏi
				var updatedCauHois = new List<CauHoi>();
				foreach (var cauHoiDto in mauDanhGiaDto.DanhSachCauHoi)
				{
					if (cauHoiDto.MaCauHoi == 0)
					{
						// Thêm câu hỏi mới
						var newCauHoi = new CauHoi
						{
							NoiDung = cauHoiDto.NoiDung,
							DiemToiDa = cauHoiDto.DiemToiDa,
							MaMau = maMau
						};
						updatedCauHois.Add(newCauHoi);
						await _context.CAUHOI.AddAsync(newCauHoi);
					}
					else
					{
						// Cập nhật câu hỏi hiện có
						var existingCauHoi = mauDanhGia.CauHois
							.FirstOrDefault(c => c.MaCauHoi == cauHoiDto.MaCauHoi);

						if (existingCauHoi != null)
						{
							existingCauHoi.NoiDung = cauHoiDto.NoiDung;
							existingCauHoi.DiemToiDa = cauHoiDto.DiemToiDa;
							updatedCauHois.Add(existingCauHoi);
							_context.CAUHOI.Update(existingCauHoi);
						}
						else
						{
							throw new Exception($"Câu hỏi với MaCauHoi {cauHoiDto.MaCauHoi} không tồn tại trong mẫu");
						}
					}
				}

				// Lưu thay đổi
				var result = await _context.SaveChangesAsync();
				if (result == 0)
				{
					throw new Exception("Cập nhật thất bại, không có thay đổi nào được lưu");
				}

				// Trả về DTO với dữ liệu đã cập nhật
				return new UpdateMauDanhGiaDto
				{
					TenMau = mauDanhGia.TenMau,
					LoaiDanhGia = mauDanhGia.LoaiDanhGia,
					DanhSachCauHoi = updatedCauHois.Select(c => new CauHoiDto
					{
						MaCauHoi = c.MaCauHoi,
						NoiDung = c.NoiDung,
						DiemToiDa = c.DiemToiDa
					}).ToList()
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi cập nhật mẫu đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Có lỗi xảy ra: {ex.Message}");
			}
		}
	}
}
