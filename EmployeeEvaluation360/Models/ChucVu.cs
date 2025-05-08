using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	[Table("CHUCVU")]
	public class ChucVu
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaChucVu")]
		public int MaChucVu { get; set; }

		[Required]
		[Column("TenChucVu", TypeName = "VARCHAR(50)")]
		public string TenChucVu { get; set; }

		[Required]
		[Column("TrangThai", TypeName = "NVARCHAR(20)")]
		public string TrangThai { get; set; } = "Active";
		public ICollection<NguoiDung_ChucVu> NguoiDungChucVus { get; set; }
	}
}
