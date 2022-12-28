using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using Common;
using CsvHelper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace TestProject;

public class GenerateTestData
{
    [Test]
    public void GenerateCSV()
    {
        var config = new Config();
        config.Add("dataSize", 1_000_000);
        
        var generator = new Generator(config);
        List<MarketOrderVm> marketOrderVms = generator.Execute();
        using (var writer = new StreamWriter(@"mkt-time.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<MarketOrderVmMap>();
            csv.WriteRecords(marketOrderVms);
        }
    }

    [Test]
    public void GeneratePromScaleMetric()
    {
        var config = new Config();
        config.Add("dataSize", 1);
        Generator generator = new Generator(config);
        var metrics = generator.Metrics();
        TestContext.WriteLine(JsonConvert.SerializeObject(metrics, Formatting.Indented));
    }

    [Test]
    public async Task PostToPromScale()
    {
        var config = new Config();
        config.Add("dataSize", 1_000_000);
        Generator generator = new Generator(config);
        var metrics = generator.Metrics();

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:9201")
        };
        var url = new Uri("http://localhost:9201/write");
        foreach (var metric in metrics)
        {
            var json = JsonConvert.SerializeObject(metric);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            _ = await httpClient.PostAsync(url, data);
            //var result = await response.Content.ReadAsStringAsync();
            //TestContext.WriteLine(result);
            
            // var postRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            // {
            //     Content = JsonContent.Create(metric),
            //     
            // };
            // var postResponse = await httpClient.SendAsync(postRequest);
            // postResponse.EnsureSuccessStatusCode();

            // await httpClient.PostAsJsonAsync("http://localhost:9201/write", metric);
        }
        
    }
    

    [Test]
    public void ShowESTimestamp()
    {
        TestContext.WriteLine(DateTimeOffset.UtcNow.ToString("yyyyMMdd'T'hhmmssZ"));
    }

    [Test]
    public async Task InsertAll()
    {
        var config = new Config();
        config.Add("dataSize", 1_000_000);

        var generator = new Generator(config);
        List<MarketOrderVm> marketOrderVms = generator.Execute();

        await using var dbContext = new MarketOrdersContext();
        var sw = Stopwatch.StartNew();
        await dbContext.BulkInsertAsync(marketOrderVms);
        TestContext.WriteLine($"Insert All took {sw.Elapsed}");
    }
    
    [Test]
    public async Task DebugTempTableNotDropped()
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
        while(true)
        {
            List<MarketOrderVm> marketOrderVms = generator.Execute();
            await using var dbContext = new MarketOrdersContext();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
           
            await dbContext.BulkInsertAsync(marketOrderVms, bulkConfig);
            await transaction.CommitAsync();

            await Task.Delay(2000);
        }
    }
    

    [Test]
    public async Task ArchiveWithRawSql()
    {
        await using var dbContext = new MarketOrdersContext();
        var sw = Stopwatch.StartNew();
        var lines =  await dbContext.Database.ExecuteSqlRawAsync(
        @"truncate table histo.""MarketOrderVms"";
          insert into histo.""MarketOrderVms"" select * from daily.""MarketOrderVms"";"); //19s

        TestContext.WriteLine($"Archive {lines} lines took {sw.Elapsed}");
    }

    [Test]
    public async Task ArchiveWithFile()
    {
        await using var dbContext = new MarketOrdersContext();
        var sw = Stopwatch.StartNew();
        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
        var lines = await dbContext.Database.ExecuteSqlRawAsync(
        
        @"truncate table histo.""MarketOrderVms"";
          
          -- copy table data to file
          copy (select * from daily.""MarketOrderVms"") to 'c:\yi\data\MarketOrderVms.copy';
          
          -- copy data from file to table
          copy histo.""MarketOrderVms"" from 'c:\yi\data\MarketOrderVms.copy';");

        TestContext.WriteLine($"Archive {lines} lines took {sw.Elapsed}"); // 00:00:59.0979567
    }
}