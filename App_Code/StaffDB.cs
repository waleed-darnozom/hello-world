using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;


/// <summary>
/// Summary description for StaffDB
/// </summary>
public class StaffDB
{
    private SqlConnection _sqlConnection;
    private SqlCommand _sqlCommand;
    private int _success;

    public StaffDB()
    {
        _success = OpenConnection();
    }

    private int GetConnection()
    {
        try
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["Integration"].ConnectionString;
            _sqlConnection = new SqlConnection(connectionString);
            _success = 0;
        }
        catch (Exception)
        {
            throw new ArgumentException("-1");
        }
        return _success;
    }

    private int OpenConnection()
    {
        try
        {
            if (GetConnection() != 0)
            {
                throw new Exception();
            }
            if (_sqlConnection.State == ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }
            _success = 0;
        }
        catch (Exception)
        {
            throw new ArgumentException("-2");
        }
        return _success;
    }

    private int CloseConnection()
    {
        try
        {
            if (_sqlConnection.State != ConnectionState.Closed)
                _sqlConnection.Close();
            _success = 0;
        }
        catch (Exception)
        {
            throw new ArgumentException("-3");
        }
        return _success;
    }

    public int Execute(String sqlStatement)
    {
        try
        {
            if (!String.IsNullOrEmpty(sqlStatement.Trim()))
            {
                OpenConnection();
                _sqlCommand = new SqlCommand(sqlStatement, _sqlConnection);
                _sqlCommand.ExecuteNonQuery();
                _success = 0;
            }
            else
                throw new ArgumentException("-4");
        }
        catch (SqlException e)
        {
            if (e.ErrorCode == -2146232060)
            {
                throw new ArgumentException("-6");
            }
            else
                throw new ArgumentException("-5");
        }
        catch (Exception e)
        {
            throw new ArgumentException("-5");
        }
        finally
        {
            _success = CloseConnection();
        }
        return _success;
    }

    public DataTable Search(String sqlStatement, String dataTableName)
    {
        try
        {
            OpenConnection();
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(sqlStatement, _sqlConnection);
            DataSet ds = new DataSet();

            myDataAdapter.Fill(ds, dataTableName);

            DataTable dt = ds.Tables[dataTableName];
            return dt;
        }
        catch (Exception)
        {
            throw new ArgumentException("-5");
        }
        finally
        {
            CloseConnection();
        }
    }

    public DataSet RetrieveDataset(String sqlStatement, string tableName)
    {
        OpenConnection();
        SqlDataAdapter myDataAdapter = new SqlDataAdapter(sqlStatement, _sqlConnection);
        DataSet ds = new DataSet();
        try
        {
            myDataAdapter.Fill(ds, tableName);
        }
        catch (Exception)
        {
            throw new ArgumentException("-7");
        }
        return ds;
    }
}