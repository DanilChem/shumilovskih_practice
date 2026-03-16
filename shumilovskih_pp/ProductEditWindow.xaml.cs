using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace shumilovskih_pp
{
    public partial class ProductEditWindow : Window
    {
        private readonly IProductService _productService;
        private Product _selectedProduct;
        private bool _isEditMode;

        public ProductEditWindow(IProductService productService, Product productToEdit = null)
        {
            InitializeComponent();
            _productService = productService;
            _selectedProduct = productToEdit;
            _isEditMode = productToEdit != null;

            if (_isEditMode)
            {
                TitleTextBlock.Text = "Редактирование продукции";
                LoadProductData();
            }
        }

        private void LoadProductData()
        {
            if (_selectedProduct != null)
            {
                ProductNameTextBox.Text = _selectedProduct.ProductName;
                ArticleTextBox.Text = _selectedProduct.Article;
                TypeTextBox.Text = _selectedProduct.Type;
                DescriptionTextBox.Text = _selectedProduct.Description;
                MinPriceTextBox.Text = _selectedProduct.MinPrice.ToString();
                PackageSizeTextBox.Text = _selectedProduct.PackageSize;
                WeightTextBox.Text = _selectedProduct.Weight.ToString();
                StandardNumberTextBox.Text = _selectedProduct.StandardNumber;
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
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                {
                    MessageBox.Show("Наименование продукции обязательно", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
                {
                    MessageBox.Show("Артикул обязателен", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(MinPriceTextBox.Text, out decimal minPrice) || minPrice < 0)
                {
                    MessageBox.Show("Цена должна быть неотрицательным числом", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal weight = 0;
                if (!string.IsNullOrWhiteSpace(WeightTextBox.Text))
                {
                    if (!decimal.TryParse(WeightTextBox.Text, out weight) || weight < 0)
                    {
                        MessageBox.Show("Вес должен быть неотрицательным числом", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (_isEditMode && _selectedProduct != null)
                {
                    _selectedProduct.ProductName = ProductNameTextBox.Text.Trim();
                    _selectedProduct.Article = ArticleTextBox.Text.Trim();
                    _selectedProduct.Type = TypeTextBox.Text.Trim();
                    _selectedProduct.Description = DescriptionTextBox.Text.Trim();
                    _selectedProduct.MinPrice = minPrice;
                    _selectedProduct.PackageSize = PackageSizeTextBox.Text.Trim();
                    _selectedProduct.Weight = weight;
                    _selectedProduct.StandardNumber = StandardNumberTextBox.Text.Trim();
                    _selectedProduct.ModifiedDate = DateTime.Now;

                    await _productService.UpdateAsync(_selectedProduct);
                    MessageBox.Show("Продукция обновлена", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var product = new Product
                    {
                        ProductName = ProductNameTextBox.Text.Trim(),
                        Article = ArticleTextBox.Text.Trim(),
                        Type = TypeTextBox.Text.Trim(),
                        Description = DescriptionTextBox.Text.Trim(),
                        MinPrice = minPrice,
                        PackageSize = PackageSizeTextBox.Text.Trim(),
                        Weight = weight,
                        StandardNumber = StandardNumberTextBox.Text.Trim(),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    await _productService.AddAsync(product);
                    MessageBox.Show("Продукция добавлена", "Информация",
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