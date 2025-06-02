using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IKetQuaDanhGiaService
	{
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetAllPaged(int page, int pageSize, string? search, int? maDotDanhGia);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetBadCurrentPaged(int page, int pageSize, string? search, int? maDotDanhGia);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetGoodCurrentPaged(int page, int pageSize, string? search, int? maDotDanhGia);
		Task<PagedResult<KetQua_DanhGiaChiTietDto>> GetLatestPaged(int page, int pageSize, string? search, int? maDotDanhGia);
		Task<List<KetQua_DanhGiaChiTietDto>> GetAll(int? maDotDanhGia);
		Task<List<KetQua_DanhGiaChiTietDto>> GetBadCurrent(int? maDotDanhGia);
		Task<List<KetQua_DanhGiaChiTietDto>> GetGoodCurrent(int? maDotDanhGia);
		Task<List<KetQua_DanhGiaChiTietDto>> GetLatest(int? maDotDanhGia);
		Task<List<KetQua_DanhGiaDto>> getKetQuaDanhGiaByMaNguoiDung(string maNguoiDung);
	}
}
