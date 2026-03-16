using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using shumilovskih_library.Context;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace shumilovskih_pp
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationContext _context;
        private readonly IPartnerService _partnerService;
        private readonly ISaleHistoryService _saleHistoryService;
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;
        private readonly IPartnerTypeService _partnerTypeService;
        private Partner _selectedPartner;


        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationContext();
            _partnerService = new PartnerService(_context, new DiscountService());
            _saleHistoryService = new SaleHistoryService(_context);
            _discountService = new DiscountService();
            _productService = new ProductService(_context);
            _partnerTypeService = new PartnerTypeService(_context);
            LoadPartners();
            UpdateStatus("Приложение запущено.");
        }


        private async void LoadPartners()
        {
            try
            {
                var partners = await _partnerService.GetAllPartnersAsync();
                PartnersDataGrid.ItemsSource = partners;
                UpdateStatus($"Загружено партнеров: {partners.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки партнеров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Ошибка загрузки партнеров");
            }
        }

        private async void LoadPartnerSales(Partner partner)
        {
            try
            {
                if (partner == null)
                {
                    SalesDataGrid.ItemsSource = new List<SaleHistory>();
                    TotalSalesTextBlock.Text = "0 ₽";
                    DiscountTextBlock.Text = "0%";
                    return;
                }
                var sales = await _saleHistoryService.GetByPartnerIdAsync(partner.PartnerId);
                SalesDataGrid.ItemsSource = sales;
                var totalSales = await _partnerService.GetPartnerTotalSalesAsync(partner.PartnerId);
                var discount = await _partnerService.GetPartnerDiscountAsync(partner.PartnerId);
                TotalSalesTextBlock.Text = $"{totalSales:N2} ₽";
                DiscountTextBlock.Text = $"{discount * 100:N0}% ({_discountService.GetDiscountLevel(totalSales)})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PartnersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartnersDataGrid.SelectedItem is Partner partner)
            {
                _selectedPartner = partner;
                LoadPartnerSales(partner);
            }
        }

        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartnersWindow(_partnerService, _partnerTypeService, null);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadPartners();
                UpdateStatus("Партнер добавлен");
            }
        }

        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new PartnersWindow(_partnerService, _partnerTypeService, _selectedPartner);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadPartners();
                LoadPartnerSales(_selectedPartner);
                UpdateStatus("Партнер изменен");
            }
        }

        private async void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show(
                $"Вы действительно хотите удалить партнера \"{_selectedPartner.CompanyName}\"?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _partnerService.DeletePartnerAsync(_selectedPartner.PartnerId);
                    MessageBox.Show("Партнер успешно удален", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _selectedPartner = null;
                    LoadPartners();
                    LoadPartnerSales(null);
                    UpdateStatus("Партнер удален");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatus("Ошибка удаления партнера");
                }
            }
        }


        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new ProductEditWindow(_productService);
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                var viewWindow = new ProductsWindow(_productService);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
                UpdateStatus("Продукция добавлена и список обновлён");
            }
        }

        private void ViewProducts_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProductsWindow(_productService);
            window.Owner = this;
            window.ShowDialog();
            UpdateStatus("Список продукции обновлен");
        }

        private void RefreshProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Список продукции обновлен");
        }

        private async void AddSale_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера для добавления продажи", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addWindow = new SaleEditWindow(_saleHistoryService, _productService, _selectedPartner);
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                var viewWindow = new SalesHistoryWindow(_saleHistoryService, _productService, _selectedPartner);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
                LoadPartnerSales(_selectedPartner);
                UpdateStatus("Продажа добавлена и история обновлена");
            }
        }

        private void ViewSales_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера для просмотра истории продаж", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new SalesHistoryWindow(_saleHistoryService, _productService, _selectedPartner);
            window.Owner = this;
            window.ShowDialog();
            LoadPartnerSales(_selectedPartner);
            UpdateStatus("История продаж обновлена");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Подсистема работы с партнерами\nВерсия 1.0\n" +
                "Разработал: shumilovskih\nГруппа: ПП 20.01\n2026 год",
                "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}