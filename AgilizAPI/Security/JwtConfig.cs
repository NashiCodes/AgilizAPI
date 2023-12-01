#region

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace AgilizAPI.Security;

public static class JwtConfig
{
    public static void ConfigureJWT(this IServiceCollection Services, IConfiguration config)
    {
        var key =
            Encoding.ASCII.GetBytes(config.GetValue<string>("PrivateKey") ?? throw new InvalidOperationException());
        Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken            = true;
            x.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(key),
                ValidateIssuer           = false,
                ValidateAudience         = false
            };
        });
    }

    public static void UseJWT(this IApplicationBuilder app)
    {
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
    }
}