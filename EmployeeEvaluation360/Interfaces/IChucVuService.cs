using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IChucVuService
	{
		Task<IEnumerable<ChucVu>> GetAllChucVuAsync();

		Task<ChucVu> GetChucVuByIdAsync(int maChucVu);

		Task<IEnumerable<ChucVu>> GetActiveChucVuAsync();

		Task<ChucVu> CreateChucVuAsync(ChucVu chucVu);

		Task<ChucVu> UpdateChucVuAsync(int maChucVu, ChucVu chucVu);

		Task<bool> DeleteChucVuAsync(int maChucVu);

		Task<bool> SetStatusAsync(int maChucVu, string trangThai);

		Task<IEnumerable<NguoiDung>> GetNguoiDungByChucVuAsync(int maChucVu);
		Task<bool> ChucVuExistsAsync(int maChucVu);
	}
}
