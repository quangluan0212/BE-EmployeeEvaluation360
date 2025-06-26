using EmployeeEvaluation360.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.DTOs
{
	public class MauDanhGiaDto
	{
		public int MaMauDanhGia { get; set; }
		public string TenMauDanhGia { get; set; } = string.Empty;
		public string LoaiDanhGia { get; set; } = string.Empty;
		public string TrangThai { get; set; } = string.Empty;
	}
	public class MauDanhGiaCauHoiDto
	{
		public int MaDanhGia { get; set; }
		public int MaMauDanhGia { get; set; }
		public string TenDotDanhGia { get; set; }
		public List<CauHoiDto> DanhSachCauHoi { get; set; }
	}
	public class DDD_MauDanhGiaDto
	{
		public int MaDanhGia { get; set; }
		public string LoaiDanhGia { get; set; } = string.Empty;
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
	public class CreateMauDanhGiaDto
	{
		[Required(ErrorMessage = "Tên mẫu không được để trống")]
		public string TenMau { get; set; } = string.Empty;
		[Required(ErrorMessage = "Trạng thái không được để trống")]
		public string LoaiDanhGia { get; set; } = string.Empty;
		[Required(ErrorMessage = "Danh sách câu hỏi không được để trống")]
		public List<CreateCauHoiDto> DanhSachCauHoi { get; set; }
	}

	public class UpdateMauDanhGiaDto
	{
		public string TenMau { get; set; } = string.Empty;
		public string LoaiDanhGia { get; set; } = string.Empty;
		public List<CauHoiDto> DanhSachCauHoi { get; set; }
	}
	public class GetMauDanhGia
	{
		public int MaMauDanhGia { get; set; }
		public string TenMauDanhGia { get; set; } = string.Empty;
		public string LoaiDanhGia { get; set; } = string.Empty;
		public string TrangThai { get; set; } = string.Empty;
		public List<CauHoiDto> DanhSachCauHoi { get; set; }
	}
}
