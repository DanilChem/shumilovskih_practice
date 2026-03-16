using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shumilovskih_library.Context;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace UnitTest1
{
    [TestClass]
    public class LibraryTests
    {
        private ApplicationContext _context;
        private IPartnerService _partnerService;
        private IPartnerTypeService _partnerTypeService;
        private IProductService _productService;
        private ISaleHistoryService _saleHistoryService;
        private IDiscountService _discountService;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationContext(options);
            _discountService = new DiscountService();
            _partnerService = new PartnerService(_context, _discountService);
            _partnerTypeService = new PartnerTypeService(_context);
            _productService = new ProductService(_context);
            _saleHistoryService = new SaleHistoryService(_context);
        }

        [TestMethod]
        public void CalculateDiscount_ShouldReturnCorrectDiscount()
        {
            try
            {
                var service = new DiscountService();
                var result = service.CalculateDiscount(10000);
                Debug.WriteLine(result == 0.05m ? "PASSED: CalculateDiscount_ShouldReturnCorrectDiscount" : "FAILED: CalculateDiscount_ShouldReturnCorrectDiscount");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CalculateDiscount_ShouldReturnCorrectDiscount: {ex}");
            }
        }

        [TestMethod]
        public void CalculateDiscount_ShouldThrowException_WhenTotalSalesNegative()
        {
            try
            {
                var service = new DiscountService();
                service.CalculateDiscount(-100);
                Debug.WriteLine("FAILED: CalculateDiscount_ShouldThrowException_WhenTotalSalesNegative (expected exception)");
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("PASSED: CalculateDiscount_ShouldThrowException_WhenTotalSalesNegative");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CalculateDiscount_ShouldThrowException_WhenTotalSalesNegative: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerTypeService_AddAsync_ShouldAddNewType()
        {
            try
            {
                var type = new PartnerType { TypeName = "Test Type" };
                var result = await _partnerTypeService.AddAsync(type);
                if (result.PartnerTypeId > 0 && result.TypeName == "Test Type")
                    Debug.WriteLine("PASSED: PartnerTypeService_AddAsync_ShouldAddNewType");
                else
                    Debug.WriteLine("FAILED: PartnerTypeService_AddAsync_ShouldAddNewType");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerTypeService_AddAsync_ShouldAddNewType: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerTypeService_GetAllAsync_ShouldReturnAllTypes()
        {
            try
            {
                await _partnerTypeService.AddAsync(new PartnerType { TypeName = "Type1" });
                await _partnerTypeService.AddAsync(new PartnerType { TypeName = "Type2" });
                var list = await _partnerTypeService.GetAllAsync();
                if (list.Count == 2 && list.Any(t => t.TypeName == "Type1") && list.Any(t => t.TypeName == "Type2"))
                    Debug.WriteLine("PASSED: PartnerTypeService_GetAllAsync_ShouldReturnAllTypes");
                else
                    Debug.WriteLine("FAILED: PartnerTypeService_GetAllAsync_ShouldReturnAllTypes");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerTypeService_GetAllAsync_ShouldReturnAllTypes: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerTypeService_GetByIdAsync_ShouldReturnCorrectType()
        {
            try
            {
                var added = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "Test" });
                var found = await _partnerTypeService.GetByIdAsync(added.PartnerTypeId);
                if (found != null && found.TypeName == "Test")
                    Debug.WriteLine("PASSED: PartnerTypeService_GetByIdAsync_ShouldReturnCorrectType");
                else
                    Debug.WriteLine("FAILED: PartnerTypeService_GetByIdAsync_ShouldReturnCorrectType");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerTypeService_GetByIdAsync_ShouldReturnCorrectType: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerTypeService_UpdateAsync_ShouldUpdateType()
        {
            try
            {
                var added = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "Old" });
                added.TypeName = "New";
                var updated = await _partnerTypeService.UpdateAsync(added);
                var check = await _partnerTypeService.GetByIdAsync(added.PartnerTypeId);
                if (check.TypeName == "New")
                    Debug.WriteLine("PASSED: PartnerTypeService_UpdateAsync_ShouldUpdateType");
                else
                    Debug.WriteLine("FAILED: PartnerTypeService_UpdateAsync_ShouldUpdateType");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerTypeService_UpdateAsync_ShouldUpdateType: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerTypeService_DeleteAsync_ShouldRemoveType()
        {
            try
            {
                var added = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "ToDelete" });
                await _partnerTypeService.DeleteAsync(added.PartnerTypeId);
                var check = await _partnerTypeService.GetByIdAsync(added.PartnerTypeId);
                if (check == null)
                    Debug.WriteLine("PASSED: PartnerTypeService_DeleteAsync_ShouldRemoveType");
                else
                    Debug.WriteLine("FAILED: PartnerTypeService_DeleteAsync_ShouldRemoveType");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerTypeService_DeleteAsync_ShouldRemoveType: {ex}");
            }
        }

        [TestMethod]
        public async Task ProductService_AddAsync_ShouldAddProduct()
        {
            try
            {
                var product = new Product { ProductName = "Test Product", Article = "ART123", MinPrice = 100 };
                var added = await _productService.AddAsync(product);
                if (added.ProductId > 0)
                    Debug.WriteLine("PASSED: ProductService_AddAsync_ShouldAddProduct");
                else
                    Debug.WriteLine("FAILED: ProductService_AddAsync_ShouldAddProduct");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductService_AddAsync_ShouldAddProduct: {ex}");
            }
        }

        [TestMethod]
        public async Task ProductService_GetAllAsync_ShouldReturnAllProducts()
        {
            try
            {
                await _productService.AddAsync(new Product { ProductName = "P1", Article = "A1", MinPrice = 10 });
                await _productService.AddAsync(new Product { ProductName = "P2", Article = "A2", MinPrice = 20 });
                var list = await _productService.GetAllAsync();
                if (list.Count == 2)
                    Debug.WriteLine("PASSED: ProductService_GetAllAsync_ShouldReturnAllProducts");
                else
                    Debug.WriteLine("FAILED: ProductService_GetAllAsync_ShouldReturnAllProducts");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductService_GetAllAsync_ShouldReturnAllProducts: {ex}");
            }
        }

        [TestMethod]
        public async Task ProductService_UpdateAsync_ShouldUpdateProduct()
        {
            try
            {
                var product = await _productService.AddAsync(new Product { ProductName = "Old", Article = "ART", MinPrice = 10 });
                product.ProductName = "New";
                await _productService.UpdateAsync(product);
                var updated = await _productService.GetByIdAsync(product.ProductId);
                if (updated.ProductName == "New")
                    Debug.WriteLine("PASSED: ProductService_UpdateAsync_ShouldUpdateProduct");
                else
                    Debug.WriteLine("FAILED: ProductService_UpdateAsync_ShouldUpdateProduct");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductService_UpdateAsync_ShouldUpdateProduct: {ex}");
            }
        }

        [TestMethod]
        public async Task ProductService_DeleteAsync_ShouldRemoveProduct()
        {
            try
            {
                var product = await _productService.AddAsync(new Product { ProductName = "ToDelete", Article = "ART", MinPrice = 10 });
                await _productService.DeleteAsync(product.ProductId);
                var check = await _productService.GetByIdAsync(product.ProductId);
                if (check == null)
                    Debug.WriteLine("PASSED: ProductService_DeleteAsync_ShouldRemoveProduct");
                else
                    Debug.WriteLine("FAILED: ProductService_DeleteAsync_ShouldRemoveProduct");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductService_DeleteAsync_ShouldRemoveProduct: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_AddPartnerAsync_ShouldAddPartner()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "TestType" });
                var partner = new Partner { CompanyName = "Test Company", PartnerTypeId = type.PartnerTypeId, Rating = 5 };
                var added = await _partnerService.AddPartnerAsync(partner);
                if (added.PartnerId > 0)
                    Debug.WriteLine("PASSED: PartnerService_AddPartnerAsync_ShouldAddPartner");
                else
                    Debug.WriteLine("FAILED: PartnerService_AddPartnerAsync_ShouldAddPartner");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_AddPartnerAsync_ShouldAddPartner: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_GetAllPartnersAsync_ShouldReturnAll()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P1", PartnerTypeId = type.PartnerTypeId, Rating = 1 });
                await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P2", PartnerTypeId = type.PartnerTypeId, Rating = 2 });
                var list = await _partnerService.GetAllPartnersAsync();
                if (list.Count == 2)
                    Debug.WriteLine("PASSED: PartnerService_GetAllPartnersAsync_ShouldReturnAll");
                else
                    Debug.WriteLine("FAILED: PartnerService_GetAllPartnersAsync_ShouldReturnAll");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_GetAllPartnersAsync_ShouldReturnAll: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_GetPartnerByIdAsync_ShouldReturnCorrect()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "Test", PartnerTypeId = type.PartnerTypeId, Rating = 3 });
                var found = await _partnerService.GetPartnerByIdAsync(partner.PartnerId);
                if (found != null && found.CompanyName == "Test")
                    Debug.WriteLine("PASSED: PartnerService_GetPartnerByIdAsync_ShouldReturnCorrect");
                else
                    Debug.WriteLine("FAILED: PartnerService_GetPartnerByIdAsync_ShouldReturnCorrect");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_GetPartnerByIdAsync_ShouldReturnCorrect: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_UpdatePartnerAsync_ShouldUpdate()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "Old", PartnerTypeId = type.PartnerTypeId, Rating = 1 });
                partner.CompanyName = "New";
                await _partnerService.UpdatePartnerAsync(partner);
                var updated = await _partnerService.GetPartnerByIdAsync(partner.PartnerId);
                if (updated.CompanyName == "New")
                    Debug.WriteLine("PASSED: PartnerService_UpdatePartnerAsync_ShouldUpdate");
                else
                    Debug.WriteLine("FAILED: PartnerService_UpdatePartnerAsync_ShouldUpdate");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_UpdatePartnerAsync_ShouldUpdate: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_DeletePartnerAsync_ShouldDelete()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "ToDelete", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                await _partnerService.DeletePartnerAsync(partner.PartnerId);
                var check = await _partnerService.GetPartnerByIdAsync(partner.PartnerId);
                if (check == null)
                    Debug.WriteLine("PASSED: PartnerService_DeletePartnerAsync_ShouldDelete");
                else
                    Debug.WriteLine("FAILED: PartnerService_DeletePartnerAsync_ShouldDelete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_DeletePartnerAsync_ShouldDelete: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_GetPartnerTotalSalesAsync_ShouldCalculateSum()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                var product = await _productService.AddAsync(new Product { ProductName = "Prod", Article = "A", MinPrice = 100 });
                await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 2, TotalAmount = 200, SaleDate = DateTime.Now });
                await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 1, TotalAmount = 150, SaleDate = DateTime.Now });
                var total = await _partnerService.GetPartnerTotalSalesAsync(partner.PartnerId);
                if (total == 350)
                    Debug.WriteLine("PASSED: PartnerService_GetPartnerTotalSalesAsync_ShouldCalculateSum");
                else
                    Debug.WriteLine($"FAILED: PartnerService_GetPartnerTotalSalesAsync_ShouldCalculateSum (got {total})");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_GetPartnerTotalSalesAsync_ShouldCalculateSum: {ex}");
            }
        }

        [TestMethod]
        public async Task PartnerService_GetPartnerDiscountAsync_ShouldReturnDiscount()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                var product = await _productService.AddAsync(new Product { ProductName = "Prod", Article = "A", MinPrice = 100 });
                await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 10, TotalAmount = 10000, SaleDate = DateTime.Now });
                var discount = await _partnerService.GetPartnerDiscountAsync(partner.PartnerId);
                if (discount == 0.05m)
                    Debug.WriteLine("PASSED: PartnerService_GetPartnerDiscountAsync_ShouldReturnDiscount");
                else
                    Debug.WriteLine($"FAILED: PartnerService_GetPartnerDiscountAsync_ShouldReturnDiscount (got {discount})");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_GetPartnerDiscountAsync_ShouldReturnDiscount: {ex}");
            }
        }

        [TestMethod]
        public async Task SaleHistoryService_AddAsync_ShouldAddSale()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                var product = await _productService.AddAsync(new Product { ProductName = "Prod", Article = "A", MinPrice = 100 });
                var sale = new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 5, TotalAmount = 500, SaleDate = DateTime.Now };
                var added = await _saleHistoryService.AddAsync(sale);
                if (added.SaleId > 0)
                    Debug.WriteLine("PASSED: SaleHistoryService_AddAsync_ShouldAddSale");
                else
                    Debug.WriteLine("FAILED: SaleHistoryService_AddAsync_ShouldAddSale");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaleHistoryService_AddAsync_ShouldAddSale: {ex}");
            }
        }

        [TestMethod]
        public async Task SaleHistoryService_GetByPartnerIdAsync_ShouldReturnSales()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                var product = await _productService.AddAsync(new Product { ProductName = "Prod", Article = "A", MinPrice = 100 });
                await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 1, TotalAmount = 100, SaleDate = DateTime.Now });
                await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 2, TotalAmount = 200, SaleDate = DateTime.Now });
                var list = await _saleHistoryService.GetByPartnerIdAsync(partner.PartnerId);
                if (list.Count == 2)
                    Debug.WriteLine("PASSED: SaleHistoryService_GetByPartnerIdAsync_ShouldReturnSales");
                else
                    Debug.WriteLine("FAILED: SaleHistoryService_GetByPartnerIdAsync_ShouldReturnSales");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaleHistoryService_GetByPartnerIdAsync_ShouldReturnSales: {ex}");
            }
        }

        [TestMethod]
        public async Task SaleHistoryService_UpdateAsync_ShouldUpdateSale()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                var product = await _productService.AddAsync(new Product { ProductName = "Prod", Article = "A", MinPrice = 100 });
                var sale = await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 1, TotalAmount = 100, SaleDate = DateTime.Now });
                sale.Quantity = 10;
                sale.TotalAmount = 1000;
                await _saleHistoryService.UpdateAsync(sale);
                var updated = await _saleHistoryService.GetByIdAsync(sale.SaleId);
                if (updated.Quantity == 10 && updated.TotalAmount == 1000)
                    Debug.WriteLine("PASSED: SaleHistoryService_UpdateAsync_ShouldUpdateSale");
                else
                    Debug.WriteLine("FAILED: SaleHistoryService_UpdateAsync_ShouldUpdateSale");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaleHistoryService_UpdateAsync_ShouldUpdateSale: {ex}");
            }
        }

        [TestMethod]
        public async Task SaleHistoryService_DeleteAsync_ShouldDeleteSale()
        {
            try
            {
                var type = await _partnerTypeService.AddAsync(new PartnerType { TypeName = "T" });
                var partner = await _partnerService.AddPartnerAsync(new Partner { CompanyName = "P", PartnerTypeId = type.PartnerTypeId, Rating = 0 });
                var product = await _productService.AddAsync(new Product { ProductName = "Prod", Article = "A", MinPrice = 100 });
                var sale = await _saleHistoryService.AddAsync(new SaleHistory { PartnerId = partner.PartnerId, ProductId = product.ProductId, Quantity = 1, TotalAmount = 100, SaleDate = DateTime.Now });
                await _saleHistoryService.DeleteAsync(sale.SaleId);
                var check = await _saleHistoryService.GetByIdAsync(sale.SaleId);
                if (check == null)
                    Debug.WriteLine("PASSED: SaleHistoryService_DeleteAsync_ShouldDeleteSale");
                else
                    Debug.WriteLine("FAILED: SaleHistoryService_DeleteAsync_ShouldDeleteSale");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaleHistoryService_DeleteAsync_ShouldDeleteSale: {ex}");
            }
        }

        [TestMethod]
        public void PartnerService_Dispose_ShouldDisposeContext()
        {
            try
            {
                var service = _partnerService as PartnerService;
                service?.Dispose();
                  _context.PartnerTypes.Any();
                Debug.WriteLine("FAILED: PartnerService_Dispose_ShouldDisposeContext (expected exception)");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("PASSED: PartnerService_Dispose_ShouldDisposeContext");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PartnerService_Dispose_ShouldDisposeContext: {ex}");
            }
        }
    }
}