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
	[Authorize]
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
		public async Task<ActionResult> GetAllNguoiDung()
		{
			var nguoiDungs = await _nguoiDungService.GetAllNguoiDungAsync();
			return Ok(Success(nguoiDungs));
		}

		[HttpPost]
		[Route("create")]
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

		[HttpPost("login")]
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
			return Ok(Success(new { data }, "Đăng nhập thành công !!!"));
		}

		[HttpGet]
		[Route("nguoidung/getchucvu")]
		public async Task<ActionResult> GetChucVuByMaNguoiDung(string maNguoiDung)
		{
			var nguoiDung = await _nguoiDungService.GetNguoiDungByIdAsync(maNguoiDung);
			if (nguoiDung == null)
			{
				return NotFound(Error<string>("Không tìm thấy người dùng"));
			}
			var chucVu = await _nguoiDungService.GetChucVuByNguoiDungAsync(maNguoiDung);
			if (chucVu == null)
			{
				return NotFound(Error<string>("Không tìm thấy chức vụ"));
			}
			return Ok(Success(chucVu));
		}

		[HttpPost("nguoidung/themchucvu")]
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
	}
}
