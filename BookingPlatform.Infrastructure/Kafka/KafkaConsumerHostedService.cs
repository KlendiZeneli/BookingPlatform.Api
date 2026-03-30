using Confluent.Kafka;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookingPlatform.Infrastructure.Kafka;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaOptions _options;
    private readonly ConsumerConfig _config;
    private readonly INotificationService _notifications;
    private readonly string[] _topics = new[]
    {
        KafkaTopics.BookingCreated,
        KafkaTopics.BookingStatusChanged,
        KafkaTopics.ReviewCreated,
        KafkaTopics.OwnerProfileVerified,
        KafkaTopics.UserRegistered
    };

    public KafkaConsumerHostedService(IOptions<KafkaOptions> options, INotificationService notifications)
    {
        _options = options.Value;
        _notifications = notifications;
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

                    HandleMessage(cr.Topic, cr.Message.Value, cancellationToken).GetAwaiter().GetResult();

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

    private async Task HandleMessage(string topic, string payload, CancellationToken ct)
    {
        try
        {
            switch (topic)
            {
                case KafkaTopics.BookingCreated:
                {
                    var evt = JsonSerializer.Deserialize<BookingCreatedEvent>(payload);
                    if (evt is null) return;
                    await _notifications.NotifyUserAsync(evt.GuestId,
                        $"Your booking request has been created for {evt.StartDate:yyyy-MM-dd} to {evt.EndDate:yyyy-MM-dd}.");
                    break;
                }
                case KafkaTopics.BookingStatusChanged:
                {
                    var evt = JsonSerializer.Deserialize<BookingStatusChangedEvent>(payload);
                    if (evt is null) return;
                    await _notifications.NotifyUserAsync(evt.GuestId,
                        $"Your booking status changed: {evt.OldStatus} → {evt.NewStatus}.");
                    break;
                }
                case KafkaTopics.OwnerProfileVerified:
                {
                    var evt = JsonSerializer.Deserialize<OwnerProfileVerifiedEvent>(payload);
                    if (evt is null) return;
                    var message = evt.Approved
                        ? "Your owner profile has been verified. You can now list properties."
                        : "Your owner profile verification was rejected. Please check notes and re-apply.";
                    await _notifications.NotifyUserAsync(evt.UserId, message);
                    break;
                }
                case KafkaTopics.ReviewCreated:
                {
                    var evt = JsonSerializer.Deserialize<ReviewCreatedEvent>(payload);
                    if (evt is null) return;
                    await _notifications.NotifyUserAsync(evt.GuestId,
                        "Your review has been submitted successfully.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Kafka] Notification dispatch error for topic {topic}: {ex.Message}");
        }
    }
}
