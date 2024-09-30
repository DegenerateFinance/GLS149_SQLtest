namespace GLS149_SQLtest;

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
        ApplicationConfiguration.Initialize();
        Application.Run(new Form3());
    }
}

// dotnet ef dbcontext optimize --context Gls149TestContext --project GLS149_SQLtest --startup-project GLS149_SQLtest --output-dir MyCompiledModels --namespace MyCompiledModels

// Scaffold-DbContext  "server=192.168.2.69;database=gls149_test;user=remote;password=remote;" Pomelo.EntityFrameworkCore.MySql -OutputDir MyScaffoldedModels -f

// Scaffold-DbContext  "server=192.168.2.69;database=gls149_test;user=remote;password=remote;TrustServerCertificate=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir MyScaffoldedModels -f
