using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	[Table("CAUHOI")]
	public class CauHoi
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaCauHoi")]
		public int MaCauHoi { get; set; }

		[Required]
		[Column("MaMau")]
		public int MaMau { get; set; }

		[Column("NoiDung", TypeName = "NVARCHAR(500)")]
		public string NoiDung { get; set; }

		[Column("DiemToiDa")]
		public int? DiemToiDa { get; set; }

		// Navigation
		[ForeignKey("MaMau")]
		public MauDanhGia MauDanhGia { get; set; }

		public ICollection<DanhGia_CauHoi> DanhGiaCauHois { get; set; }
	}
	[Table("DANHGIA_CAUHOI")]
	public class DanhGia_CauHoi
	{
		[Column("MaDanhGia")]
		public int MaDanhGia { get; set; }

		[Column("MaCauHoi")]
		public int MaCauHoi { get; set; }

		[Column("TraLoi")]
		public int? TraLoi { get; set; }

		// Navigation
		[ForeignKey("MaDanhGia")]
		public DanhGia DanhGia { get; set; }

		[ForeignKey("MaCauHoi")]
		public CauHoi CauHoi { get; set; }
	}
}
