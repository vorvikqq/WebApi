using WebApi.DTOs.Stock;
using WebApi.Models;

namespace WebApi.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock> CreateAsync(Stock stock);
    }
}
