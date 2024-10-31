using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GLS149_SQLtest.CQueryTester;

namespace GLS149_SQLtest.TestModels;

partial class Gls149TestContext : DbContext
{
    private readonly string _connectionString;
    private readonly CQueryTester.ConectionTypeEnum _conectionTypeEnum;
    private Action<string> _loggerAction;

    public Gls149TestContext(string connectionString)
    {
        _connectionString = connectionString;
        _conectionTypeEnum = ConectionTypeEnum.MySQL;
        SetLogger(Console.WriteLine);
    }
    public void SetLogger(Action<string> loggerAction)
    {
        _loggerAction = loggerAction;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_conectionTypeEnum == CQueryTester.ConectionTypeEnum.MySQL)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    _connectionString,
                    new MySqlServerVersion(new Version(8, 0, 36))); // MySQL version
            }
        }
    }
}
