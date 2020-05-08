using SqlForDummies.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlForDummies.Common
{
    class CreateBackupBatchfile : Connect
    {
        public string bcpContent; 
        public CreateBackupBatchfile(string tableName, ConnectionDetails connDetails)
        {
            string line = "echo **********==========**export**==========***********>>bcp_export.log" +
            Environment.NewLine +
            "echo " +
            tableName +
            ">> bcp_export.log" +
            Environment.NewLine +
            "echo %date% %time%>> bcp_export.log" +
            Environment.NewLine +
            "bcp "+ connDetails.DbName +".dbo." +
            tableName +
            " out " +
            "C:\\temp\\" +
            tableName +
            ".BCP"+
            " -n -S " +
            connDetails.ServerName +
            " -U" +
            connDetails.Username +
            " -P" + connDetails.Password +
            ">> bcp_export.log" +
            Environment.NewLine;
            bcpContent = line;
        }
    }
}
