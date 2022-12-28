namespace API;

public class TimeTick : BackgroundService
{
    private readonly ILogger<TimeTick> logger;
    private readonly PeriodicTimer timer;
    private long seconds = 0;
    
    public TimeTick(ILogger<TimeTick> logger)
    {
        this.logger = logger;
        timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            logger.LogInformation($"/// {++seconds} sec elapsed");
        }
    }
}
