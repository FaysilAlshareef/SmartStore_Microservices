using SmartStore.PaymentsAPI.Services.Stripe;
using Stripe;

namespace SmartStore.PaymentsAPI.Extensions
{
    public static class StripeInfrastructure
    {
        public static IServiceCollection AddStripeInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeSettings:SecretKey");

            return services
                .AddSingleton<CustomerService>()
                .AddSingleton<ChargeService>()
                .AddSingleton<TokenService>()
                .AddSingleton<IStripeAppService, StripeAppService>();
        }
    }
}
