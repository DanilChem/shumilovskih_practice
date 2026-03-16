using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace shumilovskih_pp
{
    public partial class SaleEditWindow : Window
    {
        private readonly ISaleHistoryService _saleHistoryService;
        private readonly IProductService _productService;
        private readonly Partner _partner;
        private readonly SaleHistory _saleToEdit;
        private readonly bool _isEditMode;

        public SaleEditWindow(ISaleHistoryService saleHistoryService, IProductService productService, Partner partner)
            : this(saleHistoryService, productService, partner, null)
        {
        }

        public SaleEditWindow(ISaleHistoryService saleHistoryService, IProductService productService, Partner partner, SaleHistory saleToEdit)
        {
            InitializeComponent();
            _saleHistoryService = saleHistoryService;
            _productService = productService;
            _partner = partner;
            _saleToEdit = saleToEdit;
            _isEditMode = saleToEdit != null;

            PartnerNameTextBlock.Text = $"Партнер: {partner.CompanyName}";
            SaleDatePicker.SelectedDate = DateTime.Today;

            if (_isEditMode)
            {
                Title = "Редактирование продажи";
                QuantityTextBox.Text = _saleToEdit.Quantity.ToString();
                TotalAmountTextBox.Text = _saleToEdit.TotalAmount.ToString();
                SaleDatePicker.SelectedDate = _saleToEdit.SaleDate;
            }
            else
            {
                Title = "Добавление продажи";
            }

            LoadProducts();
        }

        private async void LoadProducts()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                ProductComboBox.ItemsSource = products;
                if (_isEditMode && _saleToEdit != null)
                {
                    ProductComboBox.SelectedValue = _saleToEdit.ProductId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите продукцию", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Количество должно быть больше 0", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(TotalAmountTextBox.Text, out decimal totalAmount) || totalAmount < 0)
                {
                    MessageBox.Show("Сумма должна быть неотрицательной", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!SaleDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату продажи", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditMode && _saleToEdit != null)
                {
                    _saleToEdit.ProductId = (int)ProductComboBox.SelectedValue;
                    _saleToEdit.Quantity = quantity;
                    _saleToEdit.TotalAmount = totalAmount;
                    _saleToEdit.SaleDate = SaleDatePicker.SelectedDate.Value;

                    await _saleHistoryService.UpdateAsync(_saleToEdit);
                    MessageBox.Show("Продажа обновлена", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var sale = new SaleHistory
                    {
                        PartnerId = _partner.PartnerId,
                        ProductId = (int)ProductComboBox.SelectedValue,
                        Quantity = quantity,
                        TotalAmount = totalAmount,
                        SaleDate = SaleDatePicker.SelectedDate.Value,
                        CreatedDate = DateTime.Now
                    };

                    await _saleHistoryService.AddAsync(sale);
                    MessageBox.Show("Продажа добавлена", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}