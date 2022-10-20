using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Azure.Cloud.ActuallyWorking
{
    public static class OrderReserve
    {
        [FunctionName("OrderReserve")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                log.LogInformation("Checking for proper Header");

                string headerExpected = "OrderToReserve";
                if (!DoesContainProperHeader(request, headerExpected))
                {
                    log.LogWarning("Improper Headers detected");
                    log.LogWarning("BadRequest returned");
                    return new BadRequestErrorMessageResult("Please provide proper headers");
                }

                log.LogInformation("Setting Up - Connections to the Connected Services");

                log.LogInformation("Start - KeyVault");
                string keyVaultName = "keyvaulttestazuretest";
                var keyVaultUri = "https://" + keyVaultName + ".vault.azure.net";

                var secretClient = new SecretClient(new Uri(keyVaultUri),
                    new DefaultAzureCredential());
                var cosmosDbSecret = await secretClient.GetSecretAsync("ConnectionStrings--CosmosDb"); //This sh@t changes the name of it, then why allow it to set it at the beginning??????

                log.LogInformation("DONE - KeyVault");

                log.LogInformation("Start - Database configuring");
                string databaseId = "eShopDb";
                var cosmosDb =
                    await CosmosDbManager.SetupDatabaseAsync(cosmosDbSecret.Value.Value, databaseId, log);

                log.LogInformation("Start -  DbContainer configuring");
                string containerId = "OrderReservations";
                string partitionKeyPath = "/orders";
                Container cosmosContainer =
                    await CosmosDbManager.SetupContainerAsync(database: cosmosDb, containerId, partitionKeyPath, log);

                log.LogInformation("DONE - Database & Container configured");

                log.LogInformation("Processing request");

                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                var itemToReserveDto = new ReservationDto(id: Guid.NewGuid(), OrderToReserve: JsonConvert.DeserializeObject<OrderDtoToReserve>(requestBody));

                log.LogInformation("Creating - Item To Reserve in DbContainer");
                var containerResponse = await cosmosContainer.CreateItemAsync(itemToReserveDto);
                log.LogInformation($"DONE - Container Response: {containerResponse.StatusCode}");

                return new OkObjectResult("Function executed successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BadRequestErrorMessageResult("Something bad happened");
            }
        }

        private static bool DoesContainProperHeader(HttpRequest request, string headerExpected)
        {
            return request.Headers.Select(x => x.Key).Contains(headerExpected);
        }
    }
}