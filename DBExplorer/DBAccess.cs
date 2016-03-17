using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBExplorer.Models;

namespace DBExplorer
{
    public class DBAccess
    {
        private SqlConnection _connection;
        public DBAccess(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<DBTable> GetAllTables()
        {
            var lst = new List<DBTable>();
            var query = "SELECT name Name, max_column_id_used No FROM sys.tables";

            try 
	        {
                _connection.Open();

                SqlCommand command = new SqlCommand(query);
                command.Connection = _connection;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var table = new DBTable
                    {
                        Name = (string)reader["Name"],
                        NumberOfColumns = (int)reader["No"],
                    };

                    lst.Add(table);
                }

                _connection.Close();

                return lst;
	        }
	        catch (Exception ex)
	        {
                throw ex;
	        }
            finally
            {
                _connection.Close();
            }
        }

        public List<DBColumn> GetTableInfo(string tableName)
        {
            var lst = new List<DBColumn>();
            var query = "select " + 
                        "st.name [Table], sc.name [Column], stp.name [Type], sc.is_nullable [Nullable], sep.value [Description] " +
                        "from sys.tables st " + 
                        "inner join sys.columns sc on st.object_id = sc.object_id " +
	                    "inner join sys.types stp on sc.system_type_id = stp.system_type_id " + 
                        "left join sys.extended_properties sep on st.object_id = sep.major_id " +
                        "and sc.column_id = sep.minor_id " +
                        "and sep.name = 'MS_Description' " +
                        "where st.name = '{0}' and stp.name <> 'sysname'" +
	                    "order by sc.column_id";

            try
            {
                _connection.Open();
                SqlCommand command = new SqlCommand(string.Format(query, tableName));
                command.Connection = _connection;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var column = new DBColumn
                    {
                        Name = (string)reader["Column"],
                        Type = (string)reader["Type"],
                        Nullable = (bool)reader["Nullable"],
                        Description = reader["Description"].ToString()
                    };

                    lst.Add(column);
                }

                return lst;
            }
            catch (Exception ex)
            {   
                throw ex;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
