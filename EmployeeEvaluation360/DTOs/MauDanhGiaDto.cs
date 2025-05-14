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

	public class DanhGiaTraLoiDto
	{
		public int MaDanhGia { get; set; }
		public List<CauHoiTraloiDto> CauHoiTraLoi { get; set; }
	}

	public class MauDanhGiaCauHoiTraLoiDto
	{
		public int MaDanhGia { get; set; }
		public List<CauTraloiDto> DanhSachCauTraLoi { get; set; }
	}
}
