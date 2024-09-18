namespace GLS149_SQLtest;
using System.Data.Odbc;

using INIGestor;
using LogLib;

public partial class Form1 : Form
{
    static private IniManager? s_IniManager_Connection;
    static private IniManager? s_IniManager_Queries;
    static private LogManager? s_LogManager;

    static public List<string> Queries { get; set; } = new List<string>();
    public Form1()
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
            string logFile = s_IniManager_Queries.GetString("GENERAL", "ArchivoLog", "out", ref sectionsError);
            string logDir = s_IniManager_Queries.GetString("GENERAL", "DirectorioLog", "Logs", ref sectionsError);
            s_LogManager = new LogManager(logDir, logFile, 69);

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

        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            try
            {
                // Open the connection
                connection.Open();
                MessageBox.Show("Connection opened successfully.");

                // INSERT query example
                string insertQuery = "INSERT INTO gls149_test.univerre (UVRTabla, UVRKeyN01, UVRTxt01, UVRNum01, UVRNum02, UVRBar01, UVRBar02) VALUES ('K10CINT149_1', 0, 'Código de barras ', 200, 222, 333, 444);";
                using (OdbcCommand insertCommand = new OdbcCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("Column1", "Value1");
                    insertCommand.Parameters.AddWithValue("Column2", "Value2");
                    try
                    {
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        MessageBox.Show($"Rows inserted: {rowsAffected}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"INSERT Error: {ex.Message}");
                    }
                }

                // SELECT query example
                string selectQuery = "SELECT * FROM gls149_test.univerre WHERE UVRTabla = 'K10CINT149_1' AND UVRKeyN01 = 0 LIMIT 50;";

                try
                {
                    using (OdbcCommand selectCommand = new OdbcCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("Column1", "Value1");

                        using (OdbcDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string result = "";
                                // get number of columns
                                int columns = reader.FieldCount;
                                for (int i = 0; i < columns; i++)
                                {
                                    result += $"Column {i}: ";
                                    result += ODBCReaderToString(reader, i);
                                }
                                MessageBox.Show(result);
                            }
                        }
                    }
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

        s_LogManager?.FinalizeLogManager();
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
