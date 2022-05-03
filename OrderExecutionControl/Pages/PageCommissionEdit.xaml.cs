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
using System.IO;
using System.Reflection;
using FilePath = System.IO.Path;

namespace OrderExecutionControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageEditComission.xaml
    /// </summary>
    public partial class PageCommissionEdit : Page
    {
        private commission currentCommission = new commission();
        public PageCommissionEdit(commission selectCommission)
        {
            InitializeComponent();

            if (selectCommission != null)
            {
                currentCommission = selectCommission;
                
            }
            DataContext = currentCommission;

        }
        
        private void Save_button(object sender, RoutedEventArgs e)
        {
            DateTime dt;
            StringBuilder errors = new StringBuilder();
            if (name_commission == null || text_commission == null)
                errors.AppendLine("Заполните все поля");
            
            else if (!DateTime.TryParse(start_date_commission.Text, out dt) || !DateTime.TryParse(end_date_commission.Text, out dt))
                errors.AppendLine("Дата/ы имет неверный формат");

            else if (currentCommission.start_date_commission > currentCommission.end_date_commission)
                errors.AppendLine("Дата окончания должна быть меньше даты начала");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            currentCommission.date_of_registration_commssion = DateTime.Now;
            if (currentCommission.code_commission == 0)
            {
                if (OrderExecutionControlEntities.GetContext().commission.Find() == null)
                {
                    currentCommission.code_commission = 1;
                }
                else
                {
                    int d = OrderExecutionControlEntities.GetContext().commission.Max(p => p.code_commission) + 1;
                    currentCommission.code_commission = d;
                }
                currentCommission.attached_file = "";
                currentCommission.code_status = 1;
                currentCommission.code_employee = Manager.UserCode;
                OrderExecutionControlEntities.GetContext().commission.Add(currentCommission);
            }
            try
            {
                OrderExecutionControlEntities.GetContext().SaveChanges();
                MessageBox.Show("Поручение сохранено");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Cancel_button(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
        Microsoft.Win32.OpenFileDialog dl1 = new Microsoft.Win32.OpenFileDialog();
        private void Load_file_button(object sender, RoutedEventArgs e)
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
                    currentCommission.attached_file = FilePath.GetFileName(dl1.FileName);
                    attached_file.Text = currentCommission.attached_file;
                    MessageBox.Show("Файл загружен");
                }
            }
        }
    }
}
