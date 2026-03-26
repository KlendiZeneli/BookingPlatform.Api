namespace BookingPlatform.Infrastructure.Kafka;

public record KafkaOptions
(
    string BootstrapServers,
    string ClientId,
    string GroupId
);
