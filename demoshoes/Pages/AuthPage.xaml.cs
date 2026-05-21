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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void SingInButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginBox.Text) || string.IsNullOrEmpty(PassBox.Password))
            {
                MessageBox.Show("Введите логин или пароль");
                return;
            }

            var user = Entities.GetContext().Users.AsNoTracking().FirstOrDefault(u => u.Login == LoginBox.Text && u.Password == PassBox.Password);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден");
            }

            switch (user.RoleID)
            {
                case 1:
                    NavigationService.Navigate(new AdminPage(user));
                    break;
                case 2:
                    NavigationService.Navigate(new ManagerPage(user));
                    break;
                case 3:
                    NavigationService.Navigate(new UserPage(user));
                    break;
                default:
                    MessageBox.Show("Неизвестная роль пользователя");
                    break;
            }

        }

        private void SingInGuestButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GuestPage());
        }
    }
}
