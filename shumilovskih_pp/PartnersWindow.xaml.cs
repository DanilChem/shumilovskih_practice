using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using shumilovskih_library.Models;
using shumilovskih_library.Services;

namespace shumilovskih_pp
{
    public partial class PartnersWindow : Window
    {
        private readonly IPartnerService _partnerService;
        private readonly IPartnerTypeService _partnerTypeService;
        private Partner _selectedPartner;
        private bool _isEditMode;

        public PartnersWindow(IPartnerService partnerService, IPartnerTypeService partnerTypeService, Partner partnerToEdit = null)
        {
            InitializeComponent();
            _partnerService = partnerService;
            _partnerTypeService = partnerTypeService;
            _selectedPartner = partnerToEdit;
            _isEditMode = partnerToEdit != null;

            if (_isEditMode)
            {
                Title = "Редактирование партнера - shumilovskih_pp";
            }
            else
            {
                Title = "Добавление партнера - shumilovskih_pp";
            }

            this.Loaded += async (s, e) => await LoadPartnerTypesAsync();
        }

        private async System.Threading.Tasks.Task LoadPartnerTypesAsync()
        {
            try
            {
                var partnerTypes = await _partnerTypeService.GetAllAsync();
                PartnerTypeComboBox.ItemsSource = partnerTypes;
                PartnerTypeComboBox.DisplayMemberPath = "TypeName";
                PartnerTypeComboBox.SelectedValuePath = "PartnerTypeId";

                if (_isEditMode && _selectedPartner != null)
                {
                    PartnerTypeComboBox.SelectedValue = _selectedPartner.PartnerTypeId;
                    LoadPartnerData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartnerData()
        {
            if (_selectedPartner == null) return;

            CompanyNameTextBox.Text = _selectedPartner.CompanyName;
            RatingTextBox.Text = _selectedPartner.Rating.ToString();
            InnTextBox.Text = _selectedPartner.Inn;
            AddressTextBox.Text = _selectedPartner.Address;
            DirectorNameTextBox.Text = _selectedPartner.DirectorName;
            PhoneTextBox.Text = _selectedPartner.Phone;
            EmailTextBox.Text = _selectedPartner.Email;
            LogoTextBox.Text = _selectedPartner.Logo;
            SalesLocationsTextBox.Text = _selectedPartner.SalesLocations;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*",
                Title = "Выберите файл логотипа"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LogoTextBox.Text = openFileDialog.FileName;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text))
                {
                    MessageBox.Show("Наименование компании обязательно", "Ошибка валидации",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int partnerTypeId;

                if (PartnerTypeComboBox.SelectedItem == null)
                {
                    string newTypeName = PartnerTypeComboBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(newTypeName))
                    {
                        MessageBox.Show("Выберите или введите тип партнера", "Ошибка валидации",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var existingType = (await _partnerTypeService.GetAllAsync())
                        .FirstOrDefault(pt => pt.TypeName.ToLower() == newTypeName.ToLower());

                    if (existingType != null)
                    {
                        partnerTypeId = existingType.PartnerTypeId;
                    }
                    else
                    {
                        var newPartnerType = new PartnerType
                        {
                            TypeName = newTypeName
                        };

                        var createdType = await _partnerTypeService.AddAsync(newPartnerType);
                        partnerTypeId = createdType.PartnerTypeId;

                        MessageBox.Show($"Новый тип партнера \"{newTypeName}\" добавлен", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    partnerTypeId = (int)PartnerTypeComboBox.SelectedValue;
                }

                if (!int.TryParse(RatingTextBox.Text, out int rating) || rating < 0)
                {
                    MessageBox.Show("Рейтинг должен быть неотрицательным числом", "Ошибка валидации",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditMode && _selectedPartner != null)
                {
                    _selectedPartner.CompanyName = CompanyNameTextBox.Text.Trim();
                    _selectedPartner.PartnerTypeId = partnerTypeId;
                    _selectedPartner.Rating = rating;
                    _selectedPartner.Inn = InnTextBox.Text.Trim();
                    _selectedPartner.Address = AddressTextBox.Text.Trim();
                    _selectedPartner.DirectorName = DirectorNameTextBox.Text.Trim();
                    _selectedPartner.Phone = PhoneTextBox.Text.Trim();
                    _selectedPartner.Email = EmailTextBox.Text.Trim();
                    _selectedPartner.Logo = LogoTextBox.Text.Trim();
                    _selectedPartner.SalesLocations = SalesLocationsTextBox.Text.Trim();
                    _selectedPartner.ModifiedDate = DateTime.Now;

                    await _partnerService.UpdatePartnerAsync(_selectedPartner);
                    MessageBox.Show("Партнер успешно обновлен", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var partner = new Partner
                    {
                        CompanyName = CompanyNameTextBox.Text.Trim(),
                        PartnerTypeId = partnerTypeId,
                        Rating = rating,
                        Inn = InnTextBox.Text.Trim(),
                        Address = AddressTextBox.Text.Trim(),
                        DirectorName = DirectorNameTextBox.Text.Trim(),
                        Phone = PhoneTextBox.Text.Trim(),
                        Email = EmailTextBox.Text.Trim(),
                        Logo = LogoTextBox.Text.Trim(),
                        SalesLocations = SalesLocationsTextBox.Text.Trim(),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    await _partnerService.AddPartnerAsync(partner);
                    MessageBox.Show("Партнер успешно добавлен", "Информация",
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