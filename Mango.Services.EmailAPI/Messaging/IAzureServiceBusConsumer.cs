namespace Mango.Services.EmailAPI.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        public Task Strat();
        public Task Stop();
    }
}
