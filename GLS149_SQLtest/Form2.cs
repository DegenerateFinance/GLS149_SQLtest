namespace GLS149_SQLtest;

using INIGestor;
using LogLib;
using Insight.Database;
using Insight.Database.Providers.Default;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Data.Odbc;

public partial class Form2 : Form
{
    static private IniManager? s_IniManager_Connection;
    static private IniManager? s_IniManager_Queries;
    static private LogManager? s_GeneralLogManager;

    static public List<string> Queries { get; set; } = new List<string>();
    public Form2()
    {
        InitializeComponent();
    }
    private void Form1_Load(object sender, EventArgs e)
    {
        try
        {
            List<string> sectionsError = new List<string>();
            s_IniManager_Connection = new IniManager("FicherosINI//SQLConn.ini");
            TbDriver.Text = s_IniManager_Connection.GetString("GENERAL", "Driver",  "MySQL ODBC 8.4 ANSI Driver", ref sectionsError);
            TbServer.Text = s_IniManager_Connection.GetString("GENERAL", "Server", "localhost", ref sectionsError);
            TbDatabase.Text = s_IniManager_Connection.GetString("GENERAL", "Database", "gls149_test", ref sectionsError);
            TbUser.Text = s_IniManager_Connection.GetString("GENERAL", "User", "root", ref sectionsError);
            TbPassword.Text = s_IniManager_Connection.GetString("GENERAL", "Password", "root", ref sectionsError);

            s_IniManager_Queries = new IniManager("FicherosINI//Queries.ini");
            string logFile = s_IniManager_Queries.GetString("GENERAL", "ArchivoLog", "logs", ref sectionsError);
            string logDir = s_IniManager_Queries.GetString("GENERAL", "DirectorioLog", "Logs", ref sectionsError);
            s_GeneralLogManager = new LogManager(logDir, logFile, 69);

            if (sectionsError.Count > 0)
            {
                string error = "Error reading sections: ";
                foreach (string section in sectionsError)
                {
                    error += section + ", ";
                }
                MessageBox.Show(error);
            }
        }
        catch
        {

        }
    }

    private void BtnConnect_Click(object sender, EventArgs e)
    {
        string driver = TbDriver.Text;
        string server = TbServer.Text;
        string database = TbDatabase.Text;
        string user = TbUser.Text;
        string password = TbPassword.Text;

        // ODBC connection string
        string connectionString = $"Driver={{{driver}}};Server={server};Database={database};Uid={user};Pwd={password};";

        // Register ODBC as a database provider for Insight.Database
        //Insight.Database.SqlInsightDbProvider.RegisterProvider();

        // Create an ODBC connection
        using (var connection = new OdbcConnection(connectionString))
        {
            // set connection timeout
            connection.ConnectionTimeout = 1;
            try
            {
                DateTime date = DateTime.Now;
                // Open the connection
                connection.Open();
                int elapsed = (DateTime.Now - date).Milliseconds;
                MessageBox.Show("Connection opened successfully in " + elapsed + " ms.");

                // Example 1: INSERT query using Insight.Database
                var insertQuery = $"INSERT INTO {WangOrm2.GetTableName(typeof(TableUniverre))} ({WangOrm2.GetColumnName(typeof(TableUniverre), nameof(TableUniverre.UVRTabla))}, UVRKeyN01, UVRTxt01, UVRNum01, UVRNum02, UVRBar01, UVRBar02) VALUES ('K10CINT149_1', 0, 'Código de barras ', 200, 222, 333, 444);";
                var parameters = new { Column1 = "Value1", Column2 = "Value2" };
                //connection.ExecuteSql(insertQuery, parameters);


                // Example 2: SELECT query using Insight.Database
                var selectQuery = $"SELECT " +
                    $"{WangOrm2.GetColumnName(typeof(TableUniverre), nameof(TableUniverre.UVRTabla))}, " +
                    $"{WangOrm2.GetColumnName(typeof(TableUniverre), nameof(TableUniverre.UVRKeyN01))} " +
                    $"FROM " +
                    $"{WangOrm2.GetTableName(typeof(TableUniverre))} " +
                    $"WHERE {WangOrm2.GetColumnName(typeof(TableUniverre), nameof(TableUniverre.UVRTabla))} = @UVRTabla AND " +
                    $"{WangOrm2.GetColumnName(typeof(TableUniverre), nameof(TableUniverre.UVRKeyN01))} = @UVRKeyN01";

                try
                {
                    var result = connection.QuerySql<TableUniverre>(selectQuery, new { UVRTabla = "K10CINT149_1", UVRKeyN01 = 0 });

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"SELECT Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Close the connection
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    MessageBox.Show("Connection closed.");
                }
            }
        }


    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        s_IniManager_Connection?.SetValue("GENERAL", "Driver", TbDriver.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Server", TbServer.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Database", TbDatabase.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "User", TbUser.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Password", TbPassword.Text);

        s_GeneralLogManager?.FinalizeLogManager();
    }
    #region Helpers
    private string ODBCReaderToString(OdbcDataReader reader, int index)
    {
        try
        {
            if (reader.IsDBNull(index))
            {
                return "NULL";
            }
            // GET TYPE OF COLUMN
            Type type = reader.GetFieldType(index);
            if (type == typeof(string))
            {
                return reader.GetString(index);
            }
            else if (type == typeof(int))
            {
                return reader.GetInt32(index).ToString();
            }
            else if (type == typeof(double))
            {
                return reader.GetDouble(index).ToString();
            }
            else if (type == typeof(DateTime))
            {
                return reader.GetDateTime(index).ToString();
            }
            else
            {
                return "ERR";
            }
        }
        catch
        {
            return "ERR";
        }
    }
    #endregion Helpers
}
