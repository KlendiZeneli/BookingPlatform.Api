using BookingPlatform.Application.Common.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookingPlatform.Infrastructure.Kafka;

public class KafkaProducer : IEventProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;

    public KafkaProducer(IOptions<KafkaOptions> options)
    {
        _options = options.Value;
        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            ClientId = _options.ClientId
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T @event, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(@event);
        await _producer.ProduceAsync(topic, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = json }, ct);
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(5));
        _producer?.Dispose();
    }
}
