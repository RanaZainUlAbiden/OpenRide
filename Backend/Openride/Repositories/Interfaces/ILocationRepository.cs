namespace Openride.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        Task LogLocationAsync(string driverId, double lat, double lng);
        Task<(double lat, double lng)?> GetLastLocationAsync(string driverId);
        Task<IEnumerable<object>> GetNearbyDriversAsync(double lat, double lng, double radiusKm);
    }
}