using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EComm.ProductCatalog.Model
{
    public interface IProductCatalogService : IService
    {
        //IEnumerable was causing this problem: 
        // https://github.com/Azure/service-fabric-issues/issues/515
        // then I use List :)
        Task<List<Product>> GetAllProducts();

        Task AddProduct( Product product );
    }
}
