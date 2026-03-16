using System;

namespace shumilovskih_library.Services
{
    public class DiscountService : IDiscountService
    {
        private const decimal ThresholdLevel1 = 10000m;
        private const decimal ThresholdLevel2 = 50000m;
        private const decimal ThresholdLevel3 = 300000m;

        private const decimal DiscountLevel0 = 0.00m;   // 0%
        private const decimal DiscountLevel1 = 0.05m;   // 5%
        private const decimal DiscountLevel2 = 0.10m;   // 10%
        private const decimal DiscountLevel3 = 0.15m;   // 15%


        public decimal CalculateDiscount(decimal totalSales)
        {
            if (totalSales < 0)
            {
                throw new ArgumentException("Сумма продаж не может быть отрицательной", "totalSales");
            }

            if (totalSales < ThresholdLevel1)
            {
                return DiscountLevel0;
            }
            else if (totalSales < ThresholdLevel2)
            {
                return DiscountLevel1;
            }
            else if (totalSales < ThresholdLevel3)
            {
                return DiscountLevel2;
            }
            else
            {
                return DiscountLevel3;
            }
        }

        public int GetDiscountPercent(decimal totalSales)
        {
            return (int)(CalculateDiscount(totalSales) * 100);
        }

        public string GetDiscountLevel(decimal totalSales)
        {
            if (totalSales < ThresholdLevel1)
            {
                return "Базовый (0%)";
            }
            else if (totalSales < ThresholdLevel2)
            {
                return "Бронзовый (5%)";
            }
            else if (totalSales < ThresholdLevel3)
            {
                return "Серебряный (10%)";
            }
            else
            {
                return "Золотой (15%)";
            }
        }
    }
}