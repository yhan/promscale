using Microsoft.EntityFrameworkCore;
using TestProject;

namespace API;

public class ParallelQueryDb : BackgroundService
{
    private readonly IDbContextFactory<MarketOrdersContext> factory;
    private readonly ILogger<ParallelQueryDb> logger;
    public ParallelQueryDb(IDbContextFactory<MarketOrdersContext> factory, 
        ILogger<ParallelQueryDb> logger)
    {
        this.factory = factory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Parallel.ForEach(Enumerable.Range(1, 5),
        i => {
            // no DbContext dispose
            var context = factory.CreateDbContext();
            context.Database.ExecuteSqlRaw("SELECT pg_sleep(10);");
            logger.LogInformation($"Run #{i} Slept 10 sec");
        });
    }
}