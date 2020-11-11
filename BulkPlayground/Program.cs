using Oracle.ManagedDataAccess.Client;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Z.BulkOperations;

namespace BulkPlayground
{

    class Customer
    {
        public int Id { get; set; }
    }


    class Account
    {
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Oracle();
            //SqlServer();
        }

        private static void SqlServer()
        {
            using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Dupa;Integrated Security=True"))
            {
                connection.Open();
                var dt = new DataTable();
                dt.Columns.Add("Name");

                for (var i = 1; i < 1000; i++)
                    dt.Rows.Add("Name " + i + 1);

                using (var transaction = connection.BeginTransaction())
                using (var sqlBulk = new SqlBulkCopy(connection, copyOptions: SqlBulkCopyOptions.Default, transaction))
                {
                    sqlBulk.DestinationTableName = "dbo.Account";
                    sqlBulk.WriteToServer(dt);
                }
            }
        }

        private static void Oracle()
        {
            string[] names = { "Namedf" };
            var p_names = new OracleParameter 
            {
                OracleDbType = OracleDbType.Varchar2,
                Value = names
            };

            using var connection = new OracleConnection("Data Source=localhost:1521/xe; User Id=kamil; Password=password;");
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = "insert into Account(Name) values (:1)";
            cmd.ArrayBindCount = names.Length;
            cmd.Parameters.Add(p_names);
            cmd.ExecuteNonQuery();
            transaction.Commit();
        }

    }
}
