namespace GarcissLink.Services;

public class WebAlwaysUpOnRender(
    ILogger<WebAlwaysUpOnRender> logger,
    IHttpClientFactory httpClientFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var httpClient = httpClientFactory.CreateClient();

                var response = await httpClient.GetAsync("https://garciss-link.onrender.com/swagger/index.html");

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}