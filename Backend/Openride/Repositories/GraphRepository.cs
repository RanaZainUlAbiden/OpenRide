using Neo4j.Driver;
using Openride.Repositories.Interfaces;

namespace Openride.Repositories
{
    public class GraphRepository : IGraphRepository
    {
        private readonly IDriver _driver;
        public GraphRepository(IDriver driver) => _driver = driver;

        public async Task CreateUserNodeAsync(string userId, string name)
        {
            await using var session = _driver.AsyncSession();
            await session.RunAsync(
                "MERGE (u:User {id: $id}) SET u.name = $name",
                new { id = userId, name });
        }

        public async Task CreateDriverNodeAsync(string driverId, string name)
        {
            await using var session = _driver.AsyncSession();
            await session.RunAsync(
                "MERGE (d:Driver {id: $id}) SET d.name = $name",
                new { id = driverId, name });
        }

        public async Task CreateRideRelationshipAsync(string riderId, string driverId, string rideId)
        {
            await using var session = _driver.AsyncSession();
            await session.RunAsync(@"
                MATCH (r:User {id: $riderId})
                MATCH (d:Driver {id: $driverId})
                MERGE (r)-[rel:TOOK_RIDE {rideId: $rideId}]->(d)
                SET rel.at = datetime()",
                new { riderId, driverId, rideId });
        }

        public async Task<IEnumerable<string>> GetDriversRiderHasUsedAsync(string riderId)
        {
            await using var session = _driver.AsyncSession();
            var result = await session.RunAsync(
                "MATCH (u:User {id: $riderId})-[:TOOK_RIDE]->(d:Driver) RETURN d.id AS driverId",
                new { riderId });
            var records = await result.ToListAsync();
            return records.Select(r => r["driverId"].As<string>());
        }

        public async Task<double> GetTrustScoreAsync(string riderId, string driverId)
        {
            await using var session = _driver.AsyncSession();
            var result = await session.RunAsync(@"
                MATCH (u:User {id: $riderId})-[r:TOOK_RIDE]->(d:Driver {id: $driverId})
                RETURN count(r) AS rideCount",
                new { riderId, driverId });
            var record = await result.SingleAsync();
            var count = record["rideCount"].As<long>();
            return Math.Min(count * 10.0, 100.0); // 10 points per ride, max 100
        }
    }
}