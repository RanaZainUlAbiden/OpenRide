using MongoDB.Bson;
using MongoDB.Driver;
using Openride.Repositories.Interfaces;

namespace Openride.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public LocationRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BsonDocument>("location_logs");
        }

        public async Task LogLocationAsync(string driverId, double lat, double lng)
        {
            var doc = new BsonDocument
            {
                { "driverId", driverId },
                { "lat", lat },
                { "lng", lng },
                { "timestamp", DateTime.UtcNow }
            };
            await _collection.InsertOneAsync(doc);
        }

        public async Task<(double lat, double lng)?> GetLastLocationAsync(string driverId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("driverId", driverId);
            var sort = Builders<BsonDocument>.Sort.Descending("timestamp");
            var doc = await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync();

            if (doc == null) return null;
            return (doc["lat"].AsDouble, doc["lng"].AsDouble);
        }

        public async Task<IEnumerable<object>> GetNearbyDriversAsync(double lat, double lng, double radiusKm)
        {
            // Get latest location per driver and filter by rough distance
            var pipeline = new[]
            {
                new BsonDocument("$sort", new BsonDocument("timestamp", -1)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$driverId" },
                    { "lat", new BsonDocument("$first", "$lat") },
                    { "lng", new BsonDocument("$first", "$lng") }
                })
            };

            var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return results
                .Where(r => GetDistance(lat, lng, r["lat"].AsDouble, r["lng"].AsDouble) <= radiusKm)
                .Select(r => new { driverId = r["_id"].AsString, lat = r["lat"].AsDouble, lng = r["lng"].AsDouble });
        }

        private double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLng = ToRad(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private double ToRad(double deg) => deg * Math.PI / 180;
    }
}