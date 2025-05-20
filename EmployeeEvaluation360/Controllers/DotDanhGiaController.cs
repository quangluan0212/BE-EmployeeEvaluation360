using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DotDanhGiaController : BaseController
	{
		private readonly IDotDanhGiaService _service;
		public DotDanhGiaController(IDotDanhGiaService dddService)
		{
			_service = dddService;
		}

		[HttpPut("admin-update-dot-danh-gia")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateDotDanhGia(UpdateDotDanhGia updateDotDanhGia)
		{
			var ddd = await _service.UpdateDotDanhGia(updateDotDanhGia);
			if (ddd == null)
			{
				return BadRequest(Error<string>("Lỗi khi cập nhật đợt đánh giá !!!"));
			}
			else
			{
				return Ok(Success(ddd));
			}
		}

		[HttpPut("admin-ket-thuc-dot-danh-gia")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> KetThucDotDanhGia(int maDotDanhGia)
		{
			var ddd = await _service.KetThucDotDanhGia(maDotDanhGia);
			if (ddd == null)
			{
				return BadRequest(Error<string>("Lỗi khi kết thúc đợt đánh giá !!!"));
			}
			else
			{
				return Ok(Success(ddd));
			}
		}

		[HttpGet("admin-get-danh-sach-dot-danh-gia")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> getDanhSachDotDanhGia()
		{
			var ddd = await _service.getDanhSachDotDanhGia();
			if (ddd == null)
			{
				return BadRequest(Error<string>("Lỗi khi lấy danh sách đợt đánh giá !!!"));
			}
			else		
				return Ok(Success(ddd));				
		}


		[HttpGet("dot-danh-gia-hien-tai")]
		[Authorize]
		public async Task<IActionResult> getDotDanhGiaHienTai()
		{
			var ddd = await _service.GetDotDanhGiaActivesAsync();
			return Ok(Success(ddd));
		}

		[HttpPost("them-dot-danh-gia")]
		[Authorize(Roles = "Admin")]
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
