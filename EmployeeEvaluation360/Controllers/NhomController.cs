using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class NhomController : BaseController
	{
		private readonly INhomService _nhomService;
		public NhomController(INhomService nhomService)
		{
			_nhomService = nhomService;
		}

		[HttpGet("danh-sach-nhom-theo-leader")]
		public async Task<IActionResult> GetDanhSachNhomTheoLeader(string maNguoiDung)
		{
			if (string.IsNullOrEmpty(maNguoiDung))
			{
				return BadRequest(Error<string>("Mã người dùng không được để trống."));
			}
			var result = await _nhomService.GetDanhSachNhomByLeader(maNguoiDung);
			return Ok(Success(result));
		}
		[Authorize(Roles = "Admin")]
		[HttpDelete("xoa-thanh-vien-khoi-nhom")]
		public async Task<IActionResult> XoaThanhVienKhoiNhom(int maNhom, string maNguoiDung)
		{
			if (string.IsNullOrEmpty(maNguoiDung))
			{
				return BadRequest(Error<string>("Mã người dùng không được để trống."));
			}
			var isDeleted = await _nhomService.XoaThanhVien(maNhom, maNguoiDung);
			if (!isDeleted)
			{
				return BadRequest(Error<string>("Có lỗi khi xóa thành viên khỏi nhóm!"));
			}
			return Ok(Success<string>("Thành viên đã được xóa khỏi nhóm thành công."));
		}
		[Authorize(Roles = "Admin")]
		[HttpGet("danh-sach-nguoi-dung-khong-co-trong-nhom")]
		public async Task<IActionResult> GetDanhSachNguoiDungKhongCoTrongNhom(int maNhom)
		{
			var result = await _nhomService.LayDanhSachNguoiDungKhongCoTrongNhomAsync(maNhom);
			return Ok(Success(result));
		}

		[HttpGet("danh-sach-nhom")]
		public async Task<IActionResult> GetDanhSachNhom(int page = 1, int pageSize = 10, string? search = null)
		{
			var result = await _nhomService.GetAllNhomPagedAsync(page, pageSize, search);
			return Ok(Success(result));
		}
		[Authorize(Roles = "Admin")]
		[HttpPost("them-nhom")]
		public async Task<IActionResult> CreateNhom([FromBody] CreateNhomDto nhom)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var createdNhom = await _nhomService.ThemNhom(nhom);
			if (createdNhom == null)
			{
				return BadRequest(Error<string>("Có lỗi thì thêm nhóm !!!"));
			}
			return Ok(Success(createdNhom.ToDto()));
		}
		[Authorize(Roles = "Admin")]
		[HttpPut("cap-nhat-nhom")]
		public async Task<IActionResult> UpdateNhom(int maNhom, [FromBody] UpdateNhomDto updateDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var updatedNhom = await _nhomService.UpdateNhomAsync(maNhom, updateDto);
			if (updatedNhom == null)
			{
				return BadRequest(Error<string>("Có lỗi thì cập nhật nhóm !!!"));
			}
			return Ok(Success(updatedNhom.ToDto()));
		}
		[Authorize(Roles = "Admin")]
		[HttpDelete("xoa-nhom")]
		public async Task<IActionResult> DeleteNhom(int maNhom)
		{
			var nhom = await _nhomService.GetNhomByIdAsync(maNhom);
			if (nhom == null)
			{
				return NotFound(Error<string>("Nhóm không tồn tại!"));
			}

			if (nhom.NhomNguoiDungs != null && nhom.NhomNguoiDungs.Any())
			{
				nhom.TrangThai = "Deleted";
				var updatedNhom = await _nhomService.UpdateNhomAsync(maNhom, new UpdateNhomDto
				{
					TenNhom = nhom.TenNhom,
					MaDuAn = nhom.MaDuAn,
					TrangThai = nhom.TrangThai
				});

				if (updatedNhom == null)
				{
					return BadRequest(Error<string>("Có lỗi khi cập nhật trạng thái nhóm!"));
				}

				return Ok(Success(updatedNhom.ToDto(), "Nhóm đã được cập nhật trạng thái thành 'Deleted'."));
			}

			var isDeleted = await _nhomService.DeleteNhomAsync(maNhom);
			if (!isDeleted)
			{
				return BadRequest(Error<string>("Có lỗi khi xóa nhóm!"));
			}

			return Ok(Success<string>("Nhóm đã được xóa thành công."));
		}
		[Authorize(Roles = "Admin")]
		[HttpPost("them-thanh-vien-vao-nhom")]
		public async Task<IActionResult> ThemNhanVienVaoNhom([FromBody] ThemNhanVienVaoNhomDto addDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var nhom = await _nhomService.GetNhomByIdAsync(addDto.MaNhom);
			if (nhom == null)
			{
				return NotFound(Error<string>("Nhóm không tồn tại!"));
			}
			var result = await _nhomService.ThemNhanVienVaoNhom(addDto);
			if (result == null)
			{
				return BadRequest(Error<string>("Có lỗi khi thêm nhân viên vào nhóm!"));
			}
			var nhomNguoiDungs = result.Select(x => new Nhom_NguoiDungDto
			{
				MaNhom = x.MaNhom,
				MaNguoiDung = x.MaNguoiDung,
				TrangThai = x.TrangThai
			}).ToList();
			return Ok(Success(nhomNguoiDungs));
		}

		[HttpGet("danh-sach-thanh-vien-nhom")]
		public async Task<IActionResult> GetDanhSachThanhVienNhom(int maNhom, int page = 1, int pageSize = 10, string? search = null)
		{
			var nhom = await _nhomService.GetNhomByIdAsync(maNhom);
			if (nhom == null)
			{
				return NotFound(Error<string>("Nhóm không tồn tại!"));
			}
			var result = await _nhomService.GetDanhSachThanhVienNhom(maNhom, page, pageSize, search);
			return Ok(Success(result));
		}

		[HttpGet("danh-sach-nhom-theo-nguoi-dung")]
		public async Task<IActionResult> GetAllNhomByMaNguoiDung(string maNguoiDung)
		{
			var result = await _nhomService.GetAllNhomByMaNguoiDung(maNguoiDung);
			if (result == null)
			{
				return NotFound(Error<string>("Không tìm thấy nhóm nào!"));
			}

			return Ok(Success(result));
		}

		[HttpGet("get-danh-sach-nhom-by-ma-nguoi-dung")]
		public async Task<IActionResult> GetDanhSachNhomVaThanhVien([FromQuery] string maNguoiDung)
		{
			if (string.IsNullOrEmpty(maNguoiDung))
				return BadRequest("Mã người dùng không được để trống.");

			var data = await _nhomService.LayDanhSachNhomVaThanhVienAsync(maNguoiDung);
			return Ok(Success(data));
		}
	}
}
