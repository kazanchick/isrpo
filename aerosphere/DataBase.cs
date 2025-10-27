using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aerosphere
{
    public class DataBase
    {
        private string connStr = "Server=ADCLG1;Database=Курсовая;Trusted_Connection=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connStr);
        }
    }
}
