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

namespace OrderExecutionControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class PageLogin : Page
    {
        public PageLogin()
        {
            InitializeComponent();
        }

        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            var user = OrderExecutionControlEntities.GetContext().employee.ToList().Find(p => p.login == login.Text);
            if (password.Password == "" || login.Text == "")
            {
                MessageBox.Show("Заполните пустые поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (user == null || user.password != password.Password)
                errors.AppendLine("Неверный логин или пароль, попробуйте заново.");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Manager.UserCode = user.code_employee;
            Manager.UserName.Text = user.surname_employee + " " + user.name_employee + " " + user.patronymic_employee + "  —  " + user.position.name_position;
            
            Manager.ButtonExit.Visibility = Visibility.Visible;
            switch (user.position.position_priority)
            {
                case 1:
                    Manager.NameCurrentPage.Text = "Выданные поручения";
                    Manager.Button_Received_Commission.Visibility = Visibility.Collapsed;
                    Manager.Button_Issued_Commission.Visibility = Visibility.Visible;
                    Manager.Button_Employee.Visibility = Visibility.Visible;
                    Manager.MainFrame.Navigate(new Pages.PageCommission(1));
                    break;
                case 10:
                    Manager.NameCurrentPage.Text = "Выданные поручения";
                    Manager.Button_Received_Commission.Visibility = Visibility.Visible;
                    Manager.Button_Issued_Commission.Visibility = Visibility.Visible;
                    Manager.Button_Employee.Visibility = Visibility.Visible;
                    Manager.MainFrame.Navigate(new Pages.PageCommission(1));
                    break;
                case 100:
                    Manager.NameCurrentPage.Text = "Полученные поручения";
                    Manager.Button_Received_Commission.Visibility = Visibility.Visible;
                    Manager.Button_Issued_Commission.Visibility = Visibility.Collapsed;
                    Manager.Button_Employee.Visibility = Visibility.Visible;
                    Manager.MainFrame.Navigate(new Pages.PageCommission(2));
                    break;
                default:
                    break;
            }
        }
    }
}
