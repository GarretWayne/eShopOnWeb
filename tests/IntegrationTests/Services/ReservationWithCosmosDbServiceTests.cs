using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Services;
public class ReservationWithCosmosDbServiceTests
{
    //this test is to check if the Azure Function can be reached via HTTP call
    [Fact]
    public async void ConnectionToAzureFunctionIsOk_Http()
    {
        var azureFuncUri = new Uri("http://testesty.azurewebsites.net/api/HttpTrigger2?");

        using (var request = new HttpRequestMessage(HttpMethod.Post, azureFuncUri))
        {
            var client = new HttpClient();
            request.Headers.Add("Test",new []{"This test was successful!"});

            using (var response = await client
                       .PostAsync(request.RequestUri, request.Content))

            {
                Assert.True(response.Headers.Contains("TestSuccess"));
                Assert.True(response.StatusCode == HttpStatusCode.OK);
                Assert.Equivalent("This test was successful!",response.Content);
            }
        }
    }
}
