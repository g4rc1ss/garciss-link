namespace GarcissLink.Services;

public class WebAlwaysUpOnRender(
    ILogger<WebAlwaysUpOnRender> logger,
    IHttpClientFactory httpClientFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();

                using HttpResponseMessage response = await httpClient
                    .GetAsync("https://garciss-link.onrender.com/swagger/index.html", stoppingToken)
                    .ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
