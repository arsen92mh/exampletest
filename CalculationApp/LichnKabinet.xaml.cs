using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CalculationApp
{
    /// <summary>
    /// Логика взаимодействия для LichnKabinet.xaml
    /// </summary>
    public partial class LichnKabinet : Window
    {
        CalculationEntities db = new CalculationEntities();
        Users user = new Users();
        public LichnKabinet()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            var UserPhoto = db.Users.Where(rec => rec.IdUser == AuthorizationClass.IdUser).ToList();
            StPhoto.DataContext = UserPhoto;

            var datas = db.Users.Where(rec => rec.IdUser == AuthorizationClass.IdUser).ToList();
            StDatas.DataContext = datas;


            switch (AuthorizationClass.Role)
            {
                case 1:
                    StEmployees.Visibility = Visibility.Visible;
                    StTasks.Visibility = Visibility.Visible;
                    break;
                case 3:
                    StEmployeesBuh.Visibility = Visibility.Visible;
                    break;
                case 2:
                    StTasksEmplo.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMainWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            var userphoto = db.Users.Where(rec => rec.IdUser == AuthorizationClass.IdUser).FirstOrDefault();
            fileDialog.Filter = "Изображения (*.png, *.jpg, *.jpeg, *.gif)|*.png;*.jpg;*.jpeg;*.gif|Все файлы (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                Img.Source = new BitmapImage(new Uri(fileDialog.FileName));

                userphoto.Photo = File.ReadAllBytes(fileDialog.FileName);
            }
            db.SaveChanges();
        }

        private void BtnPasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (TxtPassword.Visibility == Visibility.Visible)
            {
                PbPassword.Password = TxtPassword.Text;
                TxtPassword.Visibility = Visibility.Collapsed;
                PbPassword.Visibility = Visibility.Visible;
                ImagePassword.Source = new BitmapImage(new Uri("/Images/free-icon-hide-2767146.png", UriKind.Relative));
            }
            else if (TxtPassword.Visibility == Visibility.Collapsed)
            {
                TxtPassword.Text = PbPassword.Password;
                TxtPassword.Visibility = Visibility.Visible;
                PbPassword.Visibility = Visibility.Collapsed;
                ImagePassword.Source = new BitmapImage(new Uri("/Images/free-icon-eye-158746.png", UriKind.Relative));
            }
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            EmployeesWindow employeesWindow = new EmployeesWindow();
            employeesWindow.Show();
        }

        private void BtnTasks_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            CalculationOrdersWindow calculationOrdersWindow = new CalculationOrdersWindow();
            calculationOrdersWindow.Show();
        }

        private void BtnEmployeesBuh_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            EmployeesWindowBuh employeesWindowBuh = new EmployeesWindowBuh();
            employeesWindowBuh.Show();
        }

        private void BtnTasksEmplo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            AssignedTasksWindow assigned = new AssignedTasksWindow();
            assigned.Show();
        }
    }
}
