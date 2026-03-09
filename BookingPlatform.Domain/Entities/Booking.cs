
namespace BookingPlatform.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public Guid GuestId { get; set; }
    public User Guest { get; set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int GuestCount { get; set; }

    public decimal CleaningFee { get; set; }
    public decimal AmenitiesUpCharge { get; set; }
    public decimal PriceForPeriod { get; set; }
    public decimal TotalPrice { get; set; }

    public BookingPlatform.Domain.Enums.BookingStatus BookingStatus { get; set; } = BookingPlatform.Domain.Enums.BookingStatus.Created;

    public DateTime? CreatedOnUtc { get; set; }
    public DateTime? ConfirmedOnUtc { get; set; }
    public DateTime? RejectedOnUtc { get; set; }
    public DateTime? CompletedOnUtc { get; set; }
    public DateTime? CancelledOnUtc { get; set; }

    public Review? Review { get; set; }
}
