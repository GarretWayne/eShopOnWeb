using Microsoft.eShopWeb.Infrastructure.Services;
using Newtonsoft.Json;
using System;

namespace Azure.Cloud.ActuallyWorking;

public record ReservationDto([JsonProperty("id")] Guid id, OrderDtoToReserve OrderToReserve);