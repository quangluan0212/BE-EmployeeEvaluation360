using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
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



		public async Task<string>KetThucDotDanhGia(int maDotDanhGia)
		{
			var dotDanhGia = await _context.DOT_DANHGIA.FindAsync(maDotDanhGia);
			if (dotDanhGia == null)
			{
				return "Đợt đánh giá không tồn tại";
			}
			dotDanhGia.TrangThai = "Inactive";
			var result = await _context.SaveChangesAsync();
			if (result == 0)
			{
				return "Cập nhật thất bại";
			}
			else
			{
				return "Cập nhật thành công";
			}
		}

		public async Task<string> UpdateDotDanhGia (UpdateDotDanhGia updateDotDanhGia)
		{
			var dotDanhGia = await _context.DOT_DANHGIA.FindAsync(updateDotDanhGia.MaDotDanhGia);
			if (dotDanhGia == null)
			{
				return "Đợt đánh giá không tồn tại";
			}

			dotDanhGia.TenDot = updateDotDanhGia.TenDot;
			dotDanhGia.ThoiGianBatDau = updateDotDanhGia.NgayBatDau;
			dotDanhGia.ThoiGianKetThuc = updateDotDanhGia.NgayKetThuc;

			try
			{
				var chiTietCu = _context.CHITIET_DOTDANHGIA
					.Where(c => c.MaDotDanhGia == updateDotDanhGia.MaDotDanhGia);
				_context.CHITIET_DOTDANHGIA.RemoveRange(chiTietCu);

				foreach (var mau in updateDotDanhGia.mauDanhGias)
				{
					var mauDanhGia = await _context.MAUDANHGIA.FindAsync(mau);
					if (mauDanhGia == null)
					{
						return $"Không tìm thấy mẫu đánh giá với mã: {mau}";
					}

					var chiTiet = new ChiTiet_DotDanhGia
					{
						MaDotDanhGia = updateDotDanhGia.MaDotDanhGia,
						MaMau = mau,
						LoaiNguoiDuocDanhGia = mauDanhGia.LoaiDanhGia
					};

					await _context.CHITIET_DOTDANHGIA.AddAsync(chiTiet);
				}

				var result = await _context.SaveChangesAsync();
				if (result == 0)
				{
					return "Cập nhật thất bại";
				}

				return "Cập nhật thành công";
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi cập nhật đợt đánh giá: " + ex.Message);
				return "Có lỗi xảy ra trong quá trình cập nhật";
			}
		}


		public async Task<List<DotDanhGiaDto>> getDanhSachDotDanhGia()
		{
			var listData = await _context.DOT_DANHGIA.ToListAsync();
			var listDataDto = listData.Select(x => new DotDanhGiaDto
			{
				MaDotDanhGia = x.MaDotDanhGia,
				TenDot = x.TenDot,
				ThoiGianBatDau = x.ThoiGianBatDau,
				ThoiGianKetThuc = x.ThoiGianKetThuc,
				TrangThai = x.TrangThai,
			}).ToList();
			return listDataDto;
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

		public async Task<DotDanhGiaDto> CreateDotDanhGia(CreateDotDanhGiaDto danhGiaDto)
		{
			var currentDdd = await GetDotDanhGiaActivesAsync();
			if (currentDdd != null)
			{
				return null;
			}

			var dotDanhGia = new DotDanhGia
			{
				TenDot = danhGiaDto.TenDot,
				ThoiGianBatDau = danhGiaDto.NgayBatDau,
				ThoiGianKetThuc = danhGiaDto.NgayKetThuc,
				TrangThai = "Active"
			};

			try
			{
				await _context.DOT_DANHGIA.AddAsync(dotDanhGia);
				await _context.SaveChangesAsync();

				foreach (var mau in danhGiaDto.mauDanhGias)
				{
					var mauDanhGia = await _context.MAUDANHGIA.FindAsync(mau);
					if (mauDanhGia == null)
					{
						return null;
					}

					var chiTietDotDanhGia = new ChiTiet_DotDanhGia
					{
						MaDotDanhGia = dotDanhGia.MaDotDanhGia,
						MaMau = mau,
						LoaiNguoiDuocDanhGia = mauDanhGia.LoaiDanhGia
					};

					await _context.CHITIET_DOTDANHGIA.AddAsync(chiTietDotDanhGia);
				}

				await _context.SaveChangesAsync();

				string gen = await GenDanhGia(dotDanhGia.MaDotDanhGia);
				Console.WriteLine(gen);

				return dotDanhGia.ToDto();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi tạo DotDanhGia: " + ex.Message);
				return null;
			}
		}


		public async Task<List<NguoiDungDto>> getNguoiDungActiveNotIncludeAdmin()
		{
			var nguoiDungs = await _context.NGUOIDUNG
			.Where(nd => nd.TrangThai == "Active" && !nd.NguoiDungChucVus.Any(r => r.ChucVu.TenChucVu == "Admin"))
			.Select(nd => new NguoiDungDto
			{
				MaNguoiDung = nd.MaNguoiDung,
				HoTen = nd.HoTen,
				Email = nd.Email,
				DienThoai = nd.DienThoai,
				NgayVaoCongTy = nd.NgayVaoCongTy,
				TrangThai = nd.TrangThai
			})
			.ToListAsync();

			return nguoiDungs;
		}
		public async Task<List<ThanhVienDto>> getListLeaderActive()
		{
			var leaders = await _context.NGUOIDUNG
				.Where(nd => nd.TrangThai == "Active" &&
							 nd.NguoiDungChucVus.Any(r => r.ChucVu.TenChucVu == "Leader"))
				.Select(nd => new ThanhVienDto
				{
					MaNguoiDung = nd.MaNguoiDung,
					HoTen = nd.HoTen,
					MaNhomNguoiDung = nd.NhomNguoiDungs
										.OrderBy(nnd => nnd.MaNhomNguoiDung)
										.Select(nnd => nnd.MaNhomNguoiDung)
										.FirstOrDefault(),
					ChucVu = "Leader"
				})
				.ToListAsync();

			return leaders;
		}

		public async Task<List<NguoiDungDto>> getListAdminActive()
		{
			var admins = await _context.NGUOIDUNG
				.Where(nd => nd.TrangThai == "Active" &&
							 nd.NguoiDungChucVus.Any(r => r.ChucVu.TenChucVu == "Admin"))
				.Select(nd => new NguoiDungDto
				{
					MaNguoiDung = nd.MaNguoiDung,
					HoTen = nd.HoTen,
					Email = nd.Email,
					DienThoai = nd.DienThoai,
					NgayVaoCongTy = nd.NgayVaoCongTy,
					TrangThai = nd.TrangThai
				})
				.ToListAsync();

			return admins;
		}

		public async Task<List<NhomVaThanhVienDto>> getNhomVaThanhVienCungNhomByMaNguoiDung(string maNguoiDung)
		{
			var result = await _context.NHOM_NGUOIDUNG
				.Where(nd => nd.MaNguoiDung == maNguoiDung)
				.Select(nd => nd.MaNhom)
				.Distinct()
				.ToListAsync();

			var nhoms = await _context.NHOM
				.Where(n => result.Contains(n.MaNhom))
				.Select(n => new NhomVaThanhVienDto
				{
					MaNhom = n.MaNhom,
					TenNhom = n.TenNhom,
					ThanhVien = n.NhomNguoiDungs.Select(tv => new ThanhVienDto
					{
						MaNhomNguoiDung = tv.MaNhomNguoiDung,
						MaNguoiDung = tv.NguoiDung.MaNguoiDung,
						HoTen = tv.NguoiDung.HoTen,
						ChucVu = tv.VaiTro
					}).ToList()
				})
				.ToListAsync();

			return nhoms;
		}

		public async Task<string> GenDanhGia(int maDotDanhGia)
		{
			var admins = await getListAdminActive();
			var leaders = await getListLeaderActive();
			var allNguoiDung = await getNguoiDungActiveNotIncludeAdmin();
			var danhGias = new List<DanhGia>();
			foreach (var admin in admins)
			{
				foreach (var leader in leaders)
				{					
					danhGias.Add(new DanhGia
					{
						NguoiDanhGia = admin.MaNguoiDung,
						NguoiDuocDanhGia = leader.MaNhomNguoiDung,
						MaDotDanhGia = maDotDanhGia,
						Diem = 0,
						TrangThai = "Chưa đánh giá",
						HeSo = 3,
					});
					
				}
			}
			foreach (var user in allNguoiDung)
			{
				var nhoms = await getNhomVaThanhVienCungNhomByMaNguoiDung(user.MaNguoiDung);

				foreach (var nhom in nhoms)
				{
					var thanhViens = nhom.ThanhVien;

					var currentUser = thanhViens.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);
					bool isLeader = (currentUser != null && currentUser.ChucVu == "Leader");

					foreach (var tv in thanhViens)
					{
						int heSo = 1;

						if (user.MaNguoiDung == tv.MaNguoiDung)
							heSo = 2;

						else if (isLeader && user.MaNguoiDung != tv.MaNguoiDung)
							heSo = 3;

						danhGias.Add(new DanhGia
						{
							NguoiDanhGia = user.MaNguoiDung,
							NguoiDuocDanhGia = tv.MaNhomNguoiDung,
							MaDotDanhGia = maDotDanhGia,							
							Diem = 0,
							TrangThai = "Chưa đánh giá",
							HeSo = heSo,
						});
					}
				}
			}

			danhGias = danhGias
				.GroupBy(d => new { d.NguoiDanhGia, d.NguoiDuocDanhGia })
				.Select(g => g.First())
				.ToList();

			await _context.DANHGIA.AddRangeAsync(danhGias);
			await _context.SaveChangesAsync();

			return $"Đã tạo {danhGias.Count} đánh giá.";
		}
	}
}
