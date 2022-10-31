using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services.Bases;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services.Azure;

internal class ReservationWithServiceBusToBlob : ReservationServiceBase
{
    public ReservationWithServiceBusToBlob(IConfiguration configuration, IAppLogger<ReservationWithAzureFunctionToCosmosDb> logger, IAzureCommunicatorService communicator, IOrderRequestContentAssembler assembler, ISecretBrokerService secretBrokerService) : base(configuration, logger, communicator, assembler, secretBrokerService)
    {
    }

    protected override async Task SendOrderAsync(string content)
    {
        RequestTargetUri =
            await RetrieveRequestTargetUri("SecretNames:ReservationWithServiceBusToBlob");
        await _communicator.SendOrderRequestAsync(content, RequestTargetUri);
    }
}
