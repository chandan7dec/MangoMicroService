namespace Mango.Services.RewardAPI.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        public Task Strat();
        public Task Stop();
    }
}
