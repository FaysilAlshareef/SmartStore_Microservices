using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace SmartStore.Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthentication("Bearer")
                 .AddJwtBearer("Bearer", opt =>
                 {
                     opt.Authority = builder.Configuration["ApiUrls:IdentityServer"];
                     opt.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateAudience = false
                     };
                 });

        builder.Services.AddOcelot();

        var app = builder.Build();

        //app.MapGet("/", () => "Hello World!");
        //app.UseAuthentication();
        app.UseOcelot();
        app.Run();
    }
}
