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

        public async Task<StockDto?> GetByIdAsync(int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
                return null;

            return stock.ToStockDto();
        }

        public async Task<Stock> CreateAsync(CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDto();

            await _stockRepo.CreateAsync(stockModel);

            return stockModel;
        }

        public async Task<bool> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var updatedCount = await _stockRepo.UpdateAsync(id, stockDto);

            return updatedCount > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedCount = await _stockRepo.DeleteAsync(id);

            return deletedCount > 0;
        }

        public async Task<bool> IsExistAsnyc(int id)
        {
            return await _stockRepo.IsExistAsnyc(id);
        }


    }
}
