using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DotDanhGiaController :BaseController
	{
		private readonly IDotDanhGiaService _service;
		public DotDanhGiaController(IDotDanhGiaService dddService)
		{
			_service = dddService;
		}

		[HttpGet("dot-danh-gia-hien-tai")]
		public async Task<IActionResult> getDotDanhGiaHienTai()
		{
			var ddd = await _service.GetDotDanhGiaActivesAsync();
			return Ok(Success(ddd));
		}

		[HttpPost("them-dot-danh-gia")]
		public async Task<IActionResult> createDotDanhGia(CreateDotDanhGiaDto createDotDanhGiaDto)
		{
			var ddd = await _service.CreateDotDanhGia(createDotDanhGiaDto);
			if (ddd == null)
			{
				return BadRequest(Error<string>("Lỗi khi tạo đợt đánh giá !!!"));
			}
			else
			{
				return Ok(Success(ddd));
			}
		}

	}
}
