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
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private Entities db = new Entities();
        private List<Order> orders;
        private User currentUser;
        public OrdersPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            UserFIO.Text = user?.FIO;
            LoadOrders();
        }

        private void LoadOrders()
        {
            orders = db.Orders.Include("Address").Include("Status").ToList();
            UpdateOrders();
        }

        private void UpdateOrders()
        {
            if (orders == null || !orders.Any()) return;
            var query = orders.AsEnumerable();
            OrdersListView.ItemsSource = query.ToList();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}
