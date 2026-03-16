using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace shumilovskih_pp
{
    public partial class ProductsWindow : Window
    {
        private readonly IProductService _productService;
        private Product _selectedProduct;

        public ProductsWindow(IProductService productService)
        {
            InitializeComponent();
            _productService = productService;
            LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                ProductsDataGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductsDataGrid.SelectedItem as Product;
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProductEditWindow(_productService);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                await LoadProducts();
            }
        }

         private async void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите продукцию для редактирования",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new ProductEditWindow(_productService, _selectedProduct);
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                await LoadProducts();
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите продукцию для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show(
                $"Вы действительно хотите удалить продукцию \"{_selectedProduct.ProductName}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _productService.DeleteAsync(_selectedProduct.ProductId);
                    MessageBox.Show("Продукция успешно удалена", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadProducts();
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
            await LoadProducts();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}