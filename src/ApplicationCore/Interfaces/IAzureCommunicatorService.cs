using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IAzureCommunicatorService
{
    public Task SendOrderRequestAsync(string content, string uriString);
}
