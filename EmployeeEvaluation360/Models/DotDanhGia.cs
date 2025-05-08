using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	[Table("DOT_DANHGIA")]
	public class DotDanhGia
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaDotDanhGia")]
		public int MaDotDanhGia { get; set; }

		[Required]
		[Column("TenDot", TypeName = "NVARCHAR(255)")]
		public string TenDot { get; set; }

		[Required]
		[Column("ThoiGianBatDau", TypeName = "DATETIME")]
		public DateTime ThoiGianBatDau { get; set; }

		[Required]
		[Column("ThoiGianKetThuc", TypeName = "DATETIME")]
		public DateTime ThoiGianKetThuc { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Chưa bắt đầu";

		public ICollection<ChiTiet_DotDanhGia> ChiTietDotDanhGias { get; set; }
		public ICollection<DanhGia> DanhGias { get; set; }
	}
	[Table("CHITIET_DOTDANHGIA")]
	public class ChiTiet_DotDanhGia
	{
		[Column("MaDotDanhGia")]
		public int MaDotDanhGia { get; set; }

		[Column("MaMau")]
		public int MaMau { get; set; }

		[Required]
		[Column("LoaiNguoiDuocDanhGia", TypeName = "VARCHAR(50)")]
		public string LoaiNguoiDuocDanhGia { get; set; }

		[ForeignKey("MaDotDanhGia")]
		public DotDanhGia DotDanhGia { get; set; }

		[ForeignKey("MaMau")]
		public MauDanhGia MauDanhGia { get; set; }
	}
}
