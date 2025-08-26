using WebApi.Application.DTOs.Stock;
using WebApi.Application.Mappers;
using WebApi.Application.Queries;
using WebApi.Application.Repositories.Interfaces;
using WebApi.Application.Services.Interfaces;
using WebApi.Domain.Models;

namespace WebApi.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepo;

        public StockService(IStockRepository stockRepository)
        {
            _stockRepo = stockRepository;
        }

        public async Task<IEnumerable<StockDto>> GetAllAsync(QueryObject query)
        {
            var stocks = await _stockRepo.GetAllAsync(query);

            var stockDtos = stocks.Select(s => s.ToStockDto());

            return stockDtos;
        }

        public async Task<StockDto> GetByIdAsync(int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
                throw new KeyNotFoundException("stock doesn't found");

            return stock.ToStockDto();
        }

        public async Task<Stock> CreateAsync(CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDto();

            await _stockRepo.CreateAsync(stockModel);

            return stockModel;
        }

        public async Task UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var updatedCount = await _stockRepo.UpdateAsync(id, stockDto);

            if (updatedCount == 0)
                throw new KeyNotFoundException("stock doesn't found");
        }

        public async Task DeleteAsync(int id)
        {
            var deletedCount = await _stockRepo.DeleteAsync(id);

            if (deletedCount == 0)
                throw new KeyNotFoundException("stock doesn't found");
        }

        public async Task<bool> IsExistAsnyc(int id)
        {
            return await _stockRepo.IsExistAsnyc(id);
        }


    }
}
