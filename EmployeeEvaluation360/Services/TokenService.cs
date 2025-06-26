using EmployeeEvaluation360.Interfaces;
using EmployeeEvaluation360.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeEvaluation360.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
		private readonly SymmetricSecurityKey _key;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
		}

		public string GenerateToken(NguoiDung nguoiDung)
		{
			var claims = new List<Claim>
			{
				new Claim("maNguoiDung", nguoiDung.MaNguoiDung.ToString()),
				new Claim("hoTen", nguoiDung.HoTen),
			};

			var roles = nguoiDung.NguoiDungChucVus?
				.Select(c => c.ChucVu.TenChucVu)
				.Where(r => !string.IsNullOrEmpty(r))
				.ToList() ?? new List<string>();

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));

			}

			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddMinutes(600),
				SigningCredentials = creds,
				Issuer = _configuration["JWT:Issuer"],
				Audience = _configuration["JWT:Audience"]
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}


		public string? GetUserIdFromToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
				return jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			}
			catch
			{
				return null;
			}
		}
	}
}
