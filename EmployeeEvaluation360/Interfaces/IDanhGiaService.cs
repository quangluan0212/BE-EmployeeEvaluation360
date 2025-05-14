using EmployeeEvaluation360.DTOs;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IDanhGiaService
	{
		Task<string> XacDinhLoaiDanhGiaAsync(string nguoiDanhGia, int nguoiDuocDanhGia);
		Task<MauDanhGiaCauHoiDto> GetFormDanhGiaCauHoiAsync(int maDanhGia);	
		Task<List<AdminGetDanhGiaDto>> AdminGetListDanhGiaAsync(string maNguoiDung);
		Task<List<NhanVienGetDanhGiaDto>> NhanVienGetDanhGiaAsync(string maNguoiDung);
		Task<bool> SubmitDanhGia(DanhGiaTraLoiDto giaTraLoiDto);
		Task<MauDanhGiaCauHoiTraLoiDto> GetCauTraLoiTheoMaDanhGiaAsync(int maDanhGia);
	}
}
