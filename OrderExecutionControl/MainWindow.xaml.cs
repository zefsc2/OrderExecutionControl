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

namespace OrderExecutionControl
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.UserName = NameUser;
            Manager.NameCurrentPage = NameCurrentPage;
            Manager.ButtonExit = ButtonExit;
            MainFrame.Navigate(new Pages.PageLogin());
            Manager.MainFrame = MainFrame;
            Manager.Button_Employee = Button_Employee;
            Manager.Button_Issued_Commission = Button_Issued_Commission;
            Manager.Button_Received_Commission = Button_Received_Commission;
        }

        private void Button_Click_Issued_Commission(object sender, RoutedEventArgs e)
        {
            if (Manager.UserCode == 0)
            {
                MessageBox.Show("Сначала войдите в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            NameCurrentPage.Text = "Выданные поручения";
            MainFrame.Navigate(new Pages.PageCommission(1));
        }
        private void Button_Click_Received_Commission(object sender, RoutedEventArgs e)
        {
            if (Manager.UserCode == 0)
            {
                MessageBox.Show("Сначала войдите в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            NameCurrentPage.Text = "Полученные поручения";
            MainFrame.Navigate(new Pages.PageCommission(2));
        }

        private void Button_Click_Employee(object sender, RoutedEventArgs e)
        {
            if (Manager.UserCode == 0)
            {
                MessageBox.Show("Сначала войдите в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            NameCurrentPage.Text = "Сотрудники";
            MainFrame.Navigate(new Pages.PageEmployee());
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.PageLogin());
            Manager.UserCode = 0;
            Manager.UserName.Text = null;
            Manager.NameCurrentPage.Text = null;
            Manager.ButtonExit.Visibility = Visibility.Hidden;
        }
    }
}
