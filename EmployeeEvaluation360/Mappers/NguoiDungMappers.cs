using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Mappers
{
	public static class NguoiDungMappers
	{
		public static NguoiDungDto ToDto(this NguoiDung nguoiDung)
		{
			if (nguoiDung == null) return null;
			return new NguoiDungDto
			{
				MaNguoiDung = nguoiDung.MaNguoiDung,
				HoTen = nguoiDung.HoTen,
				Email = nguoiDung.Email,
				DienThoai = nguoiDung.DienThoai,
				NgayVaoCongTy = nguoiDung.NgayVaoCongTy,
				TrangThai = nguoiDung.TrangThai
			};
		}

		public static NguoiDung ToEntity(this CreateNguoiDungDto createDto)
		{
			if (createDto == null) 
				return null;
			return new NguoiDung
			{
				HoTen = createDto.HoTen,
				Email = createDto.Email,
				DienThoai = createDto.DienThoai,
				MatKhau = createDto.MatKhau,
				TrangThai = "Active",
				NgayVaoCongTy = DateTime.Now
			};
		}
	}
}
