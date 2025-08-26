using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs.Stock;
using WebApi.Application.Mappers;
using WebApi.Application.Queries;
using WebApi.Application.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var stockDtos = await _stockService.GetAllAsync(query);

            return Ok(stockDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            StockDto stock;
            try
            {
                stock = await _stockService.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            return Ok(stock);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = await _stockService.CreateAsync(stockDto);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _stockService.UpdateAsync(id, updateDto);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            var stockModel = updateDto.ToStockFromUpdateDto();

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await _stockService.DeleteAsync(id);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }
    }
}
