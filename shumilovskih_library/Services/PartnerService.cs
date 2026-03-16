using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shumilovskih_library.Context;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public class PartnerService : IPartnerService, IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly IDiscountService _discountService;
        private bool _disposed;

        public PartnerService(ApplicationContext context, IDiscountService discountService)
        {
            _context = context ?? throw new ArgumentNullException("context");
            _discountService = discountService ?? throw new ArgumentNullException("discountService");
        }

        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            return await _context.Partners
                .Include(p => p.PartnerType)
                .OrderBy(p => p.PartnerId)
                .ToListAsync();
        }

        public async Task<Partner> GetPartnerByIdAsync(int partnerId)
        {
            return await _context.Partners
                .Include(p => p.PartnerType)
                .FirstOrDefaultAsync(p => p.PartnerId == partnerId);
        }

        public async Task<Partner> AddPartnerAsync(Partner partner)
        {
            if (partner == null)
            {
                throw new ArgumentNullException("partner");
            }

            ValidatePartner(partner);

            var existing = await _context.Partners
                .FirstOrDefaultAsync(p => p.CompanyName == partner.CompanyName);

            if (existing != null)
            {
                throw new InvalidOperationException(string.Format(
                    "Партнёр с названием '{0}' уже существует", partner.CompanyName));
            }

            partner.CreatedDate = DateTime.Now;
            partner.ModifiedDate = DateTime.Now;

            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();

            return partner;
        }

        public async Task<Partner> UpdatePartnerAsync(Partner partner)
        {
            if (partner == null)
            {
                throw new ArgumentNullException("partner");
            }

            ValidatePartner(partner);

            var existing = await _context.Partners.FindAsync(partner.PartnerId);
            if (existing == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Партнёр с ID {0} не найден", partner.PartnerId));
            }

            existing.CompanyName = partner.CompanyName;
            existing.PartnerTypeId = partner.PartnerTypeId;
            existing.Rating = partner.Rating;
            existing.Address = partner.Address;
            existing.DirectorName = partner.DirectorName;
            existing.Phone = partner.Phone;
            existing.Email = partner.Email;
            existing.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeletePartnerAsync(int partnerId)
        {
            var partner = await _context.Partners.FindAsync(partnerId);
            if (partner == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Партнёр с ID {0} не найден", partnerId));
            }

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PartnerType>> GetAllPartnerTypesAsync()
        {
            return await _context.PartnerTypes
                .OrderBy(pt => pt.TypeName)
                .ToListAsync();
        }

        public async Task<List<SaleHistory>> GetPartnerSalesHistoryAsync(int partnerId)
        {
            return await _context.SalesHistory
                .Include(sh => sh.Product)
                .Where(sh => sh.PartnerId == partnerId)
                .OrderByDescending(sh => sh.SaleDate)
                .ToListAsync();
        }

        public async Task<decimal> GetPartnerTotalSalesAsync(int partnerId)
        {
            return await _context.SalesHistory
                .Where(sh => sh.PartnerId == partnerId)
                .SumAsync(sh => sh.TotalAmount);
        }

        public async Task<decimal> GetPartnerDiscountAsync(int partnerId)
        {
            var totalSales = await GetPartnerTotalSalesAsync(partnerId);
            return _discountService.CalculateDiscount(totalSales);
        }

        private void ValidatePartner(Partner partner)
        {
            if (string.IsNullOrWhiteSpace(partner.CompanyName))
            {
                throw new ArgumentException("Наименование компании обязательно для заполнения");
            }

            if (partner.Rating < 0)
            {
                throw new ArgumentException("Рейтинг должен быть неотрицательным числом");
            }

            if (!string.IsNullOrWhiteSpace(partner.Email))
            {
                var emailAttribute = new EmailAddressAttribute();
                if (!emailAttribute.IsValid(partner.Email))
                {
                    throw new ArgumentException("Некорректный формат email адреса");
                }
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}