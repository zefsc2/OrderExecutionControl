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
using System.Text.RegularExpressions;

namespace OrderExecutionControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeeEdit.xaml
    /// </summary>
    
    public partial class PageEmployeeEdit : Page
    {
        private string oldLogin = "";
        private employee currentEmployee = new employee();
        public PageEmployeeEdit(employee selectEmployee)
        {
            InitializeComponent();
            if (selectEmployee != null)
            {
                currentEmployee = selectEmployee;
                oldLogin = currentEmployee.login;
            }
            DataContext = currentEmployee;
            ComboDepartment.ItemsSource = OrderExecutionControlEntities.GetContext().department.ToList();
            ComboPosition.ItemsSource = OrderExecutionControlEntities.GetContext().position.ToList();
        }

        private void Cancel_button(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void Save_button(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (surname_employee == null || name_employee == null || patronymic_employee == null 
                || telephone_employee == null || ComboDepartment.SelectedItem == null || ComboPosition.SelectedItem == null
                || login == null || password == null)
                errors.AppendLine("Заполните все поля");
            else
            {
                if (!Regex.IsMatch(telephone_employee.Text, @"\d{11}"))
                    errors.AppendLine("Телефон имеет не верный формат\n(длина телефона должна быть равна 11 символам)\n ");
                ////if (OrderExecutionControlEntities.GetContext().employee.ToList().Find(f => f.login == login.Text && f.login != oldLogin) != null)
                ////    errors.AppendLine("Пользовать с таким логином уже существует\n");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (currentEmployee.code_employee == 0)
            {
                if (OrderExecutionControlEntities.GetContext().employee.Find() == null)
                {
                    currentEmployee.code_employee = 1;
                }
                else
                {
                    int d = OrderExecutionControlEntities.GetContext().employee.Max(p => p.code_employee) + 1;
                    currentEmployee.code_employee = d;
                }
                
                OrderExecutionControlEntities.GetContext().employee.Add(currentEmployee);
            }
            try
            {
                OrderExecutionControlEntities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
