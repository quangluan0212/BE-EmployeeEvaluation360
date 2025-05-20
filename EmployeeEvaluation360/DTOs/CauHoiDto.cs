namespace EmployeeEvaluation360.DTOs
{
	public class CauHoiDto
	{
		public int MaCauHoi { get; set; }
		public string NoiDung {  get; set; }
		public int DiemToiDa { get; set; }
	}

	public class CreateCauHoiDto
	{
		public string NoiDung { get; set; }
		public int DiemToiDa { get; set; }
	}
	public class CauHoiTraloiDto
	{
		public int MaCauHoi { get; set; }
		public int TraLoi { get; set; }
	}

	public class CauTraloiDto
	{
		public int MaCauHoi { get; set; }
		public string NoiDung { get; set; } = string.Empty;
		public int TraLoi { get; set; }
	}
}
