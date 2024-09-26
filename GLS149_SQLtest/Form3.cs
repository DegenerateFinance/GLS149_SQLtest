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

        using (var context = new Gls149TestContext(new DbContextOptions<Gls149TestContext>()))
        {
            context.SetConnectionType(connectionString, ConectionTypeEnum);
            // connection timeout
            context.Database.SetCommandTimeout(1);

            #region InsightConnection
            try
            {
                DateTime delete = DateTime.Now;
                // delete where UVRTabla = "K10CINT149_1", UVRKeyN01 = 0
                var dels = context.Univerres.Where(u => u.UVRTabla == "K10CINT149_1" && u.UVRKeyN01 == 0).ToList();
                foreach (var univerre in dels)
                {
                    context.Univerres.Remove(univerre);
                }
                context.SaveChanges();

                MessageBox.Show("Delete successful in " + (DateTime.Now - delete).TotalMilliseconds + " milliseconds");

                DateTime query = DateTime.Now;
                var newEntity = new Univerre { UVRTabla = "K10CINT149_1", UVRKeyN01 = 0, UVRTxt01 = "Código de barras", UVRNum01 = 200, UVRNum02 = 222, UVRBar01 = 333, UVRBar02 = 444 };
                context.Univerres.Add(newEntity);
                context.SaveChanges();
                MessageBox.Show("INSERT successful in " + (DateTime.Now - query).TotalMilliseconds + " milliseconds");
            }
            catch (DbUpdateException dbEx)
            {
                var sqlException = dbEx.InnerException as Exception;  // Or SqlException for SQL Server
                //var sqlException = dbEx.InnerException as MySqlException;// MySqlException for MySQL
                if (sqlException != null)
                {
                    MessageBox.Show("SQL Error: " + sqlException.Message);
                }
                else
                {
                    MessageBox.Show("An error occurred: " + dbEx.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("General error: " + ex.Message);
            }


            #endregion InsightConnection

        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        s_IniManager_Connection?.SetValue("GENERAL", "Driver", CbDbEngine.SelectedIndex == 0 ? "MySQl" : "SQLServer");
        s_IniManager_Connection?.SetValue("GENERAL", "Server", TbServer.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Database", TbDatabase.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "User", TbUser.Text);
        s_IniManager_Connection?.SetValue("GENERAL", "Password", TbPassword.Text);

        s_GeneralLogManager?.FinalizeLogManager();
    }

    private void BtnQuery_Click(object sender, EventArgs e)
    {
        //MessageBox.Show(CQueryTester.GenerateRabdomCb(13, false));
    }
}



