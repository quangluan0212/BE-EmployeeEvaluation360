using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface IChucVuService
	{

		Task<IEnumerable<ChucVu>> GetAllChucVuAsync();
		Task<PagedResult<ChucVuDto>> GetAllChucVuPagedAsync(int page, int pageSize, string? search);

		Task<ChucVu> GetChucVuByIdAsync(int maChucVu);

		Task<IEnumerable<ChucVu>> GetActiveChucVuAsync();

		Task<ChucVu> CreateChucVuAsync(ChucVu chucVu);

		Task<ChucVu> UpdateChucVuAsync(int maChucVu, ChucVu chucVu);

		Task<bool> DeleteChucVuAsync(int maChucVu);

		Task<bool> SetStatusAsync(int maChucVu, string trangThai);

		Task<IEnumerable<NguoiDung>> GetNguoiDungByChucVuAsync(int maChucVu);
		Task<bool> ChucVuExistsAsync(int maChucVu);
		Task<CapNhatChucVuDto> CapNhatChucVuChoNguoiDung(CapNhatChucVuDto capNhatChucVu);
		Task<int> GetCapBacChucVu(string maNguoiDung, int maChucVu);
	}
}
