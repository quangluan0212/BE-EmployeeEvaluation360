using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.DTOs
{
	public class NguoiDungDto
	{
		public string MaNguoiDung { get; set; }

		public string HoTen { get; set; }

		public string Email { get; set; }

		public string DienThoai { get; set; }

		public DateTime? NgayVaoCongTy { get; set; }

		public string TrangThai { get; set; }
	}

	public class CreateNguoiDungDto
	{
		[Required(ErrorMessage = "Họ tên không được để trống")]
		[StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
		public string HoTen { get; set; }

		[Required(ErrorMessage = "Email không được để trống")]
		[EmailAddress(ErrorMessage = "Email không đúng định dạng")]
		[StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
		public string Email { get; set; }

		[StringLength(12, ErrorMessage = "Số điện thoại không được vượt quá 12 ký tự")]
		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa chữ số")]
		public string DienThoai { get; set; }

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		[StringLength(60, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 60 ký tự")]
		public string MatKhau { get; set; }
		public int MaChucVu { get; set; }

	}

	public class UpdateNguoiDungDto
	{
		[Required(ErrorMessage = "Họ tên không được để trống")]
		[StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
		public string HoTen { get; set; }

		[Required(ErrorMessage = "Email không được để trống")]
		[EmailAddress(ErrorMessage = "Email không đúng định dạng")]
		[StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
		public string Email { get; set; }

		[StringLength(12, ErrorMessage = "Số điện thoại không được vượt quá 12 ký tự")]
		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa chữ số")]
		public string DienThoai { get; set; }

		public DateTime NgayVaoCongTy { get; set; }

		[Required(ErrorMessage = "Trạng thái không được để trống")]
		[StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
		public string TrangThai { get; set; } = "Active";

		// Mật khẩu là tùy chọn khi cập nhật
		[StringLength(60, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 60 ký tự")]
		public string MatKhau { get; set; }
	}

	public class ChangePasswordDto
	{
		[Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = "Mật khẩu mới không được để trống")]
		[StringLength(60, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 60 ký tự")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
		public string ConfirmPassword { get; set; }
	}

	public class ResetPasswordDto
	{
		[Required(ErrorMessage = "Mật khẩu mới không được để trống")]
		[StringLength(60, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 60 ký tự")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginDto
	{
		[Required(ErrorMessage = "Mã người dùng không được để trống")]
		public string MaNguoiDung { get; set; }

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string MatKhau { get; set; }
	}

	public class AddChucVuDto
	{
		[Required(ErrorMessage = "Mã người dùng không được để trống")]
		public string MaNguoiDung { get; set; }
		[Required(ErrorMessage = "Mã chức vụ không được để trống")]
		public int MaChucVu { get; set; }
	}
}
