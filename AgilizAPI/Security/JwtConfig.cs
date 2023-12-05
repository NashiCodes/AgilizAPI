#region

using System.Text;
using AgilizAPI.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace AgilizAPI.Security;

public static class JwtConfig
{
    public static IServiceCollection ConfigureJWT(this IServiceCollection Services, IConfiguration config)
    {
        var key =
            Encoding.ASCII.GetBytes(config.GetValue<string>("PrivateKey") ?? throw new InvalidOperationException());
        Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(key),
                ValidateIssuer           = false,
                ValidateAudience         = false
            };
        });

        return Services;
    }

    public static IServiceCollection ConfigureClaims(this IServiceCollection Services)
    {
        foreach (var VARIABLE in IdentityData.getRoles())
            Services.AddAuthorization(options =>
            {
                options.AddPolicy(VARIABLE.Key,
                                  policy => policy.RequireClaim("Role", VARIABLE.Value));
            });

        return Services;
    }

    public static void UseJWT(this IApplicationBuilder app)
    {
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
    }
}