using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	public class KetQuaDanhGiaController : BaseController
	{
		private readonly IKetQuaDanhGiaService _service;
		public KetQuaDanhGiaController(IKetQuaDanhGiaService ketQuaDanhGiaService)
		{
			_service = ketQuaDanhGiaService;
		}

		[Authorize(Roles ="Admin")]
		[HttpGet("get-all-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetAllPaged(page, pageSize, search);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		//[Authorize(Roles = "Admin")]
		[HttpGet("get-latest-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetCurrentKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null, int? year = null)
		{
			try
			{
				var result = await _service.GetLatestPaged(page, pageSize, search, year);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-good-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetGoodKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetGoodCurrentPaged(page, pageSize, search);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-bad-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetBadKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null)
		{
			try
			{
				var result = await _service.GetBadCurrentPaged(page, pageSize, search);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-all-ket-qua-danh-gia")]
		public async Task<IActionResult> GetKetQuaDanhGiaResult()
		{
			try
			{
				var result = await _service.GetAll();
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-latest-ket-qua-danh-gia")]
		public async Task<IActionResult> GetCurrentKetQuaDanhGiaResult()
		{
			try
			{
				var result = await _service.GetLatest();
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-good-ket-qua-danh-gia")]
		public async Task<IActionResult> GetGoodKetQuaDanhGiaResult()
		{
			try
			{
				var result = await _service.GetGoodCurrent();
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-bad-ket-qua-danh-gia")]
		public async Task<IActionResult> GetBadKetQuaDanhGiaResult()
		{
			try
			{
				var result = await _service.GetBadCurrent();
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
