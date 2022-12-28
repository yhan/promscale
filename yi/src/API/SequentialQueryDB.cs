using Microsoft.EntityFrameworkCore;
using TestProject;

namespace API;

public class SequentialQueryDB : IHostedService
{
    private readonly MarketOrdersContext context;
    private readonly ILogger<SequentialQueryDB> logger;

    public SequentialQueryDB(MarketOrdersContext context, ILogger<SequentialQueryDB> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    private void Loop(object? obj)
    {
        while (true)
        {
            logger.LogInformation($"cnt={context.MarketOrderVms.Count()}");
            context.Database.ExecuteSqlRaw("SELECT pg_sleep(10);");
            logger.LogInformation("*** Slept 10 sec");
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Thread wdb = new Thread(Loop);
        wdb.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("LongRunningProcess Stopped");
    }
}