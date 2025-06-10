using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	//[Authorize(Roles = "Admin")]
	public class DuAnController : BaseController
	{
		private readonly IDuAnService _duAnService;
		public DuAnController(IDuAnService duAnService) 
		{
			_duAnService = duAnService;
		}
		[HttpPut("ket-thuc-du-an")]
		public async Task<IActionResult> UpdateDuAn(int maDuAn)
		{
			var result = await _duAnService.KetThucDuAnAsync(maDuAn);
			if (result == false)
			{
				return NotFound(Error<string>("Kết thúc dự án không thành công."));
			}
			return Ok(Success<string>("Kết thúc dự án thành công."));
		}

		[HttpGet("simple-danh-sach-du-an")]
		public async Task<IActionResult> GetSimpleDanhSachDuAn()
		{
			var duans = await _duAnService.GetAllDuAnAsync();
			var duansDto = duans.Select(x => new DuAnIdNameDto
			{
				MaDuAn = x.MaDuAn,
				TenDuAn = x.TenDuAn,
			});
			return Ok(Success(duansDto));
		}

		[HttpGet("danh-sach-du-an")]
		public async Task<IActionResult> GetDanhSachDuAn(int page = 1, int pageSize = 10, string? search = null)
		{
			var result = await _duAnService.GetAllDuAnPagedAsync(page, pageSize, search);
			if (result == null)
			{
				return NotFound(Error<string>("Không tìm thấy dự án nào."));
			}
			return Ok(Success(result));
		}

		[HttpPost("them-du-an")]
		public async Task<IActionResult> ThemDuAn([FromBody] CreateDuAnDto createDuAnDto)
		{
			if(ModelState.IsValid == false)
			{
				return BadRequest(Error<string>("Dữ liệu không hợp lệ."));
			}
			var result = await _duAnService.ThemDuAn(createDuAnDto);
			if (result == null)
			{
				return BadRequest(Error<string>("Thêm dự án không thành công."));
			}
			return Ok(Success(result.ToDto()));
		}
		[HttpGet("chi-tiet-du-an")]
		public async Task<IActionResult> GetDuAnById(int maDuAn)
		{
			var result = await _duAnService.GetDuAnByIdAsync(maDuAn);
			if (result == null)
			{
				return NotFound(Error<string>("Không tìm thấy dự án."));
			}
			return Ok(Success(result));
		}
		[HttpPut("cap-nhat-du-an")]
		public async Task<IActionResult> UpdateDuAn(int maDuAn, [FromBody] UpdateDuAnDto updateDto)
		{
			if (updateDto == null)
			{
				return BadRequest(Error<string>("Dữ liệu không hợp lệ."));
			}
			var result = await _duAnService.UpdateDuAnAsync(maDuAn, updateDto);
			if (result == null)
			{
				return NotFound(Error<string>("Cập nhật dự án không thành công."));
			}
			return Ok(Success(result));
		}

		[HttpDelete("xoa-du-an")]
		public async Task<IActionResult> DeleteDuAn(int maDuAn)
		{
			var result = await _duAnService.DeleteDuAnAsync(maDuAn);
			if (!result)
			{
				return NotFound(Error<string>("Xóa dự án không thành công."));
			}
			return Ok(Success<string>("Xóa dự án thành công."));
		}

		[HttpGet("du-an-active")]
		public async Task<IActionResult> GetDuAnActive()
		{
			var result = await _duAnService.getDuAnActive();
			if (result == null)
			{
				return NotFound(Error<string>("Không tìm thấy dự án nào."));
			}
			return Ok(Success(result));
		}
	}
}
