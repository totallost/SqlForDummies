using SqlForDummies.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for ChooseMethod.xaml
    /// </summary>
    public partial class ChooseMethod : Page
    {
        private string _connection;
        private ConnectionDetails ConnectionDetails;

        public ChooseMethod(string cnn, ConnectionDetails connectionDetails)
        {
            InitializeComponent();
            _connection = cnn;
            ConnectionDetails = connectionDetails;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            var command = item.Name;
            NavigationService.Navigate(new ChooseTable(_connection, command, ConnectionDetails));
        }
    }
}
