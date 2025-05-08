using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeEvaluation360.Models
{
	[Table("NGUOIDUNG")]
	public class NguoiDung
	{
		[Key]
		[Column("MaNguoiDung", TypeName = "VARCHAR(10)")]
		public string MaNguoiDung { get; set; }

		[Required]
		[Column("HoTen", TypeName = "NVARCHAR(100)")]
		public string HoTen { get; set; }

		[Required]
		[Column("Email", TypeName = "VARCHAR(50)")]
		public string Email { get; set; }

		[Column("DienThoai", TypeName = "VARCHAR(10)")]
		public string DienThoai { get; set; }

		[Required]
		[Column("MatKhau", TypeName = "VARCHAR(60)")]
		public string MatKhau { get; set; }

		[Column("NgayVaoCongTy", TypeName = "DATETIME")]
		public DateTime NgayVaoCongTy { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";

		public ICollection<Nhom_NguoiDung> NhomNguoiDungs { get; set; }
		public ICollection<DanhGia> DanhGias { get; set; }
		public ICollection<NguoiDung_ChucVu> NguoiDungChucVus { get; set; }
		public ICollection<KetQua_DanhGia> KetQuaDanhGias { get; set; }
	}

	[Table("NGUOIDUNG_CHUCVU")]
	public class NguoiDung_ChucVu
	{
		[Column("MaNguoiDung", TypeName = "VARCHAR(10)")]
		public string MaNguoiDung { get; set; }

		[Column("MaChucVu")]
		public int MaChucVu { get; set; }

		[Required]
		[Column("CapBac")]
		public int CapBac { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";

		[ForeignKey("MaNguoiDung")]
		public NguoiDung NguoiDung { get; set; }

		[ForeignKey("MaChucVu")]
		public ChucVu ChucVu { get; set; }
	}
}
