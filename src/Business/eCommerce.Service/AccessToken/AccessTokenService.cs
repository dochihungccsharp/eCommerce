using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;
using eCommerce.Shared.Configurations;
using eCommerce.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace eCommerce.Service.AccessToken;

public class AccessTokenService : IAccessTokenService
{
    private readonly JwtSetting _jwtSetting;

    public AccessTokenService(IConfiguration configuration)
    {
        _jwtSetting = configuration.GetOptions<JwtSetting>() ?? throw new ArgumentNullException(nameof(configuration));
    }
    public AuthorizedResponseModel GenerateJwtToken(UserContextModel userContext)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSetting.Key);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("UserId", userContext.Id),
                new Claim("Username", userContext.Username),
                new Claim("Fullname", userContext.Fullname ?? ""),
                new Claim("Email", userContext.Email),
            }),
            Expires = DateTime.UtcNow.AddDays(_jwtSetting.ExpiredDay),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
            
            
        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        var refreshToken = Guid.NewGuid().ToString();
        var issuedTime = DateTime.UtcNow;
        var expiredTime = issuedTime.AddDays(_jwtSetting.ExpiredDay);
            
        return new AuthorizedResponseModel(accessToken, refreshToken, issuedTime, expiredTime);
    }

    public UserContextModel ParseJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSetting.Key);
        tokenHandler.ValidateToken(token, new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        }, out SecurityToken validatedToken);
            
        var jwtToken = (JwtSecurityToken)validatedToken;
            
        var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;
        var username = jwtToken.Claims.First(x => x.Type == "Username").Value;
        var fullname = jwtToken.Claims.First(x => x.Type == "Fullname").Value;
        var email = jwtToken.Claims.First(x => x.Type == "Email").Value;

        return new UserContextModel()
        {
            Id = userId,
            Username = username,
            Fullname = fullname,
            Email = email
        };
        
    }
}