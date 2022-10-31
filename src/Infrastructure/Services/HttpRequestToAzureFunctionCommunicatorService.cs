using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services.Bases;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class HttpRequestToAzureFunctionCommunicatorService : AzureCommunicatorServiceBase
{
    public HttpRequestToAzureFunctionCommunicatorService(IAppLogger<HttpRequestToAzureFunctionCommunicatorService> logger) : base(logger)
    {
    }

    public override async Task SendOrderRequestAsync(string content, string uriString)
    {
        try
        {
            //Forming the URi
            var requestUri = new Uri(uriString);

            //get client
            var httpClient = new HttpClient();

            //Forming the Message
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(content)
            };
            //Adding Proper Header
            httpRequestMessage.Headers.Add("OrderToReserve", "true");

            //Sending the Message
            var response = await httpClient.SendAsync(httpRequestMessage);


            _logger.LogInformation(response.IsSuccessStatusCode
                ? "DONE - Reservation COMPLETED"
                : "FAILED - Reservation FAILED");


        }
        catch (Exception e)
        {
            _logger.LogCritical("BAD - Reaching the AzureFunc somewhere gone wrong", e);

        }
    }
}
