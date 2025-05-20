using System.ComponentModel.DataAnnotations;

namespace EmployeeEvaluation360.DTOs
{
	public class DotDanhGiaDto
	{
		public int MaDotDanhGia { get; set; }
		public string TenDot {  get; set; } = string.Empty;
		public DateTime ThoiGianBatDau { get; set; }
		public DateTime ThoiGianKetThuc {  get; set; }
		public string TrangThai { get; set; } = string.Empty;
	}
	public class DotDanhGiaActiveDto 
	{
		public int MaDotDanhGia { set; get; }
		public string TenDot { set; get; } = string.Empty;
	}

	public class UpdateDotDanhGia
	{
		public int MaDotDanhGia { set; get; }
		public string TenDot { set; get; } = string.Empty;
		public DateTime NgayBatDau { set; get; }
		[DateRange("NgayBatDau")]
		public DateTime NgayKetThuc { set; get; }
		public List<int> mauDanhGias { set; get; } = new List<int>();
	}
	
	public class CreateDotDanhGiaDto() 
	{

		[Required(ErrorMessage = "Tên đợt đánh giá không được để trống")]
		public string TenDot { set; get; } = string.Empty;
		[Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
		public DateTime NgayBatDau { set; get; }
		[Required(ErrorMessage = "Ngày kết thúc không được để trống")]
		[DateRange("NgayBatDau")]
		public DateTime NgayKetThuc { set; get; }
		public List<int> mauDanhGias { set; get; } = new List<int>();
	}
	public class DateRangeAttribute : ValidationAttribute
	{
		private readonly string _startDateProperty;

		public DateRangeAttribute(string startDateProperty)
		{
			_startDateProperty = startDateProperty;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var endDate = (DateTime)value;

			var property = validationContext.ObjectType.GetProperty(_startDateProperty);
			if (property == null)
				return new ValidationResult($"Không tìm thấy thuộc tính {_startDateProperty}");

			var startDate = (DateTime)property.GetValue(validationContext.ObjectInstance);

			if (endDate < startDate)
			{
				return new ValidationResult("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");
			}

			return ValidationResult.Success;
		}
	}
}
