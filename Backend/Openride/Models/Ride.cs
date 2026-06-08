namespace Openride.Models
{
    public class Ride
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RiderId { get; set; }
        public Guid DriverId { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public double DropoffLat { get; set; }
        public double DropoffLng { get; set; }
        public string Status { get; set; } = "requested"; // requested, accepted, ongoing, completed, cancelled
        public decimal Fare { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public User Rider { get; set; } = null!;
        public Driver Driver { get; set; } = null!;
    }
}