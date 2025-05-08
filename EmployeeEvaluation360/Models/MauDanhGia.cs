using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	public class MauDanhGia
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaMau")]
		public int MaMau { get; set; }

		[Required]
		[Column("TenMau", TypeName = "NVARCHAR(255)")]
		public string TenMau { get; set; }

		[Required]
		[Column("LoaiDanhGia", TypeName = "NVARCHAR(50)")]
		public string LoaiDanhGia { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";

		public ICollection<ChiTiet_DotDanhGia> ChiTietDotDanhGias { get; set; }
		public ICollection<CauHoi> CauHois { get; set; }
	}
}
