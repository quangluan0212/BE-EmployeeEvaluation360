using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.DTOs
{
	public class MauDanhGiaDto
	{
	}
	public class MauDanhGiaCauHoiDto
	{
		public int MaDanhGia {  get; set; }
		public int MaMauDanhGia {  get; set; }
		public string TenDotDanhGia { get; set;}
		public List<CauHoiDto> DanhSachCauHoi { get; set; }
	}
}
