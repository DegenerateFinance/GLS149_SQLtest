using INIGestor;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLS149_SQLtest;

public class CQueryTester
{
    static public string logDir = "Logs/Outs";
    static public string logFile = "out";
    static private LogManager? s_QueriesOutLogManager;

    static public void Initialize(IniManager iniManager, ref List<string> sectionsError)
    {
        logDir = iniManager.GetString("GENERAL", "DirectorioLog", "Logs/Outs", ref sectionsError);
        logFile = iniManager.GetString("GENERAL", "ArchivoLog", "out", ref sectionsError);
    }
}
