using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DuAnController : BaseController
	{
		private readonly IDuAnService _duAnService;
		public DuAnController(IDuAnService duAnService)
		{
			_duAnService = duAnService;
		}

		[HttpGet("danh-sach-du-an")]
		public async Task<IActionResult> GetDanhSachDuAn(int page = 1, int pageSize = 10)
		{
			var result = await _duAnService.GetAllDuAnPagedAsync(page, pageSize);
			if (result == null)
			{
				return NotFound(Error<string>("Không tìm thấy dự án nào."));
			}
			return Ok(Success(result));
		}

	}
}
