using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IKetQuaDanhGiaService
	{
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetAll(int page, int pageSize, string? search);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetBadCurrent(int page, int pageSize, string? search);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetGoodCurrent(int page, int pageSize, string? search);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetLatest(int page, int pageSize, string? search);
	}
}
