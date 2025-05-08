using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using EmployeeEvaluation360.Models;
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

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ChucVu>>> GetChucVus()
		{
			var chucVus = await _chucVuService.GetAllChucVuAsync();
			if (chucVus == null || !chucVus.Any())
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}
			var chucVuDtos = chucVus.Select(c => c.ToDto()).ToList();
			return Ok(Success(chucVuDtos));
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<ChucVu>> GetChucVu(int id)
		{
			var chucVu = await _chucVuService.GetChucVuByIdAsync(id);

			if (chucVu == null)
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}

			return Ok(chucVu);
		}


		[HttpPost]
		[Route("create")]
		//[Authorize(Roles = "Admin")]
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
			return CreatedAtAction(nameof(GetChucVu), new { id = createdChucVu.MaChucVu }, createdChucVu);
		}

		[HttpPut("{id}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateChucVu(int id, ChucVuUpdateDto chucVuDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var chucVu = new ChucVu
			{
				MaChucVu = id,
				TenChucVu = chucVuDto.tenChucVu,
				TrangThai = chucVuDto.trangThai ?? "Active"
			};

			var updatedChucVu = await _chucVuService.UpdateChucVuAsync(id, chucVu);
			if (updatedChucVu == null)
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}

			return Ok(updatedChucVu);
		}

		[HttpDelete("{id}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteChucVu(int id)
		{
			if (!await _chucVuService.ChucVuExistsAsync(id))
			{
				return NotFound(new { message = "Không tìm thấy chức vụ" });
			}

			var result = await _chucVuService.DeleteChucVuAsync(id);
			if (!result)
			{
				return BadRequest(new { message = "Không thể xóa chức vụ này vì đang có người dùng sử dụng" });
			}

			return NoContent();
		}


		//[HttpPatch("{id}/status")]
		//[Authorize(Roles = "Admin")]
		//public async Task<IActionResult> SetStatus(int id, [FromBody] StatusDto statusDto)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	if (!await _chucVuService.ChucVuExistsAsync(id))
		//	{
		//		return NotFound(new { message = "Không tìm thấy chức vụ" });
		//	}

		//	var result = await _chucVuService.SetStatusAsync(id, statusDto.TrangThai);
		//	if (!result)
		//	{
		//		return BadRequest(new { message = "Không thể cập nhật trạng thái" });
		//	}

		//	return NoContent();
		//}
	}
}
