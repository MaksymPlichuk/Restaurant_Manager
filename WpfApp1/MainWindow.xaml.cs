using DataAccess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RestaurantDbContext context { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void myTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (myTabControl.SelectedItem is TabItem selectedTab)
                {
                    if (selectedTab.Header.ToString()=="Menu")
                    {
                        LoadMenuData();
                    }
                }
            }
        }
        private void LoadMenuData()
        {
            //треба заповнити БД
            //var data = context.
            //menuDataGrid.ItemsSource = data;
        }
    }
}