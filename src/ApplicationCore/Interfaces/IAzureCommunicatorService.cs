using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IAzureCommunicatorService
{
    public Task OnSendOrderAsync(string content, string uriString);
}
