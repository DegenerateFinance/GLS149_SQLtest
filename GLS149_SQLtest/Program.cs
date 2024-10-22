using Microsoft.EntityFrameworkCore;

namespace GLS149_SQLtest;

using GLS149_SQLtest.TestModels;
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
                    MessageBox.Show("Connected to database");
                }
                DateTime start = DateTime.Now;

                GlobalTest test_obj = new GlobalTest() { property1 = 69, Property2 = "2", Dt3 = DateTime.Now };
                context.GlobalTests.Add(test_obj);

                DateTime end_q1 = DateTime.Now;

                GlobalTest test_obj2 = new GlobalTest() { property1 = 269, Property2 = "2", Dt3 = DateTime.Now };
                context.GlobalTests.Add(test_obj2);
                context.SaveChanges();


                MessageBox.Show($"Time 1: {(end_q1 - start).TotalMilliseconds} ms \n Time2 {(DateTime.Now - end_q1).TotalMilliseconds} ms");
                MessageBox.Show("INSERTED, GlobalId: " + test_obj2.GlobalId);

                // COUNT WHERE property1 = "1"
                int count = context.GlobalTests.Count(x => x.property1 == 1);


                DateTime select1 = DateTime.Now;
                // SELECT WHERE property1 = "1" LIMIT 100
                List<GlobalTest> list = context.GlobalTests.Where(x => x.property1 == 1).Take(100).ToList();


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
                    RoundedDt3 = new DateTime(x.Dt3.Value.Year, x.Dt3.Value.Month, x.Dt3.Value.Day, x.Dt3.Value.Hour, x.Dt3.Value.Minute, 0)
                })
                .Select(g => new
                {
                    MaxProd30 = g.Max(x => x.property1),
                    FechaLectura = g.Key
                })
                .OrderByDescending(x => x.MaxProd30).Take(100).ToList();

                DateTime fechaLectura = test3[0].FechaLectura.RoundedDt3;

                MessageBox.Show($"Max: {fechaLectura.ToString("yyyy-MM-dd HH:mm:ss.fff")} {test3[0].MaxProd30} ");

                MessageBox.Show($"\n Time 1: {(select2 - select1).TotalMilliseconds} ms \n Time 2: {(DateTime.Now - select2).TotalMilliseconds} ms");

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
        }


    }
}

// dotnet ef dbcontext optimize --context Gls149TestContext --project GLS149_SQLtest --startup-project GLS149_SQLtest --output-dir MyCompiledModels --namespace MyCompiledModels

// Scaffold-DbContext  "server=192.168.2.69;database=gls149_test;user=remote;password=remote;" Pomelo.EntityFrameworkCore.MySql -OutputDir MyScaffoldedModels -f

// Scaffold-DbContext  "server=192.168.2.69;database=gls149_test;user=remote;password=remote;TrustServerCertificate=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir MyScaffoldedModels -f
