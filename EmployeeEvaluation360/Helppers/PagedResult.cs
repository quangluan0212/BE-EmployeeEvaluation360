﻿namespace EmployeeEvaluation360.Helppers
{
	public class PagedResult<T>
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public IEnumerable<T> Items { get; set; }
	}

}
