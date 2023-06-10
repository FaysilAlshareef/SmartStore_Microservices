namespace SmartStore.PaymentsAPI.Models.Stripe
{
    public record AddStripeCustomer
    (
       string Email,
        string Name,
        AddStripeCard CreditCard
    );
}
