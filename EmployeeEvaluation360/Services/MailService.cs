using EmployeeEvaluation360.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Database;
using Microsoft.EntityFrameworkCore;
using System.Security;

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
			catch (Exception ex)
			{
				throw new Exception($"Lỗi khi đặt lại mật khẩu: {ex.Message}");
			}
		}

		public async Task<string> SendCaptchaEmailAsync(string toEmail, string captchaCode)
		{
			try
			{
				var smtpHost = _configuration["SmtpSettings:Host"];
				var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
				var smtpUsername = _configuration["SmtpSettings:Username"];
				var smtpPassword = _configuration["SmtpSettings:Password"];
				var senderEmail = _configuration["SmtpSettings:SenderEmail"];
				var senderName = _configuration["SmtpSettings:SenderName"];

				var email = new MimeMessage();
				email.From.Add(new MailboxAddress(senderName, senderEmail));
				email.To.Add(new MailboxAddress("", toEmail));
				email.Subject = "Password Reset CAPTCHA";

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

		public async Task<string> SendMailUserAddedGroup(string toEmail, string groupName, string groupId, string userName, string userId)
		{
			try
			{
				// Kiểm tra đầu vào
				if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(groupName) ||
					string.IsNullOrWhiteSpace(groupId) || string.IsNullOrWhiteSpace(userName) ||
					string.IsNullOrWhiteSpace(userId))
				{
					return "Thông tin email, nhóm hoặc người dùng không hợp lệ.";
				}

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
				email.Subject = $"Thông báo: Bạn đã được thêm vào nhóm {groupName}";

				// Tạo nội dung email HTML
				var bodyBuilder = new BodyBuilder
				{
					HtmlBody = $@"<!DOCTYPE html>
                    <html>
                        <head>
                            <meta charset=""utf-8"">
                            <title>Thông báo thêm vào nhóm</title>
                            <style>
                                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
                                .header {{ background-color: #f8f8f8; padding: 10px; text-align: center; border-radius: 5px 5px 0 0; }}
                                .content {{ padding: 20px; }}
                                .footer {{ text-align: center; font-size: 12px; color: #777; margin-top: 20px; }}
                                .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px; }}
                            </style>
                        </head>
                        <body>
                            <div class=""container"">
                                <div class=""header"">
                                    <h2>Thông báo từ Hệ thống đánh giá nhân sự</h2>
                                </div>
                                <div class=""content"">
                                    <p>Xin chào <strong>{SecurityElement.Escape(userName)}</strong>,</p>
                                    <p>Bạn đã được thêm vào nhóm <strong>{SecurityElement.Escape(groupName)}</strong>.</p>
                                    <p>Thông tin người dùng:</p>
                                    <ul>
                                        <li><strong>Mã người dùng:</strong> {SecurityElement.Escape(userId)}</li>
                                        <li><strong>Tên người dùng:</strong> {SecurityElement.Escape(userName)}</li>
                                    </ul>
                                    <p>Vui lòng đăng nhập vào hệ thống để xem chi tiết nhóm và bắt đầu tham gia.</p>
                                    <p style=""text-align: center;"">
                                        <a href=""https://quangluanle.id.vn/group-members-page"" class=""button"">Xem chi tiết nhóm</a>
                                    </p>
                                </div>
                                <div class=""footer"">
                                    <p>Đây là email tự động, vui lòng không trả lời trực tiếp. Để được hỗ trợ, liên hệ qua <a href=""mailto:{senderEmail}"">{senderEmail}</a>.</p>
                                    <p>© {DateTime.Now.Year} Hệ thống đánh giá nhân sự</p>
                                </div>
                            </div>
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