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
using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;


namespace OrderExecutionControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageSubtask.xaml
    /// </summary>
    public partial class PageSubtask : Page
    {
        public int currentType { get; set; }
        private commission curentCommission { get; set; }
        private List<subtask> items { get; set; }
        public PageSubtask(commission selectCommission, int type)
        {
            InitializeComponent();
            currentType = type;
            if (currentType == 1)
                buttonAddSubtusk.Visibility = Visibility.Visible;
            else
                buttonAddSubtusk.Visibility = Visibility.Collapsed;
            if (selectCommission.code_status == 4 || selectCommission.code_status == 5)
                buttonAddSubtusk.Visibility = Visibility.Collapsed;
            curentCommission = selectCommission;
            qwe.Text = currentType.ToString();
            var listStatus = OrderExecutionControlEntities.GetContext().status.ToList();
            listStatus.Insert(0, new status { name_status = "Любой статус" });
            ComboBoxStatus.ItemsSource = listStatus;
            ComboBoxStatus.SelectedIndex = 0;
            ComboBoxTerm.ItemsSource = new string[] { "Любой срок", "Cрок вышел", "Время есть" };
            ComboBoxTerm.SelectedIndex = 0;
        }
        private void ReloadListViewSubtask(commission сurentCommission, int type)
        {
            switch (type)
            {
                case 1:
                    var sourceList = OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(w => w.code_commission == сurentCommission.code_commission);
                    sourceList = sourceList.Where(p => p.name_subtask.ToLower().Contains(TextBoxSearch.Text.ToLower()) || p.text_subtask.ToLower().Contains(TextBoxSearch.Text.ToLower())).ToList();
                    if (ComboBoxStatus.SelectedIndex > 0)
                        sourceList = sourceList.Where(w => w.status.code_status == ComboBoxStatus.SelectedIndex).ToList();
                    if (ComboBoxTerm.SelectedIndex == 1)
                        sourceList = sourceList.Where(w => w.end_date_subtask < DateTime.Now).ToList();
                    if (ComboBoxTerm.SelectedIndex == 2)
                        sourceList = sourceList.Where(w => w.end_date_subtask > DateTime.Now).ToList();
                    items = sourceList;
                    ListViewSubtask.ItemsSource = sourceList;
                    break;
                case 2:
                    var sourceList2 = OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(w => w.code_commission == сurentCommission.code_commission && w.code_employee == Manager.UserCode);
                    sourceList = sourceList2.Where(p => p.name_subtask.ToLower().Contains(TextBoxSearch.Text.ToLower()) || p.text_subtask.ToLower().Contains(TextBoxSearch.Text.ToLower())).ToList();
                    if (ComboBoxStatus.SelectedIndex > 0)
                        sourceList2 = sourceList.Where(w => w.status.code_status == ComboBoxStatus.SelectedIndex).ToList();
                    if (ComboBoxTerm.SelectedIndex == 1)
                        sourceList2 = sourceList.Where(w => w.end_date_subtask < DateTime.Now).ToList();
                    if (ComboBoxTerm.SelectedIndex == 2)
                        sourceList2 = sourceList.Where(w => w.end_date_subtask > DateTime.Now).ToList();
                    items = sourceList;
                    ListViewSubtask.ItemsSource = sourceList2;
                    break;
                default:
                    break;
            }
        }
        private void AcceptSubtask(object sender, RoutedEventArgs e)
        {
            subtask currentSubtask = (sender as Button).DataContext as subtask;
            if ((DateTime.Now - currentSubtask.end_date_subtask).Value.TotalDays < 0)
            {
                currentSubtask.code_status = 4;
                try
                {
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    MessageBox.Show("Выполнено в срок");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                currentSubtask.code_status = 5;
                try
                {
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    MessageBox.Show("Выполнено с опозданием");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            ReloadListViewSubtask(curentCommission, currentType);
        }
        private void ReworkSubtask(object sender, RoutedEventArgs e)
        {
            subtask currentSubtask = (sender as Button).DataContext as subtask;
            currentSubtask.code_status = 3;
            try
            {
                OrderExecutionControlEntities.GetContext().SaveChanges();
                MessageBox.Show("Отправленно на доработку");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            ReloadListViewSubtask(curentCommission, currentType);
        }
        private void EditSubtask(object sender, RoutedEventArgs e)
        {
            var g = ListViewSubtask.ActualWidth;
            var r = (((sender as Button).Parent as StackPanel).Parent as Grid).Parent as Border;
            subtask currentSubtask = (sender as Button).DataContext as subtask;
            Manager.SFrame.Navigate(new PageSubtaskEdit(currentSubtask, curentCommission.code_commission));
        }
        private void CancelSubtask(object sender, RoutedEventArgs e)
        {
            subtask currentSubtask = (sender as Button).DataContext as subtask;
            if (MessageBox.Show($"Вы точно хотите удалить подзадачу \"{currentSubtask.name_subtask}\"?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    OrderExecutionControlEntities.GetContext().subtask.Remove(currentSubtask);
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    MessageBox.Show("Подзадача удалена");
                    ReloadListViewSubtask(curentCommission, currentType);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void ButtonClilkPBack(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void SendSubtask(object sender, RoutedEventArgs e)
        {
            subtask currentSubtask = (sender as Button).DataContext as subtask;
            currentSubtask.code_status = 2;
            try
            {
                OrderExecutionControlEntities.GetContext().SaveChanges();
                MessageBox.Show("Отправленно на проверку");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            ReloadListViewSubtask(curentCommission, currentType);

        }
        private void ButtonClickNewSubtask(object sender, RoutedEventArgs e)
        {
            Manager.SFrame.Navigate(new PageSubtaskEdit(null, curentCommission.code_commission));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadListViewSubtask(curentCommission, currentType);
        }

        private void OpenClose_Click(object sender, RoutedEventArgs e)
        {   
            if (searchfilter.Height == 128)
                searchfilter.Height = 0;
            else
                searchfilter.Height = 128;
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReloadListViewSubtask(curentCommission, currentType);
        }

        private void File_open_Click(object sender, RoutedEventArgs e)
        {
            var attachedfile = ((sender as Button).DataContext as subtask).attached_file;
            if (attachedfile == "")
            {
                MessageBox.Show($"Файл не прикреплен");
                return;
            }
            if (!File.Exists(FilePath.Combine(System.IO.Directory.GetCurrentDirectory() + @"\\Files\\", attachedfile)))
            {
                MessageBox.Show($"Файл {attachedfile} не найден");
                return;
            }
            Process.Start(FilePath.Combine(System.IO.Directory.GetCurrentDirectory() + @"\\Files\\", attachedfile));
        }

        private void page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReloadListViewSubtask(curentCommission, currentType);
        }

        private readonly string TemplateFileName2 = System.IO.Directory.GetCurrentDirectory() + @"\Template2.docx";
        private void Export_Word_Click(object sender, RoutedEventArgs e)
        {
            Word.Application app = new Word.Application();
            app.Visible = false;
            try
            {
                Word.Document doc = app.Documents.Add();
                int i = 1;
                foreach (var item in items)
                {
                    doc.Content.Paragraphs.Add();
                    doc.Content.Paragraphs.Add().Range.InsertFile(TemplateFileName2);
                    ReplaceWordStub("{number}", i.ToString(), doc);
                    ReplaceWordStub("{name_subtask}", item.name_subtask, doc);
                    ReplaceWordStub("{text_subtask}", item.text_subtask, doc);
                    ReplaceWordStub("{name_employee}", item.employee.surname_employee + " " + item.employee.name_employee + " " + item.employee.patronymic_employee, doc);
                    ReplaceWordStub("{name_status}", item.status.name_status, doc);
                    ReplaceWordStub("{date_of_registration_subtask}", ((DateTime)item.date_of_registration_subtask).ToString("yyyy.MM.dd HH:mm"), doc);
                    ReplaceWordStub("{start_date_subtask}", ((DateTime)item.start_date_subtask).ToString("yyyy.MM.dd HH:mm"), doc);
                    ReplaceWordStub("{end_date_subtask}", ((DateTime)item.end_date_subtask).ToString("yyyy.MM.dd HH:mm"), doc);
                    i++;
                }

                doc.SaveAs2(curentCommission.name_commission);
                app.Visible = true;
                app.DocumentBeforeClose += docClose;
                //app.ActiveDocument.Close(SaveChanges: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void docClose(Word.Document Doc, ref bool Cancel)
        {
            (Doc.Parent as Word.Application).Quit();
        }
        private void ReplaceWordStub(string stubToReplace, string text, Word.Document doc)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }
    }
}
