using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace MSGraphApiMVP;

class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    //private readonly ITokenAcquisition _tokenAcquisition;
    private readonly GraphServiceClient _graphServiceClient;

    public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, GraphServiceClient graphServiceClient) {
        _logger = logger;
        //_tokenAcquisition = tokenAcquisition;
        _graphServiceClient = graphServiceClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        //var token = await _tokenAcquisition.GetAuthenticationResultForAppAsync("https://graph.microsoft.com/.default",
        //    tokenAcquisitionOptions: new TokenAcquisitionOptions { CancellationToken = cancellationToken });
        //await _graphServiceClient.Users.GetAsync(cancellationToken: cancellationToken);

        //await TokenAcquirerFactory.GetDefaultInstance().GetTokenAcquirer()
        //    .GetTokenForAppAsync("https://graph.microsoft.com/.default", cancellationToken: cancellationToken);

        var users = await _graphServiceClient.Users.GetAsync(o => o.Options.WithAppOnly(), cancellationToken);
        _logger.LogInformation("{Users}", users is null ? "Null" : string.Join(", ", users.Value.Select(u => u.DisplayName)));

        //await _tokenAcquisition.GetAccessTokenForAppAsync("https://graph.microsoft.com/.default");
    }
}