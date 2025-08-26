using WebApi.Application.DTOs.Stock;
using WebApi.Application.Queries;
using WebApi.Domain.Models;

namespace WebApi.Application.Services.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllAsync(QueryObject query);
        Task<StockDto> GetByIdAsync(int id);
        Task<Stock> CreateAsync(CreateStockRequestDto stock);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<bool> IsExistAsnyc(int id);
    }
}
