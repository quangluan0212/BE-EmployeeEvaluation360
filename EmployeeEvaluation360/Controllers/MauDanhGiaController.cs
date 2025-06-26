using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class MauDanhGiaController : BaseController
	{
		private readonly IMauDanhGiaService _mauDanhGiaService;
		public MauDanhGiaController(IMauDanhGiaService mauDanhGiaService)
		{
			_mauDanhGiaService = mauDanhGiaService;
		}

		[HttpDelete("admin-xoa-mau-danh-gia")]
		public async Task<IActionResult> DeleteMauDanhGia(int maMau)
		{
			try
			{
				var result = await _mauDanhGiaService.DeleteMauDanhGia(maMau);
				if (result == false)
				{
					return NotFound(Error<string>("Mẫu đánh giá không tồn tại hoặc đã được sử dụng trong đánh giá."));
				}
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi xóa mẫu đánh giá: {ex.Message}");
				return StatusCode(500, Error<string>($"Có lỗi xảy ra: {ex.Message}"));
			}
		}

		[HttpPut("admin-cap-nhat-mau-danh-gia")]
		public async Task<IActionResult> UpdateMauDanhGia(int maMau, [FromBody] UpdateMauDanhGiaDto mauDanhGiaDto)
		{
			if (mauDanhGiaDto == null)
			{
				return BadRequest(Error<string>("Dữ liệu không hợp lệ"));
			}

			try
			{
				var result = await _mauDanhGiaService.UpdateMauDanhGia(maMau, mauDanhGiaDto);
				if (result == null)
				{
					return NotFound(Error<string>("Mẫu đánh giá không tồn tại hoặc cập nhật thất bại"));
				}
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi cập nhật mẫu đánh giá: {ex.Message}");
				return StatusCode(500, Error<string>($"Có lỗi xảy ra: {ex.Message}"));
			}
		}

		[HttpGet("admin-get-danh-sach-mau-danh-gia-by-mddg")]
		public async Task<IActionResult> GetMauDanhGiaByMaDotDanhGiaAsync(int MaDotDanhGia)
		{
			var result = await _mauDanhGiaService.GetMauDanhGiaByMaDotDanhGia(MaDotDanhGia);
			if (result == null || !result.Any())
			{
				return NotFound(Error<string>("Không tìm thấy mẫu đánh giá nào."));
			}
			return Ok(Success(result));
		}

		[HttpGet("admin-danh-sach-mau-danh-gia-active")]
		public async Task<IActionResult> GetAllActive()
		{
			var result = await _mauDanhGiaService.GetAllMauDanhGiaActive();
			if (result == null || !result.Any())
			{
				return NotFound(Error<string>("Không tìm thấy mẫu đánh giá nào."));
			}
			return Ok(Success(result));
		}

		[HttpGet("admin-danh-sach-mau-danh-gia")]
		public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, string? search = null)
		{
			var result = await _mauDanhGiaService.GetAllAsync(page, pageSize, search);
			if (result == null || !result.Items.Any())
			{
				return NotFound(Error<string>("Không tìm thấy mẫu đánh giá nào."));
			}	
			return Ok(Success(result));
		}

		[HttpGet("admin-get-thong-tin-mau-danh-gia")]
		public async Task<IActionResult> GetMauDanhGiaById(int maMau)
		{
			var result = await _mauDanhGiaService.GetMauDanhGiaById(maMau);
			return Ok(Success(result));
		}

		[HttpPost("tao-mau-danh-gia")]
		public async Task<IActionResult> CreateMauDanhGia([FromBody] CreateMauDanhGiaDto mauDanhGiaDto)
		{
			if (mauDanhGiaDto == null)
			{
				return BadRequest("Dữ liệu không hợp lệ");
			}
			var result = await _mauDanhGiaService.CreateMauDanhGia(mauDanhGiaDto);

			return Ok(Success(result));
		}
	}
}
