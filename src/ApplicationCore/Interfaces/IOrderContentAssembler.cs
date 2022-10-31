using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IOrderContentAssembler
{
    public Task<string> AssembleRequestContentAsync(object o);
}
