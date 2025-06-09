using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

		[HttpGet("admin-get-all-danh-sach-chua-danh-gia")]
		public async Task<IActionResult> AdminGetDanhSachDanhGia([FromQuery] int? maDotDanhGia = null)
		{
			var danhSachChuaDanhGia = await _danhGiaService.GetDanhSachNguoiThieuDanhGiaAsync(maDotDanhGia);
			if (danhSachChuaDanhGia == null)
			{
				return NotFound(Error<string>("Không tìm thấy danh sách người chưa đánh giá !"));
			}
			return Ok(Success(danhSachChuaDanhGia));
		}

		[HttpGet("admin-get-all-danh-sach-chua-danh-gia-paged")]
		public async Task<IActionResult> AdminGetDanhSachDanhGia([FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? search = null, int? maDotDanhGia = null)
		{
			var danhSachChuaDanhGia = await _danhGiaService.GetDanhSachNguoiThieuDanhGiaPagedAsync(page, pageSize, search, maDotDanhGia);
			if (danhSachChuaDanhGia == null)
			{
				return NotFound(Error<string>("Không tìm thấy danh sách người chưa đánh giá !"));
			}
			return Ok(Success(danhSachChuaDanhGia));
		}

		[HttpGet("admin-get-all-danh-sach-danh-gia")]
		public async Task<IActionResult> AdminGetDanhSachDanhGia([FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? search = null)
		{
			var danhSachDanhGia = await _danhGiaService.AdminGetAllDanhGiaAsync(page, pageSize, search);
			if (danhSachDanhGia == null)
			{
				return NotFound(Error<string>("Không tìm thấy danh sách đánh giá."));
			}
			return Ok(Success(danhSachDanhGia));
		}

		[HttpGet("admin-get-all-danh-sach-tu-danh-gia")]
		public async Task<IActionResult> AdminGetDanhSachTuDanhGia([FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? search = null)
		{
			var danhSachDanhGia = await _danhGiaService.AdminGetAllTuDanhGiaAsync(page, pageSize, search);
			if (danhSachDanhGia == null)
			{
				return NotFound(Error<string>("Không tìm thấy danh sách đánh giá."));
			}
			return Ok(Success(danhSachDanhGia));
		}

		[HttpGet("admin-get-all-danh-sach-danh-gia-cheo")]
		public async Task<IActionResult> AdminGetDanhSachDanhGiaCheo([FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? search = null)
		{
			var danhSachDanhGia = await _danhGiaService.NhanVienGetAllDanhGiaCheoAsync(page, pageSize, search);
			if (danhSachDanhGia == null)
			{
				return NotFound(Error<string>("Không tìm thấy danh sách đánh giá."));
			}
			return Ok(Success(danhSachDanhGia));
		}

		[HttpGet("get-danh-gia-by-id/{maDanhGia}")]
		public async Task<IActionResult> LayCauTraLoi(int maDanhGia)
		{
			try
			{
				var result = await _danhGiaService.GetCauTraLoiTheoMaDanhGiaAsync(maDanhGia);
				return Ok(Success(result));
			}
			catch (Exception ex)
			{
				return BadRequest(Error<string>(ex.Message));
			}
		}

		[HttpPost("submit-danh-gia")]
		public async Task<IActionResult> SubmitDanhGia([FromBody] DanhGiaTraLoiDto danhGiaDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _danhGiaService.SubmitDanhGia(danhGiaDTO);
			if (result != null)
			{
				return Ok(Success(result));
			}
			return BadRequest(ModelState);
		}


		[HttpGet]
		[Route("get-form-danh-gia")]
		public async Task<IActionResult> DanhGia([FromQuery] int maDanhGia)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var formDanhGia = await _danhGiaService.GetFormDanhGiaCauHoiAsync(maDanhGia);
			if (formDanhGia != null)
			{
				return Ok(Success(formDanhGia));
			}
			return BadRequest(ModelState);
		}

		[HttpGet("admin-danh-sach-danh-gia-leader")]
		public async Task<IActionResult> AdminGetListDanhGiaLeader([FromQuery]string maNguoiDung)
		{
			if (maNguoiDung.IsNullOrEmpty())
			{
				return BadRequest(Error<string>("Đã xảy ra lỗi vui lòng đăng nhập lại !!!"));
			}
			var listDanhGia = await _danhGiaService.AdminGetListDanhGiaAsync(maNguoiDung);
			return Ok(Success(listDanhGia));
		}


		[HttpGet("user-danh-sach-danh-gia-team")]
		public async Task<IActionResult> UserGetListDanhGiaTeam([FromQuery] string maNguoiDung)
		{
			if (maNguoiDung.IsNullOrEmpty())
			{
				return BadRequest(Error<string>("Đã xảy ra lỗi vui lòng đăng nhập lại !!!"));
			}
			var listDanhGia = await _danhGiaService.NhanVienGetDanhGiaAsync(maNguoiDung);
			return Ok(Success(listDanhGia));
		}
	}
}
