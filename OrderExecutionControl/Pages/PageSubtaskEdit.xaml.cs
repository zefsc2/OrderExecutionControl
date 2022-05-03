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
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using FilePath = System.IO.Path;

namespace OrderExecutionControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageEditSubtask.xaml
    /// </summary>
    public partial class PageSubtaskEdit : Page
    {
        private subtask currentSubtask = new subtask();
        private int currentcodeCommission;
        public PageSubtaskEdit(subtask selectSubtask, int selectCodeCommission)
        {
            InitializeComponent();
            currentcodeCommission = selectCodeCommission;
            if (selectSubtask != null)
            { 
                currentSubtask = selectSubtask;
            }
            DataContext = currentSubtask;
            var emp = OrderExecutionControlEntities.GetContext().employee.ToList().Find(f => f.code_employee == Manager.UserCode);
            if (emp.position.position_priority == 1)
            {
                employeeComboBox.ItemsSource = OrderExecutionControlEntities.GetContext().employee.ToList();
            }
            else
            {
                employeeComboBox.ItemsSource = OrderExecutionControlEntities.GetContext().employee.ToList().FindAll(f => f.code_department == emp.code_department);
            }
        }

        private void Cancel_button(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void Save_button(object sender, RoutedEventArgs e)
        {
            DateTime dt;
            StringBuilder errors = new StringBuilder();
            if (name_subtask == null || employeeComboBox.SelectedItem == null || text_subtask == null)
                errors.AppendLine("Заполните все поля");
            else if (!DateTime.TryParse(start_date_subtask.Text, out dt) || !DateTime.TryParse(end_date_subtask.Text, out dt))
                errors.AppendLine("Дата/ы имет неверный формат");
            else if (currentSubtask.start_date_subtask > currentSubtask.end_date_subtask)
                errors.AppendLine("Дата окончания должна быть меньше даты начала");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            currentSubtask.date_of_registration_subtask = DateTime.Now;
            if (currentSubtask.code_subtask == 0)
            {
                if (OrderExecutionControlEntities.GetContext().subtask.Count() == 0)
                {
                    currentSubtask.code_subtask = 1;
                }
                else
                {
                    int d = OrderExecutionControlEntities.GetContext().subtask.Max(p => p.code_subtask) + 1;
                    currentSubtask.code_subtask = d;
                }
                currentSubtask.attached_file = "";
                currentSubtask.code_status = 1;
                currentSubtask.code_commission = currentcodeCommission;
                OrderExecutionControlEntities.GetContext().subtask.Add(currentSubtask);
            }
            try
            {
                OrderExecutionControlEntities.GetContext().SaveChanges();
                MessageBox.Show("Подзадача сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        Microsoft.Win32.OpenFileDialog dl1 = new Microsoft.Win32.OpenFileDialog();
        private void Load_File_button(object sender, RoutedEventArgs e)
        {
            dl1.FileName = "";
            dl1.DefaultExt = ".*";
            Nullable<bool> result = dl1.ShowDialog();
            if (result == true)
            {
                if (File.Exists(FilePath.Combine(System.IO.Directory.GetCurrentDirectory() + @"/Files/", FilePath.GetFileName(dl1.FileName))))
                {
                    MessageBox.Show("Данное имя недоступно");
                    return;
                }
                else
                {
                    File.Copy(dl1.FileName, FilePath.Combine(System.IO.Directory.GetCurrentDirectory() + @"/Files/", FilePath.GetFileName(dl1.FileName)));
                    currentSubtask.attached_file = FilePath.GetFileName(dl1.FileName);
                    attached_file.Text = currentSubtask.attached_file;
                    MessageBox.Show("Файл загружен");
                }
            }
        }
    }
}
