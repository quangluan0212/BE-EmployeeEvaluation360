using EmployeeEvaluation360.DTOs;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IDotDanhGiaService
	{
		Task<DotDanhGiaDto> GetDotDanhGiaActivesAsync();
	}
}
