using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class AzureFuncToDbCommunicatorService :IAzureOrderReservationCommunicatorService
{
    private readonly IAppLogger<AzureFuncToDbCommunicatorService> _logger;

    public AzureFuncToDbCommunicatorService(IAppLogger<AzureFuncToDbCommunicatorService> logger)
    {
        _logger = logger;
    }

    public async Task OnPostOrderAsync(string content, string uriString)
    {
        try
        {
            var requestUri = new Uri(uriString);

            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
            {
                var client = new HttpClient();
                request.Headers.Add("OrderToReserve", "");
                request.Content = new StringContent(content);

                using (var response = await client
                           .PostAsync(request.RequestUri, request.Content))

                {

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("DONE - Reservation COMPLETED");

                    }

                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("BAD - Reaching the AzureFunc somewhere gone wrong",e);

        }
    }
}
