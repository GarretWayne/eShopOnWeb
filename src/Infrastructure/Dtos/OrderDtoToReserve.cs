using System;
using System.Collections.Generic;
using System.Linq;
using BlazorAdmin.Pages.CatalogItemPage;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Dtos;

public record OrderDtoToReserve
{
    public string BuyerId { get; init; }
    public Address ShipToAddress { get; init; }
    public List<OrderItem> ListOfItems { get; init; }
    public decimal FinalPrice { get; init; }

    [Obsolete(
        $"Should not be able to initialize Record without an actual Order. Azure Functions should NOT know what an Order is, and should NOT KNOW of this class AT ALL")]
    public OrderDtoToReserve()
    {

    }

    public OrderDtoToReserve(Order order)
    {

        BuyerId = order.BuyerId;
        ShipToAddress = order.ShipToAddress;
        ListOfItems = order.OrderItems.Select(x => x).ToList();
        FinalPrice = (from items in ListOfItems
                      select items.UnitPrice * items.Units).Sum();
    }

}
