namespace Openride.Repositories.Interfaces
{
    public interface IGraphRepository
    {
        Task CreateUserNodeAsync(string userId, string name);
        Task CreateDriverNodeAsync(string driverId, string name);
        Task CreateRideRelationshipAsync(string riderId, string driverId, string rideId);
        Task<IEnumerable<string>> GetDriversRiderHasUsedAsync(string riderId);
        Task<double> GetTrustScoreAsync(string riderId, string driverId);
    }
}