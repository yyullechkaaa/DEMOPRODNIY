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
    /// Логика взаимодействия для GuestPage.xaml
    /// </summary>
    public partial class GuestPage : Page
    {
        private Entities db = new Entities();
        private List<ShoesProduct> products;
        public GuestPage()
        {
            InitializeComponent();

            LoadShoesProducts();
        }

        private void UpdateShoesProducts()
        {
            if (products == null || !products.Any()) return;
            var query = products.AsEnumerable();
            
            ShoesProductListView.ItemsSource = query.ToList();
        }

        private void LoadShoesProducts()
        {
            products = db.ShoesProducts.Include("Supplier").ToList();
            UpdateShoesProducts();
        }
    }
}
