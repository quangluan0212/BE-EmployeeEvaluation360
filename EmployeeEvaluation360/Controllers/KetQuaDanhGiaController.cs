using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KetQuaDanhGiaController : BaseController
	{
		private readonly IKetQuaDanhGiaService _service;
		public KetQuaDanhGiaController(IKetQuaDanhGiaService ketQuaDanhGiaService)
		{
			_service = ketQuaDanhGiaService;
		}
		[HttpGet("GetKetQuaDanhGia")]
		public async Task<IActionResult> GetKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetAll(page, pageSize, search);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("GetCurrentKetQuaDanhGia")]
		public async Task<IActionResult> GetCurrentKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetCurrent(page, pageSize, search);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpGet("GetGoodKetQuaDanhGia")]
		public async Task<IActionResult> GetGoodKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetGoodCurrent(page, pageSize, search);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpGet("GetBadKetQuaDanhGia")]
		public async Task<IActionResult> GetBadKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetBadCurrent(page, pageSize, search);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
