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

namespace demoshoes.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private Entities db = new Entities();
        private List<ShoesProduct> products;
        private User currentUser;
        public UserPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            UserFIO.Text = user?.FIO;

            var supplierscombo = db.Suppliers.ToList();
            supplierscombo.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Все поставщики" });
            SupplierFilter.ItemsSource = supplierscombo;
            SupplierFilter.DisplayMemberPath = "SupplierName";
            SupplierFilter.SelectedValuePath = "SupplierID";
            SupplierFilter.SelectedIndex = 0;

            LoadShoesProducts();
        }
        private void LoadShoesProducts()
        {
            products = db.ShoesProducts.Include("Supplier").ToList();
            UpdateShoesProducts();
        }

        private void UpdateShoesProducts()
        {
            if (products == null || !products.Any()) return;
            var query = products.AsEnumerable();
            // Поиск
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchword = SearchBox.Text.ToLower().Split(' ');
                query = query.Where(x =>
                {
                    var text = (x.ProductName + " " + x.Category.CategoryName + " " + x.Description + " " + x.Manufacturer.ManufacturerName + " " + x.Supplier.SupplierName + " " + x.Unit.UnitName).ToLower();
                    return searchword.All(word => text.Contains(word));
                });
            }

            //Фильтрация
            if (SupplierFilter.SelectedIndex > 0)
            {
                int selectedid = (int)SupplierFilter.SelectedValue;
                query = query.Where(x => x.SupplierID == selectedid);
            }

            //Сортировка
            if (SortBox.SelectedIndex == 0)
            {
                query = query.OrderBy(x => x.CountStore);

            }
            else
            {
                query = query.OrderByDescending(x => x.CountStore);
            }
            ShoesProductListView.ItemsSource = query.ToList();
        }


        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateShoesProducts();
        }

        private void SupplierFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShoesProducts();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShoesProducts();
        }
    }
}
