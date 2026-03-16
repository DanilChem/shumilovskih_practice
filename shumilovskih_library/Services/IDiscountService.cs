namespace shumilovskih_library.Services
{
    public interface IDiscountService
    {
        decimal CalculateDiscount(decimal totalSales);
        int GetDiscountPercent(decimal totalSales);
        string GetDiscountLevel(decimal totalSales);
    }
}