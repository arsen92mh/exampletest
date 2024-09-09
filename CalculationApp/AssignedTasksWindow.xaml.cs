using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
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
    /// Логика взаимодействия для AssignedTasksWindow.xaml
    /// </summary>
    public partial class AssignedTasksWindow : Window
    {
        CalculationEntities db = new CalculationEntities();
        List<AssignedTasks> Tasks= new List<AssignedTasks>();
        public AssignedTasksWindow()
        {
            InitializeComponent();
            Tasks = db.AssignedTasks.Where(rec => rec.IdUser == AuthorizationClass.IdUser).OrderByDescending(rec=>rec.AppointmentDate).ToList();
            LvTasks.ItemsSource = Tasks;

            WindowState = WindowState.Maximized;
        }

        private void BtnLichnKabinet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LichnKabinet lichnKabinet = new LichnKabinet();
            lichnKabinet.Show();
        }

        public void Poisk()
        {
            Tasks = db.AssignedTasks.Where(rec => rec.IdUser == AuthorizationClass.IdUser).ToList();
            string poiskname = txtName.Text;
            DateTime dateM;
            if (poiskname != "")
            {
                Tasks = Tasks.Where(n => n.Tasks.Name.Contains(poiskname)).ToList();
            }
            if (DpAppointmentDate.SelectedDate.HasValue)
            {
                dateM = DpAppointmentDate.SelectedDate.Value;
                Tasks = Tasks.Where(rec => rec.AppointmentDate == dateM).ToList();
            }
            else
            {
                var datas = Tasks.Where(rec => rec.IdUser == AuthorizationClass.IdUser).ToList();
                Tasks = datas;
            }
            LvTasks.ItemsSource = Tasks;
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Poisk();
        }

        private void DpAppointmentDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Poisk();
        }

        private void LvTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var status = LvTasks.SelectedItem as AssignedTasks;
            if(status!= null)
            {
                if (status.IdStatus == 2)
                {
                    StStatus.Visibility = Visibility.Visible;
                }
                else
                {
                    StStatus.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("Не выбрана задача!");
            }
        }

        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentTime = DateTime.Today;
            var status = LvTasks.SelectedItem as AssignedTasks;
            if (status != null)
            {
                if (status.IdStatus == 2)
                {
                    if (currentTime > status.DeadlineDate)
                    {
                        TimeSpan? different = currentTime - status.DeadlineDate;
                        status.IdStatus = 3;

                        MessageBox.Show($"Статус задачи был изменен на: {status.Status.Name}. Так как вы задержали работу на {different.Value.TotalDays} дней, это повлияет на вашу зарплату!");
                        LvTasks.ItemsSource = db.AssignedTasks.Where(rec => rec.IdUser == AuthorizationClass.IdUser).OrderByDescending(rec => rec.AppointmentDate).ToList();
                        StStatus.Visibility = Visibility.Collapsed;
                        status.DateOfCompletion = currentTime;
                        db.SaveChanges();
                    }
                    else
                    {
                        status.IdStatus = 1;
                        status.DateOfCompletion = currentTime;
                        LvTasks.ItemsSource = db.AssignedTasks.Where(rec => rec.IdUser == AuthorizationClass.IdUser).OrderByDescending(rec => rec.AppointmentDate).ToList();
                        StStatus.Visibility = Visibility.Collapsed;
                        db.SaveChanges();
                    }

                }
                else
                {
                    MessageBox.Show("Данная задача выполнена!");
                }
            }
            else
            {
                MessageBox.Show("Не выбрана задача!");
            }
        }

        private void BtnRaschetZP_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            int month = date.Month;

            var cont = AuthorizationClass.IdUser;

            var UserSalary = db.Users.Where(rec => rec.IdUser == cont).FirstOrDefault();

            var usertasks = db.AssignedTasks.Where(rec => rec.IdUser == cont && rec.IdStatus == 1 && rec.AppointmentDate.Month == month).ToList();
            decimal costtasks = usertasks.Sum(rec => rec.CostTask);

            decimal? salary = (costtasks + UserSalary.CategoriesOfEmployees.MinSalary) - ((costtasks + UserSalary.CategoriesOfEmployees.MinSalary) * 13 / 100);

            TxtSalary.Text = $"= {salary:N2}";
            StSalary.Visibility = Visibility.Visible;
        }
    }
}
