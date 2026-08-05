using LRQA_ProductsAPI.Models;
using LRQA_ProductsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LRQA_ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        // GET /api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            var created = await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT /api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var updated = await _productRepository.UpdateAsync(product);

            if (!updated)
            {
                return NotFound();
            }

            // return 204 as no need to repeat what the client has sent
            return NoContent();
        }

        // DELETE /api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            // return 204 as no need to return the now deleted entity
            return NoContent();
        }
    }
}
