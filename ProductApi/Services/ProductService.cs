using EPiServer;
using EPiServer.Core; // Ensure this namespace is correct
using EPiServer.ServiceLocation;
using ProductApi.Models;
using EPiServer.DataAccess;
using EPiServer.Security;


namespace ProductApi.Services
{
    [ServiceConfiguration(typeof(ProductService))]
    public class ProductService
    {
        private readonly IContentRepository _contentRepository;

        public ProductService(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public void CreateProduct(string name, string description, decimal price)
        {
            // Get a new default instance of ProductPage
            var product = _contentRepository.GetDefault<ProductPage>(ContentReference.StartPage);
            product.Name = name;
            product.Description = description;
            product.Price = price;

            // Save the product with the Publish action
            _contentRepository.Save(product, SaveAction.Publish,AccessLevel.NoAccess);
        }
    }
}
