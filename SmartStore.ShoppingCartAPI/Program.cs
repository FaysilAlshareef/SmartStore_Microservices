using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartStore.MessageBus.Interfaces;
using SmartStore.MessageBus;
using SmartStore.ShoppingCartAPI.Dtos;
using SmartStore.ShoppingCartAPI.Helpers;
using SmartStore.ShoppingCartAPI.Repository;
using SmartStore.ShoppingCartAPI.ShoppingCartData;
using SmartStore.ShoppingCartAPI.RabbitMQSender;

namespace SmartStore.ShoppingCartAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Allow Dependancy injection for RedisDbContext

            builder.Services.AddDbContext<ShoppingCartDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped(typeof(ICartRepository), typeof(CartRepository));
            builder.Services.AddScoped(typeof(ICouponRepository), typeof(CouponRepository));
            builder.Services.AddScoped(typeof(ShoppingCartResponseDto));
            builder.Services.AddSingleton<IMessageBus, AzureServiceBus>();
            builder.Services.AddSingleton<IRabbitMQCheckoutMessageSender, RabbitMQCheckoutMessageSender>();


            builder.Services.AddControllers();

            builder.Services.AddHttpClient<ICouponRepository, CouponRepository>(
                hc => hc.BaseAddress = new Uri(builder.Configuration["ApiUrls:CouponApi"])
                );

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
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "SmartStore.ShoppingCartApi" });
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