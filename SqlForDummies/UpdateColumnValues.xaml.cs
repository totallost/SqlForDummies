using SqlForDummies.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for UpdateColumnValues.xaml
    /// </summary>
    public partial class UpdateColumnValues : Page
    {
        private string _connection;
        private ObservableCollection<TableColumns> ColumnList;
        private string TableName;
        private string Action;
        private ConnectionDetails connectionDetails;

        public UpdateColumnValues(string cnn, ObservableCollection<TableColumns> columnList, string tableName, string action, ConnectionDetails connDetails) 
        {
            InitializeComponent();
            _connection = cnn;
            ColumnList = columnList;
            ColumnListBox.ItemsSource = columnList;
            TableNameLabel.Content = tableName;
            TableName = tableName;
            Action = action;
            connectionDetails = connDetails;
            if (action != "UpdateDb")
                UpdateInfoLabel.Visibility = Visibility.Hidden;
        }

        private bool CheckIfValuesAreCurrect()
        {
            if (ColumnList.Count == 0)
            {
                MessageBox.Show("לא בחרת עמודות", "Warining", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            foreach (var item in ColumnList)
            {
                //check if missing a WhatToCheck value
                if (item.WhatToCheck == null)
                {
                    MessageBox.Show($"Error: {item.ColumnName.ToUpper()} is missing what to compare","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                //checking if column value should be a num and it is empty 
                if (item.FromValue == null)
                {
                    if (item.ColumnType.ToLower() == "bigint" || item.ColumnType.ToLower() == "numeric")
                        item.FromValue = "0";
                    else
                    {
                        item.FromValue = "''";
                    }
                }

                if (int.Parse(item.WhatToCheck) > 0)
                    item.ToValue = null;
               else
                {
                    if (item.ColumnType.ToLower() == "bigint" || item.ColumnType.ToLower() == "numeric")
                    {
                        if (item.ToValue == null)
                        {
                            item.ToValue = "0";
                        }
                    }
                    else
                    {
                        if(item.ToValue==null)
                            item.ToValue = "''";  
                    }
                }
                    
            }
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var isOk = CheckIfValuesAreCurrect();
            if (isOk)
            {
                if(Action== "UpdateDb")
                {
                    NavigationService.Navigate(new ChooseColumnToUpdate(_connection, TableName, ColumnList.ToList(), Action, connectionDetails));
                }
                else
                {
                    NavigationService.Navigate(new Confirmation(_connection, TableName, ColumnList.ToList(), Action, connectionDetails));
                }                
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
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                var val = value as ComboBoxItem;
                switch (val.Content)
                {
                    case "בין":
                        return true;
                    case "שווה":
                    case "שונה":
                    case "דומה":
                        return false;
                }
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
                return null;
        }
    }
}
