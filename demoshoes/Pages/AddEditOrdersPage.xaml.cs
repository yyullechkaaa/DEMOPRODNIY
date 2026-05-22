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
    /// Логика взаимодействия для AddEditOrdersPage.xaml
    /// </summary>
    public partial class AddEditOrdersPage : Page
    {
        private readonly Order currentOrder;
        private readonly User currentUser;
        private readonly int? editOrderId;
        private Entities db = new Entities();
        public AddEditOrdersPage(Order order, User user)
        {
            InitializeComponent();
            currentUser = user;
            StatusCombo.ItemsSource = db.Statuses.ToList();
            StatusCombo.DisplayMemberPath = "StatusName";
            StatusCombo.SelectedValuePath = "StatusID";
            UserCombo.ItemsSource = db.Users.ToList();
            UserCombo.DisplayMemberPath = "FIO";
            UserCombo.SelectedValuePath = "UserID";
            AddressCombo.ItemsSource = db.Addresses.ToList();
            AddressCombo.DisplayMemberPath = "AddressName";
            AddressCombo.SelectedValuePath = "AddressID";

            if (order != null)
            {
                editOrderId = order.OrderID;
                currentOrder = order;
                LoadInfo();
            }
            else
            {
                currentOrder = new Order();
            }
        }

        private void LoadInfo()
        {
            ArticleOrderBox.Text = currentOrder.ArticleOrder;
            DateOrderBox.SelectedDate = currentOrder.DateOrder;
            DateDeliveryBox.SelectedDate = currentOrder.DateDelivery;
            AddressCombo.SelectedValue = currentOrder.AddressID;
            UserCombo.SelectedValue = currentOrder.UserID;
            StatusCombo.SelectedValue = currentOrder.StatusID;
            CodeBox.Text = currentOrder.Code.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateInputs(out int code);
                Order order;
                if (editOrderId.HasValue)
                {
                    order = db.Orders.Find(editOrderId.Value);
                    if (order == null) throw new Exception("Продукт не найден!");
                }
                else
                {
                    order = new Order();
                    db.Orders.Add(order);
                }

                order.ArticleOrder = ArticleOrderBox.Text;
                order.DateOrder = DateOrderBox.SelectedDate.Value;
                order.DateDelivery = DateDeliveryBox.SelectedDate.Value;
                order.AddressID = (int)AddressCombo.SelectedValue;
                order.UserID = (int)UserCombo.SelectedValue;
                order.StatusID = (int)StatusCombo.SelectedValue;
                order.Code = int.Parse(CodeBox.Text);

                db.SaveChanges();
                MessageBox.Show("Сохранено успешно!", "Успех");
                NavigationService.Navigate(new AdminPage(currentUser));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateInputs(out int code)
        {
            if (string.IsNullOrWhiteSpace(ArticleOrderBox.Text)) throw new Exception("Введите артикул");
            if (string.IsNullOrWhiteSpace(CodeBox.Text)) throw new Exception("Введите код заказа");
            if (DateOrderBox.SelectedDate == null) throw new Exception("Введите дату заказа");
            if (DateDeliveryBox.SelectedDate == null) throw new Exception("Введите дату доставки");
            if (AddressCombo.SelectedValue == null) throw new Exception("Выберите адрес");
            if (UserCombo.SelectedValue == null) throw new Exception("Выберите клиента");
            if (StatusCombo.SelectedValue == null) throw new Exception("Выберите статус");

            code = int.Parse(CodeBox.Text);

            if (code < 0) throw new Exception("Код не может быть отрицательным!");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
