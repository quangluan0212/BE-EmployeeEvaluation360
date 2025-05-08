using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface ITokenService
	{
		string GenerateToken(NguoiDung nguoiDung);
		string? GetUserIdFromToken(string token);
	}
}
