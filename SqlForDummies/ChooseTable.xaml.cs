using SqlForDummies.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for ChooseTable.xaml
    /// </summary>
    public partial class ChooseTable : Page
    {
        private string _connection;
        private string Action;
        private ObservableCollection<string> Tables;
        private ConnectionDetails connectionDetails;
        public ChooseTable(string cnn, string action, ConnectionDetails connDetails)
        {
            InitializeComponent();
            _connection = cnn;
            Action = action;
            Tables = new ObservableCollection<string>(GetDbTables());
            TableBox.ItemsSource = Tables;
            TableBox.IsTextSearchEnabled = true;
            connectionDetails = connDetails;
        }
        private List<string> GetDbTables()
        {
            var tables = new List<string>();
            var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    cnn.Open();

                    var query = new SqlCommand(sql, cnn);
                    var answer = query.ExecuteReader();
                    while (answer.Read())
                    {
                        tables.Add(answer.GetValue(0).ToString());
                    }
                    tables.Sort();
                    cnn.Close();
                }
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.ToString(), "Error connecting to DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tables;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TableBox.SelectedValue == null)
                MessageBox.Show("בחר טבלה", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string Table = TableBox.SelectedValue.ToString();
                NavigationService.Navigate(new ChooseColumns(_connection, Action, Table, connectionDetails));
            }
            
        }

        private void BckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
