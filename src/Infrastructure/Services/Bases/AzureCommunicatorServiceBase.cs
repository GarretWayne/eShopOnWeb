using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Services.Bases;

public abstract class AzureCommunicatorServiceBase : IAzureCommunicatorService
{
    protected IAppLogger<HttpRequestToAzureFunctionCommunicatorService> _logger;

    public AzureCommunicatorServiceBase(IAppLogger<HttpRequestToAzureFunctionCommunicatorService> logger)
    {
        _logger = logger;
    }

    public abstract Task SendOrderRequestAsync(string content, string uriString);
}
