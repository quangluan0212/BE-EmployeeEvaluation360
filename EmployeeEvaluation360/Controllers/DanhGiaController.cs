using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DanhGiaController : BaseController
	{
		private readonly IDanhGiaService _danhGiaService;
		public DanhGiaController(IDanhGiaService danhGiaService)
		{
			_danhGiaService = danhGiaService;
		}

		[HttpGet]
		[Route("danh-gia")]
		public async Task<IActionResult> DanhGia([FromQuery] string nguoiDanhGia,[FromQuery] int nguoiDuocDanhGia,[FromQuery] int maDotDanhGia)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var formDanhGia = await _danhGiaService.GetFormDanhGiaCauHoiAsync(nguoiDanhGia, nguoiDuocDanhGia, maDotDanhGia);
			if (formDanhGia != null)
			{
				return Ok(Success(formDanhGia));
			}
			return BadRequest(ModelState);
		}
	}
}
