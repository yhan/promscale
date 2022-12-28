using System.Diagnostics;
using API;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using TestProject;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<TimeTick>();
//builder.Services.AddHostedService<ParallelQueryDb>();
//builder.Services.AddHostedService<SequentialQueryDB>();


var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();

string cnxString = CnxString(config);

builder.Services.AddDbContextFactory<MarketOrdersContext>(optionsBuilder => {
    optionsBuilder.UseNpgsql(cnxString)
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine);
}, ServiceLifetime.Singleton);

builder.Services.AddDbContext<MarketOrdersContext>(optionsBuilder => {
    optionsBuilder.UseNpgsql(cnxString)
        .AddInterceptors(new ConnectionInterceptor())
        .EnableSensitiveDataLogging()
        .LogTo(s => Debug.WriteLine(s));
},
ServiceLifetime.Singleton);

// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(config)
//     .Enrich.WithThreadId()
//     .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.WithThreadId());

WebApplication app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

string CnxString(IConfiguration config)
{
    var pooling = bool.Parse(config["DbCnxPooling"]);
    var connectionString = pooling ? config.GetConnectionString("WithPooling") : config.GetConnectionString("WithoutPooling");
    return connectionString;
}