namespace GLS149_SQLtest;

using INIGestor;
using LogLib;
using Insight.Database;
using Insight.Database.Providers.Default;

using System;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.EntityFrameworkCore;
using GLS149_SQLtest.Models;
using MySqlConnector;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            s_IniManager_Connection = new IniManager("FicherosINI//SQLConn.ini");
            CbDbEngine.SelectedIndex = s_IniManager_Connection.GetString("GENERAL", "Driver", "MySQL", ref sectionsError).Contains("MySQL", StringComparison.InvariantCultureIgnoreCase) ? 0 : 1;
            TbServer.Text = s_IniManager_Connection.GetString("GENERAL", "Server", "localhost", ref sectionsError);
            TbDatabase.Text = s_IniManager_Connection.GetString("GENERAL", "Database", "gls149_test", ref sectionsError);
            TbUser.Text = s_IniManager_Connection.GetString("GENERAL", "User", "root", ref sectionsError);
            TbPassword.Text = s_IniManager_Connection.GetString("GENERAL", "Password", "root", ref sectionsError);

            s_IniManager_Queries = new IniManager("FicherosINI//Queries.ini");
            string logFile = s_IniManager_Queries.GetString("GENERAL", "ArchivoLog", "log", ref sectionsError);
            string logDir = s_IniManager_Queries.GetString("GENERAL", "DirectorioLog", "Logs", ref sectionsError);
            s_GeneralLogManager = new LogManager(logFile, logDir, 69);

            CQueryTester.Initialize("FicherosINI//Queries.ini", ref sectionsError);

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

        Gls149TestContext.ConectionTypeEnum ConectionTypeEnum = CbDbEngine.SelectedIndex == 0 ? Gls149TestContext.ConectionTypeEnum.MySQL : Gls149TestContext.ConectionTypeEnum.SQLServer;

        if (ConectionTypeEnum == Gls149TestContext.ConectionTypeEnum.SQLServer)
        {
            connectionString = $"Server={server};Database={database};User Id={user};Password={password}; TrustServerCertificate=True; Encrypt=False;";
        }
        else if (ConectionTypeEnum == Gls149TestContext.ConectionTypeEnum.MySQL)
        {
            connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
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
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        s_IniManager_Connection?.SetValue("GENERAL", "Driver", CbDbEngine.SelectedIndex == 0 ? "MySQl" : "SQLServer");
        s_IniManager_Connection?.SetValue("GENERAL", "Server", TbServer.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Database", TbDatabase.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "User", TbUser.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Password", TbPassword.Text);

        s_GeneralLogManager?.FinalizeLogManager();
        CQueryTester.FinalizeQueryTester();
    }
}



