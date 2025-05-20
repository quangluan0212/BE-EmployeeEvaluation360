using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class MauDanhGiaController : BaseController
	{
		private readonly IMauDanhGiaService _mauDanhGiaService;
		public MauDanhGiaController(IMauDanhGiaService mauDanhGiaService)
		{
			_mauDanhGiaService = mauDanhGiaService;
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

		[HttpGet("thong-tin-mau-danh-gia/{maMau}")]
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
