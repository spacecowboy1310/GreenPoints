using GreenPointsAPI.Properties;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GreenPointsAPI.Services;

internal class TokenService
{
    internal static string GenerateToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        byte[] key = ApiSettings.GenerateSecretByte();

        List<Claim> claims = new (){ new Claim(ClaimTypes.Name, user.Username.ToString()) };
        foreach(Role role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString()));
        }

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
