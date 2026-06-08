using Microsoft.AspNetCore.Mvc;
using Openride.Models;
using Openride.Services;

namespace Openride.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideController : ControllerBase
    {
        private readonly RideService _rideService;
        public RideController(RideService rideService)
        {
            _rideService = rideService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var user = await _rideService.RegisterUserAsync(req.Name, req.Phone, req.Email);
            return Ok(user);
        }

        [HttpPost("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationRequest req)
        {
            await _rideService.UpdateDriverLocationAsync(req.DriverId, req.Lat, req.Lng);
            return Ok(new { message = "Location updated" });
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyDrivers([FromQuery] double lat, [FromQuery] double lng)
        {
            var drivers = await _rideService.FindNearbyDriversAsync(lat, lng);
            return Ok(drivers);
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteRide([FromBody] CompleteRideRequest req)
        {
            await _rideService.CompleteRideAsync(req.RideId, req.RiderId, req.DriverId);
            return Ok(new { message = "Ride completed and graph updated" });
        }

        [HttpGet("trust")]
        public async Task<IActionResult> GetTrustScore([FromQuery] Guid riderId, [FromQuery] Guid driverId)
        {
            var score = await _rideService.GetTrustScoreAsync(riderId, driverId);
            return Ok(new { trustScore = score });
        }
    }

    public record RegisterRequest(string Name, string Phone, string Email);
    public record LocationRequest(string DriverId, double Lat, double Lng);
    public record CompleteRideRequest(Guid RideId, Guid RiderId, Guid DriverId);
}