using EmployeeEvaluation360.DTOs;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IDanhGiaService
	{
		Task<string> XacDinhLoaiDanhGiaAsync(string nguoiDanhGia, int nguoiDuocDanhGia);
		Task<int> TinhHeSoAsync(string nguoiDanhGia, int nguoiDuocDanhGia);
		Task<MauDanhGiaCauHoiDto> GetFormDanhGiaCauHoiAsync(string MaNguoiDanhGia, int MaNguoiDungDanhGia, int MaDotDanhGia);		
	}
}
