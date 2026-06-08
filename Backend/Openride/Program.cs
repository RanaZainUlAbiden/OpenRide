using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Neo4j.Driver;
using Openride.Data;
using Openride.Repositories;
using Openride.Repositories.Interfaces;
using Openride.Services;

var builder = WebApplication.CreateBuilder(args);

// ── PostgreSQL ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// ── MongoDB ─────────────────────────────────────────────
var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
var mongoDatabase = mongoClient.GetDatabase(builder.Configuration["MongoDB:DatabaseName"]);
builder.Services.AddSingleton(mongoDatabase);

// ── Neo4j ────────────────────────────────────────────────
var neo4jDriver = GraphDatabase.Driver(
    builder.Configuration.GetConnectionString("Neo4j"),
    AuthTokens.Basic(
        builder.Configuration["Neo4j:Username"]!,
        builder.Configuration["Neo4j:Password"]!));
builder.Services.AddSingleton(neo4jDriver);

// ── Repositories & Services ──────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IGraphRepository, GraphRepository>();
builder.Services.AddScoped<RideService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── Auto migrate PostgreSQL on startup ───────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();