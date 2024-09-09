using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
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

namespace CalculationApp
{
    /// <summary>
    /// Логика взаимодействия для EmployeesWindow.xaml
    /// </summary>
    public partial class EmployeesWindow : Window
    {
        CalculationEntities db = new CalculationEntities();
        Users user = new Users();
        AssignedTasks assignedTasks = new AssignedTasks();
        List<Users> ListUsers = new List<Users>();
        public EmployeesWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            LvEmployees.ItemsSource = db.Users.OrderBy(rec=>rec.Surname).ToList();

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

            cmbEmployee.ItemsSource = db.Users.Where(rec=>rec.IdRole == 2).OrderBy(rec=>rec.Surname).ToList();
            cmbOrder.ItemsSource = db.Projects.ToList();

            LvTasks.ItemsSource = db.AssignedTasks.OrderByDescending(rec=>rec.AppointmentDate).ToList();

        }
        public void SortFiltrPoisk()
        {
            ListUsers = db.Users.ToList();
            if (cmbSortRole.SelectedIndex>0)
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
            StDeleteTask.Visibility = Visibility.Collapsed;
        }

        private void BtnLichnKabinet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LichnKabinet lichnKabinet = new LichnKabinet();
            lichnKabinet.Show();
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

        private void BtnVisibleList_Click(object sender, RoutedEventArgs e)
        {
            if (LvEmployees.Visibility == Visibility.Collapsed)
            {
                LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                LvEmployees.Height = 894;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                StNaznach.Visibility = Visibility.Collapsed;
                LvEmployees.Visibility = Visibility.Visible;
                StSortDelete.Visibility = Visibility.Visible;
            }
        }

        private void cmbEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployee.SelectedItem == null)
            {
                StOrder.Visibility = Visibility.Collapsed;
            }
            else
            {
                StOrder.Visibility = Visibility.Visible;
            }
        }

        private void cmbOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbOrder.SelectedItem == null)
            {
                StTask.Visibility = Visibility.Collapsed;
            }
            else
            {
                StTask.Visibility = Visibility.Visible;
                var emplo = cmbEmployee.SelectedItem as Users;
                int? idcategoryemplo = emplo.IdCategory;
                int cont = (int)cmbOrder.SelectedValue;
                cmbTask.ItemsSource = db.AssignedTasks.Where(cc => cc.IdProject == cont && cc.Tasks.Difficulties.IdCategory == idcategoryemplo && cc.IdUser == null).ToList();
            }
        }

        private void cmbTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbTask.SelectedItem == null)
            {
                StDates.Visibility = Visibility.Collapsed;
            }
            else
            {
                StDates.Visibility = Visibility.Visible;

                DateTime currentTime = DateTime.Today;
                TxtAppointmentDate.Text = currentTime.ToShortDateString();
                var task = (AssignedTasks)cmbTask.SelectedItem;

                DateTime dates = currentTime.AddDays(task.Tasks.Time + 1);
                TxtDeadlineDate.Text = $"{dates.ToShortDateString()}";
            }
        }

        private void BtnVisibleAssigned_Click(object sender, RoutedEventArgs e)
        {
            if (StNaznach.Visibility == Visibility.Collapsed)
            {
                StNaznach.Visibility = Visibility.Visible;
                LvEmployees.Visibility = Visibility.Collapsed;
                LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                LvEmployees.Height = 894;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                StSortDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void btnZapis_Click(object sender, RoutedEventArgs e)
        {
            var emplo = (Users)cmbEmployee.SelectedItem;
            var order = (Projects)cmbOrder.SelectedItem;
            var task = (AssignedTasks)cmbTask.SelectedItem;
            var selecttask = task.IdTask;
            var selectorder = order.IdProject;
            if (cmbEmployee.SelectedItem != null && cmbOrder.SelectedItem != null && cmbTask.SelectedItem != null)
            {
                var dobav = db.AssignedTasks.Where(rec => rec.IdTask == selecttask && rec.IdProject == selectorder).FirstOrDefault();
                dobav.IdUser = (int)cmbEmployee.SelectedValue;
                dobav.AppointmentDate = DateTime.Parse(TxtAppointmentDate.Text);
                dobav.DeadlineDate = DateTime.Parse(TxtDeadlineDate.Text);
                dobav.IdStatus = 2;
                db.SaveChanges();
                MessageBox.Show($"Вы успешно назначили задачу: «{cmbTask.Text}». Сотрудник: {cmbEmployee.Text}");
                cmbEmployee.Text = "";
                cmbOrder.Text = "";
                cmbTask.Text = "";
                TxtAppointmentDate.Text = "";
                TxtDeadlineDate.Text = "";
                StOrder.Visibility = Visibility.Collapsed;
                StTask.Visibility = Visibility.Collapsed;
                StDates.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Выберите необходимые данные!");
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
                StDeleteTask.Visibility = Visibility.Visible;

                var istActiv = db.AssignedTasks.Where(rec => rec.IdUser == merActiv.IdUser).OrderByDescending(rec => rec.AppointmentDate).ToList();
                LvTasks.ItemsSource = istActiv;

                TxtTasks.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Visible;
            }
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var assigned = (AssignedTasks)LvTasks.SelectedItem;
            if (assigned == null)
            {
                MessageBox.Show("Зачада не выбрана!");
            }
            else
            {
                MessageBoxResult f = MessageBox.Show("Вы действительно хотите удалить назначенную задачу на данного сотрудника?", "Удаление", MessageBoxButton.YesNo);
                if (f == MessageBoxResult.Yes)
                {
                    assigned.IdUser = null;
                    assigned.DeadlineDate = null;
                    assigned.Status = null;
                    db.SaveChanges();
                    LvTasks.ItemsSource = db.AssignedTasks.ToList();
                    LvEmployees.VerticalAlignment = VerticalAlignment.Top;
                    LvEmployees.Height = 894;
                    LvTasks.Visibility = Visibility.Collapsed;
                    TxtTasks.Visibility = Visibility.Collapsed;
                }
            }
        }

    }
}
