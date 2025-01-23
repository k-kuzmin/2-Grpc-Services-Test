using Currency_WorkerService;
using Database_Service;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddGrpcClient<CurrencyService.CurrencyServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcServices:Currency"]);
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
