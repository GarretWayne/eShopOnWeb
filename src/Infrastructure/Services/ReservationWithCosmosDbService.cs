using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.eShopWeb.Infrastructure.Services.Bases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class ReservationWithCosmosDbService : ReservationServiceBase
{
    public ReservationWithCosmosDbService(IConfiguration configuration,
        IAppLogger<ReservationWithCosmosDbService> logger,
        IAzureCommunicatorService communicator,
        IOrderRequestContentAssembler assembler,
        ISecretBrokerService secretBrokerService
    ) : base(configuration, logger, communicator, assembler, secretBrokerService)
    {

    }

    protected override async Task SendOrderAsync(string content)
    {
        string requestTargetUri = await RetrieveRequestTargetUri("SecretNames:OrderReserveAzureFunc");

        await _communicator.SendOrderRequestAsync(content, requestTargetUri);
    }

    
}
