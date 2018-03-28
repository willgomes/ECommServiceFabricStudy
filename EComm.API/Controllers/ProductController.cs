using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EComm.API.Model;
using EComm.ProductCatalog.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace EComm.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private readonly IProductCatalogService _productCatalogService;

        public ProductController()
        {
            ServiceProxyFactory proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            _productCatalogService = proxyFactory.CreateServiceProxy<IProductCatalogService>(new Uri("fabric:/EComm/EComm.ProductCatalog"), new ServicePartitionKey(new Random().Next(int.MaxValue)));
        }

        // GET api/product
        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> Get()
        {
            try
            {
                IEnumerable<Product> allProducts = await _productCatalogService.GetAllProducts();

                return allProducts.Select(product => new ApiProduct
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    IsAvailable = product.Availability > 0
                });
            }
            catch ( Exception )
            {
                throw;
            }
        }

        // GET api/product/5
        [HttpGet("{id}")]
        public string Get( int id )
        {
            return "value";
        }

        // POST api/product
        [HttpPost]
        public async Task Post( [FromBody] ApiProduct product )
        {
            Product newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Availability = 150
            };

            await _productCatalogService.AddProduct(newProduct);
        }

        // PUT api/product/5
        [HttpPut("{id}")]
        public void Put( int id, [FromBody]string value )
        {
        }

        // DELETE api/product/5
        [HttpDelete("{id}")]
        public void Delete( int id )
        {
        }
    }
}
