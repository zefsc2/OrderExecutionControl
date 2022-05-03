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
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class PageEmployee : Page
    {
        public PageEmployee()
        {
            InitializeComponent();
            if (OrderExecutionControlEntities.GetContext().employee.ToList().Find(f => f.code_employee == Manager.UserCode).code_position != 1)
            {
                AddEmployee.Visibility = Visibility.Collapsed;
                DeleteEmployee.Visibility = Visibility.Collapsed;
                columnEdit.Visibility = Visibility.Collapsed;
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageEmployeeEdit(null));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            employee emp = (sender as Button).DataContext as employee;
            Manager.MainFrame.Navigate(new PageEmployeeEdit(emp));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && Manager.UserCode != 0)
            {
                UpdateEmployee();
            }
        }

        private void UpdateEmployee()
        {
            OrderExecutionControlEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            var emp = OrderExecutionControlEntities.GetContext().employee.ToList();
            int d = OrderExecutionControlEntities.GetContext().employee.ToList().Find(x => x.code_employee == Manager.UserCode).code_department;
            for (int i = 0; i < emp.Count; i++)
            {
                if (Manager.UserCode != 1 && (emp[i].code_department != 1 && emp[i].code_department != d))
                {
                    emp.Remove(emp[i]);
                    i--;
                }
            }
            emp = emp.Where(p => p.surname_employee.ToLower().Contains(TextBoxSearch.Text.ToLower()) || p.name_employee.ToLower().Contains(TextBoxSearch.Text.ToLower()) || p.patronymic_employee.ToLower().Contains(TextBoxSearch.Text.ToLower())).ToList();
            EmployeeGrid.ItemsSource = emp;
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            var deleteEmployee = EmployeeGrid.SelectedItems.Cast<employee>().ToList();
            if (deleteEmployee.Find(f => f.code_employee == Manager.UserCode) != null)
            {
                MessageBox.Show("Нельзя удалить текущего пользователя", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Вы точно хотите удалить следующие {deleteEmployee.Count()} элемента(ов)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    OrderExecutionControlEntities.GetContext().employee.RemoveRange(deleteEmployee);
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены");
                    EmployeeGrid.ItemsSource = OrderExecutionControlEntities.GetContext().employee.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEmployee();
        }
    }
}
