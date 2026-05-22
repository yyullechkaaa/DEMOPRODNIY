using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace demoshoes.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductsPage.xaml
    /// </summary>
    public partial class AddEditProductsPage : Page
    {
        private readonly ShoesProduct currentProduct;
        private readonly User currentUser;
        private string selectedPhotoPath = null;
        private bool isNewPhoto = false;
        private readonly int? editProductId;
        private Entities db = new Entities();
        public AddEditProductsPage(ShoesProduct product, User user)
        {
            InitializeComponent();
            currentUser = user;
            UnitCombo.ItemsSource = db.Units.ToList();
            UnitCombo.DisplayMemberPath = "UnitName";
            UnitCombo.SelectedValuePath = "UnitID";
            SupplierCombo.ItemsSource = db.Suppliers.ToList();
            SupplierCombo.DisplayMemberPath = "SupplierName";
            SupplierCombo.SelectedValuePath = "SupplierID";
            ManufactBox.ItemsSource = db.Manufacturers.ToList();
            ManufactBox.DisplayMemberPath = "ManufacturerName";
            ManufactBox.SelectedValuePath = "ManufacturerID";
            CategoryBox.ItemsSource = db.Categories.ToList();
            CategoryBox.DisplayMemberPath = "CategoryName";
            CategoryBox.SelectedValuePath = "CategoryID";

            if (product != null) {
                editProductId = product.ProductID;
                currentProduct = product;
                LoadInfo();
            }
            else
            {
                currentProduct = new ShoesProduct();
                PhotoProduct.Source = new BitmapImage(new Uri("images/picture.png", UriKind.Relative));
            }
        }

        private void LoadInfo() {
            ArticleBox.Text = currentProduct.Article;
            NameBox.Text = currentProduct.ProductName;
            PhotoProduct.Source = GetImageSource(currentProduct.Photo);
            UnitCombo.SelectedValue = currentProduct.UnitID;
            PriceBox.Text = currentProduct.Price.ToString();
            SupplierCombo.SelectedValue = currentProduct.SupplierID;
            ManufactBox.SelectedValue = currentProduct.ManufacturerID;
            CategoryBox.SelectedValue = currentProduct.CategoryID;
            DiscountBox.Text = currentProduct.Discount.ToString();
            CountBox.Text = currentProduct.CountStore.ToString();
            DescriptionBox.Text = currentProduct.Description;
        }

        private BitmapImage GetImageSource(string photoPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(photoPath))
                {
                    string fileName = System.IO.Path.GetFileName(photoPath.TrimStart('/', '\\').Replace('/','\\'));
                    string[] paths =
                    {
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", fileName),
                        System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "images", fileName))
                    };

                    foreach (var path in paths.Where(File.Exists))
                        return new BitmapImage(new Uri(path, UriKind.Absolute));

                }
            }
            catch { }
            return new BitmapImage(new Uri("images/picture.png", UriKind.Relative));
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Images (*.png; *.jpg)|*.png;*.jpg"
            };
            if (dialog.ShowDialog() == true)
            {
                selectedPhotoPath = dialog.FileName;
                isNewPhoto = true;
                PhotoProduct.Source = new BitmapImage(new Uri(selectedPhotoPath));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateInputs(out int countstore, out int discount, out int price);
                ShoesProduct product;
                if (editProductId.HasValue)
                {
                    product = db.ShoesProducts.Find(editProductId.Value);
                    if (product == null) throw new Exception("Продукт не найден!");
                }
                else
                {
                    product = new ShoesProduct();
                    db.ShoesProducts.Add(product);
                }

                product.Article = ArticleBox.Text;
                product.ProductName = NameBox.Text;
                product.UnitID = (int)UnitCombo.SelectedValue;
                product.Price = int.Parse(PriceBox.Text);
                product.Description = DescriptionBox.Text;
                product.SupplierID = (int)SupplierCombo.SelectedValue;
                product.ManufacturerID = (int)ManufactBox.SelectedValue;
                product.CategoryID = (int)CategoryBox.SelectedValue;
                product.Discount = int.Parse(DiscountBox.Text);
                product.CountStore = int.Parse(CountBox.Text);

                SavePhoto(product);
                db.SaveChanges();
                MessageBox.Show("Сохранено успешно!", "Успех");
                NavigationService.Navigate(new AdminPage(currentUser));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ValidateInputs(out int countstore, out int discount, out int price)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)) throw new Exception("Введите название");
            if (string.IsNullOrWhiteSpace(ArticleBox.Text)) throw new Exception("Введите артикул");
            if (string.IsNullOrWhiteSpace(DescriptionBox.Text)) throw new Exception("Введите описание");
            if (string.IsNullOrWhiteSpace(PriceBox.Text)) throw new Exception("Введите цену");
            if (string.IsNullOrWhiteSpace(DiscountBox.Text)) throw new Exception("Введите скидку");
            if (string.IsNullOrWhiteSpace(CountBox.Text)) throw new Exception("Введите количество на складе");
            if (SupplierCombo.SelectedValue == null) throw new Exception("Выберите поставщика");
            if (ManufactBox.SelectedValue == null) throw new Exception("Выберите производителя");
            if (CategoryBox.SelectedValue == null) throw new Exception("Выберите категорию");
            if (UnitCombo.SelectedValue == null) throw new Exception("Выберите единицу измерения");

            countstore = int.Parse(CountBox.Text);
            discount = int.Parse(DiscountBox.Text);
            price = int.Parse(PriceBox.Text);

            if (price < 0) throw new Exception("Цена не может быть отрицательной!");
            if (discount < 0) throw new Exception("Скидка не может быть отрицательной!");
            if (countstore < 0) throw new Exception("Количество не может быть отрицательным!");
        }

        private void SavePhoto(ShoesProduct product)
        {
            if (!isNewPhoto || selectedPhotoPath == null) return;

            string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            if (editProductId.HasValue && !string.IsNullOrWhiteSpace(product.Photo))
            {
                try
                {
                    string oldFileName = System.IO.Path.GetFileName(product.Photo.TrimStart('/', '\\').Replace('/', '\\'));
                    string oldPath = System.IO.Path.Combine(imagesFolder, oldFileName);
                    if (File.Exists(oldPath))
                        File.Exists(oldPath);
                }
                catch { }
            }

            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(selectedPhotoPath);
            string newPath = System.IO.Path.Combine(imagesFolder, fileName);
            System.IO.File.Copy(selectedPhotoPath, newPath, true);
            product.Photo = fileName;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
