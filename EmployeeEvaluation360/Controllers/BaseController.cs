using EmployeeEvaluation360.Helppers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeEvaluation360.Controllers
{
	[ApiController]
	public class BaseController : ControllerBase
	{
		protected ApiResponse<T> Success<T>(T data, string message = "Thành công !!!", int code = 200)
		{
			return new ApiResponse<T>(code, message, data);
		}

		protected ApiResponse<T> Error<T>(string errorMessage, int code = 400)
		{
			return new ApiResponse<T>(code, "Thất bại !!!", default, errorMessage);
		}
	}
}
