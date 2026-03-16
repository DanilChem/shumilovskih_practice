using System.Collections.Generic;
using System.Threading.Tasks;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public interface ISaleHistoryService
    {
        Task<List<SaleHistory>> GetAllAsync();
        Task<SaleHistory> GetByIdAsync(int id);
        Task<List<SaleHistory>> GetByPartnerIdAsync(int partnerId);
        Task<SaleHistory> AddAsync(SaleHistory sale);
        Task<SaleHistory> UpdateAsync(SaleHistory sale);
        Task DeleteAsync(int id);
    }
}