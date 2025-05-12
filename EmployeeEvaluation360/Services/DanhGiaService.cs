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
		public DanhGiaService (ApplicationDBContext applicationDBContext)
		{
			_context = applicationDBContext;
		}

		public async Task<MauDanhGiaCauHoiDto> GetFormDanhGiaCauHoiAsync(string nguoiDanhGia, int maNguoiDungDanhGia, int maDotDanhGia)
		{
			// 1. Xác định loại đánh giá:
			var loaiDanhGia = await XacDinhLoaiDanhGiaAsync(nguoiDanhGia, maNguoiDungDanhGia);

			// 2. Lấy mã mẫu đánh giá (MaMau) từ chi tiết đợt đánh giá
			var chiTiet = await _context.CHITIET_DOTDANHGIA
				.Include(ct => ct.DotDanhGia)
				.FirstOrDefaultAsync(ct => ct.MaDotDanhGia == maDotDanhGia && ct.LoaiNguoiDuocDanhGia == loaiDanhGia);
			if (chiTiet == null)
				throw new InvalidOperationException("Không tìm thấy mẫu đánh giá phù hợp.");

			int maMau = chiTiet.MaMau;
			string tenDot = chiTiet.DotDanhGia.TenDot;

			// 3. Tạo record DANHGIA nếu chưa có:
			var danhGia = await _context.DANHGIA
				.FirstOrDefaultAsync(d => d.NguoiDanhGia == nguoiDanhGia
									   && d.NguoiDuocDanhGia == maNguoiDungDanhGia
									   && d.MaDotDanhGia == maDotDanhGia);
			int maDanhGia;
			if (danhGia == null)
			{
				int heSo = await TinhHeSoAsync(nguoiDanhGia, maNguoiDungDanhGia);
				danhGia = new DanhGia
				{
					NguoiDanhGia = nguoiDanhGia,
					NguoiDuocDanhGia = maNguoiDungDanhGia,
					MaDotDanhGia = maDotDanhGia,
					Diem = 0,
					HeSo = heSo,
					TrangThai = "ChuaDanhGia"
				};
				_context.DANHGIA.Add(danhGia);
				await _context.SaveChangesAsync();
				maDanhGia = danhGia.MaDanhGia;
			}
			else
			{
				maDanhGia = danhGia.MaDanhGia;
			}


			// 4. Lấy danh sách câu hỏi của mẫu
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

		public async Task<int> TinhHeSoAsync(string nguoiDanhGia, int maNhomNguoiDung)
		{
			// 1. Tìm mã người thực tế của NguoiDuocDanhGia
			var nguoiDuocDanhGia = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNhomNguoiDung == maNhomNguoiDung)
				.Select(x => x.MaNguoiDung)
				.FirstOrDefaultAsync();

			if (nguoiDuocDanhGia == null)
				throw new Exception("Không tìm thấy người được đánh giá.");

			// 2. Nếu tự đánh giá
			if (nguoiDanhGia == nguoiDuocDanhGia)
				return 2;

			// 3. Nếu người đánh giá là Admin
			var isAdmin = await _context.NGUOIDUNG_CHUCVU
				.Join(_context.CHUCVU,
					  ndcv => ndcv.MaChucVu,
					  cv => cv.MaChucVu,
					  (ndcv, cv) => new { ndcv.MaNguoiDung, cv.TenChucVu })
				.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.TenChucVu == "Admin");

			if (isAdmin)
				return 3;

			// 4. Nếu đánh giá là Leader của cùng nhóm
			var isLeaderOfSameGroup = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNguoiDung == nguoiDanhGia && x.VaiTro == "Leader")
				.Select(x => x.MaNhom)
				.Intersect(
					_context.NHOM_NGUOIDUNG
						.Where(x => x.MaNguoiDung == nguoiDuocDanhGia)
						.Select(x => x.MaNhom)
				)
				.AnyAsync();

			if (isLeaderOfSameGroup)
				return 3;

			// 5. Mặc định: cùng team ngang hàng
			return 1;
		}


		public async Task<string> XacDinhLoaiDanhGiaAsync(string nguoiDanhGia, int maNhomNguoiDung)
		{
			// 1. Lấy MaNguoiDung của người được đánh giá
			var nguoiDuocDanhGia = await _context.NHOM_NGUOIDUNG
				.Where(x => x.MaNhomNguoiDung == maNhomNguoiDung)
				.Select(x => x.MaNguoiDung)
				.FirstOrDefaultAsync();

			if (nguoiDuocDanhGia == null)
				throw new Exception("Không tìm thấy người được đánh giá.");

			// 2. Kiểm tra tự đánh giá
			if (nguoiDanhGia == nguoiDuocDanhGia)
			{
				// Kiểm tra xem có phải Leader không
				var isLeaderSelf = await _context.NHOM_NGUOIDUNG
					.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.VaiTro == "Leader");
				return isLeaderSelf ? "LEADER" : "NHANVIEN";
			}

			// 3. Kiểm tra Admin
			var isAdmin = await _context.NGUOIDUNG_CHUCVU
				.Join(_context.CHUCVU,
					  ndcv => ndcv.MaChucVu,
					  cv => cv.MaChucVu,
					  (ndcv, cv) => new { ndcv.MaNguoiDung, cv.TenChucVu })
				.AnyAsync(x => x.MaNguoiDung == nguoiDanhGia && x.TenChucVu == "Admin");

			// 4. Kiểm tra Leader của cùng nhóm
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

			// 5. Kiểm tra role của target
			var isLeaderTarget = await _context.NHOM_NGUOIDUNG
				.AnyAsync(x => x.MaNguoiDung == nguoiDuocDanhGia && x.VaiTro == "Leader");

			// 6. Ra quyết định
			if (isAdmin && isLeaderTarget)
			{
				return "ADMIN-LEADER";
			}

			if (!isAdmin && sameTeam && isLeaderTarget)
			{
				// Nhân viên trong team đánh giá Leader
				return "NHANVIEN-LEADER";
			}

			if (sameTeam)
			{
				// Đánh giá chéo trong team (bao gồm Leader đánh giá nhân viên)
				return "TEAM";
			}

			// Nếu không rơi vào bất kỳ trường hợp nào, mặc định coi là "TEAM"
			return "TEAM";
		}

	}
}
