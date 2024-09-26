using INIGestor;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLS149_SQLtest.Models;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace GLS149_SQLtest;

public class CQueryTester
{
    static public string logDir = "Logs/Outs";
    static public string logFile = "out";
    static private LogManager? s_QueriesOutLogManager;
    static public Gls149TestContext? s_Context;

    static public List<Query> s_Queries = new List<Query>();
    public class Query
    {
        public enum QueryTypeEnum
        {
            INSERTParcelData = 1,
            SELECTandDELETEMode = 2,
            SELECTandDELETEchute = 3,
            INSERTFullChute = 4
        }

        public QueryTypeEnum Type;
        // 1. Registro que graba IDESAI. Cuando la cinta lee un paquete, debe grabar un registro con el código de barras
        // 2. Registro que graba GLS#149. Para parar la cinta, encenderla en modo carga o descarga.
        // 3. Registro que graba GLS#149. Para decir porque carril debe desviar el paquete que acaba de leer.
        // 4. Registro que graba IDESAI. Para decir que el carril por donde debe desviar el paquete está saturado.
        public List<string> Parameters = new List<string>();
        public int MsTimeout;
        public int Result;
        public int MsTimeTaken;
        
        public Query(int type, List<string> parameters, int msTimeout)
        {
            if (Enum.IsDefined(typeof(QueryTypeEnum), type))
            {
                Type = (QueryTypeEnum)type;
            }
            else
            {
                Type = QueryTypeEnum.INSERTParcelData;
            }
            Parameters = parameters;
            MsTimeout = msTimeout;
            Result = -1;
            MsTimeTaken = 0;
        }
    }

    static public void Connect(string connectionString, Gls149TestContext.ConectionTypeEnum conectionTypeEnum)
    {
        s_Context = new Gls149TestContext(connectionString, conectionTypeEnum);

        try
        {
            s_Context?.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                s_QueriesOutLogManager?.AddEntry("Connect exception: " + ex.InnerException.Message);
            }
            else
            {
                s_QueriesOutLogManager?.AddEntry("Connect exception: " + ex.Message);
            }
        }
        DateTime start = DateTime.Now;
        SELECTandDELETEMode();
        SELECTandDELETEchute();
        s_QueriesOutLogManager?.AddEntry("Connect time: " + (DateTime.Now - start).TotalMilliseconds + " ms");
    }
    static public void Initialize(IniManager iniManager, ref List<string> sectionsError)
    {
        logDir = iniManager.GetString("GENERAL", "DirectorioLog", "Logs/Outs", ref sectionsError);
        logFile = iniManager.GetString("GENERAL", "ArchivoLog", "out", ref sectionsError);
        s_QueriesOutLogManager = new LogManager(logDir, logFile, 69);

        // Load queries from INI
        // get number of queries
        int queries = iniManager.GetInt("QUERIES", "NumQueries", 0, ref sectionsError);

        for (int i = 0; i < queries; i++)
        {
            string section = "Query_" + i;
            int type = iniManager.GetInt(section, "Type", 0, ref sectionsError);
            int msTimeout = iniManager.GetInt(section, "MsTimeout", 0, ref sectionsError);
            List<string> parameters = iniManager.GetCommaSeparatedStrings(section, "Parameters", ref sectionsError);

            //; Type 1: No Parameters
            //; Type 2: string, int, int, int
            //; Type 3: No Parameters
            //; Type 4: string, int, int

            if (type == 2)
            {
                if (parameters.Count != 5)
                {
                    sectionsError.Add("Query " + i + " has wrong number of parameters. Correct is 5");
                    continue;
                }
                if (!int.TryParse(parameters[1], out int pesoGramos))
                {
                    sectionsError.Add("Query " + i + " parameter 1 pesoGramos is not an int");
                    continue;
                }
                if (!int.TryParse(parameters[2], out int largoCM))
                {
                    sectionsError.Add("Query " + i + " parameter 2 largoCM is not an int");
                    continue;
                }
                if (!int.TryParse(parameters[3], out int anchoCM))
                {
                    sectionsError.Add("Query " + i + " parameter 3 anchoCM is not an int");
                    continue;
                }
                if (!int.TryParse(parameters[4], out int altoCM))
                {
                    sectionsError.Add("Query " + i + " parameter 4 altoCM is not an int");
                    continue;
                }
            }
            else if (type == 4)
            {
                if (parameters.Count != 5)
                {
                    sectionsError.Add("Query " + i + " has wrong number of parameters. Correct is 2");
                    continue;
                }
                if (!int.TryParse(parameters[1], out int pesoGramos))
                {
                    sectionsError.Add("Query " + i + " parameter 1 RampaLlena is not an int");
                    continue;
                }
            }

            s_Queries.Add(new Query(type, parameters, msTimeout));

        }


       

    }

    // 1. Registro que graba IDESAI. Cuando la cinta lee un paquete, debe grabar un registro con el código de barras
    static public void INSERTParcelData(string barcode, int pesoGramos, int largoCM, int anchoCM, int altoCM)
    {
        // Registro que graba IDESAI. Datos de código de barras, aristas y peso.
        // UVRTabla = K10CINT149_1
        // UVRKeyN01 = 0
        // UVRTxt01 = Código de barras
        // UVRNum01 = Peso en gramos sin decimales
        // UVRNum02 = X en centímetros sin decimales
        // UVRBar01 = Y en centímetros sin decimales
        // UVRBar02 = Z en centímetros sin decimales
        // UVRTabla es el código identificativo de la cinta.Habrá 2 cintas trabajando a la vez y sobre la misma
        //tabla y esto identificará los registros a tener en cuenta por cada una.

        if (s_Context == null)
        {
            s_QueriesOutLogManager?.AddEntry("Context is null");
            return;
        }

        Univerre univerre = new Univerre
        {
            UVRTabla = "K10CINT149_1",
            UVRKeyN01 = 0,
            UVRTxt01 = barcode,
            UVRNum01 = pesoGramos,
            UVRNum02 = largoCM,
            UVRBar01 = anchoCM,
            UVRBar02 = altoCM
        };
        try
        {
            s_Context?.Univerres.Add(univerre);
            s_Context?.SaveChanges();
        }
        catch(Exception ex)
        {
            if (ex.InnerException != null)
            {
                s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: "+ex.InnerException.Message);
            }
            else
            {
                s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.Message);
            }
        }
    }

    // 2. Registro que graba GLS#149. Para parar la cinta, encenderla en modo carga o descarga.
    static public int SELECTandDELETEMode()
    {
        //  Registro que graba GLS#149. Para parar la cinta, encenderla en modo carga o descarga.
        //  UVRTabla = K10CINT149_1
        //  UVRKeyN01 = 1
        //  UVRKeyN02 = 0 / 1 / 2(0 = STOP, 1 = ON modo carga, 2 = ON modo descarga)
        //  UVRKeyN02 puede valer 0(para la cinta), 1(encenderla en modo carga) o 2(encenderla en modo descarga)
        if (s_Context == null)
        {
            s_QueriesOutLogManager?.AddEntry("Context is null");
            return -1;
        }

        int mode = -1;
        try
        {
            var univerre = s_Context?.Univerres.Where(u => u.UVRTabla == "K10CINT149_1" && u.UVRKeyN01 == 1).FirstOrDefault();
            if (univerre != null)
            {
                mode = univerre.UVRKeyN02;
                s_Context?.Univerres.Remove(univerre);
                s_Context?.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                s_QueriesOutLogManager?.AddEntry("SELECT and DELETE Mode exception: " + ex.InnerException.Message);
            }
            else
            {
                s_QueriesOutLogManager?.AddEntry("SELECT and DELETE Mode exception: " + ex.Message);
            }
        }
        return -1;
    }

    // 3. Registro que graba GLS#149. Para decir porque carril debe desviar el paquete que acaba de leer.
    static public int SELECTandDELETEchute()
    {
        //  Registro que graba GLS#149. Para decir porque carril debe desviar el paquete que acaba de leer.
        //  UVRTabla = K10CINT149_1
        //  UVRKeyN01 = 2
        //  UVRKeyN02 = Número de carril por donde debe desviar el paquete actual
        if (s_Context == null)
        {
            s_QueriesOutLogManager?.AddEntry("Context is null");
            return -1;
        }
        int lane = -1;
        try
        {
            var univerre = s_Context?.Univerres.Where(u => u.UVRTabla == "K10CINT149_1" && u.UVRKeyN01 == 2).FirstOrDefault();
            if (univerre != null)
            {
                lane = univerre.UVRKeyN02;
                s_Context?.Univerres.Remove(univerre);
                s_Context?.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                s_QueriesOutLogManager?.AddEntry("SELECT and DELETE Mode exception: " + ex.InnerException.Message);
            }
            else
            {
                s_QueriesOutLogManager?.AddEntry("SELECT and DELETE Mode exception: " + ex.Message);
            }
        }
        return -1;
    }

    // 4. Registro que graba GLS#149. Para decir que el carril por donde debe desviar el paquete está saturado.
    static public void INSERTFullChute(string barcode, int chute)
    {
        //  Registro que graba IDESAI. Cuando el carril por donde debe desviar el paquete está saturado, y la cinta lo
        //envía al 0, esta debe grabar un registro para un posterior tratamiento por parte del ERP.En este caso puede
        //haber más de un registro así que el valor identificativo del paquete(código de barras) debe estar como clave.
        //  UVRTabla = K10CINT149_1
        //  UVRKeyN01 = 4
        //  UVRKeyN02 = Número de carril saturado
        //  UVRKeyC01 = Código de barras
        if (s_Context == null)
        {
            s_QueriesOutLogManager?.AddEntry("Context is null");
            return;
        }
        Univerre univerre = new Univerre
        {
            UVRTabla = "K10CINT149_1",
            UVRKeyN01 = 4,
            UVRKeyN02 = chute,
            UvrkeyC01 = barcode
        };
        try
        {
            s_Context?.Univerres.Add(univerre);
            s_Context?.SaveChanges();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                s_QueriesOutLogManager?.AddEntry("INSERT Full Chute exception: " + ex.InnerException.Message);
            }
            else
            {
                s_QueriesOutLogManager?.AddEntry("INSERT Full Chute exception: " + ex.Message);
            }
        }
    }

    #region Helpers
    static private string GenerateRabdomCb(int characters, bool numsOnly)
    {
        string result = "";
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string nums = "0123456789";
        string all = chars + nums;
        Random random = new Random();
        for (int i = 0; i < characters; i++)
        {
            result += numsOnly ? nums[random.Next(0, nums.Length)] : all[random.Next(0, all.Length)];
        }
        return result;
    }

    static private int GenerateRandomInt(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }

    #endregion Helpers
}