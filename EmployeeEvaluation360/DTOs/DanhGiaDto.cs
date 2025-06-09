namespace EmployeeEvaluation360.DTOs
{
	public class DanhGiaDto
	{
		public int MaDanhGia { get; set; }
		public string MaNguoiDanhGia { get; set; } = string.Empty;
		public string NguoiDanhGia { get; set; } = string.Empty;
		public string maNguoiDuocDanhGia { get; set; } = string.Empty;
		public string NguoiDuocDanhGia { get; set; } = string.Empty;
		public int MaDotDanhGia { get; set; }
		public string TenDotDanhGia { get; set; } = string.Empty;
		public decimal Diem { get; set; }
	}

	public class NguoiThieuDanhGiaDto
	{
		public string MaNguoiDung { get; set; } = string.Empty;
		public string HoTen { get; set; } = string.Empty;
		public string TenChucVu { get; set; } = string.Empty;
		public int SoDanhGiaConThieu { get; set; }
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
		public string MaNguoiDuocDanhGia { set; get; } = string.Empty;
		public string HoTen { set; get; } = string.Empty;
		public string TenChucVu { set; get; } = string.Empty ;
		public string TrangThai { set; get; } = string.Empty;
	}
}
