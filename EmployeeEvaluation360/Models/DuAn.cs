using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	public class DuAn
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaDuAn")]
		public int MaDuAn { get; set; }

		[Required]
		[Column("TenDuAn", TypeName = "NVARCHAR(100)")]
		public string TenDuAn { get; set; }

		[Column("MoTa", TypeName = "NVARCHAR(255)")]
		public string MoTa { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";
		public ICollection<Nhom> Nhoms { get; set; }
	}
}
