using Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = new HostBuilder()
    .ConfigureServices(services =>
    services.AddHostedService<Receiver>());
await hostBuilder.RunConsoleAsync();