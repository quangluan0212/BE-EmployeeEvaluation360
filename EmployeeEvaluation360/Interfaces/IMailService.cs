using EmployeeEvaluation360.DTOs;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IMailService
	{
		Task<string> SendCaptchaEmailAsync(string toEmail, string captchaCode);
		Task<bool> ResetPasswordByEmailAsync(ResetPasswordRequestDto requestDto);
		Task<string> SendMailUserAddedGroup(string toEmail, string groupName, string groupId, string userName, string userId);
	}
}
