using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;

namespace CalculationApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Tasks> LstTasks = new List<Tasks>();
        List<Services> LstServices = new List<Services>();
        CalculationEntities db = new CalculationEntities();
        public MainWindow()
        {
            InitializeComponent();
            LvService.ItemsSource = db.Services.ToList();
            LvTasks.ItemsSource = db.Tasks.ToList();
            LvTasks2.ItemsSource = db.Tasks.ToList();

            WindowState = WindowState.Maximized;


            switch (AuthorizationClass.Role)
            {
                case 1:
                    BtnAuthorization.Visibility = Visibility.Collapsed;
                    BtnLichnKabinet.Visibility = Visibility.Visible;
                    break;
                case 2:
                    BtnAuthorization.Visibility = Visibility.Collapsed;
                    BtnLichnKabinet.Visibility = Visibility.Visible;
                    break;
                case 3:
                    BtnAuthorization.Visibility = Visibility.Collapsed;
                    BtnLichnKabinet.Visibility = Visibility.Visible;
                    break;
            }

            CmbTasksDifficult.Items.Add("Все сложности");
            string[] difficults = db.Tasks.Select(p => p.Difficulties.Name).Distinct().ToArray();
            foreach (string difficult in difficults)
            {
                CmbTasksDifficult.Items.Add(difficult);
            }

            LstTasks = db.Tasks.ToList();
        }
        private void BtnAuthorization_Click(object sender, RoutedEventArgs e)
        {
            LvService.Visibility = Visibility.Visible;
            LvService.VerticalAlignment = VerticalAlignment.Top;
            LvService.Height = 864;
            TxtService.Visibility = Visibility.Visible;
            LvTasks.Visibility = Visibility.Collapsed;
            TxtTasks.Visibility = Visibility.Collapsed;
            StSortSetvices.Visibility = Visibility.Visible;
            StSortTasks.Visibility = Visibility.Collapsed;
            CmbSortPrice.SelectedIndex = -1;
            AuthorizationWindow authorization = new AuthorizationWindow();
            authorization.Show();
        }
        private void BtnVisibilityList_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            if (LvService.Visibility == Visibility.Collapsed && TxtService.Visibility == Visibility.Collapsed)
            {
                switch (AuthorizationClass.Role)
                {
                    case 3:
                        LvService.Visibility = Visibility.Visible;
                        LvService.VerticalAlignment = VerticalAlignment.Top;
                        LvService.Height = 864;
                        TxtService.Visibility = Visibility.Visible;
                        LvTasks2.Visibility = Visibility.Collapsed;
                        TxtTasks.Visibility = Visibility.Collapsed;
                        BorderDatas.Visibility = Visibility.Collapsed;
                        StSortSetvices.Visibility = Visibility.Visible;
                        btnUpdate1.Visibility = Visibility.Visible;
                        break;
                }
                LvService.Visibility = Visibility.Visible;
                LvService.VerticalAlignment = VerticalAlignment.Top;
                LvService.Height = 864;
                TxtService.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                StSortSetvices.Visibility = Visibility.Visible;
                btnUpdate1.Visibility = Visibility.Visible;
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortPoisk();
        }

        private void SortPoisk()
        {
            LstServices = db.Services.ToList();
            string poiskname = txtName.Text;
            if (poiskname != "")
            {
                LstServices = LstServices.Where(n => n.Name.Contains(poiskname)).ToList();
            }
            if (CmbSortPrice.SelectedIndex==0)
            {
                LstServices = LstServices.OrderByDescending(rec=>rec.Cost).ToList();
            }
            if (CmbSortPrice.SelectedIndex == 1)
            {
                LstServices = LstServices.OrderBy(rec => rec.Cost).ToList();
            }
            LvService.VerticalAlignment = VerticalAlignment.Top;
            LvService.Height = 864;
            LvTasks.Visibility = Visibility.Collapsed;
            TxtTasks.Visibility = Visibility.Collapsed;
            LvService.ItemsSource = LstServices;
            StSortTasks.Visibility = Visibility.Collapsed;

            switch (AuthorizationClass.Role)
            {
                case 3:
                    LvService.VerticalAlignment = VerticalAlignment.Top;
                    LvService.Height = 864;
                    LvTasks2.Visibility = Visibility.Collapsed;
                    TxtTasks.Visibility = Visibility.Collapsed;
                    LvService.ItemsSource = LstServices;
                    StSortTasks.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); //Принудительное закрытие приложения
        }

        private void BtnLichnKAbinet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LichnKabinet lichnKabinet = new LichnKabinet();
            lichnKabinet.Show();
        }

        private void LvTasks2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var merActiv = LvTasks2.SelectedItem as Tasks;
            if (merActiv == null)
            {
                StTasksDatas.Visibility = Visibility.Collapsed;
            }
            else
            {
                var istActiv = db.Tasks.Where(rec => rec.IdTask == merActiv.IdTask).ToList();
                StTasksDatas.DataContext = istActiv;
                BorderDatas.Visibility = Visibility.Visible;
            }
        }

        private void btnSaveTasks_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
            LvService.ItemsSource = db.Services.ToList();
            LvTasks.ItemsSource = db.Tasks.ToList();
            LvTasks2.ItemsSource = db.Tasks.ToList();
        }

        private void LvService_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var merActiv = LvService.SelectedItem as Services;
            if (merActiv == null)
            {
                MessageBox.Show("Услуга не выбрана!");
            }
            else
            {
                LvService.Height = 380;
                LvService.VerticalAlignment = VerticalAlignment.Top;

                var istActiv = db.Tasks.Where(rec => rec.IdService == merActiv.IdService).ToList();
                LvTasks.ItemsSource = istActiv;

                TxtTasks.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Visible;
                StSortTasks.Visibility = Visibility.Visible;
                CmbTasksDifficult.SelectedIndex = 0;
            }
            switch(AuthorizationClass.Role)
            {
                case 3:
                    var merActiv1 = LvService.SelectedItem as Services;
                    if (merActiv1 == null)
                    {
                        MessageBox.Show("Услуга не выбрана!");
                    }
                    else
                    {
                        LvService.Height = 380;
                        LvService.VerticalAlignment = VerticalAlignment.Top;

                        var istActiv = db.Tasks.Where(rec => rec.IdService == merActiv1.IdService).ToList();
                        LvTasks2.ItemsSource = istActiv;

                        TxtTasks.Visibility = Visibility.Visible;
                        LvTasks2.Visibility = Visibility.Visible;
                        StSortTasks.Visibility = Visibility.Visible;
                        BorderDatas.Visibility = Visibility.Collapsed;
                        CmbTasksDifficult.SelectedIndex = 0;
                    }
                    break;
            }
        }

        private void CmbSortPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortPoisk();
        }

        private void CmbTasksDifficult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var merActiv = LvService.SelectedItem as Services;
            if (merActiv != null)
            {
                var istActiv = db.Tasks.Where(rec => rec.IdService == merActiv.IdService).ToList();
            }
            if (CmbTasksDifficult.SelectedIndex > 0)
            {
                LvTasks.ItemsSource = db.Tasks.Where(n => n.Difficulties.Name == CmbTasksDifficult.SelectedItem.ToString() && n.IdService == merActiv.IdService).ToList();
                LvTasks2.ItemsSource = db.Tasks.Where(n => n.Difficulties.Name == CmbTasksDifficult.SelectedItem.ToString() && n.IdService == merActiv.IdService).ToList();
            }
            if (CmbTasksDifficult.SelectedIndex == 0)
            {
                LvTasks.ItemsSource = db.Tasks.Where(n => n.IdService == merActiv.IdService).ToList();
                LvTasks2.ItemsSource = db.Tasks.Where(n => n.IdService == merActiv.IdService).ToList();
            }
        }

        private void btnUpdate1_Click(object sender, RoutedEventArgs e)
        {
            LvService.Visibility = Visibility.Visible;
            LvService.VerticalAlignment = VerticalAlignment.Top;
            LvService.Height = 864;
            TxtService.Visibility = Visibility.Visible;
            LvTasks.Visibility = Visibility.Collapsed;
            TxtTasks.Visibility = Visibility.Collapsed;
            StSortSetvices.Visibility = Visibility.Visible;
            StSortTasks.Visibility = Visibility.Collapsed;
            CmbSortPrice.SelectedIndex = -1;
            BorderDatas.Visibility = Visibility.Collapsed;
        }
    }
}
