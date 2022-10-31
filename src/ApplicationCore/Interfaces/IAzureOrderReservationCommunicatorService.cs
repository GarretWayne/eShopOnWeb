using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IAzureOrderReservationCommunicatorService
{
    public Task OnSendOrderAsync(string content, string uriString);
}
