using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace shumilovskih_pp
{
    public partial class SalesHistoryWindow : Window
    {
        private readonly ISaleHistoryService _saleHistoryService;
        private readonly IProductService _productService;
        private readonly IDiscountService _discountService;
        private readonly Partner _partner;
        private SaleHistory _selectedSale;

        public SalesHistoryWindow(ISaleHistoryService saleHistoryService, IProductService productService, Partner partner)
        {
            InitializeComponent();
            _saleHistoryService = saleHistoryService;
            _productService = productService;
            _discountService = new DiscountService();
            _partner = partner;
            PartnerNameTextBlock.Text = $"Партнер: {partner.CompanyName}";
            LoadSales();
        }

        private async Task LoadSales()
        {
            try
            {
                var sales = await _saleHistoryService.GetByPartnerIdAsync(_partner.PartnerId);
                SalesDataGrid.ItemsSource = sales;
                var totalSales = sales.Sum(s => s.TotalAmount);
                var discount = _discountService.CalculateDiscount(totalSales);
                TotalSalesTextBlock.Text = $"{totalSales:N2} ₽";
                DiscountTextBlock.Text = $"{discount * 100:N0}% ({_discountService.GetDiscountLevel(totalSales)})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SalesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSale = SalesDataGrid.SelectedItem as SaleHistory;
        }

        private async void AddSale_Click(object sender, RoutedEventArgs e)
        {
            var window = new SaleEditWindow(_saleHistoryService, _productService, _partner);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                await LoadSales();
            }
        }

        private async void EditSale_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSale == null)
            {
                MessageBox.Show("Выберите продажу для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new SaleEditWindow(_saleHistoryService, _productService, _partner, _selectedSale);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                await LoadSales();
            }
        }

        private async void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSale == null)
            {
                MessageBox.Show("Выберите продажу для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show("Вы действительно хотите удалить запись о продаже?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _saleHistoryService.DeleteAsync(_selectedSale.SaleId);
                    MessageBox.Show("Запись о продаже успешно удалена", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadSales();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadSales();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}