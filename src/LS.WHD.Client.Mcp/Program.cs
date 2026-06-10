using LS.WHD.Client;
using LS.WHD.Client.Mcp;
using LS.WHD.Client.Mcp.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        // Only log warnings and above to stderr so that stdout stays clean for MCP JSON.
        logging.SetMinimumLevel(LogLevel.Warning);
        logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
    })
    .ConfigureServices(services =>
    {
        var options = WhdConfiguration.LoadOptions();
        var whdClient = new WhdClient(options);

        services.AddSingleton(whdClient);
        services.AddSingleton<WhdClientService>();

        services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<TicketTools>()
            .WithTools<ClientTools>()
            .WithTools<AssetTools>()
            .WithTools<LocationTools>()
            .WithTools<RequestTypeTools>()
            .WithTools<NoteTools>()
            .WithTools<LookupTools>();
    })
    .Build();

await host.RunAsync();
