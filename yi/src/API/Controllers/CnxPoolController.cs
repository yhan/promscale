using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CnxPoolController
{
    private readonly MarketOrdersContext context;
    private readonly ILogger<CnxPoolController> logger;
    private readonly int noOpDurationInSec;
    
    public CnxPoolController(MarketOrdersContext context, IConfiguration config, ILogger<CnxPoolController> logger)
    {
        this.noOpDurationInSec = int.Parse(config["NoOpDurationInSec"]);
        this.context = context;
        this.logger = logger;
    }

    [HttpGet("UnitOfWorkNoTransaction")]
    public void UnitOfWorkNoTransaction()
    {
        UnitOfWork();
    }

    [HttpGet("UnitOfWorkWithTransaction")]
    public void UnitOfWorkWithTransaction()
    {
        context.Database.BeginTransaction();
        UnitOfWork();
        context.Database.CommitTransaction();
    }

    [HttpGet("ReadAndClose")]
    public int ReadAndClose()
    {
        return context.MarketOrderVms.Count();
    }


    [HttpGet("WriteAndClose")]
    public void WriteAndClose()
    {
        context.MarketOrderVms.AddRange(new Generator().Execute());
        context.SaveChanges();
    }



    
    
    private void UnitOfWork()
    {
        context.Database.ExecuteSqlRaw("SELECT pg_sleep(10);");
        logger.LogInformation($"Run #1 Slept 10 sec");

        Thread.Sleep(TimeSpan.FromSeconds(noOpDurationInSec));

        logger.LogInformation($"START Run #2 sleeping 10 sec...");
        context.Database.ExecuteSqlRaw("SELECT pg_sleep(10);");
        logger.LogInformation($"Run #2 Slept 10 sec");
    }
    
    
    
}
