namespace BookingPlatform.Infrastructure.Kafka;

public class KafkaOptions
{
    // Parameterless constructor required for IOptions binding
    public KafkaOptions() { }

    public string BootstrapServers { get; set; } = "localhost:9092";
    public string ClientId { get; set; } = "booking-platform-api";
    public string GroupId { get; set; } = "booking-platform-group";
}
