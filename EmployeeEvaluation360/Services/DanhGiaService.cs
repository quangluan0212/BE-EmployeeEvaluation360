using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class DanhGiaService : IDanhGiaService
	{
		private readonly ApplicationDBContext _context;
		public DanhGiaService(ApplicationDBContext applicationDBContext)
		{
			_context = applicationDBContext;
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
			var danhGia = await _context.DANHGIA
				.FirstOrDefaultAsync(dg => dg.MaDanhGia == giaTraLoiDto.MaDanhGia);

			if (danhGia == null)
				throw new InvalidOperationException("Không tìm thấy đánh giá.");

			if (giaTraLoiDto.CauHoiTraLoi == null || !giaTraLoiDto.CauHoiTraLoi.Any())
				throw new InvalidOperationException("Danh sách câu trả lời không được để trống.");

			var cauHoiCu = await _context.DANHGIA_CAUHOI
				.Where(ch => ch.MaDanhGia == giaTraLoiDto.MaDanhGia)
				.ToListAsync();

			if (cauHoiCu.Any())
			{
				_context.DANHGIA_CAUHOI.RemoveRange(cauHoiCu);
			}

			var danhSachTraLoi = giaTraLoiDto.CauHoiTraLoi.Select(cauTraLoi => new DanhGia_CauHoi
			{
				MaDanhGia = giaTraLoiDto.MaDanhGia,
				MaCauHoi = cauTraLoi.MaCauHoi,
				TraLoi = cauTraLoi.TraLoi
			}).ToList();

			await _context.DANHGIA_CAUHOI.AddRangeAsync(danhSachTraLoi);


			int tongDiem = danhSachTraLoi.Sum(t => t.TraLoi);
			danhGia.Diem = tongDiem;
			danhGia.TrangThai = "Đã đánh giá";

			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<List<AdminGetDanhGiaDto>> AdminGetListDanhGiaAsync(string maNguoiDung)
		{
			var danhGias = await _context.DANHGIA
				.Where(dg => dg.NguoiDanhGia == maNguoiDung)
				.Include(dg => dg.NguoiDanhGiaObj)
				.Include(dg => dg.NguoiDuocDanhGiaObj)
				.ToListAsync();

			var result = danhGias.Select(dg => new AdminGetDanhGiaDto
			{
				MaDanhGia = dg.MaDanhGia,
				MaNguoiDanhGia = dg.NguoiDanhGia,
				MaNhomNguoiDung = dg.NguoiDuocDanhGia,
				HotTen = dg.NguoiDanhGiaObj.HoTen,
				TrangThai = dg.TrangThai
			}).ToList();

			return result;
		}

		public async Task<List<NhanVienGetDanhGiaDto>> NhanVienGetDanhGiaAsync(string maNguoiDung)
		{
			var danhGias = await _context.DANHGIA
				.Where(dg => dg.NguoiDanhGia == maNguoiDung)
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



		public async Task<MauDanhGiaCauHoiDto> GetFormDanhGiaCauHoiAsync(int maDanhGia)
		{
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

			// 4. Lấy danh sách câu hỏi theo mẫu
			var cauHoiList = await _context.CAUHOI
				.Where(c => c.MaMau == maMau)
				.Select(c => new CauHoiDto
				{
					MaCauHoi = c.MaCauHoi,
					NoiDung = c.NoiDung,
					DiemToiDa = c.DiemToiDa
				})
				.ToListAsync();

			// 5. Trả về DTO
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
			var nguoiDuocDanhGia = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNhomNguoiDung == maNhomNguoiDung)
				.Select(x => x.MaNguoiDung)
				.FirstOrDefaultAsync();

			if (nguoiDuocDanhGia == null)
				throw new Exception("Không tìm thấy người được đánh giá.");

			if (nguoiDanhGia == nguoiDuocDanhGia)
			{
				var isLeaderSelf = await _context.NHOM_NGUOIDUNG
					.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.VaiTro == "Leader");
				return isLeaderSelf ? "LEADER" : "NHANVIEN";
			}

			var isAdmin = await _context.NGUOIDUNG_CHUCVU
				.Join(_context.CHUCVU,
					  ndcv => ndcv.MaChucVu,
					  cv => cv.MaChucVu,
					  (ndcv, cv) => new { ndcv.MaNguoiDung, cv.TenChucVu })
				.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.TenChucVu == "Admin");

			var leaderGroups = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNguoiDung == nguoiDanhGia && x.VaiTro == "Leader")
				.Select(x => x.MaNhom)
				.ToListAsync();

			var targetGroups = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNguoiDung == nguoiDuocDanhGia)
				.Select(x => x.MaNhom)
				.ToListAsync();

			bool sameTeam = leaderGroups.Intersect(targetGroups).Any()
							|| await _context.NHOM_NGUOIDUNG
								.Where(x => x.MaNguoiDung == nguoiDanhGia)
								.Select(x => x.MaNhom)
								.Intersect(targetGroups)
								.AnyAsync();

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
