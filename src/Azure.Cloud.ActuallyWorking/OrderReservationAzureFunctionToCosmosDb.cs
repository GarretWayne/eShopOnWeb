using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.Cloud.ActuallyWorking
{
    public static class OrderReservationAzureFunctionToCosmosDb
    {
        [FunctionName("OrderReservationAzureFunctionToCosmosDb")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                log.LogInformation("Checking for proper Header");

                string headerNameExpected = "OrderToReserve";
                if (!DoesContainProperHeader(request, headerNameExpected, logger: log))
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

        private static bool DoesContainProperHeader(HttpRequest request, string headerExpected, ILogger logger)
        {
            try
            {
                logger.LogInformation("Comparing Header To Expected");
                string result = request.Headers[headerExpected];
                logger.LogInformation($"Still works here? ");


                return string.IsNullOrEmpty(result) ? false : result.Contains("true");

            }
            catch (Exception e)
            {
                logger.LogCritical($"FAILED - Evaluation of headers FAILED Error: {e}"  );
                return false;
            }
            

        }
    }
}
