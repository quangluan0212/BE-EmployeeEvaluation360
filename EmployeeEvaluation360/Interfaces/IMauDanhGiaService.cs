using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IMauDanhGiaService
	{
		Task<List<MauDanhGia>> GetAllAsync();
		//Task<MauDanhGiaCauHoiDto?> GetByIdWithCauHoiAsync(int maMau);
	}
}
