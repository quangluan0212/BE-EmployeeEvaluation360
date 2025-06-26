using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeEvaluation360.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class KetQuaDanhGiaController : BaseController
	{
		private readonly IKetQuaDanhGiaService _service;
		public KetQuaDanhGiaController(IKetQuaDanhGiaService ketQuaDanhGiaService)
		{
			_service = ketQuaDanhGiaService;
		}

		[HttpGet("user-get-ket-qua-danh-gia")]
		public async Task<IActionResult> UserGetKetQuaDanhGia(string maNguoiDung)
		{
			try
			{
				if (maNguoiDung.IsNullOrEmpty())
				{
					return BadRequest(Error<string>("Đã có lỗi xảy ra !!!"));
				}
				var ketQuaDanhGia = await  _service.getKetQuaDanhGiaByMaNguoiDung(maNguoiDung);
				return Ok(Success(ketQuaDanhGia));
			}
			catch (Exception ex)
			{
				return BadRequest(Error<string>(ex.Message)); ;
			}
		
		}

		[Authorize(Roles ="Admin")]
		[HttpGet("get-all-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			try
			{
				var result = await _service.GetAllPaged(page, pageSize, search, maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-latest-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetCurrentKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			try
			{
				var result = await _service.GetLatestPaged(page, pageSize, search, maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-good-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetGoodKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			try
			{
				var result = await _service.GetGoodCurrentPaged(page, pageSize, search, maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-bad-ket-qua-danh-gia-paged")]
		public async Task<IActionResult> GetBadKetQuaDanhGiaPagedResult(int page = 1, int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			try
			{
				var result = await _service.GetBadCurrentPaged(page, pageSize, search, maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-all-ket-qua-danh-gia")]
		public async Task<IActionResult> GetKetQuaDanhGiaResult(int? maDotDanhGia = null)
		{
			try
			{
				var result = await _service.GetAll(maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-latest-ket-qua-danh-gia")]
		public async Task<IActionResult> GetCurrentKetQuaDanhGiaResult(int? maDotDanhGia)
		{
			try
			{
				var result = await _service.GetLatest(maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-good-ket-qua-danh-gia")]
		public async Task<IActionResult> GetGoodKetQuaDanhGiaResult(int? maDotDanhGia)
		{
			try
			{
				var result = await _service.GetGoodCurrent(maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-bad-ket-qua-danh-gia")]
		public async Task<IActionResult> GetBadKetQuaDanhGiaResult(int? maDotDanhGia)
		{
			try
			{
				var result = await _service.GetBadCurrent(maDotDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
