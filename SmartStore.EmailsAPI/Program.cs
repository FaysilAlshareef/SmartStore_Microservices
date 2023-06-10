using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartStore.EmailsAPI.Data;
using SmartStore.MessageBus.Interfaces;
using SmartStore.MessageBus;
using SmartStore.EmailsAPI.Repository;
using SmartStore.EmailsAPI.Services;
using SmartStore.EmailsAPI.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SmartStore.EmailsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Add services to the container.
            builder.Services.AddDbContext<EmailDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IEmailRepository, EmailRepository>();
            var OptionBuilder = new DbContextOptionsBuilder<EmailDbContext>();
            OptionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddSingleton(new EmailRepository(OptionBuilder.Options));
            builder.Services.AddSingleton<SmartStore.EmailsAPI.Services.IAzureServiceBusConsumer, AzureServiceBusConsumer>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseAzureServiceBusConsumer();
            app.Run();
        }
    }
}