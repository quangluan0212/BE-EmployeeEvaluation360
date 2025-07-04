﻿using EmployeeEvaluation360.Database;
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

		// Lấy danh sách đợt đánh giá theo năm
		public async Task<List<DotDanhGiaDto>> getDotDanhGiaByYear(int? year = null)
		{
			if (!year.HasValue)
			{
				var result = await _context.DOT_DANHGIA
				.Where(d => d.ThoiGianBatDau.Year == DateTime.Now.Year || d.ThoiGianKetThuc.Year == DateTime.Now.Year)
				.ToListAsync();
				return result.Select(d => d.ToDto()).ToList();
			}
			var dotDanhGias = await _context.DOT_DANHGIA
				.Where(d => d.ThoiGianBatDau.Year == year || d.ThoiGianKetThuc.Year == year && d.TrangThai == "Inactive")
				.OrderByDescending(d => d.ThoiGianKetThuc)
				.ToListAsync();
			return dotDanhGias.Select(d => d.ToDto()).ToList();
		}

		// Tạo kết quả đánh giá dựa trên mã đợt đánh giá
		public async Task<string> CreateKetQuaDanhGiaByMaDotDanhGia(int maDotDanhGia)
		{
			try
			{
				var dotDanhGia = await _context.DOT_DANHGIA.FindAsync(maDotDanhGia);
				if (dotDanhGia == null)
				{
					return "Đợt đánh giá không tồn tại";
				}
				var danhGias = await _context.DANHGIA
					.Include(d => d.NguoiDuocDanhGiaObj)
					.Where(d => d.MaDotDanhGia == maDotDanhGia && d.TrangThai == "Đã đánh giá")
					.ToListAsync();
				if (!danhGias.Any())
				{
					return "Không có đánh giá nào cho người dùng này trong đợt đánh giá hiện tại.";
				}
				// Nhóm các đánh giá theo Người được đánh giá (MaNguoiDung) và lấy các hệ số khác nhau
				var groupedDanhGias = danhGias
					.GroupBy(d => d.NguoiDuocDanhGiaObj.MaNguoiDung)
					.Select(g => new
					{
						MaNguoiDung = g.Key,
						HeSoValues = g.Select(d => d.HeSo).Distinct().ToList(),
						Evaluation = g
					})
					.Where(g => g.HeSoValues.Contains(1) && g.HeSoValues.Contains(2) && g.HeSoValues.Contains(3)).ToList();
				if (!groupedDanhGias.Any())
				{
					return "Không có người dùng nào có đủ cả ba loại đánh giá (HeSo 1, 2, 3).";
				}
				// Tính toán điểm trung bình cho mỗi người dùng
				var results = new List<string>();
				foreach (var item in groupedDanhGias)
				{
					var existingKetQua = await _context.KETQUA_DANHGIA
						.FirstOrDefaultAsync(k => k.MaNguoiDung == item.MaNguoiDung && k.ThoiGianTinh == dotDanhGia.ThoiGianKetThuc);
					if (existingKetQua != null)
					{
						results.Add($"Kết quả đánh giá của ID : {item.MaNguoiDung} đã tồn tại");
						continue;
					}
					// Tính điểm trung bình cho từng hệ số
					var tbDg1 = item.Evaluation.Where(d => d.HeSo == 1).Select(d => d.Diem).Average();
					var tbDg2 = item.Evaluation.Where(d => d.HeSo == 2).Select(d => d.Diem).Average();
					var tbDg3 = item.Evaluation.Where(d => d.HeSo == 3).Select(d => d.Diem).Average();

					var finalScore = (tbDg1 + (tbDg2 * 2) + (tbDg3 * 3)) / 6;

					var ketQuaDanhGia = new KetQua_DanhGia
					{
						MaNguoiDung = item.MaNguoiDung,
						DiemTongKet = finalScore,
						ThoiGianTinh = dotDanhGia.ThoiGianKetThuc,
						MaDotDanhGia = maDotDanhGia,
					};

					await _context.KETQUA_DANHGIA.AddAsync(ketQuaDanhGia);

					results.Add($"Tạo kết quả đánh giá cho ID: {item.MaNguoiDung} với điểm tổng kết {finalScore:F2}.");
				}
				// Lưu kết quả vào cơ sở dữ liệu
				var saveResult = await _context.SaveChangesAsync();
				if (saveResult == 0 && results.All(r => r.Contains("đã tồn tại")))
				{
					return "Không có kết quả đánh giá mới được tạo vì tất cả đã tồn tại.";
				}
				return $"Kết quả tính toán:\n{string.Join("\n", results)}";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi tính kết quả đánh giá: {ex.Message}\nStackTrace: {ex.StackTrace}");
				return $"Có lỗi xảy ra khi tính kết quả đánh giá: {ex.Message}";
			}
		}

		// Kết thúc đợt đánh giá
		public async Task<string> KetThucDotDanhGia(int maDotDanhGia)
		{
			var dotDanhGia = await _context.DOT_DANHGIA.FindAsync(maDotDanhGia);
			if (dotDanhGia == null)
			{
				return "Đợt đánh giá không tồn tại";
			}
			dotDanhGia.ThoiGianKetThuc = DateTime.Now;
			dotDanhGia.TrangThai = "Inactive";
			var result = await _context.SaveChangesAsync();
			if (result == 0)
			{
				return "Cập nhật thất bại";
			}
			else
			{
				var tinhKetQuaDanhGia = await CreateKetQuaDanhGiaByMaDotDanhGia(maDotDanhGia);
				return tinhKetQuaDanhGia;
			}
		}

		// Cập nhật đợt đánh giá
		public async Task<string> UpdateDotDanhGia(UpdateDotDanhGia updateDotDanhGia)
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
			var listData = await _context.DOT_DANHGIA.OrderByDescending(n => n.ThoiGianKetThuc).ToListAsync();
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
			var listData = await _context.DOT_DANHGIA.Where(x => x.TrangThai == "Active").ToListAsync();
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
		// Tạo đợt đánh giá mới
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
				// Tạo chi tiết đợt đánh giá cho từng mẫu đánh giá
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
				// Gọi hàm GenDanhGia để tạo các đánh giá cho đợt đánh giá mới
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

			//Kiểm tra xem trong danh sách leaders có người nào không có MaNhomNguoiDung không, nếu có thì bỏ qua
			leaders = leaders.Where(leader => leader.MaNhomNguoiDung != 0).ToList();
			if (!leaders.Any())
			{
				return new List<ThanhVienDto>();
			}

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
				.Where(nd => nd.MaNguoiDung == maNguoiDung && nd.TrangThai == "Active")
				.Select(nd => nd.MaNhom)
				.Distinct()
				.ToListAsync();

			var nhoms = await _context.NHOM
				.Where(n => result.Contains(n.MaNhom) && n.TrangThai == "Active")
				.Select(n => new NhomVaThanhVienDto
				{
					MaNhom = n.MaNhom,
					TenNhom = n.TenNhom,
					ThanhVien = n.NhomNguoiDungs.Where(tv => tv.TrangThai == "Active")
					.Select(tv => new ThanhVienDto
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
			// Tạo đánh giá cho các Admin
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
			// Tạo đánh giá cho các Leader
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
						// Nếu người đánh giá là người dùng hiện tại, thì hệ số là 2 -- tự đánh giá bản thân
						if (user.MaNguoiDung == tv.MaNguoiDung)
							heSo = 2;
						// Nếu người đánh giá là Leader và không phải là người dùng hiện tại, thì hệ số là 3 -- đánh giá Leader
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
			// Loại bỏ các đánh giá trùng lặp (nếu có) dựa trên Người đánh giá và Người được đánh giá
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
