using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs.Stock;
using WebApi.Application.Mappers;
using WebApi.Application.Queries;
using WebApi.Application.Repositories.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepository)
        {
            _stockRepo = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var stocks = await _stockRepo.GetAllAsync(query);

            var stockDtos = stocks.Select(s => s.ToStockDto());

            return Ok(stockDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
                return NotFound();

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = stockDto.ToStockFromCreateDto();

            await _stockRepo.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCount = await _stockRepo.UpdateAsync(id, updateDto);

            if (updatedCount == 0)
                return NotFound();

            var stockModel = updateDto.ToStockFromUpdateDto();

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var deletedCount = await _stockRepo.DeleteAsync(id);

            if (deletedCount == 0)
                return NotFound();

            return NoContent();
        }
    }
}
