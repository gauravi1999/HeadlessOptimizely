using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace ProductApi.Models
{
    [ContentType(DisplayName = "Product Page", GUID = "99520DB0-3D25-4607-ABD1-B12ACA13CC38", Description = "Description of Product Page")]
    public class ProductPage : PageData
    {
        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }
        public virtual string Description { get; set; }  // Ensure this property exists if you're using it

    }
}
