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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {

        private Entities db = new Entities();
        private List<ShoesProduct> products;
        private User currentUser;
        public AdminPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            UserFIO.Text = user?.FIO;

            //Для фильтрации по поставщику
            var supplierscombo = db.Suppliers.ToList();
            supplierscombo.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Все поставщики" });
            SupplierFilter.ItemsSource = supplierscombo;
            SupplierFilter.DisplayMemberPath = "SupplierName";
            SupplierFilter.SelectedValuePath = "SupplierID";
            SupplierFilter.SelectedIndex = 0;

            //Для фильтрации по диапазонам скидки
            FilterDiscountBox.Items.Insert(0, "Все диапазоны");
            FilterDiscountBox.SelectedIndex = 0;

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
            if(!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchword = SearchBox.Text.ToLower().Split(' ');
                query = query.Where(x =>
                {
                    var text = (x.ProductName + " " + x.Category.CategoryName + " " + x.Description + " " + x.Manufacturer.ManufacturerName + " " + x.Supplier.SupplierName + " " + x.Unit.UnitName).ToLower();
                    return searchword.All(word => text.Contains(word));
                });
            }

            //Фильтрация по поставщику
            if (SupplierFilter.SelectedIndex > 0) { 
            int selectedid = (int)SupplierFilter.SelectedValue;
            query= query.Where(x => x.SupplierID == selectedid);
            }

            //Фильтрация по размеру скидки
            switch (FilterDiscountBox.SelectedIndex)
            {
                case 1: // от 0 до 10,99%
                    query = query.Where(x => x.Discount >= 0 && x.Discount <= 10.99);
                    break;
                case 2: // от 11 до 14,99%
                    query = query.Where(x => x.Discount >= 11 && x.Discount <= 14.99);
                    break;
                case 3: // от 15% и более
                    query = query.Where(x => x.Discount >= 15);
                    break;
            }

            //Сортировка только по количеству
            if (SortBox.SelectedIndex == 0)
            {
                query = query.OrderBy(x => x.CountStore);

            }
            else
            {
                query = query.OrderByDescending(x => x.CountStore);
            }

            //Сортировка и по цене и по количеству
            switch (SortTwoBox.SelectedIndex)
            {
                case 0: // По возрастанию цены
                    query = query.OrderBy(x => x.Price);
                    break;
                case 1: // По убыванию цены
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case 2: // По возрастанию количества на складе
                    query = query.OrderBy(x => x.CountStore);
                    break;
                case 3: // По убыванию количества на складе
                    query = query.OrderByDescending(x => x.CountStore);
                    break;
            }


            ShoesProductListView.ItemsSource = query.ToList();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersAdminPage(currentUser));
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

        private void ShoesProductListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AddEditProductsPage((ShoesProduct)ShoesProductListView.SelectedItem, currentUser));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditProductsPage(null, currentUser));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ShoesProductListView.SelectedItem as ShoesProduct;
            if (selected == null) {
                MessageBox.Show("Выберите продукт для удаления!");
                return;
            }
            if(MessageBox.Show($"Удалить {selected.ProductName}?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.ShoesProducts.Remove(selected);
                db.SaveChanges();
                LoadShoesProducts();
            }
        }

        private void SortTwoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShoesProducts();
        }

        private void FilterDiscountBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShoesProducts();
        }
    }
}
