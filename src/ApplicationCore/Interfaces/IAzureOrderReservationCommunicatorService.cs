using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IAzureOrderReservationCommunicatorService
{
    public Task OnPostOrderAsync(string content, string uriString);
}
