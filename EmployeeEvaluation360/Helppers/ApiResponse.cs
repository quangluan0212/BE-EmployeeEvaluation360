namespace EmployeeEvaluation360.Helppers
{
	public class ApiResponse<T>
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public string Error { get; set; }
		public T Data { get; set; }

		public ApiResponse(int code, string message, T data = default, string error = null)
		{
			Code = code;
			Message = message;
			Error = error;
			Data = data;
		}
	}
}
