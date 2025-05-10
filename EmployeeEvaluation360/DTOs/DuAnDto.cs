using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.DTOs
{
	public class DuAnDto
	{
		public int MaDuAn { get; set; }
		public string TenDuAn { get; set; }
		public string MoTa { get; set; }
		public string TrangThai { get; set; }
	}

	public class CreateDuAnDto
	{
		[Required(ErrorMessage = "Tên dự án không được để trống")]
		[StringLength(100, ErrorMessage = "Tên dự án không được vượt quá 100 ký tự")]
		public string TenDuAn { get; set; }
		[Required(ErrorMessage = "Mô tả không được để trống")]
		public string MoTa { get; set; }
	}
	public class UpdateDuAnDto
	{
		[StringLength(100, ErrorMessage = "Tên dự án không được vượt quá 100 ký tự")]
		public string TenDuAn { get; set; }
		public string MoTa { get; set; }
		public string TrangThai { get; set; }
	}
	public class DuAnIdNameDto
	{
		public int MaDuAn { get; set; }
		public string TenDuAn { get; set; }
	}

}
