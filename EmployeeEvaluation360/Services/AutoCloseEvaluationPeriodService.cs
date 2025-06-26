using EmployeeEvaluation360.Database;
using Microsoft.EntityFrameworkCore;
using EmployeeEvaluation360.Interfaces;

namespace EmployeeEvaluation360.Services
{
	public class AutoCloseEvaluationPeriodService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		public AutoCloseEvaluationPeriodService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
					var dotDanhGiaService = scope.ServiceProvider.GetRequiredService<IDotDanhGiaService>();

					// Convert current UTC time to Vietnam time (UTC+7)
					var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
					var currentVietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

					// Lấy các Đợt Đánh Giá còn Active và đã đến thời gian kết thúc
					var dotDanhGia = await context.DOT_DANHGIA
						.Where(e => e.TrangThai == "Active" && e.ThoiGianKetThuc <= currentVietnamTime)
						.ToListAsync(stoppingToken);

					foreach (var evaluation in dotDanhGia)
					{
						evaluation.TrangThai = "Inactive";
						context.DOT_DANHGIA.Update(evaluation);
						await context.SaveChangesAsync(stoppingToken);

						try
						{
							var result = await dotDanhGiaService.CreateKetQuaDanhGiaByMaDotDanhGia(evaluation.MaDotDanhGia);
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Lỗi khi tính KETQUA_DANHGIA cho MaDotDanhGia {evaluation.MaDotDanhGia}: {ex.Message}");
						}
					}
					await context.SaveChangesAsync(stoppingToken);
				}
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}
		}
	}
}
