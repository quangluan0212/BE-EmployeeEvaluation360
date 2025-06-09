using EmployeeEvaluation360.Database;
using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace EmployeeEvaluation360
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployeeEvaluation360", Version = "v1" });
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header
						},
						new List<string>()
					}
				});
			});
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IChucVuService, ChucVuService>();
			builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
			builder.Services.AddScoped<ITokenService, TokenService>();
			builder.Services.AddScoped<IDuAnService, DuAnService>();
			builder.Services.AddScoped<INhomService, NhomService>();
			builder.Services.AddScoped<IDanhGiaService, DanhGiaService>();
			builder.Services.AddScoped<IDotDanhGiaService, DotDanhGiaService>();
			builder.Services.AddScoped<IMauDanhGiaService, MauDanhGiaService>();
			builder.Services.AddScoped<IMailService, MailService>();
			builder.Services.AddScoped<IKetQuaDanhGiaService, KetQuaDanhGiaService>();

			builder.Services.AddMemoryCache();


			builder.Services.AddAuthentication(options =>	
			{
				options.DefaultAuthenticateScheme =
				options.DefaultChallengeScheme =
				options.DefaultForbidScheme =
				options.DefaultScheme =
				options.DefaultSignInScheme =
				options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = builder.Configuration["JWT:Issuer"],
					ValidateAudience = true,
					ValidAudience = builder.Configuration["JWT:Audience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
					),
					RoleClaimType =  ClaimTypes.Role
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var token = context.Request.Headers["Authorization"].FirstOrDefault();
						if (!string.IsNullOrEmpty(token) && !token.StartsWith("Bearer "))
						{
							context.Request.Headers["Authorization"] = "Bearer " + token;
						}
						return Task.CompletedTask;
					}
				};
			});

			builder.Services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
				options.JsonSerializerOptions.WriteIndented = true;
			});

			builder.Services.AddDataProtection()
				.PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"));

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontend",
					policy => policy
						.WithOrigins("http://localhost:5173")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials());
			});

			//builder.Services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowAll", policy =>
			//	{
			//		policy.AllowAnyOrigin()
			//			  .AllowAnyMethod()
			//			  .AllowAnyHeader();
			//	});
			//});

			builder.Services.AddScoped<SeedData>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();				
			}
			else // redirect HTTPS khi KHÔNG phải môi trường dev
			{
				app.UseSwagger();
    			app.UseSwaggerUI();
				app.UseHttpsRedirection();
			}


			app.UseAuthentication();
			app.UseAuthorization();

			app.UseCors("AllowFrontend");
			//app.UseCors("AllowAll");

			app.MapControllers();

			using (var scope = app.Services.CreateScope())
			{
				var seed = scope.ServiceProvider.GetRequiredService<SeedData>();
				seed.EnsureAdminUser();
			}

			app.Run();
        }
    }
}
