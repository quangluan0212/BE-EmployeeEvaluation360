namespace EmployeeEvaluation360.DTOs
{
	public class KetQua_DanhGiaDto
	{
		public string MaNguoiDung { get; set; } = string.Empty;
		public decimal DiemTongKet { get; set; }
		public DateTime ThoiGianTinh { get; set; } = DateTime.Now;
	}
}
