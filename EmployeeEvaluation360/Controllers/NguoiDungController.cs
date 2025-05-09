using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class NguoiDungController : BaseController
	{
		private readonly INguoiDungService _nguoiDungService;
		private readonly ITokenService _tokenService;

		public NguoiDungController(INguoiDungService nguoiDungService, ITokenService tokenService)
		{
			_nguoiDungService = nguoiDungService;
			_tokenService = tokenService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		[Route("admin-danh-sach-nguoi-dung")]
		public async Task<ActionResult> GetAllNguoiDung(int page = 1, int pageSize = 10)
		{
			var result = await _nguoiDungService.GetNguoiDungPagedAsync(page, pageSize);
			return Ok(Success(result));
		}

		[HttpPost]
		[Route("them-nguoi-dung")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> CreateNguoiDung([FromBody] CreateNguoiDungDto nguoiDung)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var createdNguoiDung = await _nguoiDungService.CreateNguoiDungAsync(nguoiDung.ToEntity(), nguoiDung.MaChucVu);
			if (createdNguoiDung == null)
			{
				return BadRequest(Error<string>("Email already exists"));
			}
			return Ok(Success(createdNguoiDung.ToDto()));
		}

		[HttpGet]
		[Route("danh-sach-nguoi-dung-chuc-vu")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> GetAllNguoiDungWithRole(int page = 1, int pageSize = 10,string? search = null)
		{
			var result = await _nguoiDungService.GetNguoiDungWithRolePagedAsync(page, pageSize, search);
			return Ok(Success(result));
		}

		[HttpPost("dang-nhap")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			NguoiDung user = await _nguoiDungService.GetNguoiDungByIdAsync(loginDto.MaNguoiDung);
			if (user == null)
			{
				return Unauthorized();
			}
			var result = await _nguoiDungService.ValidatePasswordAsync(loginDto.MaNguoiDung, loginDto.MatKhau);
			if (!result)
			{
				return Unauthorized("Email hoặc Mật khẩu không chính xác !!!");
			}
			// Tạo token cho người dùng
			string token = _tokenService.GenerateToken(user);
			var data = new
			{
				MaNguoiDung = user.MaNguoiDung,
				HoTen = user.HoTen,
				Token = token
			};
			return Ok(Success(data));
		}

		[HttpGet]
		[Route("chi-tiet-nguoi-dung/{maNguoiDung}")]
		[Authorize]
		public async Task<ActionResult> ChiTietNguoiDung(string maNguoiDung)
		{
			var nguoiDung = await _nguoiDungService.GetNguoiDungByIdAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return NotFound(Error<string>("Không tìm thấy người dùng"));
			}
			return Ok(Success(nguoiDung.ChiTietNguoiDungDto()));
		}

		[HttpPost("them-chuc-vu")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> AddChucVuToNguoiDung([FromBody] AddChucVuDto addChucVuDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var nguoiDungExists = await _nguoiDungService.NguoiDungExistsAsync(addChucVuDto.MaNguoiDung);
			if (!nguoiDungExists)
			{
				return NotFound(Error<string>("Không tìm thấy người dùng"));
			}

			var result = await _nguoiDungService.AddChucVuAsync(addChucVuDto.MaNguoiDung, addChucVuDto.MaChucVu, 0);
			if (!result)
			{
				return BadRequest(Error<string>("Không thể thêm chức vụ cho người dùng"));
			}

			return Ok(Success<string>("Thêm chức vụ thành công"));
		}

		[HttpPut("user-cap-nhat-thong-tin-nguoi-dung")]
		[Authorize]
		public async Task<ActionResult> UpdateNguoiDung(string maNguoiDung, [FromBody] UpdateNguoiDungDto updateDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var nguoiDung = await _nguoiDungService.GetNguoiDungByIdAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return NotFound(Error<string>("Không tìm thấy người dùng"));
			}
			var updatedNguoiDung = await _nguoiDungService.UpdateNguoiDungAsync(maNguoiDung, updateDto);
			if (updatedNguoiDung == null)
			{
				return BadRequest(Error<string>("Cập nhật người dùng thất bại"));
			}
			return Ok(Success(updatedNguoiDung.ToDto()));
		}

		[HttpPut("doi-mat-khau")]
		[Authorize]
		public async Task<ActionResult> ChangePassword(string maNguoiDung, [FromBody] ChangePasswordDto changePasswordDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _nguoiDungService.ChangePasswordAsync(maNguoiDung,changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
			if (!result)
			{
				return BadRequest(Error<string>("Đổi mật khẩu thất bại"));
			}
			return Ok(Success<string>("Đổi mật khẩu thành công"));
		}
		[HttpPut]
		[Route("admin-cap-nhat-thong-tin-nguoi-dung")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> AdminUpdateNguoiDung(string maNguoiDung, [FromBody] AdminUpdateNguoiDungDto updateDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var nguoiDung = await _nguoiDungService.GetNguoiDungByIdAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return NotFound(Error<string>("Không tìm thấy người dùng"));
			}
			var updatedNguoiDung = await _nguoiDungService.AdminUpdateNguoiDungAsync(maNguoiDung, updateDto);
			if (updatedNguoiDung == null)
			{
				return BadRequest(Error<string>("Cập nhật người dùng thất bại"));
			}
			return Ok(Success(updatedNguoiDung.ToDto()));
		}
	}
}
