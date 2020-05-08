using SqlForDummies.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for ChooseColumnToUpdate.xaml
    /// </summary>
    public partial class ChooseColumnToUpdate : Page
    {
        private string _connection;
        private ObservableCollection<TableColumns> ChosenColumns;
        private string TableName;
        private string Action;
        private List<TableColumns> TableColumns;
        private ConnectionDetails connectionDetails;

        public ChooseColumnToUpdate(string cnn, string tableName, List<TableColumns> tableColumns, string action, ConnectionDetails connDetails)
        {
            InitializeComponent();
            TableName = tableName;
            Action = action;
            _connection = cnn;
            TableColumns = tableColumns;
            ChosenColumns = new ObservableCollection<TableColumns>();
            ColumnBox.ItemsSource = GetTableColumns(tableName);
            connectionDetails = connDetails;
        }
        private List<TableColumns> GetTableColumns(string table)
        {
            var columns = new List<TableColumns>();
            var sql = $"SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='{table}';";
            try
            {
                using(var cnn = new SqlConnection(_connection))
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
            bool isOk = CheckListValues();
            var columnList = ChosenColumns.ToList();
            foreach (var item in TableColumns)
            {
                columnList.Add(item);
            }
            if(isOk)
                NavigationService.Navigate(new Confirmation(_connection, TableName, columnList, Action, connectionDetails));
        }

        private void BckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        private bool CheckListValues()
        {
            if (TableColumns.Count == 0)
                return false;
            foreach(var item in ChosenColumns)
            {
                if(item.UpdateValue ==null)
                {
                    if (item.ColumnType == "bigint" || item.ColumnType == "numeric")
                        item.UpdateValue = "0";
                    else
                        item.UpdateValue = "''";
                }
            }
            return true;
        }
    }
}
