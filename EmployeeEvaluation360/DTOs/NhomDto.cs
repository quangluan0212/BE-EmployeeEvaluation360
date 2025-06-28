using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.DTOs
{
	public class NhomDto
	{
		public int MaNhom { get; set; }
		public string TenNhom { get; set; } = string.Empty;
		public int MaDuAn { get; set; }
		public string TrangThai { get; set; } = string.Empty;
	}
	public class  ListNhomDuAnDto
	{
		public int MaNhom { get; set; }
		public string TenNhom { get; set; } = string.Empty;
		public string TenDuAn { get; set; } = string.Empty;
		public string TrangThai { get; set; } = string.Empty;
	}

	public class SimpleNhomDto
	{
		public int MaNhom { get; set; }
		public string TenNhom { get; set; } = string.Empty;

	}

	public class CreateNhomDto
	{
		public string TenNhom { get; set; } = string.Empty;
		public int MaDuAn { get; set; }

	}
	public class UpdateNhomDto
	{
		public string TenNhom { get; set; } = string.Empty;
		public int MaDuAn { get; set; }
		public string TrangThai { get; set; } = string.Empty;
	}
	public class ThemNhanVienVaoNhomDto
	{
		[Required]
		public int MaNhom { get; set; }
		[Required]
		public List<string> MaNguoiDung { get; set; } = [];
	}

	public class Nhom_NguoiDungDto
	{
		public int MaNhom { get; set; }
		public string MaNguoiDung { get; set; } = string.Empty;
		public string HoTen { get; set; } = string.Empty;
		public string ChucVu { get; set; } = string.Empty;
		public string TrangThai { get; set; } = string.Empty;
	}

	public class NhomVaThanhVienDto
	{
		public int MaNhom { get; set; }
		public string TenNhom { get; set; } = string.Empty;
		public List<ThanhVienDto> ThanhVien { get; set; } = new List<ThanhVienDto>();
	}

	public class ThanhVienDto
	{
		public int MaNhomNguoiDung { get; set; }
		public string MaNguoiDung { get; set; } = string.Empty;
		public string HoTen { get; set; } = string.Empty;
		public string SoDienThoai { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string ChucVu { get; set; } = string.Empty;
	}
}
