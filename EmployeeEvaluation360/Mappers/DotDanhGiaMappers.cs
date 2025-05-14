using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Mappers
{
	public static class DotDanhGiaMappers
	{
		public static DotDanhGiaDto ToDto (this DotDanhGia dotdanhgia)
		{
			return new DotDanhGiaDto
			{
				MaDotDanhGia = dotdanhgia.MaDotDanhGia,
				TenDot = dotdanhgia.TenDot,
				ThoiGianBatDau = dotdanhgia.ThoiGianBatDau,
				ThoiGianKetThuc = dotdanhgia.ThoiGianKetThuc,
				TrangThai = dotdanhgia.TrangThai
			};
		}
	}
}
