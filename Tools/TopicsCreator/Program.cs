using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka.Admin;
using Confluent.Kafka;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var bootstrap = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP") ?? "localhost:9092";

        var adminConfig = new AdminClientConfig { BootstrapServers = bootstrap };

        using var admin = new AdminClientBuilder(adminConfig).Build();

        var topics = new List<TopicSpecification>
        {
            new TopicSpecification { Name = "user-registered", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "password-reset-requested", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "password-changed", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "password-reset-completed", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "owner-profile-created", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "owner-profile-verified", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "property-created", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "property-updated", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "property-deleted", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "booking-created", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "booking-status-changed", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "review-created", NumPartitions = 1, ReplicationFactor = 1 },
        };

        try
        {
            Console.WriteLine($"Creating topics on {bootstrap}...");
            await admin.CreateTopicsAsync(topics);
            Console.WriteLine("Topics created successfully.");
            return 0;
        }
        catch (CreateTopicsException ex)
        {
            Console.WriteLine("Some topics could not be created:");
            foreach (var result in ex.Results)
            {
                Console.WriteLine($"{result.Topic}: {result.Error.Reason}");
            }
            return 2;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
            return 1;
        }
    }
}
