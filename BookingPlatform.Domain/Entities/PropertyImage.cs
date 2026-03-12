namespace BookingPlatform.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public byte[] ImageData { get; set; } = default!;
    public string ContentType { get; set; } = default!;

    public bool IsPrimary { get; set; }
}
