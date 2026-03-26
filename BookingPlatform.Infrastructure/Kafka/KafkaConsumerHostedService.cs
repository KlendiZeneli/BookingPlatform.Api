using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Channels;

namespace BookingPlatform.Infrastructure.Kafka;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaOptions _options;
    private readonly ConsumerConfig _config;
    private readonly string[] _topics = new[] { "booking-created", "booking-status-changed", "review-created", "owner-verified", "user-registered" };

    public KafkaConsumerHostedService(IOptions<KafkaOptions> options)
    {
        _options = options.Value;
        _config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }

    private void StartConsumerLoop(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config).Build();
        consumer.Subscribe(_topics);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(cancellationToken);
                    // Basic logging; in production route to handlers
                    Console.WriteLine($"[Kafka] Received message from {cr.Topic}: {cr.Message.Value}");

                    consumer.Commit(cr);
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"[Kafka] Consume error: {ex.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            consumer.Close();
        }
    }
}
