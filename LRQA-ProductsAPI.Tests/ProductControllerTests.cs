using LRQA_ProductsAPI.Controllers;
using LRQA_ProductsAPI.Models;
using LRQA_ProductsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LRQA_ProductsAPI.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _controller = new ProductController(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Running Shoes", Price = 129.99m, Stock = 150 }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkWithProduct()
        {
            var product = new Product { Id = 1, Name = "Running Shoes", Price = 129.99m, Stock = 150 };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_IgnoresClientSuppliedId_AndReturnsCreatedAtAction()
        {
            int? idPassedToRepository = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Callback<Product>(p => idPassedToRepository = p.Id)
                .ReturnsAsync((Product p) => { p.Id = 42; return p; });

            var incoming = new Product { Id = 999, Name = "Flip Flops", Price = 19.99m, Stock = 45 };

            var result = await _controller.Create(incoming);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(0, idPassedToRepository);
            Assert.Equal(42, ((Product)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Update_ExistingId_ReturnsNoContent()
        {
            var product = new Product { Id = 1, Name = "Running Shoes", Price = 129.99m, Stock = 150 };
            _repositoryMock.Setup(r => r.UpdateAsync(product)).ReturnsAsync(true);

            var result = await _controller.Update(1, product);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            var product = new Product { Id = 99, Name = "Running Shoes", Price = 129.99m, Stock = 150 };
            _repositoryMock.Setup(r => r.UpdateAsync(product)).ReturnsAsync(false);

            var result = await _controller.Update(99, product);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingId_ReturnsNoContent()
        {
            _repositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingId_ReturnsNotFound()
        {
            _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
