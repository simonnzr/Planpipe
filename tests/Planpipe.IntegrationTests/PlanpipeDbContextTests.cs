using Microsoft.EntityFrameworkCore;
using Planpipe.Infrastructure.Data;
using Testcontainers.PostgreSql;
using Xunit;

namespace Planpipe.IntegrationTests;

public class PlanpipeDbContextTests : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    private PlanpipeDbContext? _context;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("planpipe_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<PlanpipeDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _context = new PlanpipeDbContext(options);
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
        if (_postgres != null)
        {
            await _postgres.DisposeAsync();
        }
    }

    [Fact]
    public async Task Database_CanBeCreated()
    {
        var created = await _context!.Database.EnsureCreatedAsync();
        Assert.True(created);
    }

    [Fact]
    public async Task Database_HasAllRequiredTables()
    {
        await _context!.Database.EnsureCreatedAsync();

        var tables = await _context.Database
            .SqlQueryRaw<string>("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'")
            .ToListAsync();

        Assert.Contains("ProcessingRuns", tables);
        Assert.Contains("PlanFeatures", tables);
        Assert.Contains("QuantityItems", tables);
        Assert.Contains("GroundTruthItems", tables);
        Assert.Contains("ComparisonResults", tables);
    }

    [Fact]
    public async Task DbSets_AreAccessible()
    {
        await _context!.Database.EnsureCreatedAsync();

        Assert.NotNull(_context.ProcessingRuns);
        Assert.NotNull(_context.PlanFeatures);
        Assert.NotNull(_context.QuantityItems);
        Assert.NotNull(_context.GroundTruthItems);
        Assert.NotNull(_context.ComparisonResults);
    }
}
