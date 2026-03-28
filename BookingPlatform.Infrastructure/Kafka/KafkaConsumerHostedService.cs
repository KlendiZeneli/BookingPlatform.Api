using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookingPlatform.Infrastructure.Kafka;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaOptions _options;
    private readonly ConsumerConfig _config;
    private readonly IServiceProvider _serviceProvider;

    private readonly string[] _topics = new[]
    {
        KafkaTopics.BookingCreated,
        KafkaTopics.BookingStatusChanged,
        KafkaTopics.ReviewCreated,
        KafkaTopics.OwnerProfileVerified,
        KafkaTopics.UserRegistered
    };

    public KafkaConsumerHostedService(IOptions<KafkaOptions> options, IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
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
                    Console.WriteLine($"[Kafka] Received message from {cr.Topic}: {cr.Message.Value}");

                    Task.Run(() => HandleMessageAsync(cr.Topic, cr.Message.Value, cancellationToken), cancellationToken);

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

    private async Task HandleMessageAsync(string topic, string json, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var notifications = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
        if (topic == KafkaTopics.BookingCreated)
            {
                var e = JsonSerializer.Deserialize<BookingCreatedEvent>(json);
                if (e is null) return;
                // Notify the guest that their booking was successfully created
                await notifications.SendToUserAsync(
                    e.GuestId.ToString(),
                    "BookingCreated",
                    new { e.BookingId, e.PropertyId, e.StartDate, e.EndDate, e.TotalPrice, e.OccurredAtUtc },
                    ct);
            }
            else if (topic == KafkaTopics.BookingStatusChanged)
            {
                var e = JsonSerializer.Deserialize<BookingStatusChangedEvent>(json);
                if (e is null) return;
                // Notify the guest their booking status changed
                await notifications.SendToUserAsync(
                    e.GuestId.ToString(),
                    "BookingStatusChanged",
                    new { e.BookingId, e.OldStatus, e.NewStatus, e.OccurredAtUtc },
                    ct);
            }
            else if (topic == KafkaTopics.ReviewCreated)
            {
                var e = JsonSerializer.Deserialize<ReviewCreatedEvent>(json);
                if (e is null) return;
                // Notify the guest their review was recorded
                await notifications.SendToUserAsync(
                    e.GuestId.ToString(),
                    "ReviewCreated",
                    new { e.ReviewId, e.BookingId, e.PropertyId, e.Rating, e.OccurredAtUtc },
                    ct);
            }
            else if (topic == KafkaTopics.OwnerProfileVerified)
            {
                var e = JsonSerializer.Deserialize<OwnerProfileVerifiedEvent>(json);
                if (e is null) return;
                // Notify the owner their profile verification result
                await notifications.SendToUserAsync(
                    e.UserId.ToString(),
                    "OwnerProfileVerified",
                    new { e.Approved, e.Notes, e.VerifiedAtUtc, e.OccurredAtUtc },
                    ct);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Kafka] Error handling message from {topic}: {ex.Message}");
        }
    }
}
