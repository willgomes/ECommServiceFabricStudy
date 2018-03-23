using EComm.ProductCatalog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EComm.ProductCatalog
{
    interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task AddProduct( Product product );
    }
}
