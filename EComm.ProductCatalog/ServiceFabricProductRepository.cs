using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EComm.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace EComm.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _reliableStateManager;

        public ServiceFabricProductRepository(IReliableStateManager reliableStateManager)
        {
            _reliableStateManager = reliableStateManager;
        }

        public async Task AddProduct( Product product )
        {
            IReliableDictionary<Guid, Product> products = await _reliableStateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using (ITransaction tx = _reliableStateManager.CreateTransaction() )
            {
                await products.AddOrUpdateAsync(tx, product.Id, product, ( id, value ) => product);
                await tx.CommitAsync();
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            IReliableDictionary<Guid, Product> products = await _reliableStateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            List<Product> result = new List<Product>();

            using ( ITransaction tx = _reliableStateManager.CreateTransaction() )
            {
                IAsyncEnumerable<KeyValuePair<Guid, Product>> allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using(IAsyncEnumerator<KeyValuePair<Guid, Product>> enumerator = allProducts.GetAsyncEnumerator() )
                {
                    while(await enumerator.MoveNextAsync(cancellationToken: CancellationToken.None) )
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }
            return result;
        }
    }
}
