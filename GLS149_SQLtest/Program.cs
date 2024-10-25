using Microsoft.EntityFrameworkCore;

namespace GLS149_SQLtest;

using GLS149_SQLtest.TestModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using LogLib;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        //ApplicationConfiguration.Initialize();
        //Application.Run(new Form3());

        string driver = "MySQL ODBC 8.0 ANSI Driver";
        string server = "192.168.2.69";
        string database = "gls149_test";
        string user = "remote";
        string password = "remote";
        string connectionString = $"Server={server};Database={database};User Id={user};Password={password};Connection Timeout=1";

        using (Gls149TestContext context = new Gls149TestContext(connectionString))
        {
            LogManager logger = new LogManager("logname", "logpath", 7);
            context.SetLogger(logger.AddEntry);
            try
            {
                context?.Database.SetCommandTimeout(1);
                //s_Context?.Database.EnsureCreated();
                context?.Database.OpenConnection();
                if (context == null)
                {
                    MessageBox.Show("Context is null");
                }
                if (context.Database.CanConnect())
                {
                    Console.WriteLine("Connected to database");
                }
                
                DateTime start = DateTime.Now;
                GlobalTest test_obj = new GlobalTest() { GlobalId = 69, Property2 = "2", Dt3 = DateTime.Now };
                context.GlobalTests.Add(test_obj);
                context.SaveChanges();

                DateTime end_q1 = DateTime.Now;

                GlobalTest test_obj2 = new GlobalTest() { property1 = 269, Property2 = "2", Dt3 = DateTime.Now };
                context.GlobalTests.Add(test_obj2);
                context.SaveChanges();


                Console.WriteLine($"Time 1: {(end_q1 - start).TotalMilliseconds} ms \n Time2 {(DateTime.Now - end_q1).TotalMilliseconds} ms");
                Console.WriteLine("INSERTED, GlobalId: " + test_obj2.GlobalId);

                
                // COUNT WHERE property1 = "1"
                int count = context.GlobalTests.Count(x => x.property1 == 1);
                Console.WriteLine("Count: " + count);
                

                DateTime select1 = DateTime.Now;
                // SELECT WHERE property1 = "1" LIMIT 100
                List<GlobalTest> list = context.GlobalTests.Where(x => x.property1 == 1).OrderBy(x => x.GlobalId).Take(100).ToList();


                DateTime select2 = DateTime.Now;

                // SELECT WHERE property1 = "1" AND Dt3 BETWEEN  ORDER BY Global_Id DESC LIMIT 1
                GlobalTest test = context.GlobalTests.Where(x => x.property1 == 1).OrderByDescending(x => x.GlobalId).FirstOrDefault();

                // SELECT WHERE property1 = "1" AND Dt3 BETWEEN today and today-7(days) ORDER BY Global_Id DESC LIMIT 1
                DateTime today = DateTime.Now;
                DateTime lastWeek = today.AddDays(-7);
                GlobalTest test2 = context.GlobalTests.Where(x => x.property1 == 1 && x.Dt3 >= lastWeek && x.Dt3 <= today).OrderByDescending(x => x.GlobalId).FirstOrDefault();

                // SELECT COUNT(dt3) WHERE property1 = "1" AND Dt3 BETWEEN today and today-7(days) GROUP BY dt3(minutes) ORDER BY Global_Id DESC LIMIT 1
                // We will round Dt3 to the nearest minute for grouping purposes

                //var test3 = context.GlobalTests
                //.Where(x => x.Dt3.HasValue && x.Dt3 >= lastWeek && x.Dt3 <= today)
                //.GroupBy(x => new {
                //    Year = x.Dt3.Value.Year,
                //    Month = x.Dt3.Value.Month,
                //    Day = x.Dt3.Value.Day,
                //    Hour = x.Dt3.Value.Hour,
                //    Minute = x.Dt3.Value.Minute
                //})
                //.Select(g => new
                //{
                //    MaxProd30 = g.Max(x => x.property1),
                //    FechaLectura = g.Key
                //})
                //.OrderByDescending(x => x.MaxProd30).Take(100).ToList();

                var test3 = context.GlobalTests
                .Where(x => x.Dt3.HasValue && x.Dt3 >= lastWeek && x.Dt3 <= today)
                .GroupBy(x => new
                {
                    // Ensure Dt3 has a value before using it
                    RoundedDt3 = new DateTime(x.Dt3.Value.Year, x.Dt3.Value.Month, x.Dt3.Value.Day, x.Dt3.Value.Hour, x.Dt3.Value.Minute, 0),
                })
                .Select(g => new
                {
                    MaxProd30 = g.Max(x => x.property1),
                    ModeProperty2 = g.Take(1).Select(x => x.Property2).FirstOrDefault(),
                    SelectedDt = g.Key.RoundedDt3
                })
                .OrderByDescending(x => x.MaxProd30).Take(100).ToList();

                DateTime fechaLectura = test3[0].SelectedDt;
                string modeProperty2 = test3[0].ModeProperty2;

                Console.WriteLine($"Max: {fechaLectura.ToString("yyyy-MM-dd HH:mm:ss.fff")} {test3[0].MaxProd30} ");

                Console.WriteLine($"\n Time 1: {(select2 - select1).TotalMilliseconds} ms \n Time 2: {(DateTime.Now - select2).TotalMilliseconds} ms");

                string? columnName = GetColumnName<GlobalTest>(context, nameof(GlobalTest.GlobalId));
                Console.WriteLine($"Column Name: {columnName}");


            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("INNER EXCEPTION: " + ex.InnerException.Message);
                }
                else
                {
                    MessageBox.Show("EXCEPTION: " + ex.Message);
                }
            }
            logger.FinalizeLogManager();
        }


    }

    static public string GetColumnName<TEntity>(DbContext context, string propertyName)
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity)); // Get entity type from the model
        var property = entityType.FindProperty(propertyName); // Get the property metadata

        // Get the column name from the property metadata
        var columnName = property.GetColumnName(StoreObjectIdentifier.Table(entityType.GetTableName(), entityType.GetSchema()));

        return columnName ?? throw new InvalidOperationException($"No column found for property '{propertyName}'");
    }

}

// Scaffold-DbContext  "server=192.168.2.69;database=gls149_test;user=remote;password=remote;TrustServerCertificate=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir MyScaffoldedModels -f

//Scaffold-DbContext "server=192.168.2.69;database=gls149_test;user=root;password=root;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -f -namespace GLS149_SQLtest.TestModels

//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//{
//    if (!optionsBuilder.IsConfigured)
//    {
//        // Set the connection string - you need to specify it here
//        optionsBuilder
//           .UseMySql(
//            _connectionString,
//            new MySqlServerVersion(new Version(8, 0, 36)))
//           .LogTo(_loggerAction, new[] { DbLoggerCategory.Database.Command.Name, }, LogLevel.Information);

//        optionsBuilder.EnableSensitiveDataLogging();
//        ; // Specify the MySQL version you're using
//    }
//}