using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IDanhGiaService
	{
		Task<PagedResult<DanhGiaDto>> AdminGetAllDanhGiaAsync(int page, int pageSize, string? search);
		Task<PagedResult<DanhGiaDto>> AdminGetAllTuDanhGiaAsync(int page, int pageSize, string? search);
		Task<PagedResult<DanhGiaDto>> NhanVienGetAllDanhGiaCheoAsync(int page, int pageSize, string? search);
		Task<string> XacDinhLoaiDanhGiaAsync(string nguoiDanhGia, int nguoiDuocDanhGia);
		Task<MauDanhGiaCauHoiDto> GetFormDanhGiaCauHoiAsync(int maDanhGia);	
		Task<List<AdminGetDanhGiaDto>> AdminGetListDanhGiaAsync(string maNguoiDung);
		Task<List<NhanVienGetDanhGiaDto>> NhanVienGetDanhGiaAsync(string maNguoiDung);
		Task<bool> SubmitDanhGia(DanhGiaTraLoiDto giaTraLoiDto);
		Task<MauDanhGiaCauHoiTraLoiDto> GetCauTraLoiTheoMaDanhGiaAsync(int maDanhGia);
		Task<PagedResult<NguoiThieuDanhGiaDto>> GetDanhSachNguoiThieuDanhGiaPagedAsync(int page, int pageSize, string? search, int? maDotDanhGia);
		Task<List<NguoiThieuDanhGiaDto>> GetDanhSachNguoiThieuDanhGiaAsync(int? maDotDanhGia);
	}
}
