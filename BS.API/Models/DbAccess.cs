using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BS.API.Models
{
    public class DbAccess
    {
        protected string connectionString { get; set; }

        public DbAccess(string connectionString) // Server=localhost\sqlexpress;Database=BSDB;Trusted_Connection=True;
        {
            this.connectionString = connectionString;
        }

        public SqlConnection getConnection()
        {
            SqlConnection sqlCon = new SqlConnection(this.connectionString);
            if (sqlCon.State != ConnectionState.Open)
            {
                sqlCon.Open();
                return sqlCon;
            }
            sqlCon.Close();
            return sqlCon;

        }
    }
}