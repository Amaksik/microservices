using Confluent.Kafka;
using Newtonsoft.Json;

namespace ShippingService
{
    public class UpdateDatabaseService : BackgroundService
    {

        private readonly ConsumerConfig consumerConfig;
        public UpdateDatabaseService(ConsumerConfig consumerConfig)
        {
            this.consumerConfig = consumerConfig;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("OrderProcessing Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumerHelper = new ConsumerWrapper(consumerConfig);
                string orderRequest = consumerHelper.readMessage();

                //Deserilaize 
                CountriesRequest request = JsonConvert.DeserializeObject<CountriesRequest>(orderRequest);

                //TODO:: Implement database update
                Console.WriteLine($"Info: UpdateHandler => Processing the order for {request.id}");
                request.status = OrderStatus.COMPLETED;

            }
        }
    }
}

