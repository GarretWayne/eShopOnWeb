using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.eShopWeb.Infrastructure.Services.Bases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Infrastructure.Services.Azure;

public class ReservationWithAzureFunctionToCosmosDb : ReservationServiceBase
{
    public ReservationWithAzureFunctionToCosmosDb(IConfiguration configuration,
        IAppLogger<ReservationWithAzureFunctionToCosmosDb> logger,
        IAzureCommunicatorService communicator,
        IOrderRequestContentAssembler assembler,
        ISecretBrokerService secretBrokerService
    ) : base(configuration, logger, communicator, assembler, secretBrokerService)
    {

    }

    protected override async Task SendOrderAsync(string content)
    {
        RequestTargetUri =
            await RetrieveRequestTargetUri("SecretNames:ReservationWithAzureFunctionToCosmosDb");

        await _communicator.SendOrderRequestAsync(content, RequestTargetUri);
    }


}
