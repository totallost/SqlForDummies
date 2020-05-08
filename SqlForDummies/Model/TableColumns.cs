using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlForDummies
{
    public class TableColumns
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public string WhatToCheck { get; set; }
        public string UpdateValue { get; set; }
    }
}
