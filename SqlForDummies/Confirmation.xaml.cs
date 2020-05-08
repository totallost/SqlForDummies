using SqlForDummies.Common;
using SqlForDummies.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace SqlForDummies
{
    /// <summary>
    /// Interaction logic for Confirmation.xaml
    /// </summary>
    public partial class Confirmation : Page
    {
        private string _connection;
        private string Action;
        private List<TableColumns> ColumnsList;
        private string TableName;
        private string SqlStatement;
        private string HebrewSqlStatement;
        private ConnectionDetails connectionDetails;

        public Confirmation(string cnn, string tableName, List<TableColumns> columns, string action, ConnectionDetails connDetails)
        {
            InitializeComponent();
            _connection = cnn;
            TableName = tableName;
            Action = action;
            ColumnsList = columns;
            CreateSqlText();
            SqlTextBox.Text = SqlStatement;
            HebrewDescriptionTextBox.Text = HebrewSqlStatement;
            connectionDetails = connDetails;
            if (action == "SelectDb")
            {
                CntnBtn.IsEnabled = true;
                BackupLabel.Visibility = Visibility.Hidden;
            }

        }
        //create the SQL string that is going to be send to SQL SERVER
        private void CreateSqlText()
        {
            string SqlText = "";
            string HebrewSql = "";
            string action = "";
            switch (Action)
            {
                case "UpdateDb":
                    action = "Update";
                    SqlText = $"{action.ToUpper()} {TableName} \nSET ";
                    HebrewSql = $"עדכן שורות בטבלת {TableName} כך ש\n";
                    break;
                case "DeleteDb":
                    action = "Delete";
                    SqlText = $"{action.ToUpper()} \nFROM {TableName} \nWHERE ";
                    HebrewSql = $"מחק שורות בטבלת {TableName} היכן ש\n";
                    break;
                case "SelectDb":
                    action = "Select";
                    SqlText = $"{action.ToUpper()} * \nFROM {TableName} \nWHERE ";
                    HebrewSql = $"הצג שורות בטבלת {TableName} היכן ש\n";
                    break;
            }
            //update the SET only for UPDATE statement
            if (action == "Update")
            {
                foreach(var item in ColumnsList)
                {
                    if (item.UpdateValue != null)
                    {
                        SqlText += $"{item.ColumnName} = {item.UpdateValue},";
                        HebrewSql += $"שדה {item.ColumnName} שווה ל {item.UpdateValue}\n";
                    }
                }
                SqlText = SqlText.Remove(SqlText.Length - 1);                             
                SqlText += "\nWHERE ";
                
                HebrewSql += "היכן ש";

            }
            foreach(var item in ColumnsList)
            {
                if (item.UpdateValue == null)
                {
                    switch (item.WhatToCheck)
                    {
                        case "0":
                            SqlText += $"{item.ColumnName} BETWEEN {item.FromValue} AND {item.ToValue} and\n";
                            HebrewSql += $"שדה {item.ColumnName} בין {item.FromValue} לבין {item.ToValue}\n";
                            break;
                        case "1":
                            SqlText += $"{item.ColumnName} = {item.FromValue} and\n";
                            HebrewSql += $"שדה {item.ColumnName} שווה ל {item.FromValue}\n";
                            break;
                        case "2":
                            SqlText += $"{item.ColumnName} <> {item.FromValue} and\n";
                            HebrewSql += $"שדה {item.ColumnName} שונה מ {item.FromValue}\n";
                            break;
                        case "3":
                            SqlText += $"{item.ColumnName} LIKE '%{item.FromValue}%' and\n";
                            HebrewSql += $"שדה {item.ColumnName} דומה ל {item.FromValue}\n";
                            break;
                    }
                }
            }
            SqlText = SqlText.Substring(0,SqlText.Length - 4);
            SqlText += ";";
            SqlStatement = SqlText;
            HebrewSqlStatement = HebrewSql;
            
        }

        private void BckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void BackupBtn_Click(object sender, RoutedEventArgs e)
        {
            var bcp = new CreateBackupBatchfile(TableName, connectionDetails);

            if (Directory.Exists("C:\\temp"))
            {
                //create batchfile to run bcp
                string batchFilePath = "c:\\temp\\bcp_out_" + TableName + ".bat";
                StreamWriter file = new StreamWriter(batchFilePath);
                try
                {
                    file.WriteLine(bcp.bcpContent);
                    file.Close();
                }
                catch
                {
                    MessageBox.Show("Error", "נכשל ביצירת קובץ גיבוי", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (File.Exists(batchFilePath))
                {
                    Task.Run(() => RunBcp())
                        .ContinueWith(task =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                CntnBtn.IsEnabled = true;
                            });
                            MessageBox.Show($"הגיבוי מתבצע ויהיה ב\n C:\\temp\\{TableName}.BCP\nודא שקובץ זה נוצר", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                }
                BackupLabel.Visibility = Visibility.Hidden;                
            }
            else
            {
                MessageBox.Show("חסר מסלול לגיבוי\nC:\\temp\nלא ניתן לבצע גיבוי", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void RunBcp()
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "C:\\temp\\bcp_out_" + TableName + ".bat";
            proc.StartInfo.WorkingDirectory = "C:\\temp\\";
            proc.Start();
        }

        private void CntnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Action != "SelectDb")
            {
                LoadingLabel.Visibility = Visibility.Visible;
                Task.Run(() => RunTheSql())
                    .ContinueWith(task =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            LoadingLabel.Visibility = Visibility.Hidden;
                        });

                    }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                NavigationService.Navigate(new SelectResultPage(_connection, TableName, SqlStatement));
            }
        }
        private void RunTheSql()
        {
            var sql = SqlStatement;
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    cnn.Open();

                    var query = new SqlCommand(sql, cnn);
                    var answer = query.ExecuteReader();
                    cnn.Close();
                    MessageBox.Show($"{answer.RecordsAffected} מספר הרשומות שהשתנו הוא", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
                }                                
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString(), "Error connecting to DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
    }
}
