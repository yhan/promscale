using Microsoft.EntityFrameworkCore;

namespace TestProject;

[TestFixture]
public class ConnectionPool
{
    [TestCase("Host=localhost;Port=5444;Database=hello;Username=postgres;Password=postgres;Search Path=daily;Pooling=true;Minimum Pool Size=5;Connection Idle Lifetime=5;Connection Pruning Interval=2;Application Name=API")]
    [TestCase("Host=localhost;Port=5444;Database=hello;Username=postgres;Password=postgres;Search Path=daily;Pooling=false;Application Name=API")]
    public async Task TestConnectionPool(string cnxString)
    {
        var contextOptions = new DbContextOptionsBuilder<MarketOrdersContext>()
            .UseNpgsql(cnxString)
            .LogTo(TestContext.WriteLine)
            .Options;
            
        // dbContext not disposed.
        var dbContext = new MarketOrdersContext(contextOptions);
        
        var cnt = await dbContext.MarketOrderVms.CountAsync();
        TestContext.WriteLine($"cnt={cnt}");
    }
}
