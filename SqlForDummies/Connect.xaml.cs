using SqlForDummies.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class Connect : Page
    {
        //public SqlConnection _connection;  
        public Connect()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConnectionDetails connectionDetails = new ConnectionDetails
            {
                Username = dbUser.Text,
                Password = dbPassword.Password.ToString(),
                ServerName = serverName.Text,
                DbName = dbName.Text

            };
            ConnectionLoadingLabel.Visibility = Visibility.Visible;
            var connectionString = @"Server=" + serverName.Text + ";Database=" + dbName.Text + ";User Id=" + dbUser.Text + ";Password=" + dbPassword.Password.ToString() + ";";
            Task.Run(() => ConnectToDb(connectionDetails, connectionString))
                .ContinueWith(task =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ConnectionLoadingLabel.Visibility = Visibility.Hidden;
                    });

                }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            MessageBox.Show("You Just found out that TONY is a King!", "Congratiolations", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
        }

        private void ConnectToDb(ConnectionDetails connectionDetails, string connectionString)
        {
            SqlConnection cnn;
            try
            {
                using (cnn= new SqlConnection(connectionString))
                {
                    cnn.Open();
                    cnn.Close();
                }
                this.Dispatcher.Invoke(() =>
                {
                    NavigationService.Navigate(new ChooseMethod(connectionString, connectionDetails));
                });
            }
            catch (SqlException ex)
            {
                MessageBox.Show("שגיאה בהתחברות לשרת, בדוק את פרטי ההתחברות", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
