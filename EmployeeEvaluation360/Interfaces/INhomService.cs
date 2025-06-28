using EmployeeEvaluation360.DTOs;
using EmployeeEvaluation360.Helppers;
using EmployeeEvaluation360.Models;

namespace EmployeeEvaluation360.Interfaces
{
	public interface INhomService
	{
		Task<PagedResult<ListNhomDuAnDto>> GetAllNhomPagedAsync(int page, int pageSize, string? search);
		Task<Nhom> ThemNhom(CreateNhomDto createNhomDto);
		Task<Nhom> GetNhomByIdAsync(int maNhom);
		Task<Nhom> UpdateNhomAsync(int maNhom, UpdateNhomDto updateDto);
		Task<bool> DeleteNhomAsync(int maNhom);
		Task<List<Nhom_NguoiDung>> ThemNhanVienVaoNhom(ThemNhanVienVaoNhomDto addDto);
		Task<PagedResult<Nhom_NguoiDungDto>> GetDanhSachThanhVienNhom(int maNhom, int page, int pageSize, string? search);
		Task<List<NhomDto>> GetAllNhomByMaNguoiDung(string maNguoiDung);
		Task<List<NhomVaThanhVienDto>> LayDanhSachNhomVaThanhVienAsync(string maNguoiDung);
		Task<List<NguoiDungKhongNhomDto>> LayDanhSachNguoiDungKhongCoTrongNhomAsync(int maNhom);
		Task<bool> XoaThanhVien(int maNhom, string maNguoiDung);
		Task<List<SimpleNhomDto>> GetDanhSachNhomByLeader(string maNguoiDung);
	}
}
