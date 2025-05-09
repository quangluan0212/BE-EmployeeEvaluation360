using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IDuAnService
	{
		Task<PagedResult<DuAnDto>> GetAllDuAnPagedAsync(int page, int pageSize, string? search);
		Task<DuAn> ThemDuAn(CreateDuAnDto createDuAnDto);
		Task<DuAn> GetDuAnByIdAsync(int maDuAn);
		Task<DuAn> UpdateDuAnAsync(int maDuAn, UpdateDuAnDto updateDto);
		Task<bool> DeleteDuAnAsync(int maDuAn);
	}
}
