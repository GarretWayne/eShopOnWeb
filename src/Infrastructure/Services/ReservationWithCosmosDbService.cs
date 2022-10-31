using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class ReservationWithCosmosDbService : ReservationServiceBase
{
    public ReservationWithCosmosDbService(IConfiguration configuration,
        IAppLogger<ReservationWithCosmosDbService> logger,
        IAzureOrderReservationCommunicatorService orderReservationCommunicator,
        IOrderContentAssembler assembler,
        ISecretBroker secretBroker
    ) : base(configuration, logger, orderReservationCommunicator, assembler, secretBroker)
    {

    }

    protected override async Task SendOrderAsync(string content)
    {
        string requestTargetUri = await RetrieveRequestTargetUri("SecretNames:OrderReserveAzureFunc");

        await _orderReservationCommunicator.OnSendOrderAsync(content, requestTargetUri);
    }

    
}
