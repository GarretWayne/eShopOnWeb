using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Services;
internal class ReservationService : IOrderReservationService
{
    public Task ReserveItems(Order order)
    {
        throw new NotImplementedException();
    }
}
