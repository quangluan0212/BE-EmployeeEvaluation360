namespace EmployeeEvaluation360.DTOs
{
	public class DotDanhGiaDto
	{
		public int MaDotDanhGia { get; set; }
		public string TenDot {  get; set; } = string.Empty;
		public DateTime ThoiGianBatDau { get; set; }
		public DateTime ThoiGianKetThuc {  get; set; }
		public string TrangThai { get; set; } = string.Empty;
	}
	public class DotDanhGiaActiveDto 
	{
		public int MaDotDanhGia { set; get; }
		public string TenDot { set; get; } = string.Empty;
	}
	
}
