namespace EmployeeEvaluation360.DTOs
{
	public class AuthDto
	{
	}
	public class ForgotPasswordRequestDto
	{
		public string Email { get; set; }
	}

	public class VerifyCaptchaRequestDto
	{
		public string Email { get; set; }
		public string CaptchaCode { get; set; }
	}

	public class ResetPasswordRequestDto
	{
		public string Email { get; set; }
		public string NewPassword { get; set; }
		public string CaptchaCode { get; set; }
	}
}
