using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartStore.MessageBus;
using SmartStore.MessageBus.Interfaces;
using SmartStore.OrdersAPI.Data;
using SmartStore.OrdersAPI.Extensions;
using SmartStore.OrdersAPI.RabbitMQSender;
using SmartStore.OrdersAPI.Repository;
using SmartStore.OrdersAPI.Services;


namespace SmartStore.OrdersAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Allow Dependancy injection for RedisDbContext

            var optionBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddHostedService<RabbitMQCheckoutConsumer>();
            builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

            builder.Services.AddSingleton(new OrderRepository(optionBuilder.Options));
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
            builder.Services.AddSingleton<IMessageBus, AzureServiceBus>();
            builder.Services.AddSingleton<IRabbitMQPaymentRequestMessageSender, RabbitMQPaymentRequestMessageSender>();
            builder.Services.AddControllers()

               ;
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
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "SmartStore.OrderApi" });
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

            app.UseAzureServiceBusConsumer();
            app.Run();
        }
    }
}