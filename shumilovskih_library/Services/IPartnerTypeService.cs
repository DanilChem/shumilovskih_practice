using System.Collections.Generic;
using System.Threading.Tasks;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public interface IPartnerTypeService
    {
        Task<List<PartnerType>> GetAllAsync();
        Task<PartnerType> GetByIdAsync(int id);
        Task<PartnerType> AddAsync(PartnerType partnerType);
        Task<PartnerType> UpdateAsync(PartnerType partnerType);
        Task DeleteAsync(int id);
    }
}