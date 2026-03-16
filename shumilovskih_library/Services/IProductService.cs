using System.Collections.Generic;
using System.Threading.Tasks;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}