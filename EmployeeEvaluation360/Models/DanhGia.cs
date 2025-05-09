using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	[Table("DANHGIA")]
	public class DanhGia
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaDanhGia")]
		public int MaDanhGia { get; set; }

		[Required]
		[Column("NguoiDanhGia", TypeName = "VARCHAR(10)")]
		public string NguoiDanhGia { get; set; }

		[Required]
		[Column("NguoiDuocDanhGia")]
		public int NguoiDuocDanhGia { get; set; }

		[Required]
		[Column("MaDotDanhGia")]
		public int MaDotDanhGia { get; set; }

		[Required]
		[Column("Diem", TypeName = "DECIMAL(5,2)")]
		public decimal Diem { get; set; }
		[Required]
		[Column("HeSo")]
		public int HeSo { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Pending";

		// Navigation
		[ForeignKey("NguoiDanhGia")]
		public NguoiDung NguoiDanhGiaObj { get; set; }

		[ForeignKey("NguoiDuocDanhGia")]
		public Nhom_NguoiDung NguoiDuocDanhGiaObj { get; set; }

		[ForeignKey("MaDotDanhGia")]
		public DotDanhGia DotDanhGia { get; set; }

		public ICollection<DanhGia_CauHoi> DanhGiaCauHois { get; set; }
	}
}
