using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface INguoiDungService
	{
		Task<NguoiDung> AdminUpdateNguoiDungAsync(string maNguoiDung, AdminUpdateNguoiDungDto updateDto);
		Task<PagedResult<DanhSachNguoiDungDto>> GetNguoiDungWithRolePagedAsync(int page, int pageSize, string? search);
		Task<PagedResult<NguoiDung>> GetNguoiDungPagedAsync(int page, int pageSize);
		Task<ProfileNguoiDungDto> GetProfileAsync(string maNguoiDung);
		Task<NguoiDung> GetNguoiDungByIdAsync(string maNguoiDung);

		Task<NguoiDung> GetNguoiDungByEmailAsync(string email);

		Task<IEnumerable<NguoiDung>> GetActiveNguoiDungAsync();

		Task<NguoiDung> CreateNguoiDungAsync(NguoiDung nguoiDung, int? maChucVu);

		Task<NguoiDung> UpdateNguoiDungAsync(string maNguoiDung, UpdateNguoiDungDto updateDto);

		Task<bool> ChangePasswordAsync(string maNguoiDung, string currentPassword, string newPassword);

		Task<bool> ResetPasswordAsync(string maNguoiDung, string newPassword);

		Task<bool> DeleteNguoiDungAsync(string maNguoiDung);

		Task<bool> SetStatusAsync(string maNguoiDung, string trangThai);

		Task<IEnumerable<ChucVu>> GetChucVuByNguoiDungAsync(string maNguoiDung);

		Task<bool> AddChucVuAsync(string maNguoiDung, int maChucVu, int capBac);

		Task<bool> RemoveChucVuAsync(string maNguoiDung, int maChucVu);

		Task<IEnumerable<Nhom>> GetNhomByNguoiDungAsync(string maNguoiDung);

		Task<bool> NguoiDungExistsAsync(string maNguoiDung);

		Task<bool> EmailExistsAsync(string email);

		Task<bool> ValidatePasswordAsync(string maNguoiDung, string password);

		Task<string> GenerateMaNguoiDungAsync(DateTime NgayVaoCongTy);
		Task<List<AdminActiveDto>> GetAdminActivesAsync(); 
	}
}
