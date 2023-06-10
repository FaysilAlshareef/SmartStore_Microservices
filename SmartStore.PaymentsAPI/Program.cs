using SmartStore.MessageBus;
using SmartStore.MessageBus.Interfaces;
using SmartStore.PaymentsAPI.Extensions;
using SmartStore.PaymentsAPI.Repository;
using SmartStore.PaymentsAPI.Services;

namespace SmartStore.PaymentsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Add Stripe Infrastructure

            builder.Services.AddStripeInfrastructure(builder.Configuration);
            builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>(); 
            builder.Services.AddSingleton<IMessageBus,AzureServiceBus>();  
           

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