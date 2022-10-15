using System.Collections.Generic;
using System.Linq;
using BlazorAdmin.Pages.CatalogItemPage;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class OrderDtoToReserve
{
    public string BuyerId { get; set; }
    public Address ShipToAddress { get; set; }
    public List<OrderItem> ListOfItems { get; set; }
    public decimal FinalPrice { get; set; }

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
