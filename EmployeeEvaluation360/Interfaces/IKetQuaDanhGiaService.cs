using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IKetQuaDanhGiaService
	{
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetAllPaged(int page, int pageSize, string? search);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetBadCurrentPaged(int page, int pageSize, string? search);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetGoodCurrentPaged(int page, int pageSize, string? search);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetLatestPaged(int page, int pageSize, string? search, int? year);
		Task<List<KetQua_DanhGiaChiTietDto>> GetAll();
		Task<List<KetQua_DanhGiaChiTietDto>> GetBadCurrent();
		Task<List<KetQua_DanhGiaChiTietDto>> GetGoodCurrent();
		Task<List<KetQua_DanhGiaChiTietDto>> GetLatest();
	}
}
