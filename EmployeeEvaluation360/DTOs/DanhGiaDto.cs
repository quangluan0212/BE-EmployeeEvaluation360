namespace EmployeeEvaluation360.DTOs
{
	public class DanhGiaDto
	{
	}
	public class TaoDanhGiaDto
	{
		public string NguoiDanhGia { get; set; } = string.Empty;
		public int NguoiDuocDanhGia { get; set; }
		public int MaDotDanhGia { get; set; }
	}

	public class AdminGetDanhGiaDto
	{
		public int MaDanhGia { get; set; }
		public string MaNguoiDanhGia { set; get; } = string.Empty;
		public int MaNhomNguoiDung { set; get; }
		public string HotTen {  set; get; } = string.Empty;
		public string TrangThai {  set; get; } = string.Empty;
	}

	public class NhanVienGetDanhGiaDto 
	{
		public string TenNhom { set; get; } = string.Empty;
		public List<ThanhVienDanhGiaCheoDto> thanhViens { get; set; }
	}
	public class ThanhVienDanhGiaCheoDto
	{
		public int MaDanhGia { get; set; }
		public string HoTen { set; get; } = string.Empty;
		public string TenChucVu { set; get; } = string.Empty ;
		public string TrangThai { set; get; } = string.Empty;
	}
}
