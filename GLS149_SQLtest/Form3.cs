namespace GLS149_SQLtest;

using INIGestor;
using LogLib;
using System.Diagnostics;

using System;
using System.Collections.Generic;

using System.Data.Odbc;

public partial class Form3 : Form
{
    static private IniManager? s_IniManager_Connection;
    static private IniManager? s_IniManager_Queries;
    static private LogManager? s_GeneralLogManager;

    static public List<string> Queries { get; set; } = new List<string>();
    public Form3()
    {
        InitializeComponent();
    }
    private void Form1_Load(object sender, EventArgs e)
    {
        try
        {
            List<string> sectionsError = new List<string>();
            string sqlconnINI = "FicherosINI//SQLConn.ini";
            s_IniManager_Connection = new IniManager(sqlconnINI);
            string engine = s_IniManager_Connection.GetString("GENERAL", "Engine", "MySQL", ref sectionsError);
            if (engine.Contains("ODBC", StringComparison.InvariantCultureIgnoreCase))
            {
                CbDbEngine.SelectedIndex = 2;
            }
            else if (engine.Contains("SQLServer", StringComparison.InvariantCultureIgnoreCase))
            {
                CbDbEngine.SelectedIndex = 1;
            }
            else if (engine.Contains("MySQL", StringComparison.InvariantCultureIgnoreCase))
            {
                CbDbEngine.SelectedIndex = 0;
            }
            TbODBCDriver.Text = s_IniManager_Connection.GetString("GENERAL", "Driver", "MySQL ODBC 8.0 ANSI Driver", ref sectionsError);
            TbServer.Text = s_IniManager_Connection.GetString("GENERAL", "Server", "localhost", ref sectionsError);
            TbDatabase.Text = s_IniManager_Connection.GetString("GENERAL", "Database", "gls149_test", ref sectionsError);
            TbUser.Text = s_IniManager_Connection.GetString("GENERAL", "User", "root", ref sectionsError);
            TbPassword.Text = s_IniManager_Connection.GetString("GENERAL", "Password", "root", ref sectionsError);

            string queriesINI = "FicherosINI//Queries.ini";
            s_IniManager_Queries = new IniManager(queriesINI);
            string logFile = s_IniManager_Queries.GetString("GENERAL", "ArchivoLog", "log", ref sectionsError);
            string logDir = s_IniManager_Queries.GetString("GENERAL", "DirectorioLog", "Logs", ref sectionsError);
            s_GeneralLogManager = new LogManager(logFile, logDir, 69);

            CQueryTester.Initialize(queriesINI, ref sectionsError);

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
        catch (Exception ex)
        {
            MessageBox.Show("Error loading the form: " + ex.Message);
        }
    }

    private void BtnConnect_Click(object sender, EventArgs e)
    {
        string server = TbServer.Text;
        string database = TbDatabase.Text;
        string user = TbUser.Text;
        string password = TbPassword.Text;

        string connectionString = "";
        CQueryTester.ConectionTypeEnum ConectionTypeEnum = CQueryTester.ConectionTypeEnum.NotSelected;
        if (CbDbEngine.SelectedIndex == 0)
        {
            ConectionTypeEnum = CQueryTester.ConectionTypeEnum.MySQL;
        }
        else if (CbDbEngine.SelectedIndex == 1)
        {
            ConectionTypeEnum = CQueryTester.ConectionTypeEnum.SQLServer;
        }
        else if (CbDbEngine.SelectedIndex == 2)
        {
            ConectionTypeEnum = CQueryTester.ConectionTypeEnum.ODBC;
        }

        if (ConectionTypeEnum == CQueryTester.ConectionTypeEnum.NotSelected)
        {
            MessageBox.Show("Select a database engine");
            return;
        }


        if (ConectionTypeEnum == CQueryTester.ConectionTypeEnum.ODBC)
        {
            if (string.IsNullOrEmpty(TbODBCDriver.Text))
            {
                MessageBox.Show("ODBC Driver is required");
                return;
            }
            string driver = TbODBCDriver.Text;
            // ODBC connection string
            connectionString = $"Driver={{{driver}}};Server={server};Database={database};Uid={user};Pwd={password};Connection Timeout=1";
            if (!CQueryTester.Connect(connectionString, ConectionTypeEnum))
            {
                MessageBox.Show("Error connecting to the database");
                return;
            }
            if (!CQueryTester.IsConnected())
            {
                MessageBox.Show("Error connecting to the database");
                return;
            }
            MessageBox.Show("Connection successful");
            return;
        }


        if (ConectionTypeEnum == CQueryTester.ConectionTypeEnum.SQLServer)
        {
            connectionString = $"Server={server};Database={database};User Id={user};Password={password}; TrustServerCertificate=True; Encrypt=False;Connect Timeout=1;";
        }
        else if (ConectionTypeEnum == CQueryTester.ConectionTypeEnum.MySQL)
        {
            connectionString = $"Server={server};Database={database};User Id={user};Password={password};Connection Timeout=1";
        }
        if (!CQueryTester.Connect(connectionString, ConectionTypeEnum))
        {
            MessageBox.Show("Error connecting to the database");
            return;
        }
        if (!CQueryTester.IsConnected())
        {
            MessageBox.Show("Error connecting to the database");
            return;
        }
        MessageBox.Show("Connection successful");
    }
    private void BtnQuery_Click(object sender, EventArgs e)
    {
        if (!CQueryTester.IsConnected())
        {
            MessageBox.Show("Not connected to the database");
            return;
        }
        CQueryTester.RunLoadedQueries();
        MessageBox.Show("Query executed successfully. See Outs/out");
    }
    
    private void BtnOpenOurFolder_Click(object sender, EventArgs e)
    {
        if (CQueryTester.s_QueriesOutLogManager?.LogFullPath is not string p)
        {
            MessageBox.Show("No Path");
            return;
        }
        Process.Start("explorer.exe", p);
    }

    private void CbDbEngine_SelectedIndexChanged(object sender, EventArgs e)
    {
        LblODBCDriver.Visible = CbDbEngine.SelectedIndex == 2;
        TbODBCDriver.Visible = CbDbEngine.SelectedIndex == 2;
    }
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (CbDbEngine.SelectedIndex == 0)
        {
            s_IniManager_Connection?.SetValue("GENERAL", "Engine", "MySQL");
        }
        else if (CbDbEngine.SelectedIndex == 1)
        {
            s_IniManager_Connection?.SetValue("GENERAL", "Engine", "SQLServer");
        }
        else if (CbDbEngine.SelectedIndex == 2)
        {
            s_IniManager_Connection?.SetValue("GENERAL", "Engine", "ODBC");
        }
        s_IniManager_Connection?.SetValue("GENERAL", "Driver", TbODBCDriver.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Server", TbServer.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Database", TbDatabase.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "User", TbUser.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Password", TbPassword.Text);

        s_GeneralLogManager?.FinalizeLogManager();
        CQueryTester.FinalizeQueryTester();
    }
}



