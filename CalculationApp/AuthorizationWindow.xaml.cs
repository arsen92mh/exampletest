using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CalculationApp
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        CalculationEntities db = new CalculationEntities();
        public AuthorizationWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void BtnAuthor_Click(object sender, RoutedEventArgs e)
        {
            int Login = Convert.ToInt32(txtLogin.Text);
            string Password = txtPassword.Password;
            if (db.Users.FirstOrDefault(rec => rec.Password == Password && rec.IdUser == Login) != null)
            {
                Users user = db.Users.FirstOrDefault(rec => rec.Password == Password && rec.IdUser == Login);
                AuthorizationClass.IdUser = user.IdUser;
                AuthorizationClass.Name = user.Surname+" "+user.Name+" "+user.Patronymic;
                AuthorizationClass.Role = user.IdRole;
                this.Close();
                switch (AuthorizationClass.Role)
                {
                    case 1:
                        CalculationOrdersWindow calculationOrdersWindow = new CalculationOrdersWindow();
                        calculationOrdersWindow.Show();
                        break;
                    case 2:
                        AssignedTasksWindow assignedTasksWindow = new AssignedTasksWindow();
                        assignedTasksWindow.Show();
                        break;
                    case 3:
                        EmployeesWindowBuh employeesWindowBuh = new EmployeesWindowBuh();
                        employeesWindowBuh.Show();
                        break;
                }
                MessageBox.Show("Добро пожаловать в систему, " + AuthorizationClass.Name);
                this.Close();
                txtLogin.Clear();
                txtPassword.Clear();
            }
            else
            {
                MessageBox.Show("Вы неправильно ввели логин или пароль!");
                return;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
