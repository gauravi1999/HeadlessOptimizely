using EPiServer;
using EPiServer.Core;
using ProductApi.Models;

namespace ProductApi.Services
{
    public class CmsApiClient
    {
        private readonly IContentRepository _contentRepository;

        public CmsApiClient(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public ProductPage GetProductById(ContentReference contentReference)
        {
            return _contentRepository.Get<ProductPage>(contentReference);
        }

        public ContentReference CreateProduct(ProductPage productPage)
        {
            var contentReference = _contentRepository.Save(productPage, EPiServer.DataAccess.SaveAction.Publish, EPiServer.Security.AccessLevel.NoAccess);
            return contentReference;
        }

        public void UpdateProduct(ProductPage productPage)
        {
            _contentRepository.Save(productPage, EPiServer.DataAccess.SaveAction.Publish, EPiServer.Security.AccessLevel.NoAccess);
        }

        public void DeleteProduct(ContentReference contentReference)
        {
            _contentRepository.Delete(contentReference, true);
        }
    }
}
