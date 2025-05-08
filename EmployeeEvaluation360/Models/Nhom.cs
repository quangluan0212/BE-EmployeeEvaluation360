using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	[Table("NHOM")]
	public class Nhom
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaNhom")]
		public int MaNhom { get; set; }

		[Required]
		[Column("TenNhom", TypeName = "NVARCHAR(100)")]
		public string TenNhom { get; set; }

		[Required]
		[Column("MaDuAn")]
		public int MaDuAn { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";

		[ForeignKey("MaDuAn")]
		public DuAn DuAn { get; set; }
		public ICollection<Nhom_NguoiDung> NhomNguoiDungs { get; set; }
	}
	[Table("NHOM_NGUOIDUNG")]
	public class Nhom_NguoiDung
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaNhomNguoiDung")]
		public int MaNhomNguoiDung { get; set; }

		[Required]
		[Column("MaNhom")]
		public int MaNhom { get; set; }

		[Required]
		[Column("MaNguoiDung", TypeName = "VARCHAR(10)")]
		public string MaNguoiDung { get; set; }

		[Required]
		[Column("VaiTro", TypeName = "NVARCHAR(20)")]
		public string VaiTro { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";

		[ForeignKey("MaNhom")]
		public Nhom Nhom { get; set; }

		[ForeignKey("MaNguoiDung")]
		public NguoiDung NguoiDung { get; set; }

		public ICollection<DanhGia> DanhGias { get; set; }
	}
}
