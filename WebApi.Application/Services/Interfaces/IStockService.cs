using WebApi.Application.DTOs.Stock;
using WebApi.Application.Queries;
using WebApi.Domain.Models;

namespace WebApi.Application.Services.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllAsync(QueryObject query);
        Task<StockDto?> GetByIdAsync(int id);
        Task<Stock> CreateAsync(CreateStockRequestDto stock);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<bool> IsExistAsnyc(int id);
    }
}
