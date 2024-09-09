using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;

namespace CalculationApp
{
    /// <summary>
    /// Логика взаимодействия для EmployeesWindowBuh.xaml
    /// </summary>
    public partial class EmployeesWindowBuh : Window
    {
        CalculationEntities db = new CalculationEntities();
        Users user = new Users();
        AssignedTasks assignedTasks = new AssignedTasks();
        List<Users> ListUsers = new List<Users>();
        List<AssignedTasks> tasks;
        public EmployeesWindowBuh()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            LvEmployees.ItemsSource = db.Users.OrderBy(rec => rec.Surname).ToList();
            cmbRole.ItemsSource = db.Roles.Where(rec => rec.IdRole == 2 || rec.IdRole == 3).ToList();
            cmbCategory.ItemsSource = db.CategoriesOfEmployees.ToList();

            cmbSortRole.Items.Add("Все должности");
            string[] roles = db.Users.Select(p => p.Roles.Name).Distinct().ToArray();
            foreach (string role in roles)
            {
                cmbSortRole.Items.Add(role);
            }

            cmbSortCategory.Items.Add("Все категории");
            string[] categories = db.Users.Select(p => p.CategoriesOfEmployees.Name).Distinct().ToArray();
            foreach (string category in categories)
            {
                cmbSortCategory.Items.Add(category);
            }


            LvTasks.ItemsSource = db.AssignedTasks.ToList();
            cmbEmployeeZP.ItemsSource = db.Users.Where(rec=>rec.IdRole == 2).ToList();
        }
        public void SortFiltrPoisk()
        {
            ListUsers = db.Users.ToList();
            if (cmbSortRole.SelectedIndex > 0)
            {
                ListUsers = ListUsers.Where(n => n.Roles.Name == cmbSortRole.SelectedItem.ToString()).ToList();
            }
            if (cmbSortCategory.SelectedIndex > 0)
            {
                var pr = db.Users.FirstOrDefault(n => n.CategoriesOfEmployees.Name == cmbSortCategory.SelectedItem.ToString() && n.Roles.Name == cmbSortRole.SelectedItem.ToString());
                if (pr != null)
                {
                    ListUsers = ListUsers.Where(n => n.CategoriesOfEmployees.Name == cmbSortCategory.SelectedItem.ToString()).ToList();
                }
                else
                {
                    cmbSortCategory.SelectedIndex = -1;
                }
            }
            LvEmployees.ItemsSource = ListUsers;
            LvEmployees.VerticalAlignment = VerticalAlignment.Top;
            LvEmployees.Height = 894;
            LvTasks.Visibility = Visibility.Collapsed;
            TxtTasks.Visibility = Visibility.Collapsed;
        }

        private void BtnLichnKabinet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LichnKabinet lichnKabinet = new LichnKabinet();
            lichnKabinet.Show();
        }

        private void BtnVisibleSt_Click(object sender, RoutedEventArgs e)
        {
            if (StRegistr.Visibility == Visibility.Collapsed)
            {
                StRegistr.Visibility = Visibility.Visible;
                LvEmployees.Visibility = Visibility.Collapsed;
                LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                LvEmployees.Height = 894;
                StPhoto.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                StSortDelete.Visibility = Visibility.Collapsed;
                VisibleDatasZP.Visibility = Visibility.Collapsed;
                LvTasksZP.Visibility = Visibility.Collapsed;
                StZP.Visibility = Visibility.Collapsed;
                StZP2.Visibility = Visibility.Collapsed;
            }
        }

        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRole.SelectedItem != null)
            {
                var selectedItem = cmbRole.SelectedItem as Roles;
                if (selectedItem.IdRole == 2)
                {
                    StCmb2.Visibility = Visibility.Visible;
                }
                else
                {
                    StCmb2.Visibility = Visibility.Collapsed;
                }
            }
        }
        private bool CheckPassword()
        {
            string pass1 = txtPassword.Text;
            string pass11 = PbPassword.Password;
            if (pass1.Length >= 5 && pass1.Length <= 50 || pass11.Length >= 5 && pass11.Length <= 50) return true;
            else
            {
                MessageBox.Show("Пароль должен содержать минимум 5 символов  и максимум 50!");
                return false;
            }
        }
        private bool CheckPhone()
        {
            string phone = txtPhone.Text;
            Regex phoneRegex = new Regex(@"^8[0-9]{10}");
            if (phoneRegex.IsMatch(phone)) return true;
            else
            {
                MessageBox.Show("Неверный формат телефона");
                return false;
            }
        }
        private bool CheckDatas()
        {
            string surname = txtSurname.Text;
            string name = txtName.Text;
            string patronymic = txtPatronymic.Text;
            string email = txtEmail.Text;
            if (surname != null && name != null && patronymic != null && email != null && cmbRole.SelectedItem != null) return true;
            else
            {
                MessageBox.Show("Введите все необходимые данные");
                return false;
            }
        }
        private void ClearUserInput()
        {
            txtSurname.Text = "";
            txtName.Text = "";
            txtPatronymic.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            cmbRole.Text = "";
            cmbCategory.Text = "";
            txtPassword.Text = "";
            PbPassword.Password = "";
        }

        private void BtnRegistr_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPassword() == true && CheckPhone() == true && CheckDatas() == true)
            {
                if (cmbCategory.SelectedValue == null)
                {
                    user.IdUser = db.Users.OrderByDescending(rec => rec.IdUser).FirstOrDefault().IdUser + 1;
                    user.Surname = txtSurname.Text;
                    user.Name = txtName.Text;
                    user.Patronymic = txtPatronymic.Text;
                    user.IdRole = (int)cmbRole.SelectedValue;
                    user.Phone = txtPhone.Text;
                    user.Email = txtEmail.Text;
                    user.IdCategory = null;
                    user.Password = PbPassword.Password;
                    db.Users.Add(user);
                    db.SaveChanges();
                    MessageBox.Show("Вы успешно зарегистрировали!");
                    LvEmployees.ItemsSource = db.Users.ToList();
                    ClearUserInput();
                }
                else
                {
                    user.IdUser = db.Users.OrderByDescending(rec => rec.IdUser).FirstOrDefault().IdUser + 1;
                    user.Surname = txtSurname.Text;
                    user.Name = txtName.Text;
                    user.Patronymic = txtPatronymic.Text;
                    user.IdRole = (int)cmbRole.SelectedValue;
                    user.Phone = txtPhone.Text;
                    user.Email = txtEmail.Text;
                    user.IdCategory = (int)cmbCategory.SelectedValue;
                    user.Password = PbPassword.Password;
                    db.Users.Add(user);
                    db.SaveChanges();
                    MessageBox.Show("Вы успешно зарегистрировали!");
                    LvEmployees.ItemsSource = db.Users.ToList();
                    ClearUserInput();
                }
            }
        }

        private void cmbSortRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortFiltrPoisk();
        }

        private void cmbSortCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSortCategory.SelectedIndex == -1) return;
            SortFiltrPoisk();
        }

        private void BtnDeleteEmployees_Click(object sender, RoutedEventArgs e)
        {
            var emplo = (Users)LvEmployees.SelectedItem;
            if (emplo == null)
            {
                MessageBox.Show("Сотрудник не выбран!");
            }
            else
            {
                MessageBoxResult f = MessageBox.Show("Вы действительно хотите удалить сотрудника?", "Удаление", MessageBoxButton.YesNo);
                if (f == MessageBoxResult.Yes)
                {
                    db.Users.Remove(emplo);
                    if (assignedTasks.IdUser == emplo.IdUser)
                    {
                        db.AssignedTasks.Remove(assignedTasks);
                    }
                    db.SaveChanges();
                    LvEmployees.ItemsSource = db.Users.ToList();
                    LvTasks.ItemsSource = db.AssignedTasks.ToList();
                    LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                    LvEmployees.Height = 894;
                    LvTasks.Visibility = Visibility.Collapsed;
                    TxtTasks.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnVisibleList_Click(object sender, RoutedEventArgs e)
        {
            if (LvEmployees.Visibility == Visibility.Collapsed)
            {
                StRegistr.Visibility = Visibility.Collapsed;
                LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                LvEmployees.Height = 894;
                StPhoto.Visibility = Visibility.Collapsed;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                LvEmployees.Visibility = Visibility.Visible;
                StSortDelete.Visibility = Visibility.Visible;
                VisibleDatasZP.Visibility = Visibility.Collapsed;
                LvTasksZP.Visibility = Visibility.Collapsed;
                StZP.Visibility = Visibility.Collapsed;
                StZP2.Visibility = Visibility.Collapsed;
            }
        }

        private void LvEmployees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var merActiv = LvEmployees.SelectedItem as Users;
            if (merActiv == null)
            {
                MessageBox.Show("Сотрудник не выбран!");
            }
            else
            {
                LvEmployees.Height = 460;
                LvEmployees.VerticalAlignment = VerticalAlignment.Top;

                var istActiv = db.AssignedTasks.Where(rec => rec.IdUser == merActiv.IdUser).ToList();
                LvTasks.ItemsSource = istActiv;

                TxtTasks.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Visible;
            }
        }

        private void PhotoPrikrep_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                Photos.Source = new BitmapImage(new Uri(fileDialog.FileName));

                user.Photo = File.ReadAllBytes(fileDialog.FileName);
            }
            db.SaveChanges();

        }

        private void BtnPasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Visibility == Visibility.Visible)
            {
                PbPassword.Password = txtPassword.Text;
                txtPassword.Visibility = Visibility.Collapsed;
                PbPassword.Visibility = Visibility.Visible;
                ImagePassword.Source = new BitmapImage(new Uri("/Images/free-icon-hide-2767146.png", UriKind.Relative));
            }
            else if (txtPassword.Visibility == Visibility.Collapsed)
            {
                txtPassword.Text = PbPassword.Password;
                txtPassword.Visibility = Visibility.Visible;
                PbPassword.Visibility = Visibility.Collapsed;
                ImagePassword.Source = new BitmapImage(new Uri("/Images/free-icon-eye-158746.png", UriKind.Relative));
            }
        }

        private void cmbEmployeeZP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployeeZP.SelectedIndex!= -1)
            {
                DateTime date = DateTime.Now;
                int month = date.Month;

                int cont = (int)cmbEmployeeZP.SelectedValue;
                var UserDatas = db.Users.Where(rec => rec.IdUser == cont).ToList();
                StDatasZP.DataContext = UserDatas;
                StZP2.DataContext = UserDatas;

                LvTasksZP.Visibility = Visibility.Visible;
                LvTasksZP.ItemsSource = db.AssignedTasks.Where(rec => rec.IdUser == cont && rec.IdStatus == 1 || rec.IdUser == cont && rec.IdStatus == 3).OrderByDescending(rec=>rec.AppointmentDate).ToList();
                BtnRaschetZP.Visibility = Visibility.Visible;
                BtRL.Visibility = Visibility.Collapsed;
                StSalary.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnRaschitZp_Click(object sender, RoutedEventArgs e)
        {
            if (StZP.Visibility == Visibility.Collapsed && StZP2.Visibility == Visibility.Collapsed)
            {
                StZP.Visibility = Visibility.Visible;
                StZP2.Visibility = Visibility.Visible;
                VisibleDatasZP.Visibility = Visibility.Visible;
                StRegistr.Visibility = Visibility.Collapsed;
                LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                LvEmployees.Height = 894;
                StPhoto.Visibility = Visibility.Collapsed;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                LvEmployees.Visibility = Visibility.Collapsed;
                StSortDelete.Visibility = Visibility.Collapsed;
                cmbEmployeeZP.SelectedIndex = -1;
            }
        }

        private void BtnRaschetZP_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            int month = date.Month;

            int cont = (int)cmbEmployeeZP.SelectedValue;

            var UserSalary = db.Users.Where(rec => rec.IdUser == cont).FirstOrDefault();

            var usertasks = db.AssignedTasks.Where(rec => rec.IdUser == cont && rec.IdStatus == 1 && rec.AppointmentDate.Month == month).ToList();
            decimal costtasks = usertasks.Sum(rec => rec.CostTask);

            decimal? salary = (costtasks + UserSalary.CategoriesOfEmployees.MinSalary) - ((costtasks + UserSalary.CategoriesOfEmployees.MinSalary) * 13 / 100);

            TxtSalary.Text = $"= {salary:N2}";
            StSalary.Visibility = Visibility.Visible;
            BtRL.Visibility = Visibility.Visible;
        }

        private void BtnRashetList_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            int month = date.Month;


            tasks = db.AssignedTasks.Where(rec => rec.IdUser == (int)cmbEmployeeZP.SelectedValue && rec.IdStatus == 1 && rec.AppointmentDate.Month == month).ToList();

            Word.Application wordApp = new Word.Application();
            wordApp.Visible = true;

            var surname = cmbEmployeeZP.SelectedValue;

            // Создаем новый документ
            Word.Document doc = wordApp.Documents.Add();

            // Заголовок документа
            Word.Paragraph header = doc.Paragraphs.Add();
            header.Range.Text = "Общество с ограниченной ответственностью «АртКлён»";
            header.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            header.Range.Font.Name = "Times New Roman";
            header.Range.Font.Size = 14;
            header.Range.InsertParagraphAfter();
            header.Range.InsertParagraphAfter();

            // Заголовок отчета
            Word.Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = $"Расчётный лист заработной платы сотрудника";
            title.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.Font.Name = "Times New Roman";
            title.Range.Font.Size = 16;
            title.Range.InsertParagraphAfter();
            title.Range.InsertParagraphAfter();

            Word.Paragraph employee = doc.Paragraphs.Add();
            employee.Range.Text = $"Сотрудник: {cmbEmployeeZP.Text} {TxtNameEmployeeZP.Text} {TxtPatronymicEmployeeZP.Text}.";
            employee.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            employee.Range.Font.Name = "Times New Roman";
            employee.Range.Font.Size = 14;
            employee.Range.InsertParagraphAfter();

            Word.Paragraph employeecategory = doc.Paragraphs.Add();
            employeecategory.Range.Text = $"{TxtCategoryEmployeeZP.Text}.";
            employeecategory.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            employeecategory.Range.Font.Name = "Times New Roman";
            employeecategory.Range.Font.Size = 14;
            employeecategory.Range.InsertParagraphAfter();

            Word.Paragraph employeecategorysimilar = doc.Paragraphs.Add();
            employeecategorysimilar.Range.Text = $"{TxtSalaryEmployeeZP.Text}.";
            employeecategorysimilar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            employeecategorysimilar.Range.Font.Name = "Times New Roman";
            employeecategorysimilar.Range.Font.Size = 14;
            employeecategorysimilar.Range.InsertParagraphAfter(); 

            Word.Paragraph NDFL = doc.Paragraphs.Add();
            NDFL.Range.Text = $"Налог на доходы физических лиц = 13%.";
            NDFL.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            NDFL.Range.Font.Name = "Times New Roman";
            NDFL.Range.Font.Size = 14;
            NDFL.Range.InsertParagraphAfter();
            NDFL.Range.InsertParagraphAfter();

            Word.Paragraph Salary = doc.Paragraphs.Add();
            Salary.Range.Text = $"Заработная плата за данный месяц с учётом выполненных задач и вычета налога составляет: {TxtSalary.Text}";
            Salary.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            Salary.Range.Font.Name = "Times New Roman";
            Salary.Range.Font.Size = 14;
            Salary.Range.InsertParagraphAfter();
            Salary.Range.InsertParagraphAfter();


            Word.Paragraph TableName = doc.Paragraphs.Add();
            TableName.Range.Text = $"Таблица 1 - Содержание выполненных задач за месяц.";
            TableName.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            TableName.Range.Font.Name = "Times New Roman";
            TableName.Range.Font.Size = 14;
            TableName.Range.InsertParagraphAfter();

            // Добавляем таблицу
            Word.Table table = doc.Tables.Add(title.Range, 1, 4);
            table.Borders.Enable = 1;
            table.Cell(1, 1).Range.Text = "Наименование задачи";
            table.Cell(1, 2).Range.Text = "Дата назначения";
            table.Cell(1, 3).Range.Text = "Дедлайн";
            table.Cell(1, 4).Range.Text = "Отчисление от задачи";
            table.Range.Font.Name = "Times New Roman";
            table.Range.Font.Size = 14;


            // Заполняем таблицу
            foreach (var task in tasks)
            {
                Word.Row row = table.Rows.Add();
                row.Cells[1].Range.Text = task.Tasks.Name;
                row.Cells[2].Range.Text = task.AppointmentDate.ToString("dd.MM.yyyy");
                row.Cells[3].Range.Text = task.DeadlineDate.ToString();
                row.Cells[4].Range.Text = task.CostTask.ToString();
                row.Range.Font.Name = "Times New Roman";
                row.Range.Font.Size = 14;
            }

            

            // Дата внизу страницы
            Word.Paragraph footer = doc.Paragraphs.Add();
            footer.Range.Text = $"Дата: {DateTime.Now.ToString("dd.MM.yyyy")}";
            footer.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            footer.Range.Font.Name = "Times New Roman";
            footer.Range.Font.Size = 14;
            footer.Range.InsertParagraphAfter();
        }
    }
}
