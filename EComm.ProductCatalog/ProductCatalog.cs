using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using EComm.ProductCatalog.Model;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;


namespace EComm.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        private IProductRepository _productRepository;

        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProduct( Product product )
        {
            await _productRepository.AddProduct(product);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener>(base.CreateServiceReplicaListeners())
            {
                new ServiceReplicaListener(c => new FabricTransportServiceRemotingListener(c, this)),
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _productRepository = new ServiceFabricProductRepository(this.StateManager);

            Product product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "F1 2017 - PC",
                Description = "F1 2017 Jogo de Corrida",
                Price = 55.70,
                Availability = 50
            };

            Product product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Caneca - I AM PROGRAMMER - TÉRMICA",
                Description = "CANECA TÉRMICA DESENVOLVIMENTO",
                Price = 30.00,
                Availability = 20
            };

            Product product3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "MONITOR SAMSUNG 20' IPS",
                Description = "MONITOR COMUM IPS",
                Price = 550.35,
                Availability = 100
            };


            await _productRepository.AddProduct(product1);
            await _productRepository.AddProduct(product2);
            await _productRepository.AddProduct(product3);

            IEnumerable<Product> all = await _productRepository.GetAllProducts();
        }
    }
}
