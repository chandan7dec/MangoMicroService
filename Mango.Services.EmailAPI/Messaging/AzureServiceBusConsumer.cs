using Azure.Messaging.ServiceBus;
using Mango.Service.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartprocessor;
        private ServiceBusProcessor _registerUserprocessor;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartprocessor = client.CreateProcessor(emailCartQueue);
            _registerUserprocessor = client.CreateProcessor(registerUserQueue);

        }

        public async Task Stop()
        {
            _emailCartprocessor.StopProcessingAsync();
            _emailCartprocessor.DisposeAsync();

            _registerUserprocessor.StopProcessingAsync();
            _registerUserprocessor.DisposeAsync();
        }

        public async Task Strat()
        {
            _emailCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartprocessor.StartProcessingAsync();

            _registerUserprocessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserprocessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserprocessor.StartProcessingAsync();
        }

        

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs arg)
        {
           //this is where we will receive cart message
           var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //TODO  try to log email
                await _emailService.EmailCartAndLog(objMessage);
                await arg.CompleteMessageAsync(arg.Message);
            }catch(Exception ex)
            {
                throw;
            }
        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs arg)
        {
            //this is where we will receive registration message
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //TODO  try to log email
                await _emailService.RegisterUserEmailAndLog(email);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
