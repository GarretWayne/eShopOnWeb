using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IOrderRequestContentAssembler
{
    public Task<string> AssembleRequestContentAsync(object o);
}
