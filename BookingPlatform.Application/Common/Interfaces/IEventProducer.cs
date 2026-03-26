using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T @event, CancellationToken ct = default);
}
