using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Mappers
{
	public static class NhomMappers
	{
		public static NhomDto ToDto(this Nhom nhom)
		{
			return new NhomDto
			{
				Id = nhom.MaNhom,
				TenNhom = nhom.TenNhom,
				MaDuAn = nhom.MaDuAn,
				TrangThai = nhom.TrangThai
			};
		}
		public static Nhom ToEntity(this NhomDto nhomDto)
		{
			return new Nhom
			{
				MaNhom = nhomDto.Id,
				TenNhom = nhomDto.TenNhom,
				MaDuAn = nhomDto.MaDuAn,
				TrangThai = nhomDto.TrangThai
			};
		}
		public static Nhom CreateToEntity(this CreateNhomDto createNhomDto)
		{
			return new Nhom
			{
				TenNhom = createNhomDto.TenNhom,
				MaDuAn = createNhomDto.MaDuAn,
				TrangThai = "Active"
			};
		}
		public static Nhom UpdateToEntity(this UpdateNhomDto updateNhomDto)
		{
			return new Nhom
			{
				TenNhom = updateNhomDto.TenNhom,
				MaDuAn = updateNhomDto.MaDuAn,
				TrangThai = updateNhomDto.TrangThai
			};
		}
	}
}
