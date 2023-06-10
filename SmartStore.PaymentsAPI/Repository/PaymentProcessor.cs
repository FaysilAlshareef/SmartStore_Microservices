using SmartStore.PaymentsAPI.Dtos;
using SmartStore.PaymentsAPI.Models.Stripe;
using SmartStore.PaymentsAPI.Services.Stripe;
using Stripe;
using System.Collections.Immutable;

namespace SmartStore.PaymentsAPI.Repository
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly IStripeAppService _stripeService;
        private readonly IConfiguration _configuration;

        public PaymentProcessor(IStripeAppService stripeService, IConfiguration configuration)
        {
            _stripeService = stripeService;
            _configuration = configuration;
        }
        public async Task<bool> ProcessPayment(PaymentRequestMessageDto paymentMessage)
        {
            //https://blog.christian-schou.dk/implement-stripe-payments-in-asp-net6/
            // Payment Gatway Logic Code Here

            var customerId = await GetStripeCustomerId(paymentMessage);

            if (customerId == null) return false;

            var AddStripePayment = new AddStripePayment(
                customerId,
                paymentMessage.Email,
                Description: $"Payment for Order with :{paymentMessage.OrderId}",
                Currency: "usd",
                Amount: (long)paymentMessage.OrderTotal * 100
                );
            var payment = await _stripeService.AddStripePaymentAsync(AddStripePayment, CancellationToken.None);
            if (payment != null && payment.PaymentId!=null) { }
            return true;
            
           
        }

        private async Task<string> GetStripeCustomerId(PaymentRequestMessageDto paymentMessage)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var options = new CustomerSearchOptions
            {
                Query = $"email:'{paymentMessage.Email}'",
            };
            var service = new CustomerService();
            var customer = service.Search(options);

            //check if customer is existed
            if (customer.Data.Count != 0)
            {
                var id = customer.Data.Select(c=>c.Id).FirstOrDefault();
                return id;
            }

            // create new customer
            AddStripeCustomer stripeCustomer = new AddStripeCustomer(
                   paymentMessage.Email,
                   paymentMessage.Name,
                   CreateStripCard(paymentMessage)

                    );
            StripeCustomer createdCustomer = await _stripeService
                .AddStripeCustomerAsync(stripeCustomer, CancellationToken.None);
            return createdCustomer.CustomerId;


        }
        private AddStripeCard CreateStripCard(PaymentRequestMessageDto paymentMessage)
        {
            //Get expirationDate and sepatate them 
            var ExpiryMonthYear = paymentMessage.ExpiryMonthYear.ToString();
            var expirationYear = ExpiryMonthYear.Substring(ExpiryMonthYear.Length - 2);
            var expirationMonth = ExpiryMonthYear.Substring(0, 2);

            // Create Object from stripe Card
            var stripeCard = new AddStripeCard(
                paymentMessage.Name,
                paymentMessage.CardNumber,
                expirationYear,
                expirationMonth,
                paymentMessage.CVV
                );

            return stripeCard;
        }
    }
}
