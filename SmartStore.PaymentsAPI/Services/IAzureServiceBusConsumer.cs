namespace SmartStore.PaymentsAPI.Services
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
