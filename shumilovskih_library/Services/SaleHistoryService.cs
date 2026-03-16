using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shumilovskih_library.Context;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public class SaleHistoryService : ISaleHistoryService, IDisposable
    {
        private readonly ApplicationContext _context;
        private bool _disposed;

        public SaleHistoryService(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException("context");
        }

        public async Task<List<SaleHistory>> GetAllAsync()
        {
            return await _context.SalesHistory
                .Include(sh => sh.Partner)
                .Include(sh => sh.Product)
                .OrderBy(sh => sh.SaleId) 
                .ToListAsync();
        }

        public async Task<SaleHistory> GetByIdAsync(int id)
        {
            return await _context.SalesHistory
                .Include(sh => sh.Partner)
                .Include(sh => sh.Product)
                .FirstOrDefaultAsync(sh => sh.SaleId == id);
        }

        public async Task<List<SaleHistory>> GetByPartnerIdAsync(int partnerId)
        {
            return await _context.SalesHistory
                .Include(sh => sh.Product)
                .Where(sh => sh.PartnerId == partnerId)
                .OrderBy(sh => sh.SaleId)  
                .ToListAsync();
        }

        public async Task<SaleHistory> AddAsync(SaleHistory sale)
        {
            if (sale == null)
            {
                throw new ArgumentNullException("sale");
            }

            if (sale.Quantity <= 0)
            {
                throw new ArgumentException("Количество должно быть больше 0");
            }

            if (sale.TotalAmount < 0)
            {
                throw new ArgumentException("Сумма должна быть неотрицательной");
            }

            var partnerExists = await _context.Partners.AnyAsync(p => p.PartnerId == sale.PartnerId);
            if (!partnerExists)
            {
                throw new InvalidOperationException(string.Format(
                    "Партнёр с ID {0} не найден", sale.PartnerId));
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == sale.ProductId);
            if (!productExists)
            {
                throw new InvalidOperationException(string.Format(
                    "Продукция с ID {0} не найдена", sale.ProductId));
            }

            sale.CreatedDate = DateTime.Now;

            _context.SalesHistory.Add(sale);
            await _context.SaveChangesAsync();

            return sale;
        }

        public async Task<SaleHistory> UpdateAsync(SaleHistory sale)
        {
            if (sale == null)
            {
                throw new ArgumentNullException("sale");
            }

            var existing = await _context.SalesHistory.FindAsync(sale.SaleId);
            if (existing == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Запись о продаже с ID {0} не найдена", sale.SaleId));
            }

            existing.Quantity = sale.Quantity;
            existing.TotalAmount = sale.TotalAmount;
            existing.SaleDate = sale.SaleDate;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await _context.SalesHistory.FindAsync(id);
            if (sale == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Запись о продаже с ID {0} не найдена", id));
            }

            _context.SalesHistory.Remove(sale);
            await _context.SaveChangesAsync();
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