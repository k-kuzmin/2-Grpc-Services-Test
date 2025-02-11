using Database_Service;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Currency_WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly CurrencyService.CurrencyServiceClient _currencyService;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(
        ILogger<Worker> logger,
        CurrencyService.CurrencyServiceClient currencyService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _currencyService = currencyService;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                _logger.LogInformation("Fetching currency data...");
                using var response = await client.GetAsync("http://www.cbr.ru/scripts/XML_daily.asp", stoppingToken);
                if (response.IsSuccessStatusCode is false)
                {
                    _logger.LogError("Failed to fetch data. Status Code: {statusCode}", response.StatusCode);
                    continue;
                }

                var contentBytes = await response.Content.ReadAsByteArrayAsync(stoppingToken);
                var content = Encoding.GetEncoding("windows-1251").GetString(contentBytes);
                _logger.LogInformation("Response: {content}", content);

                await UpdateCurrencies(content, stoppingToken);
            }
            catch (TaskCanceledException ex) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Operation canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while making HTTP request.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task UpdateCurrencies(string content, CancellationToken stoppingToken)
    {
        var xml = XDocument.Parse(content);

        foreach (var element in xml.Descendants("Valute"))
        {
            var code = element.Element("CharCode")?.Value;
            var name = element.Element("Name")?.Value;
            var value = element.Element("Value")?.Value.Replace(',', '.') ?? "0";
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var rate);

            if (string.IsNullOrEmpty(code))
            {
                continue;
            }

            var request = new UpdateCurrencyRequest
            {
                Code = code,
                Name = name,
                Rate = rate,
            };
            await _currencyService.UpdateCurrencyAsync(request, cancellationToken: stoppingToken);
        }
    }
}
