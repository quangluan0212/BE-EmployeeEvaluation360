using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : BaseController
	{
		private readonly IMailService _mailService;
		private readonly IMemoryCache _memoryCache;
		private readonly ApplicationDBContext _context;
		public AuthController(IMailService mailService, IMemoryCache memoryCache, ApplicationDBContext context)
		{
			_mailService = mailService;
			_memoryCache = memoryCache;
			_context = context;
		}

		[HttpPost("send-reset-email")]
		public async Task<IActionResult> SendResetEmail([FromBody] ForgotPasswordRequestDto request)
		{
			try
			{
				var user = await _context.NGUOIDUNG.FirstOrDefaultAsync(u => u.Email == request.Email);
				if (user == null)
				{
					return BadRequest(Error<string>("Email không tồn tại trong hệ thống"));
				}
				var captchaCode = new Random().Next(100000, 999999).ToString();

				var cacheEntryOptions = new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4)
				};
				_memoryCache.Set($"Captcha_{request.Email}", captchaCode, cacheEntryOptions);

				string result = await _mailService.SendCaptchaEmailAsync(request.Email, captchaCode);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(Error<string>(ex.Message));
			}
		}

		[HttpPut("user-reset-password")]
		public async Task<IActionResult> UserResetPassword([FromBody] ResetPasswordRequestDto request)
		{
			if (!_memoryCache.TryGetValue($"Captcha_{request.Email}", out string captchaVerified))
			{
				return BadRequest(Error<string>("Bạn chưa xác thực CAPTCHA hoặc CAPTCHA đã hết hạn."));
			}
			_memoryCache.Remove($"Captcha_{request.Email}");
			var result = await _mailService.ResetPasswordByEmailAsync(request);
			if (result)
			{
				return Ok(Success("Mật khẩu đã được đặt lại thành công !!!"));
			}
			else
			{
				return BadRequest(Error<string>("Đã xảy ra lỗi khi cập nhật mật khẩu !!!"));
			}
		}


		[HttpPost("verify-captcha")]
		public IActionResult VerifyCaptcha([FromBody] VerifyCaptchaRequestDto request)
		{
			try
			{
				if (_memoryCache.TryGetValue($"Captcha_{request.Email}", out string storedCaptcha))
				{
					if (storedCaptcha == request.CaptchaCode)
					{
						return Ok(Success(new { result = "CAPTCHA verified successfully" }));
					}
					return BadRequest(Error<string>("Invalid CAPTCHA code"));
				}
				return BadRequest(Error<string>("CAPTCHA code has expired or does not exist" ));
			}
			catch (Exception ex)
			{
				return BadRequest(Error<string>(ex.Message));
			}
		}
	}
}	

