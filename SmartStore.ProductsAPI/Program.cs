using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartStore.OrdersAPI.Services;
using SmartStore.ProductsAPI.Data;
using SmartStore.ProductsAPI.Helpers;
using SmartStore.ProductsAPI.Repository;

namespace SmartStore.ProductsAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);




            // Add services to the container.

            // Register ProductsDbContext 
            //builder.Services.AddDbContext<ProductsDbContext>(options =>
            //{
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            //});
            var optionBuilder = new DbContextOptionsBuilder<ProductsDbContext>();
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddSingleton(new ProductRepository(optionBuilder.Options));
            builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthentication("Bearer")
             .AddJwtBearer("Bearer", opt =>
                {
                    opt.Authority = builder.Configuration["ApiUrls:IdentityServer"];
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "SmartStore");
                });
            });


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "SmartStore.ProductsAPI" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Enter 'Bearer' [space] and your token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id= "Bearer"
                            },
                            Scheme="outh2",
                            Name="Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                        }
                    });
            });


            var app = builder.Build();

            #region Apply Migrations And Data Seeding
            var services = app.Services.CreateScope();
            var loggerFactory = services.ServiceProvider.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.ServiceProvider.GetRequiredService<ProductsDbContext>();
                await context.Database.MigrateAsync();

                //await StoreContextSeed.SeedAsync(context, loggerFactory);

                //var IdentityContext = services.GetRequiredService<AppIdentityDbContext>();
                //await IdentityContext.Database.MigrateAsync();

                //var userManager = services.GetRequiredService<UserManager<AppUser>>();
                //await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, ex.Message);
            }
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}