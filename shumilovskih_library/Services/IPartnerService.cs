using System.Collections.Generic;
using System.Threading.Tasks;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public interface IPartnerService
    {
        Task<List<Partner>> GetAllPartnersAsync();
        Task<Partner> GetPartnerByIdAsync(int partnerId);
        Task<Partner> AddPartnerAsync(Partner partner);
        Task<Partner> UpdatePartnerAsync(Partner partner);
        Task DeletePartnerAsync(int partnerId);
        Task<List<PartnerType>> GetAllPartnerTypesAsync();
        Task<List<SaleHistory>> GetPartnerSalesHistoryAsync(int partnerId);
        Task<decimal> GetPartnerTotalSalesAsync(int partnerId);
        Task<decimal> GetPartnerDiscountAsync(int partnerId);
    }
}