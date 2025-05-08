using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.DTOs
{
	public class ChucVuDto
	{
		public int maChucVu { get; set; }

		public string tenChucVu { get; set; }

		public string trangThai { get; set; }
	}

	public class ChucVuCreateDto
	{
		[Required(ErrorMessage = "Tên chức vụ không được để trống")]
		[StringLength(50, ErrorMessage = "Tên chức vụ không được vượt quá 50 ký tự")]
		public string tenChucVu { get; set; }
	}

	public class ChucVuUpdateDto
	{
		[StringLength(50, ErrorMessage = "Tên chức vụ không được vượt quá 50 ký tự")]
		public string tenChucVu { get; set; }

		[StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
		public string trangThai { get; set; }
	}
}
