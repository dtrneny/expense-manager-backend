using System.Text;
using EmBackend.Models.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EmBackend.Configurations;

public static class AuthConfiguration
{
    public static void ConfigureAuth(this IServiceCollection services, ConfigurationManager builder)
    {
        var tokenConfiguration = builder.GetSection("JwtSettings");
        var tokenOptions = tokenConfiguration.Get<JwtSettings>();
        
        if (tokenOptions == null)
        {
            throw new InvalidOperationException("JwtSettings section is missing or not configured properly.");
        }
        
        var key = Encoding.ASCII.GetBytes(tokenOptions.Key);
        
        services.Configure<JwtSettings>(tokenConfiguration);
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}