using Openride.Models;
using Openride.Repositories.Interfaces;

namespace Openride.Services
{
    public class RideService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILocationRepository _locationRepo;
        private readonly IGraphRepository _graphRepo;

        public RideService(IUserRepository userRepo, ILocationRepository locationRepo, IGraphRepository graphRepo)
        {
            _userRepo = userRepo;
            _locationRepo = locationRepo;
            _graphRepo = graphRepo;
        }

        public async Task<User> RegisterUserAsync(string name, string phone, string email)
        {
            var user = new User { Name = name, Phone = phone, Email = email };
            var created = await _userRepo.CreateAsync(user);
            await _graphRepo.CreateUserNodeAsync(created.Id.ToString(), created.Name);
            return created;
        }

        public async Task UpdateDriverLocationAsync(string driverId, double lat, double lng)
        {
            await _locationRepo.LogLocationAsync(driverId, lat, lng);
        }

        public async Task<IEnumerable<object>> FindNearbyDriversAsync(double lat, double lng)
        {
            return await _locationRepo.GetNearbyDriversAsync(lat, lng, radiusKm: 5);
        }

        public async Task CompleteRideAsync(Guid rideId, Guid riderId, Guid driverId)
        {
            await _graphRepo.CreateRideRelationshipAsync(
                riderId.ToString(),
                driverId.ToString(),
                rideId.ToString());
        }

        public async Task<double> GetTrustScoreAsync(Guid riderId, Guid driverId)
        {
            return await _graphRepo.GetTrustScoreAsync(riderId.ToString(), driverId.ToString());
        }
    }
}