using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;
internal class ReservationService : IOrderReservationService
{
    private readonly IConfiguration _config;

    public ReservationService(IConfiguration configuration)
    {
        _config = configuration;
    }
    public Task ReserveItems(Order order)
    {
        //TODO This should generate an Order file,
        //TODO posted a request to the AzFunction App that would have uploaded the file to the Blob Storage
        return Task.CompletedTask;
    }
}
