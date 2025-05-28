namespace EmployeeEvaluation360.DTOs
{
	public class KetQua_DanhGiaDto
	{
		public string MaNguoiDung { get; set; } = string.Empty;
		public decimal DiemTongKet { get; set; }
		public DateTime ThoiGianTinh { get; set; } = DateTime.Now;
		public int MaDotDanhGia { get; set; }
	}
	public class KetQua_DanhGiaChiTietDto
	{
		public int MaKetQuaDanhGia { get; set; }
		public string MaNguoiDung { get; set; } = string.Empty;
		public string HoTen { get; set; } = string.Empty;
		public decimal DiemTongKet { get; set; }
		public DateTime ThoiGianTinh { get; set; } = DateTime.Now;
		public int MaDotDanhGia { get; set; }
		public string TenDotDanhGia { get; set; } = string.Empty;
	}
}
