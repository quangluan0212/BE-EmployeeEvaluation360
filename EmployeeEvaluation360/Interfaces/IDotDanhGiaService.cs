using EmployeeEvaluation360.DTOs;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IDotDanhGiaService
	{
		Task<List<DotDanhGiaDto>> getDanhSachDotDanhGia ();
		Task<string> UpdateDotDanhGia(UpdateDotDanhGia updateDotDanhGia);
		Task<string> KetThucDotDanhGia(int maDotDanhGia);
		Task<DotDanhGiaDto> GetDotDanhGiaActivesAsync();
		Task<DotDanhGiaDto> CreateDotDanhGia(CreateDotDanhGiaDto danhGiaDto);
		Task<List<NguoiDungDto>> getNguoiDungActiveNotIncludeAdmin();
		Task<List<ThanhVienDto>> getListLeaderActive();
		Task<List<NguoiDungDto>> getListAdminActive();
		Task<List<NhomVaThanhVienDto>> getNhomVaThanhVienCungNhomByMaNguoiDung(string maNguoiDung);
		Task<string> GenDanhGia(int maDotDanhGia);
	}
}
