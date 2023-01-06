using Confluent.Kafka;
using System;
using System.Threading;

namespace ShippingService
{
    public class ConsumerWrapper
    {
        private string _topicName;
        private ConsumerConfig _consumerConfig;
        private IConsumer<string, string> _consumer;
        private static readonly Random rand = new Random();
        public ConsumerWrapper(ConsumerConfig config)
        {
            this._topicName = "kafka-communication";
            this._consumerConfig = config;
            this._consumer = new ConsumerBuilder<string, string>(this._consumerConfig).Build();
            this._consumer.Subscribe(this._topicName);
        }
        public string readMessage()
        {
            var consumeResult = this._consumer.Consume();
            return consumeResult.Value;
        }
    }
}
