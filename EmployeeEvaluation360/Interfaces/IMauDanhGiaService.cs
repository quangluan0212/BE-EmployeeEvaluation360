using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IMauDanhGiaService
	{
		Task<PagedResult<MauDanhGiaDto>> GetAllAsync(int page, int pageSize, string search);
		Task<List<MauDanhGiaDto>> GetAllMauDanhGiaActive();
		Task<GetMauDanhGia> GetMauDanhGiaById(int maMau);
		Task<CreateMauDanhGiaDto> CreateMauDanhGia(CreateMauDanhGiaDto mauDanhGiaDto);
		Task<List<MauDanhGiaDto>> GetMauDanhGiaByMaDotDanhGia(int maDotDanhGia);
		Task<UpdateMauDanhGiaDto> UpdateMauDanhGia(int maMau, UpdateMauDanhGiaDto mauDanhGiaDto);
		Task<bool> DeleteMauDanhGia(int maMau);
	}
}
