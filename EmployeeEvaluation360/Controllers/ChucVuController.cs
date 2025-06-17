using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
using EmployeeEvaluation360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class ChucVuController : BaseController
	{
		private readonly IChucVuService _chucVuService;

		public ChucVuController(IChucVuService chucVuService)
		{
			_chucVuService = chucVuService;
		}

		[HttpGet("admin-get-cap-bac-nguoi-dung")]
		public async Task<IActionResult> GetCapBacChucVu(string maNguoiDung, int maChucVu)
		{
			if (string.IsNullOrEmpty(maNguoiDung) || maChucVu <= 0)
			{
				return BadRequest(Error<string>("Mã người dùng hoặc mã chức vụ không hợp lệ."));
			}
			try
			{
				var capBac = await _chucVuService.GetCapBacChucVu(maNguoiDung, maChucVu);
				return Ok(Success(capBac));
			}
			catch (Exception ex)
			{
				return NotFound(Error<string>(ex.Message));
			}
		}

		[HttpPut("admin-cap-nhat-chuc-vu")]
		public async Task<IActionResult> CapNhatChucVu([FromBody] CapNhatChucVuDto capNhatChucVu)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(Error<string>("Đã xảy ra lỗi !!!"));
			}

			try
			{
				var result = await _chucVuService.CapNhatChucVuChoNguoiDung(capNhatChucVu);
				return Ok(Success(result));
			}
			catch (ArgumentException ex)
			{
				return BadRequest(Error<string>(ex.Message));
			}
			catch (Exception ex)
			{
				return NotFound(Error<string>(ex.Message));
			}
		}

		[HttpGet]
		[Route("danh-sach-chuc-vu")]
		public async Task<ActionResult> GetChucVus()
		{
			var chucVus = await _chucVuService.GetAllChucVuAsync();
			if (chucVus == null || !chucVus.Any())
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}
			var chucVuDtos = chucVus.Select(c => c.ToDto()).ToList();
			return Ok(Success(chucVuDtos));
		}

		[HttpGet]
		[Route("danh-sach-chuc-vu-paged")]
		public async Task<ActionResult> GetChucVusPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? search = null)
		{
			var chucVus = await _chucVuService.GetAllChucVuPagedAsync(page, pageSize, search);
			return Ok(Success(chucVus));
		}


		[HttpGet("chi-tiet-chuc-vu")]
		public async Task<ActionResult<ChucVu>> GetChucVu(int id)
		{
			var chucVu = await _chucVuService.GetChucVuByIdAsync(id);

			if (chucVu == null)
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}

			return Ok(Success(chucVu.ToDto()));
		}


		[HttpPost]
		[Route("them-chuc-vu")]
		public async Task<ActionResult<ChucVu>> CreateChucVu(ChucVuCreateDto chucVuDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var chucVu = new ChucVu
			{
				TenChucVu = chucVuDto.tenChucVu,
				TrangThai = "Active"
			};

			var createdChucVu = await _chucVuService.CreateChucVuAsync(chucVu);
			if (createdChucVu == null)
			{
				return BadRequest(new { message = "Không thể tạo chức vụ" });
			}
			return Ok(Success(chucVu.ToDto()));
		}

		[HttpPut]
		[Route("cap-nhat-chuc-vu")]
		public async Task<ActionResult<ChucVu>> UpdateChucVu(int id, ChucVuUpdateDto chucVuDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var chucVu = await _chucVuService.GetChucVuByIdAsync(id);
			if (chucVu == null)
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}
			chucVu.TenChucVu = chucVuDto.tenChucVu;
			chucVu.TrangThai = chucVuDto.trangThai;
			var updatedChucVu = await _chucVuService.UpdateChucVuAsync(id, chucVu);
			if (updatedChucVu == null)
			{
				return BadRequest(new { message = "Không thể cập nhật chức vụ" });
			}
			return Ok(Success(updatedChucVu.ToDto()));
		}
		[HttpDelete]
		[Route("xoa-chuc-vu")]
		public async Task<ActionResult> DeleteChucVu(int id)
		{
			var chucVu = await _chucVuService.GetChucVuByIdAsync(id);
			if (chucVu == null)
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}

			var nguoiDungs = await _chucVuService.GetNguoiDungByChucVuAsync(id);
			if (nguoiDungs != null && nguoiDungs.Any())
			{
				chucVu.TrangThai = "Deleted";
				var updatedChucVu = await _chucVuService.UpdateChucVuAsync(id, chucVu);
				if (updatedChucVu == null)
				{
					return BadRequest(new { message = "Không thể cập nhật trạng thái chức vụ" });
				}
				return Ok(Success(updatedChucVu.ToDto()));
			}

			var isDeleted = await _chucVuService.DeleteChucVuAsync(id);
			if (!isDeleted)
			{
				return BadRequest(new { message = "Không thể xóa chức vụ" });
			}

			return Ok(Success(new { message = "Xóa chức vụ thành công" }));
		}
	}
}
