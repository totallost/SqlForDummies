using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {

        public LoginPage()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var truePassword = "ImWeak";
            if (entryPassword.Password.ToString() == truePassword)
                NavigationService.Navigate(new Connect());

        }
    }
}
