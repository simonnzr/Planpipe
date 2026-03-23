using Microsoft.EntityFrameworkCore;
using Planpipe.Application.Services;
using Planpipe.Core.Interfaces;
using Planpipe.Infrastructure.Data;
using Planpipe.Infrastructure.Repositories;
using Planpipe.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PlanpipeDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IProcessingRunRepository, ProcessingRunRepository>();
builder.Services.AddScoped<IQuantityRepository, QuantityRepository>();
builder.Services.AddScoped<IComparisonRepository, ComparisonRepository>();
builder.Services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
builder.Services.AddScoped<IFeatureExtractor, FeatureExtractorService>();
builder.Services.AddScoped<IQuantityDeriver, QuantityDeriverService>();
builder.Services.AddScoped<IConfidenceScorer, ConfidenceScorerService>();
builder.Services.AddScoped<IGroundTruthComparator, GroundTruthComparatorService>();
builder.Services.AddScoped<PlanIngestionPipeline>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
