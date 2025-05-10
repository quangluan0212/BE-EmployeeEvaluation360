using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Mappers
{
	public static class DuAnMappers
	{
		public static DuAnDto ToDto(this DuAn duAn)
		{
			if (duAn == null) return null;
			return new DuAnDto
			{
				MaDuAn = duAn.MaDuAn,
				TenDuAn = duAn.TenDuAn,
				MoTa = duAn.MoTa,
				TrangThai = duAn.TrangThai
			};
		}
		public static DuAn ToEntity(this CreateDuAnDto createDuAnDto)
		{
			if (createDuAnDto == null) return null;
			return new DuAn
			{
				TenDuAn = createDuAnDto.TenDuAn,
				MoTa = createDuAnDto.MoTa,
				TrangThai = "Active"
			};
		}
		public static DuAn ToEntity(this UpdateDuAnDto updateDuAnDto)
		{
			if (updateDuAnDto == null) return null;
			return new DuAn
			{
				TenDuAn = updateDuAnDto.TenDuAn,
				MoTa = updateDuAnDto.MoTa,
				TrangThai = updateDuAnDto.TrangThai
			};
		}
	}
}
