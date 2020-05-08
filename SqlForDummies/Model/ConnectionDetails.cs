using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlForDummies.Model
{
    public class ConnectionDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DbName { get; set; }
        public string ServerName { get; set; }
    }
}
