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
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Reflection;
using FilePath = System.IO.Path;
using System.Diagnostics;

namespace OrderExecutionControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewCommission.xaml
    /// </summary>
    public partial class PageCommissionView : Page
    {
        private commission currentCommission = new commission();
        private int currentType = 0;
        public PageCommissionView(commission selectCommission, int type)
        {
            InitializeComponent();
            currentCommission = selectCommission;
            currentType = type;
            LoadPage();
        }
        private void LoadPage()
        {
            commission order = OrderExecutionControlEntities.GetContext().commission.ToList().Find(x => x.code_commission == currentCommission.code_commission);
            name_commission.Text = order.name_commission.ToString();
            code_employee.Text = order.employee.surname_employee.ToString() + " " + order.employee.name_employee.ToString() + " " + order.employee.patronymic_employee.ToString();
            text_commission.Text = order.text_commission.ToString();
            code_status.Text = order.status.name_status.ToString();
            date_of_registration_commssion.Text = ((DateTime)order.date_of_registration_commssion).ToString("D");
            start_date_commission.Text = ((DateTime)order.start_date_commission).ToString("D");
            end_date_commission.Text = ((DateTime)order.end_date_commission).ToString("D");
            attached_file.Text = order.attached_file.ToString();
        }
        private void Button_Click_View_Subtask(object sender, RoutedEventArgs e)
        {
            Manager.SFrame.Navigate(new Pages.PageSubtask(currentCommission, currentType));
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите удалить поручение \"{currentCommission.name_commission}\"?\nНе завершенных подзадач: {OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(f => f.code_commission == currentCommission.code_commission && (f.code_status != 4 || f.code_status != 5)).Count}", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    OrderExecutionControlEntities.GetContext().subtask.RemoveRange(OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(f => f.code_commission == currentCommission.code_commission));
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    OrderExecutionControlEntities.GetContext().commission.Remove(currentCommission);
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    MessageBox.Show("Задача удалена");
                    Manager.MainFrame.Navigate(new Pages.PageCommission(1));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private readonly string TemplateFileName = System.IO.Directory.GetCurrentDirectory() + @"\Template.docx";
        private readonly string TemplateFileName2 = System.IO.Directory.GetCurrentDirectory() + @"\Template2.docx";
        private void Button_Click_Export(object sender, RoutedEventArgs e)
        {
            Word.Application app = new Word.Application();
            app.Visible = false;
            try
            {
                Word.Document doc = app.Documents.Open(TemplateFileName);
                ReplaceWordStub("{name_commission}", currentCommission.name_commission, doc);
                ReplaceWordStub("{text_commission}", currentCommission.text_commission, doc);
                ReplaceWordStub("{name_employee}", currentCommission.employee.surname_employee + " " + currentCommission.employee.name_employee + " " + currentCommission.employee.patronymic_employee, doc);
                ReplaceWordStub("{name_status}", currentCommission.status.name_status, doc);
                ReplaceWordStub("{date_of_registration_commssion}", ((DateTime)currentCommission.date_of_registration_commssion).ToString("yyyy.MM.dd HH:mm"), doc);
                ReplaceWordStub("{start_date_commission}", ((DateTime)currentCommission.start_date_commission).ToString("yyyy.MM.dd HH:mm"), doc);
                ReplaceWordStub("{end_date_commission}", ((DateTime)currentCommission.end_date_commission).ToString("yyyy.MM.dd HH:mm"), doc);
                int i = 1;
                foreach (var item in OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(f => f.code_commission == currentCommission.code_commission).ToList())
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

                doc.SaveAs2(currentCommission.name_commission);
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

        private void Button_Click_Accept_Commission(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите принять поручение \"{currentCommission.name_commission}\"?\nНе завершенных подзадач: {OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(f => f.code_commission == currentCommission.code_commission && (f.code_status != 4 || f.code_status != 5)).Count}\nВсе подзадачи автоматически завершатся", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var item in OrderExecutionControlEntities.GetContext().subtask.ToList().FindAll(f => f.code_commission == currentCommission.code_commission && (f.code_status != 4 || f.code_status != 5)))
                    {

                        if ((DateTime.Now - item.end_date_subtask).Value.TotalDays < 0)
                            item.code_status = 4;
                        else
                            item.code_status = 5;
                        OrderExecutionControlEntities.GetContext().subtask.Add(item);
                        OrderExecutionControlEntities.GetContext().SaveChanges();
                    }
                    if ((DateTime.Now - currentCommission.end_date_commission).Value.TotalDays < 0)
                        currentCommission.code_status = 4;
                    else
                        currentCommission.code_status = 5;
                    OrderExecutionControlEntities.GetContext().commission.Add(currentCommission);
                    OrderExecutionControlEntities.GetContext().SaveChanges();
                    MessageBox.Show("Подзадача сохранена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void File_open_Click(object sender, RoutedEventArgs e)
        {

            var attachedfile = currentCommission.attached_file;
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
    }
}
