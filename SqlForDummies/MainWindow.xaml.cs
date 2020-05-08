using System.Windows;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var login = new LoginPage();
            mainFrame.NavigationService.Navigate(login);
        }
        
    }
}
