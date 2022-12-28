using System.Diagnostics;
using Common;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using TestProject;

public class WriteToDbService : IHostedService
{
    private readonly IDbContextFactory<MarketOrdersContext> factory;
    public WriteToDbService(IDbContextFactory<MarketOrdersContext> factory)
    {
        this.factory = factory;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Thread wdb = new Thread(Loop);
        wdb.Start();
    }
    private void Loop(object? obj)
    {
        var config = new Config();
        config.Add("dataSize", 100);
        var bulkConfig = new BulkConfig
        {
            //PreserveInsertOrder = true, // true is default
            SetOutputIdentity = true,
            //BatchSize = 4000,
            UseTempDB = true,
            //CalculateStats = true
        };

        var generator = new Generator(config);
        while (true)
        {
            List<MarketOrderVm> marketOrderVms = generator.Execute();
            using var dbContext = factory.CreateDbContext();
            using var transaction = dbContext.Database.BeginTransaction();

            dbContext.BulkInsertOrUpdate(marketOrderVms, bulkConfig);
            transaction.Commit();

            Thread.Sleep(2000);
        }
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Debug.WriteLine($"{nameof(WriteToDbService)} stopped");
    }
}
