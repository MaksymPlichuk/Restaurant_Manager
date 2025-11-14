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
using System.Windows.Shapes;
using DataAccess;
using DataAccess.Entities;
using WpfApp1;

namespace MainClientWindow
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public RestaurantDbContext context { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
            context = new RestaurantDbContext();
            LoginTxt.Text = "ron_davis";
            passTxt.Text = "password1234";

        }

        private void signInBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = context.FindUserByLogin(LoginTxt.Text, passTxt.Text);
            if (user == null)
            {
                MessageBox.Show("Incorrect Login or Password!");
                return;
            }
            WpfApp1.MainWindow mainWindow = new WpfApp1.MainWindow();
            mainWindow.LoggedInUser = user;
            this.Close();
            mainWindow.Show();


        }

        private void signUpBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = context.RegistrationUser(LoginTxt.Text, passTxt.Text);
            WpfApp1.MainWindow mainWindow = new WpfApp1.MainWindow();
            mainWindow.LoggedInUser = user;
            this.Close();
            mainWindow.Show();

        }
    }
}
