using EmployeeEvaluation360.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Database;
using Microsoft.EntityFrameworkCore;

namespace EmployeeEvaluation360.Services
{
	public class MailService : IMailService
	{
		private readonly IConfiguration _configuration;
		private readonly ApplicationDBContext _context;


		public MailService(IConfiguration configuration, ApplicationDBContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		public async Task<bool> ResetPasswordByEmailAsync(ResetPasswordRequestDto requestDto)
		{
			try
			{			

				var user = await _context.NGUOIDUNG.FirstOrDefaultAsync(u => u.Email == requestDto.Email);
				if (user == null)
				{
					return false;
				}
				user.MatKhau = BCrypt.Net.BCrypt.HashPassword(requestDto.NewPassword);
				_context.NGUOIDUNG.Update(user);
				await _context.SaveChangesAsync();
				return true;
			}
			catch(Exception ex)
			{
				throw new Exception($"Lỗi khi đặt lại mật khẩu: {ex.Message}"); // Ném lỗi để xử lý ở nơi gọi			
			}
		}

		public async Task<string> SendCaptchaEmailAsync(string toEmail, string captchaCode)
		{
			try
			{
				// Lấy cấu hình SMTP từ appsettings.json
				var smtpHost = _configuration["SmtpSettings:Host"];
				var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
				var smtpUsername = _configuration["SmtpSettings:Username"];
				var smtpPassword = _configuration["SmtpSettings:Password"];
				var senderEmail = _configuration["SmtpSettings:SenderEmail"];
				var senderName = _configuration["SmtpSettings:SenderName"];

				// Tạo email message
				var email = new MimeMessage();
				email.From.Add(new MailboxAddress(senderName, senderEmail));
				email.To.Add(new MailboxAddress("", toEmail));
				email.Subject = "Password Reset CAPTCHA";

				// Tạo nội dung email
				var bodyBuilder = new BodyBuilder
				{
					HtmlBody = $@"<!DOCTYPE html>
					<html>
						<head>
							<meta charset=""utf-8"">
							<title>Xác Thực Đặt Lại Mật Khẩu</title>
						</head>
						<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
							<h2>Xác Thực Đặt Lại Mật Khẩu</h2>
							<p>Xin chào,</p>
							<p>Mã xác thực của bạn là: <strong style=""color: #007bff;"">{captchaCode}</strong></p>
							<p>Mã này sẽ hết hạn sau 2 phút.</p>
							<p>Vui lòng nhập mã này vào ô được cung cấp để tiếp tục đặt lại mật khẩu.</p>
							<p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
							<p>Trân trọng,<br>{senderName}</p>
							<hr>
							<p style=""font-size: 12px; color: #777;"">Đây là email tự động, vui lòng không trả lời trực tiếp. Để được hỗ trợ, liên hệ qua <a href=""mailto:{senderEmail}"">{senderEmail}</a>.</p>
						</body>
					</html>"
				};
				email.Body = bodyBuilder.ToMessageBody();

				// Gửi email bằng MailKit
				using var smtp = new SmtpClient();
				await smtp.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
				await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
				await smtp.SendAsync(email);
				await smtp.DisconnectAsync(true);
				return "Email đã được gửi thành công.";
			}
			catch (Exception ex)
			{
				return $"Lỗi khi gửi email: {ex.Message}";
			}
		}
	}
}
