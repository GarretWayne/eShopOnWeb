using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Azure.Cloud.ActuallyWorking
{
    internal static class CosmosDbManager
    {
        public static async Task<Database> SetupDatabaseAsync(string connectionStringCosmosDb, string databaseId, ILogger logger)
        {
            try
            {
                //Reaching the Db
                logger.LogInformation("Reaching the CosmosDb");

                CosmosClient cosmosClient = new CosmosClient(connectionStringCosmosDb);
                var cosmosDb = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

                //Logging results or Exception on failure
                LogStatusOfCreateIfNotExist(cosmosDb.StatusCode, nameof(SetupDatabaseAsync), logger);

                return cosmosDb.Database;
            }
            catch (Exception e)
            {
                logger.LogCritical("CRITICAL - Setting Up DataBase FAILED. ErrorNumber:" + e);
                throw;
            }
        }

        public static async Task<Container> SetupContainerAsync(Database database, string containerId, string partitionKeyPath, ILogger log)
        {
            try
            {
                log.LogInformation("Creating Container on CosmosDb if not exist");
                var cosmosContainer = await database.CreateContainerIfNotExistsAsync(id: containerId, partitionKeyPath: partitionKeyPath);
                LogStatusOfCreateIfNotExist(cosmosContainer.StatusCode, nameof(SetupContainerAsync), log);

                log.LogInformation($"Container {cosmosContainer.Container} Created successfully");
                return cosmosContainer.Container;
            }
            catch (Exception e)
            {
                log.LogCritical("CRITICAL - Setting up Container on Db FAILED. Exception: " + e);
                throw;
            }
        }

        private static void LogStatusOfCreateIfNotExist(HttpStatusCode statusCode, string nameOfFunc, ILogger logger)
        {
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    logger.LogInformation($"200-Existing {nameOfFunc}");
                    break;

                case HttpStatusCode.Created:
                    logger.LogInformation($"201-Created {nameOfFunc}");
                    break;

                default:
                    throw new HttpListenerException((int)statusCode, "Something happened to the Cosmos DB during CREATION or CHECKING, || StatusCode: " + statusCode + " Function that died: " + nameOfFunc);
            }
        }
    }
}