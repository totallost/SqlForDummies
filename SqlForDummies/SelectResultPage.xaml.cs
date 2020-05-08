using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for SelectResultPage.xaml
    /// </summary>
    public partial class SelectResultPage : Page
    {
        public SelectResultPage(string connectionString, string tableName, string sqlStatement)
        {
            IEnumerable<dynamic> source = null;
            //executeSql(connectionString, sqlStatement, source);

            InitializeComponent();
            executeSql(connectionString, sqlStatement, source);

        }

        private void executeSql(string connectionString, string sqlStatement, IEnumerable<dynamic> source)
        {
            try
            {
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    var answer = cnn.Query(sqlStatement).ToList();
                    source = answer;
                }

                if (source.Count() > 0)
                {
                    var newSource = ConvertToDataTable(source);
                    SelectDataGrid.ItemsSource = newSource.DefaultView;
                }
                else
                {
                    EmptySelectLabel.Visibility = Visibility.Visible;
                }
            }
            catch(SqlException ex)
            {
                EmptySelectLabel.Visibility = Visibility.Visible;
                MessageBox.Show("שגיאה בביצוע הנוסחה\nבדוק שהערכים תקינים", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable ConvertToDataTable(IEnumerable<dynamic> items)
        {
            var t = new DataTable();
            var first = (IDictionary<string, object>)items.First();
            foreach (var k in first.Keys)
            {
                var c = t.Columns.Add(k);
                var val = first[k];
                if (val != null) 
                    c.DataType = val.GetType();
            }

            foreach (var item in items)
            {
                var r = t.NewRow();
                var i = (IDictionary<string, object>)item;
                foreach (var k in i.Keys)
                {
                    var val = i[k];
                    if (val == null) 
                        val = DBNull.Value;
                    r[k] = val;
                }
                t.Rows.Add(r);
            }
            return t;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
