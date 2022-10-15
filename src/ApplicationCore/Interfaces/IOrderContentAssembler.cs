using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public interface IOrderContentAssembler
{
    public Task<string> AssembleOrderContentAsync(object o);
}
