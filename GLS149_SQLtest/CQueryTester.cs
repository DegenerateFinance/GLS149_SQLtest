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
using Microsoft.EntityFrameworkCore;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static GLS149_SQLtest.CQueryTester.Query;
using System.Data.Odbc;

namespace GLS149_SQLtest;

public class CQueryTester
{
    static public string logDir = "Logs/Outs";
    static public string logFile = "out";
    static public LogManager? s_QueriesOutLogManager { get; private set; }
    static public Gls149TestContext? s_Context;
    static public OdbcConnection? s_OdbcConnection { get; private set; }
    static private string s_ConnectionString = "";
    static private ConectionTypeEnum s_ConectionTypeEnum;
    public enum ConectionTypeEnum
    {
        NotSelected = -1,
        MySQL = 0,
        SQLServer = 1,
        ODBC = 2
    }

    static public List<Query> s_Queries = new List<Query>();
    public class Query
    {
        public enum QueryTypeEnum
        {
            NotDefined = 0,
            INSERTParcelData = 1,
            SELECTandDELETEMode = 2,
            SELECTandDELETEchute = 3,
            INSERTFullChute = 4
        }

        public QueryTypeEnum QType;
        // 1. Registro que graba IDESAI. Cuando la cinta lee un paquete, debe grabar un registro con el código de barras
        // 2. Registro que graba GLS#149. Para parar la cinta, encenderla en modo carga o descarga.
        // 3. Registro que graba GLS#149. Para decir porque carril debe desviar el paquete que acaba de leer.
        // 4. Registro que graba IDESAI. Para decir que el carril por donde debe desviar el paquete está saturado.
        
        public int Id;
        public List<string> Parameters = new List<string>();
        public int MsTimeNextQuery;
        public int MsTimeout;
        public DateTime TimeOut = DateTime.MaxValue;

        // 0 or Positive: Result
        // -1: No Response yet 
        // -2: Timed Out
        // -3: Exception
        public int Result;

        public int MsTimeTaken;
        
        public Query(int id, int type, List<string> parameters, int msTimeout, int msNextQuery)
        {
            Id = id;
            if (Enum.IsDefined(typeof(QueryTypeEnum), type))
            {
                QType = (QueryTypeEnum)type;
            }
            else
            {
                QType = QueryTypeEnum.NotDefined;
            }
            Parameters = parameters;
            MsTimeout = msTimeout;
            MsTimeNextQuery = msNextQuery;
            Result = -1;
            MsTimeTaken = 0;
        }
        public void StartQuery()
        {
            TimeOut = DateTime.Now.AddMilliseconds(MsTimeout);
        }
    }

    static public bool Connect(string connectionString, ConectionTypeEnum conectionTypeEnum)
    {
        s_ConectionTypeEnum = conectionTypeEnum;
        s_ConnectionString = connectionString;
        if (conectionTypeEnum == ConectionTypeEnum.ODBC)
        {
            s_OdbcConnection = new OdbcConnection(connectionString);
            try
            {
                s_OdbcConnection?.Open();
                if (s_OdbcConnection == null)
                {
                    s_QueriesOutLogManager?.AddEntry("Connect error: No response from database");
                    return false;
                }
                s_QueriesOutLogManager?.AddEntry("Connected to database");
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
                return false;
            }
            return true;
        }

        s_ConnectionString = connectionString ;
        s_ConectionTypeEnum = conectionTypeEnum;

        s_Context = new Gls149TestContext(connectionString, conectionTypeEnum);
        
        try
        {
            s_Context?.Database.SetCommandTimeout(1);
            s_Context?.Database.EnsureCreated();
            s_Context?.Database.OpenConnection();
            if (s_Context == null)
            {
                s_QueriesOutLogManager?.AddEntry("Context is null");
                return false;
            }
            if (s_Context.Database.CanConnect())
            {
                s_QueriesOutLogManager?.AddEntry("Connected to database");
            }
            DateTime start = DateTime.Now;
            //int chute = SyncSELECTandDELETEchute();
            //if (chute < -1)
            //{
            //    s_QueriesOutLogManager?.AddEntry("Connect error: No response from database");
            //    return false;
            //}
            //else
            //{
            //    s_QueriesOutLogManager?.AddEntry("Chute: " + chute);
            //}
            s_QueriesOutLogManager?.AddEntry("Connect time: " + (DateTime.Now - start).TotalMilliseconds + " ms");
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
            return false;
        }
        return true;
    }
    static public bool IsConnected()
    {
        if (s_ConectionTypeEnum == ConectionTypeEnum.ODBC)
        {
            if (s_OdbcConnection == null)
            {
                return false;
            }
            if (s_OdbcConnection.State != System.Data.ConnectionState.Open)
            {
                return false;
            }
            return true;
        }

        if (s_Context == null)
        {
            return false;
        }
        if (!s_Context.Database.CanConnect())
        {
            return false;
        }
        return true;
    }
    static public void Initialize(string ficheroINI, ref List<string> sectionsError)
    {
        try
        {
            IniManager iniManager = new IniManager(ficheroINI);
            logDir = iniManager.GetString("Queries", "DirectorioLog", "Outs", ref sectionsError);
            logFile = iniManager.GetString("Queries", "ArchivoLog", "out", ref sectionsError);
            s_QueriesOutLogManager = new LogManager(logFile, logDir, 69);

            // Load queries from INI
            // get number of queries
            int queries = iniManager.GetInt("QUERIES", "NumQueries", 0, ref sectionsError);

            for (int i = 0; i < queries; i++)
            {
                string section = "Query_" + i;
                int type = iniManager.GetInt(section, "Type", 0, ref sectionsError);
                int msTimeout = iniManager.GetInt(section, "MsTimeout", 0, ref sectionsError);
                List<string> parameters = iniManager.GetCommaSeparatedStrings(section, "Parameters", ref sectionsError);
                //; Type 1: string barcode, int pesoGramos, int largoCM, int anchoCM, int altoCM
                //; Type 2: No Parameters
                //; Type 3: No Parameters
                //; Type 4: string, int, int
                int msNextQuery = iniManager.GetInt(section, "MsNextQuery", 0, ref sectionsError);

                if (type == (int)Query.QueryTypeEnum.INSERTParcelData)
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
                else if (type == (int)Query.QueryTypeEnum.INSERTFullChute)
                {
                    if (parameters.Count != 2)
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

                s_Queries.Add(new Query(i, type, parameters, msTimeout, msNextQuery));
            }
            s_QueriesOutLogManager?.AddEntry("Queries loaded: " + s_Queries.Count);
        }
        catch (Exception ex)
        {
            sectionsError.Add("INI Exception: " + ex.Message);
        }
    }
    static public void RunLoadedQueries()
    {
        s_QueriesOutLogManager?.AddEntry("Running Queries. Results: \nPositive: Result \n-2: Timed Out \n-3: Exception");
        List<Thread> threads = new List<Thread>();
        foreach (Query q in s_Queries)
        {
            if (q.QType == QueryTypeEnum.INSERTParcelData)
            {
                threads.Add(new Thread(() => INSERTParcelDataThread(q.Id, q.Parameters[0], int.Parse(q.Parameters[1]), int.Parse(q.Parameters[2]), int.Parse(q.Parameters[3]), int.Parse(q.Parameters[4]))));
                threads[threads.Count - 1].Start();
            }
            else if (q.QType == QueryTypeEnum.SELECTandDELETEMode)
            {
                //threads.Add(new Thread(() => SELECTandDELETEModeThread(q.Id)));
                //threads[threads.Count - 1].Start();
            }
            else if (q.QType == QueryTypeEnum.SELECTandDELETEchute)
            {
                threads.Add(new Thread(() => SELECTandDELETEchuteThread(q.Id)));
                threads[threads.Count - 1].Start();
            }
            else if (q.QType == QueryTypeEnum.INSERTFullChute)
            {
                threads.Add(new Thread(() => INSERTFullChuteThread(q.Id, q.Parameters[0], int.Parse(q.Parameters[1]))));
                threads[threads.Count - 1].Start();
            }
            Thread.Sleep(q.MsTimeNextQuery);
        }

        // join all threads
        foreach (Thread t in threads)
        {
            t.Join();
        }

        foreach (Query q in s_Queries)
        {
            s_QueriesOutLogManager?.AddEntry($"Query_{q.Id} of \nType: {q.QType} \nResult: {q.Result} \nTimeTaken: {q.MsTimeTaken} ms");
        }
    }

    // 1. Registro que graba IDESAI. Cuando la cinta lee un paquete, debe grabar un registro con el código de barras
    static public int SyncINSERTParcelData(string barcode, int pesoGramos, int largoCM, int anchoCM, int altoCM)
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

        if (s_ConectionTypeEnum == ConectionTypeEnum.ODBC)
        {
            //use new context
            using (OdbcConnection connection = new OdbcConnection(s_ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    string insertQuery = $"EXEC K10AltaCinta @cinta='K10CINT149_1', @CodigoBarras={barcode}, @PesoGramos={pesoGramos}, @Xcms={largoCM}, @Ycms={anchoCM}, @Zcms={altoCM}";
                    using (OdbcCommand insertCommand = new OdbcCommand(insertQuery, connection))
                    {
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        // -1 for successfull execution
                        if (rowsAffected == -1)
                        {
                            return 1;
                        }
                        return rowsAffected;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.InnerException.Message);
                    }
                    else
                    {
                        s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.Message);
                    }
                    return -3;
                }
            }
            return 0;
        }


        if (s_Context == null)
        {
            s_QueriesOutLogManager?.AddEntry("Context is null");
            return -3;
        }

        //use new context
        using (Gls149TestContext context = new Gls149TestContext(s_ConnectionString, s_ConectionTypeEnum))
        {
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
                // Raw Insert
                //context?.Univerres.Add(univerre);
                //context?.SaveChanges();

                // With Procedure
                int? result = 0;
                string query = $"EXEC K10AltaCinta @cinta='K10CINT149_1', @CodigoBarras={barcode}, @PesoGramos={pesoGramos}, @Xcms={largoCM}, @Ycms={anchoCM}, @Zcms={altoCM}";
                result =context?.Database.ExecuteSqlRaw(query);
                if (result == null)
                {
                    s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data error: No response from database");
                    return -3;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.InnerException.Message);
                }
                else
                {
                    s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.Message);
                }
                return -3;
            }
        }
        return 0;
    }
    static private void INSERTParcelDataThread(int query_id, string barcode, int pesoGramos, int largoCM, int anchoCM, int altoCM)
    {
        s_Queries[query_id].StartQuery();
        DateTime start = DateTime.Now;
        int result = SyncINSERTParcelData(barcode, pesoGramos, largoCM, anchoCM, altoCM);
        if (s_Queries[query_id].TimeOut < DateTime.Now)
        {
            s_Queries[query_id].Result = -2;
            s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
            return;
        }
        s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
        s_Queries[query_id].Result = result;
    }

    // Eliminado. No hace Nada
    // 2. Registro que graba GLS#149. Para parar la cinta, encenderla en modo carga o descarga.
    //static public int SyncSELECTandDELETEMode()
    //{
    //    //  Registro que graba GLS#149. Para parar la cinta, encenderla en modo carga o descarga.
    //    //  UVRTabla = K10CINT149_1
    //    //  UVRKeyN01 = 1
    //    //  UVRKeyN02 = 0 / 1 / 2(0 = STOP, 1 = ON modo carga, 2 = ON modo descarga)
    //    //  UVRKeyN02 puede valer 0(para la cinta), 1(encenderla en modo carga) o 2(encenderla en modo descarga)
    //    //use new context
    //    using (Gls149TestContext context = new Gls149TestContext(s_ConnectionString, s_ConectionTypeEnum))
    //    {

    //        int mode = -1;
    //        try
    //        {
    //            var univerre = context?.Univerres.Where(u => u.UVRTabla == "K10CINT149_1" && u.UVRKeyN01 == 1).FirstOrDefault();
    //            if (univerre != null)
    //            {
    //                mode = univerre.UVRKeyN02;
    //                context?.Univerres.Remove(univerre);
    //                context?.SaveChanges();
    //            }
    //            return mode;
    //        }
    //        catch (Exception ex)
    //        {
    //            if (ex.InnerException != null)
    //            {
    //                s_QueriesOutLogManager?.AddEntry("SELECT and DELETE Mode exception: " + ex.InnerException.Message);
    //            }
    //            else
    //            {
    //                s_QueriesOutLogManager?.AddEntry("SELECT and DELETE Mode exception: " + ex.Message);
    //            }
    //        }
    //    }

    //    return -3;
    //}
    //static private void SELECTandDELETEModeThread(int query_id)
    //{
    //    s_Queries[query_id].StartQuery();
    //    DateTime start = DateTime.Now;
    //    int result = -1;
    //    while (result == -1)
    //    {
    //        result = SyncSELECTandDELETEMode();
    //        if (s_Queries[query_id].TimeOut < DateTime.Now)
    //        {
    //            s_Queries[query_id].Result = -2;
    //            s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
    //            return;
    //        }
    //    }
    //    s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
    //    s_Queries[query_id].Result = result;
    //}

    // 3. Registro que graba GLS#149. Para decir porque carril debe desviar el paquete que acaba de leer.
    static public int SyncSELECTandDELETEchute()
    {
        //  Registro que graba GLS#149. Para decir porque carril debe desviar el paquete que acaba de leer.
        //  UVRTabla = K10CINT149_1
        //  UVRKeyN01 = 2

        //  UVRKeyN02 = Número de carril por donde debe desviar el paquete actual
        //use new context

        if (s_ConectionTypeEnum == ConectionTypeEnum.ODBC)
        {
            //use new context
            using (OdbcConnection connection = new OdbcConnection(s_ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    string query = $"EXEC K10Obtienecarril @cinta='K10CINT149_1'";
                    int ret = -1;
                    using (OdbcCommand selectCommand = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = selectCommand.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                ret = reader.GetInt32(0);
                            }
                            if (ret == -1)
                            {
                                return -3;
                            }
                        }
                    }
                    string query2 = $"EXEC K10BorraCarril @cinta='K10CINT149_1'";
                    using (OdbcCommand deleteCommand = new OdbcCommand(query2, connection))
                    {
                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        if (rowsAffected != -1)
                        {
                            return -3;
                        }
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.InnerException.Message);
                    }
                    else
                    {
                        s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.Message);
                    }
                    return -3;
                }
            }
            return 0;
        }


        using (Gls149TestContext context = new Gls149TestContext(s_ConnectionString, s_ConectionTypeEnum))
        {
            int lane = -1;
            try
            {
                // Raw Select and Delete
                //var univerre = context?.Univerres.Where(u => u.UVRTabla == "K10CINT149_1" && u.UVRKeyN01 == 2).FirstOrDefault();
                //if (univerre != null)
                //{
                //    lane = univerre.UVRKeyN02;
                //    context?.Univerres.Remove(univerre);
                //    context?.SaveChanges();
                //}

                // With Procedure
                string query = $"EXEC K10Obtienecarril @cinta='K10CINT149_1'";
                //var res = context?.Database
                //    .ExecuteSqlRaw(query)
                //    .Select(u => new { u.UVRKeyN02 }) // Now perform client-side projection
                //    .ToList();
                List<K10Obtienecarril>? res = context?.Set<K10Obtienecarril>().FromSqlRaw(query).ToList();
                if (res == null)
                {
                    return -3;
                }
                if (res.Count > 0)
                {
                    lane = res[0].UVRKeyN02;
                    string query2 = $"EXEC K10BorraCarril @cinta='K10CINT149_1'";
                    int? result = context?.Database.ExecuteSqlRaw(query2);
                }

                return lane;
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
        }
        return -3;
    }
    static private void SELECTandDELETEchuteThread(int query_id)
    {
        s_Queries[query_id].StartQuery();
        DateTime start = DateTime.Now;
        int result = -1;
        while (result == -1)
        {
            result = SyncSELECTandDELETEchute();
            if (s_Queries[query_id].TimeOut < DateTime.Now)
            {
                s_Queries[query_id].Result = -2;
                s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
                return;
            }
        }
        s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
        s_Queries[query_id].Result = result;
    }

    // 4. Registro que graba GLS#149. Para decir que el carril por donde debe desviar el paquete está saturado.
    static public int SyncINSERTFullChute(string barcode, int chute)
    {
        //  Registro que graba IDESAI. Cuando el carril por donde debe desviar el paquete está saturado, y la cinta lo
        //envía al 0, esta debe grabar un registro para un posterior tratamiento por parte del ERP.En este caso puede
        //haber más de un registro así que el valor identificativo del paquete(código de barras) debe estar como clave.
        //  UVRTabla = K10CINT149_1
        //  UVRKeyN01 = 4
        //  UVRKeyN02 = Número de carril saturado
        //  UVRKeyC01 = Código de barras
        //use new context

        if (s_ConectionTypeEnum == ConectionTypeEnum.ODBC)
        {
            //use new context
            using (OdbcConnection connection = new OdbcConnection(s_ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    string insertQuery = $"EXEC K10AltaCarrilSaturado @cinta='K10CINT149_1', @CodigoBarras={barcode}, @CarrilSaturado={chute}";
                    using (OdbcCommand insertCommand = new OdbcCommand(insertQuery, connection))
                    {
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        // -1 for successfull execution
                        if (rowsAffected == -1)
                        {
                            return 1;
                        }
                        return rowsAffected;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.InnerException.Message);
                    }
                    else
                    {
                        s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data exception: " + ex.Message);
                    }
                    return -3;
                }
            }
            return 0;
        }

        using (Gls149TestContext context = new Gls149TestContext(s_ConnectionString, s_ConectionTypeEnum))
        {
            Univerre univerre = new Univerre
            {
                UVRTabla = "K10CINT149_1",
                UVRKeyN01 = 4,
                UVRKeyN02 = chute,
                UvrkeyC01 = barcode
            };
            try
            {
                // With Procedure
                int? result = 0;
                string query = $"EXEC K10AltaCarrilSaturado @cinta='K10CINT149_1', @CodigoBarras={barcode}, @CarrilSaturado={chute}";
                result = context?.Database.ExecuteSqlRaw(query);
                if (result == null)
                {
                    s_QueriesOutLogManager?.AddEntry("INSERT Parcel Data error: No response from database");
                    return -3;
                }

                // Raw Insert
                //context?.Univerres.Add(univerre);
                //context?.SaveChanges();

                return 0;
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
        return -3;
    }
    static private void INSERTFullChuteThread(int query_id, string barcode, int rampaLlena)
    {
        s_Queries[query_id].StartQuery();
        DateTime start = DateTime.Now;
        int result = SyncINSERTFullChute(barcode, rampaLlena);
        if (s_Queries[query_id].TimeOut < DateTime.Now)
        {
            s_Queries[query_id].Result = -2;
            s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
            return;
        }
        s_Queries[query_id].MsTimeTaken = (int)(DateTime.Now - start).TotalMilliseconds;
        s_Queries[query_id].Result = result;
    }


    static public void FinalizeQueryTester()
    {
        s_Context?.Dispose();
        s_QueriesOutLogManager?.FinalizeLogManager();
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