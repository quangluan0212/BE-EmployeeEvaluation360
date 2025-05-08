using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.Models
{
	[Table("KETQUA_DANHGIA")]
	public class KetQua_DanhGia
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("MaKetQua")]
		public int MaKetQua { get; set; }

		[Required]
		[Column("MaNguoiDung", TypeName = "VARCHAR(10)")]
		public string MaNguoiDung { get; set; }

		[Required]
		[Column("DiemTongKet", TypeName = "DECIMAL(5,2)")]
		public decimal DiemTongKet { get; set; }

		[Required]
		[Column("ThoiGianTinh", TypeName = "DATETIME")]
		public DateTime ThoiGianTinh { get; set; }

		[ForeignKey("MaNguoiDung")]
		public NguoiDung NguoiDung { get; set; }
	}
}
