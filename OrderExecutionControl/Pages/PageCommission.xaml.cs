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
    /// Логика взаимодействия для PageCommission.xaml
    /// </summary>
    public partial class PageCommission : Page
    {
        private int currentType = 0;
        public PageCommission(int type)
        {
            InitializeComponent();

            var listStatus = OrderExecutionControlEntities.GetContext().status.ToList();
            listStatus.Insert(0, new status { name_status = "Любой статус" });
            ComboBoxStatus.ItemsSource = listStatus;
            ComboBoxStatus.SelectedIndex = 0;
            ComboBoxTerm.ItemsSource = new string[] { "Любой срок", "Cрок вышел", "Время есть" };
            ComboBoxTerm.SelectedIndex = 0;
            currentType = type;
            Manager.SFrame = SFrame;
        }
        private void LoadInfoOrder(object sender, RoutedEventArgs e)
        {
            commission com = (sender as Button).DataContext as commission;    
            SFrame.Navigate(new Pages.PageCommissionView(com, currentType));
        }

        private void ButtonAddCommission_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageCommissionEdit(null));
        }
        private void UpdateListComission()
        {
            switch (currentType)
            {
                case 1:
                    var sourceList = OrderExecutionControlEntities.GetContext().commission.ToList();
                    if (OrderExecutionControlEntities.GetContext().employee.ToList().Find(f => f.code_employee == Manager.UserCode).position.position_priority != 1)
                        ListViewCommission.ItemsSource = OrderExecutionControlEntities.GetContext().commission.ToList().FindAll(f => f.employee.code_employee == Manager.UserCode);
                    sourceList = sourceList.Where(p => p.name_commission.ToLower().Contains(TextBoxSearch.Text.ToLower())).ToList();
                    if (ComboBoxStatus.SelectedIndex > 0)
                        sourceList = sourceList.Where(w => w.status.code_status == ComboBoxStatus.SelectedIndex).ToList();
                    if (ComboBoxTerm.SelectedIndex == 1)
                        sourceList = sourceList.Where(w => w.end_date_commission < DateTime.Now).ToList();
                    if (ComboBoxTerm.SelectedIndex == 2)
                        sourceList = sourceList.Where(w => w.end_date_commission > DateTime.Now).ToList();
                    ListViewCommission.ItemsSource = sourceList;
                    break;
                case 2:
                    ButtonAddCommission.Visibility = Visibility.Collapsed;
                    var subtaskList = OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(f => f.code_employee == Manager.UserCode).Select(s => s.code_commission).ToArray();
                    List<commission> sourceList2 = new List<commission>();
                    foreach (var item in subtaskList)
                        sourceList2.AddRange(OrderExecutionControlEntities.GetContext().commission.ToList().FindAll(f => f.code_commission == item).ToList());
                    ListViewCommission.ItemsSource = sourceList2;
                    break;
                default:
                    break;
            }
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && Manager.UserCode != 0)
            {
                UpdateListComission();
            }
        }

        private void OpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (searchfilter.Height == 168)
                searchfilter.Height = 40;
            else
                searchfilter.Height = 168;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListComission();
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListComission();
        }
    }
}
