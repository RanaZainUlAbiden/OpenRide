namespace Openride.Models
{
    public class Driver
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = false;
        public double Rating { get; set; } = 0.0;
        public int TotalRides { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public ICollection<Ride> Rides { get; set; } = new List<Ride>();
    }
}