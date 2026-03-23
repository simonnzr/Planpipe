using Microsoft.EntityFrameworkCore;
using Planpipe.Core.Enums;
using Planpipe.Core.Models;
using Planpipe.Infrastructure.Data;
using Planpipe.Infrastructure.Repositories;
using Testcontainers.PostgreSql;
using Xunit;

namespace Planpipe.IntegrationTests;

public class ProcessingRunRepositoryTests : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    private PlanpipeDbContext? _context;
    private ProcessingRunRepository? _repository;

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
        await _context.Database.EnsureCreatedAsync();

        _repository = new ProcessingRunRepository(_context);
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
    public async Task CreateAsync_SavesProcessingRun()
    {
        var run = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = "test.pdf",
            Status = ProcessingStatus.Pending,
            StartedAt = DateTime.UtcNow
        };

        var result = await _repository!.CreateAsync(run);

        Assert.NotNull(result);
        Assert.Equal(run.Id, result.Id);
        Assert.Equal(run.FileName, result.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectRun()
    {
        var run = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = "test2.pdf",
            Status = ProcessingStatus.Completed
        };

        await _repository!.CreateAsync(run);
        var retrieved = await _repository.GetByIdAsync(run.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(run.Id, retrieved.Id);
        Assert.Equal(run.FileName, retrieved.FileName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRuns()
    {
        var run1 = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = "test3.pdf",
            Status = ProcessingStatus.Pending
        };
        var run2 = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = "test4.pdf",
            Status = ProcessingStatus.Completed
        };

        await _repository!.CreateAsync(run1);
        await _repository.CreateAsync(run2);

        var allRuns = await _repository.GetAllAsync();

        Assert.True(allRuns.Count >= 2);
        Assert.Contains(allRuns, r => r.Id == run1.Id);
        Assert.Contains(allRuns, r => r.Id == run2.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesRun()
    {
        var run = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = "test5.pdf",
            Status = ProcessingStatus.Pending
        };

        await _repository!.CreateAsync(run);
        run.Status = ProcessingStatus.Completed;
        run.CompletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(run);

        var updated = await _repository.GetByIdAsync(run.Id);

        Assert.NotNull(updated);
        Assert.Equal(ProcessingStatus.Completed, updated.Status);
        Assert.NotNull(updated.CompletedAt);
    }

    [Fact]
    public async Task DeleteAsync_RemovesRun()
    {
        var run = new ProcessingRun
        {
            Id = Guid.NewGuid(),
            FileName = "test6.pdf",
            Status = ProcessingStatus.Pending
        };

        await _repository!.CreateAsync(run);
        await _repository.DeleteAsync(run.Id);

        var deleted = await _repository.GetByIdAsync(run.Id);

        Assert.Null(deleted);
    }
}
