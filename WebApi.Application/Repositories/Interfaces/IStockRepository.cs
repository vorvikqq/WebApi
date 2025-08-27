using WebApi.Application.DTOs.Stock;
using WebApi.Application.Queries;
using WebApi.Domain.Models;

namespace WebApi.Application.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock> CreateAsync(Stock stock);
        Task<bool> IsExistAsnyc(int id);
    }
}
