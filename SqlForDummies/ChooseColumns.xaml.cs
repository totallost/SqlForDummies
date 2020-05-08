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
    /// Interaction logic for ChooseColumns.xaml
    /// </summary>
    public partial class ChooseColumns : Page
    {
        private string _connection;
        private ObservableCollection<TableColumns> ChosenColumns;
        private string TableName;
        private string Action;
        private ConnectionDetails connectionDetails;
        public ChooseColumns(string cnn, string action, string tableName, ConnectionDetails connDetails)
        {
            InitializeComponent();

            ChosenColumns = new ObservableCollection<TableColumns>();
            _connection = cnn;
            ColumnBox.ItemsSource = GetTableColumns(tableName);
            TableName = tableName;
            Action = action;
            connectionDetails = connDetails;
        }
        private List<TableColumns> GetTableColumns(string table)
        {
            var columns = new List<TableColumns>();
            var sql = $"SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='{table}';";
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    cnn.Open();

                    var query = new SqlCommand(sql, cnn);
                    var answer = query.ExecuteReader();
                    while (answer.Read())
                    {
                        columns.Add(new TableColumns
                        {
                            ColumnName = answer.GetValue(0).ToString(),
                            ColumnType = answer.GetValue(1).ToString(),

                        });
                    }
                    cnn.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString(), "Error connecting to DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return columns;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ColumnBox.SelectedValue != null)
            {
                var selectedValue = ColumnBox.SelectedItem;
                var covertedValue = selectedValue as TableColumns;
                
                ChosenColumns.Add(new TableColumns 
                { 
                    ColumnName = covertedValue.ColumnName,
                    ColumnType = covertedValue.ColumnType
                });
                ChosenColumnListBox.ItemsSource = ChosenColumns;
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            ChosenColumns.Remove((TableColumns)item.DataContext);
        }

        private void CntnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ChosenColumns.Count > 0)
            {
                NavigationService.Navigate(new UpdateColumnValues(_connection, ChosenColumns, TableName, Action, connectionDetails));
            }
            else
            {
                MessageBox.Show("לא בחרת עמודות", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
