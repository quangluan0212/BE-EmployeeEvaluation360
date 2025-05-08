using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Mappers
{
	public static class ChucVuMappers
	{
		public static ChucVuDto ToDto(this ChucVu chucVu)
		{
			if (chucVu == null) return null;
			return new ChucVuDto
			{
				maChucVu = chucVu.MaChucVu,
				tenChucVu = chucVu.TenChucVu,
				trangThai = chucVu.TrangThai

			};
		}
		public static ChucVu ToEntity(this ChucVuCreateDto createDto)
		{
			if (createDto == null) return null;
			return new ChucVu
			{
				TenChucVu = createDto.tenChucVu,
			};
		}
	}
}
