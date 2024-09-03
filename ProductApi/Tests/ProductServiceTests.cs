using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Controllers;
using ProductApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProductApi.Tests
{
    public class ProductsControllerTests
    {
        private DbContextOptions<ProductContext> CreateNewContextOptions()
        {
            // Create a fresh instance of the in-memory database for each test
            return new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ProductsController CreateController(ProductContext context)
        {
            return new ProductsController(context);
        }

        private void SeedDatabase(ProductContext context)
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product1", Description = "Description1", Price = 10.00M },
                new Product { Id = 2, Name = "Product2", Description = "Description2", Price = 20.00M }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new ProductContext(options))
            {
                SeedDatabase(context);
                var controller = CreateController(context);

                // Act
                var result = await controller.GetProducts();

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
                var okResult = Assert.IsType<List<Product>>(actionResult.Value);
                Assert.Equal(2, okResult.Count);
            }
        }

        [Fact]
        public async Task GetProduct_ReturnsProductById()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new ProductContext(options))
            {
                SeedDatabase(context);
                var controller = CreateController(context);

                // Act
                var result = await controller.GetProduct(1);

                // Assert
                var actionResult = Assert.IsType<ActionResult<Product>>(result);
                var okResult = Assert.IsType<Product>(actionResult.Value);
                Assert.Equal(1, okResult.Id);
            }
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new ProductContext(options))
            {
                SeedDatabase(context);
                var controller = CreateController(context);

                // Act
                var result = await controller.GetProduct(99);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task PostProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new ProductContext(options))
            {
                var controller = CreateController(context);
                var newProduct = new Product { Name = "NewProduct", Description = "New Description", Price = 30.00M };

                // Act
                var result = await controller.PostProduct(newProduct);

                // Assert
                var actionResult = Assert.IsType<ActionResult<Product>>(result);
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
                var createdProduct = Assert.IsType<Product>(createdAtActionResult.Value);
                Assert.Equal("NewProduct", createdProduct.Name);
                Assert.Equal("New Description", createdProduct.Description);
                Assert.Equal(30.00M, createdProduct.Price);
            }
        }

        [Fact]
        public async Task PutProduct_UpdatesProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new ProductContext(options))
            {
                // Seed the database
                SeedDatabase(context);

                var controller = CreateController(context);

                // Create a new product instance with the same ID but different properties
                var updatedProduct = new Product
                {
                    Id = 1,
                    Name = "UpdatedProduct",
                    Description = "Updated Description",
                    Price = 40.00M
                };

                // Detach any existing entities with the same ID
                var existingProduct = context.Products.Local.FirstOrDefault(p => p.Id == updatedProduct.Id);
                if (existingProduct != null)
                {
                    context.Entry(existingProduct).State = EntityState.Detached;
                }

                // Act
                var result = await controller.PutProduct(1, updatedProduct);

                // Assert
                Assert.IsType<NoContentResult>(result);

                // Verify the update
                var product = await context.Products.FindAsync(1);
                Assert.NotNull(product);
                Assert.Equal("UpdatedProduct", product.Name);
                Assert.Equal("Updated Description", product.Description);
                Assert.Equal(40.00M, product.Price);
            }
        }



        [Fact]
        public async Task DeleteProduct_RemovesProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using (var context = new ProductContext(options))
            {
                SeedDatabase(context);
                var controller = CreateController(context);

                // Act
                var result = await controller.DeleteProduct(1);

                // Assert
                Assert.IsType<NoContentResult>(result);
                var product = await context.Products.FindAsync(1);
                Assert.Null(product);
            }
        }
    }
}
