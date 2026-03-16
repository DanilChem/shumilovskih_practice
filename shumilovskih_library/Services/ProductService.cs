using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shumilovskih_library.Context;
using shumilovskih_library.Models;

namespace shumilovskih_library.Services
{
    public class ProductService : IProductService, IDisposable
    {
        private readonly ApplicationContext _context;
        private bool _disposed;

        public ProductService(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException("context");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .OrderBy(p => p.ProductId)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new ArgumentException("Наименование продукции обязательно");
            }

            if (product.MinPrice < 0)
            {
                throw new ArgumentException("Цена должна быть неотрицательной");
            }

            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.Article == product.Article);

            if (existing != null)
            {
                throw new InvalidOperationException(string.Format(
                    "Продукция с артикулом '{0}' уже существует", product.Article));
            }

            product.CreatedDate = DateTime.Now;
            product.ModifiedDate = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            var existing = await _context.Products.FindAsync(product.ProductId);
            if (existing == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Продукция с ID {0} не найдена", product.ProductId));
            }

            existing.ProductName = product.ProductName;
            existing.Article = product.Article;
            existing.Type = product.Type;
            existing.Description = product.Description;
            existing.MinPrice = product.MinPrice;
            existing.PackageSize = product.PackageSize;
            existing.Weight = product.Weight;
            existing.StandardNumber = product.StandardNumber;
            existing.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Продукция с ID {0} не найдена", id));
            }

            _context.Products.Remove(product);
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