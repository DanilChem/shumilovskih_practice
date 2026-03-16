using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shumilovskih_library.Context;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public class PartnerTypeService : IPartnerTypeService, IDisposable
    {
        private readonly ApplicationContext _context;
        private bool _disposed;

        public PartnerTypeService(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException("context");
        }

        public async Task<List<PartnerType>> GetAllAsync()
        {
            return await _context.PartnerTypes
                .OrderBy(pt => pt.TypeName)
                .ToListAsync();
        }

        public async Task<PartnerType> GetByIdAsync(int id)
        {
            return await _context.PartnerTypes
                .FirstOrDefaultAsync(pt => pt.PartnerTypeId == id);
        }

        public async Task<PartnerType> AddAsync(PartnerType partnerType)
        {
            if (partnerType == null)
            {
                throw new ArgumentNullException("partnerType");
            }

            if (string.IsNullOrWhiteSpace(partnerType.TypeName))
            {
                throw new ArgumentException("Наименование типа партнёра обязательно");
            }

            var existing = await _context.PartnerTypes
                .FirstOrDefaultAsync(pt => pt.TypeName == partnerType.TypeName);

            if (existing != null)
            {
                throw new InvalidOperationException(string.Format(
                    "Тип партнёра '{0}' уже существует", partnerType.TypeName));
            }

            _context.PartnerTypes.Add(partnerType);
            await _context.SaveChangesAsync();

            return partnerType;
        }

        public async Task<PartnerType> UpdateAsync(PartnerType partnerType)
        {
            if (partnerType == null)
            {
                throw new ArgumentNullException("partnerType");
            }

            var existing = await _context.PartnerTypes.FindAsync(partnerType.PartnerTypeId);
            if (existing == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Тип партнёра с ID {0} не найден", partnerType.PartnerTypeId));
            }

            existing.TypeName = partnerType.TypeName;
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var partnerType = await _context.PartnerTypes.FindAsync(id);
            if (partnerType == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Тип партнёра с ID {0} не найден", id));
            }

            _context.PartnerTypes.Remove(partnerType);
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