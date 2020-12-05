using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace cgtd
{
    public class DataAccess
    {
        public static Byte[] DownloadFile(string db, string query, string col)
        {
            Byte[] data = null;
            Byte[] rawData;

            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var myData = cmd.ExecuteReader();
                    myData.Read();
                    rawData = new Byte[17000000];
                    myData.GetBytes(myData.GetOrdinal(col), 0, rawData, 0, 17000000);
                    data = rawData;
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            return data;
        }

        public static string ProsesDataFX(string[] user, string[] type, string[] mkey, string[] dkey, string[] field, string[] data)
        {
            string r = "";
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Process";
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@UserId", user[0]);
                    spc.AddWithValue("@DepartmentId", user[1]);
                    spc.AddWithValue("@Factory", user[2]);
                    spc.AddWithValue("@Table", type[1]);
                    spc.AddWithValue("@TableType", type[2]);
                    spc.AddWithValue("@ProcessType", type[3]);
                    spc.AddWithValue("@MasterKey", mkey[0]);
                    spc.AddWithValue("@MasterKey2", mkey[1]);
                    spc.AddWithValue("@DetailKey", dkey[0]);
                    spc.AddWithValue("@DetailKey2", dkey[1]);
                    spc.AddWithValue("@DetailKey3", dkey[2]);

                    int s = 0, e = 0;
                    e = field.Length;

                    while (e > s)
                    {
                        try
                        {
                            if (data[s] == null)
                            {
                                if (field[s].Substring(0, 2) == "N_")
                                    data[s] = "0";
                                else
                                    data[s] = null;
                            }
                            else
                            {
                                if (!(data[s].ToString() == "   ") && (data[s].ToString().Trim() == "" || data[s].Trim() == "__/__/____")) // set null for empty spaces
                                    data[s] = null;
                                else if (field[s] != null && field[s].Substring(0, 2) == "N_") // Numeric
                                    spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), Tools.StringDecimalIDToUS(data[s].Replace(",", "").Replace(".", ",")));
                                else if (field[s] != null && field[s].Substring(0, 2) == "D_") // DateTime // For FX data only as we cannot control date format so better to pass it as string and handle the conversion in SQLServer
                                    spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), data[s].Replace("'", "''"));
                                    //spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), Convert.ToDateTime(data[s]).ToString("yyyy/MM/dd"));
                                else if (field[s] != null)
                                {
                                    string str;
                                    str = data[s].Replace("'", "''");
                                    str = Regex.Replace(str, " +( |$)", "$1");
                                    spc.AddWithValue(field[s], str);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            r = ex.Message.ToString();
                            if (r.IndexOf('\r') > 0)
                                r = r.Substring(0, r.IndexOf('\r'));
                            return r;
                        }
                        s += 1;
                    }
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqle)
                    {
                        r = sqle.Message.ToString();
                        if (r.IndexOf('\r') > 0)
                            r = r.Substring(0, r.IndexOf('\r'));
                    }
                }
                conn.Close();
                conn.Dispose();
            }
            return r.Replace("'", "");
        }

        public static string ProsesData(string[] user, string[] type, string[] mkey, string[] dkey, string[] field, string[] data)
        {
            string r = "";
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Process";
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@UserId", user[0]);
                    spc.AddWithValue("@DepartmentId", user[1]);
                    spc.AddWithValue("@Factory", user[2]);
                    spc.AddWithValue("@Table", type[1]);
                    spc.AddWithValue("@TableType", type[2]);
                    spc.AddWithValue("@ProcessType", type[3]);
                    spc.AddWithValue("@MasterKey", mkey[0]);
                    spc.AddWithValue("@MasterKey2", mkey[1]);
                    spc.AddWithValue("@DetailKey", dkey[0]);
                    spc.AddWithValue("@DetailKey2", dkey[1]);
                    spc.AddWithValue("@DetailKey3", dkey[2]);

                    int s = 0, e = 0;
                    e = field.Length;

                    while (e > s)
                    {
                        try
                        {
                            if (data[s]==null)
                            {
                                if (field[s].Substring(0, 2) == "N_")
                                    data[s] = "0";
                                else
                                    data[s] = null;
                            }
                            else
                            {
                                if (!(data[s].ToString() == "   ") && (data[s].ToString().Trim() == "" || data[s].Trim() == "__/__/____")) // set null for empty spaces
                                    data[s] = null;
                                else if (field[s] != null && field[s].Substring(0, 2) == "N_") // Numeric
                                    spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), Tools.StringDecimalIDToUS(data[s].Replace(".", ",")));
                                else if (field[s] != null && field[s].Substring(0, 2) == "D_") // DateTime
                                    spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), Convert.ToDateTime(data[s]).ToString("yyyy/MM/dd"));
                                else if (field[s] != null)
                                {
                                    string str;
                                    str = data[s].Replace("'", "''");
                                    str = Regex.Replace(str, " +( |$)", "$1");
                                    spc.AddWithValue(field[s], str);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            r = ex.Message.ToString();
                            if (r.IndexOf('\r') > 0)
                                r = r.Substring(0, r.IndexOf('\r'));
                            return r;
                        }
                        s += 1;
                    }
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqle)
                    {
                        r = sqle.Message.ToString();
                        if (r.IndexOf('\r') > 0)
                            r = r.Substring(0, r.IndexOf('\r'));
                    }
                }
                conn.Close();
                conn.Dispose();
            }
            return r.Replace("'", "");
        }

        public static DataTable RetriveData(string[] user, string[] tabel, string[] key)
        {
            DataTable table = new DataTable();
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Get";

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;

                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@UserID", user[0]);
                    spc.AddWithValue("@DepartmentID", user[1]);
                    spc.AddWithValue("@Factory", user[2]);
                    spc.AddWithValue("@Table", tabel[1]);
                    spc.AddWithValue("@Type", tabel[2]);
                    spc.AddWithValue("@Field", tabel[3]);

                    if (key[0] != null) spc.AddWithValue("@Key1", key[0]);
                    if (key[1] != null) spc.AddWithValue("@Key2", key[1]);
                    if (key[2] != null) spc.AddWithValue("@Key3", key[2]);
                    if (key[3] != null) spc.AddWithValue("@Key4", key[3]);
                    if (key[4] != null) spc.AddWithValue("@Key5", key[4]);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(table);
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            return table;
        }

        public static DataTable RetriveDataEx(string[] user, string[] tabel, string[] key)
        {
            DataTable table = new DataTable();
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Get";

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@UserID", user[0]);
                    spc.AddWithValue("@DepartmentID", user[1]);
                    spc.AddWithValue("@Factory", user[2]);
                    spc.AddWithValue("@Table", tabel[1]);
                    spc.AddWithValue("@Type", tabel[2]);
                    spc.AddWithValue("@Field", tabel[3]);

                    if (key[0] != null) spc.AddWithValue("@Key1", key[0]);
                    if (key[1] != null) spc.AddWithValue("@Key2", key[1]);
                    if (key[2] != null) spc.AddWithValue("@Key3", key[2]);
                    if (key[3] != null) spc.AddWithValue("@Key4", key[3]);
                    if (key[4] != null) spc.AddWithValue("@Key5", key[4]);
                    try { if (key[5] != null) spc.AddWithValue("@Date1", Convert.ToDateTime(key[5]).ToString("yyyy/MM/dd")); }
                    catch (Exception) { };
                    try { if (key[6] != null) spc.AddWithValue("@Date2", Convert.ToDateTime(key[6]).ToString("yyyy/MM/dd")); }
                    catch (Exception) { };
                    if (key[7] != null) spc.AddWithValue("@Int1", Tools.StringDecimalIDToUS(key[7]));
                    if (key[8] != null) spc.AddWithValue("@Int2", Tools.StringDecimalIDToUS(key[8]));
                    if (key[9] != null) spc.AddWithValue("@Int3", Tools.StringDecimalIDToUS(key[9]));


                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(table);
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            return table;
        }

        public static string RetriveInfo(string[] user, string[] tabel, string[] key)
        {
            string r = "";
            DataTable table = new DataTable();
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Info";

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@UserID", user[0]);
                    spc.AddWithValue("@DepartmentID", user[1]);
                    spc.AddWithValue("@Factory", user[2]);
                    spc.AddWithValue("@Table", tabel[1]);
                    spc.AddWithValue("@Type", tabel[2]);
                    spc.AddWithValue("@Field", tabel[3]);

                    if (key[0] != null) spc.AddWithValue("@Key1", key[0]);
                    if (key[1] != null) spc.AddWithValue("@Key2", key[1]);
                    if (key[2] != null) spc.AddWithValue("@Key3", key[2]);
                    if (key[3] != null) spc.AddWithValue("@Key4", key[3]);
                    if (key[4] != null) spc.AddWithValue("@Key5", key[4]);
                    try { if (key[5] != null) spc.AddWithValue("@Date1", Convert.ToDateTime(key[5]).ToString("yyyy/MM/dd")); }
                    catch (Exception ) { };
                    try { if (key[6] != null) spc.AddWithValue("@Date2", Convert.ToDateTime(key[6]).ToString("yyyy/MM/dd")); }
                    catch (Exception ) { };
                    if (key[7] != null) spc.AddWithValue("@Int1", Tools.StringDecimalIDToUS(key[7]));
                    if (key[8] != null) spc.AddWithValue("@Int2", Tools.StringDecimalIDToUS(key[8]));
                    if (key[9] != null) spc.AddWithValue("@Int3", Tools.StringDecimalIDToUS(key[9]));


                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(table);
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }

            r = table.DefaultView[0][0].ToString();
            return r;
        }

        public static DataTable RetriveDownload(string[] key)
        {
            DataTable table = new DataTable();
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "rpt_TransDownload";

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    if (key[0] != null) spc.AddWithValue("@TType", key[0]);
                    if (key[1] != null) spc.AddWithValue("@BCType", key[1]);
                    if (key[2] != null) spc.AddWithValue("@BCNo", key[2]);
                    if (key[3] != null) spc.AddWithValue("@No", key[3]);
                    if (key[4] != null) spc.AddWithValue("@ProductId", key[4]);
                    if (key[2] != null) spc.AddWithValue("@SupplierId", key[5]);
                    if (key[3] != null) spc.AddWithValue("@CustomerId", key[6]);
                    if (key[4] != null) spc.AddWithValue("@UserId", key[7]);

                    try { if (key[8] != null) spc.AddWithValue("@Date", Convert.ToDateTime(key[8]).ToString("yyyy/MM/dd")); }
                    catch (Exception ) { };
                    try { if (key[9] != null) spc.AddWithValue("@BCDate", Convert.ToDateTime(key[9]).ToString("yyyy/MM/dd")); }
                    catch (Exception ) { };
                    try { if (key[10] != null) spc.AddWithValue("@DateFrom", Convert.ToDateTime(key[10]).ToString("yyyy/MM/dd")); }
                    catch (Exception ) { };
                    try { if (key[11] != null) spc.AddWithValue("@DateTo", Convert.ToDateTime(key[11]).ToString("yyyy/MM/dd")); }
                    catch (Exception ) { };

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(table);
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            return table;
        }

        public static DataTable RetriveDataReport(string SPName, string DateFrom, string DateTo, string ID)
        {
            DataTable table = new DataTable();
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = SPName;

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@DateFrom", DateFrom);
                    spc.AddWithValue("@DateTo", DateTo);
                    spc.AddWithValue("@ID", ID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(table);
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            return table;
        }

        public static DataTable LookData(string[] tabel, string[] key)
        {
            DataTable table = new DataTable();
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Look";
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@Table", tabel[1]);
                    if (key[0] != null) spc.AddWithValue("@Key1", key[0]);
                    if (key[1] != null) spc.AddWithValue("@Key2", key[1]);
                    if (key[2] != null) spc.AddWithValue("@Key3", key[2]);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(table);
                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            return table;
        }

        public static string CheckData(string[] user, string[] type, string[] mkey, string[] dkey, string[] field, string[] data)
        {
            string r = "";
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = "Transaction_Check";
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    SqlParameterCollection spc = cmd.Parameters;
                    spc.AddWithValue("@UserId", user[0]);
                    spc.AddWithValue("@DepartmentId", user[1]);
                    spc.AddWithValue("@Factory", user[2]);
                    spc.AddWithValue("@Table", type[1]);
                    spc.AddWithValue("@TableType", type[2]);
                    spc.AddWithValue("@ProcessType", type[3]);
                    spc.AddWithValue("@MasterKey", mkey[0]);
                    spc.AddWithValue("@MasterKey2", mkey[1]);
                    spc.AddWithValue("@DetailKey", dkey[0]);
                    spc.AddWithValue("@DetailKey2", dkey[1]);
                    spc.AddWithValue("@DetailKey3", dkey[2]);

                    int s = 0, e = 0;
                    e = field.Length;

                    while (e > s)
                    {
                        try
                        {
                            if (data[s] == null)
                            {
                                if (field[s].Substring(0, 2) == "N_")
                                    data[s] = "0";
                                else
                                    data[s] = null;
                            }
                            else
                            {
                                if (!(data[s].ToString() == "   ") && (data[s].ToString().Trim() == "" || data[s].Trim() == "__/__/____")) // set null for empty spaces
                                    data[s] = null;
                                else if (field[s] != null && field[s].Substring(0, 2) == "N_") // Numeric
                                    spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), Tools.StringDecimalIDToUS(data[s]));
                                else if (field[s] != null && field[s].Substring(0, 2) == "D_") // DateTime
                                    spc.AddWithValue(field[s].Substring(2, field[s].Length - 2), Convert.ToDateTime(data[s]).ToString("yyyy/MM/dd"));
                                else if (field[s] != null)
                                {
                                    string str;
                                    str = data[s].Replace("'", "''");
                                    str = Regex.Replace(str, " +( |$)", "$1");
                                    spc.AddWithValue(field[s], str);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            r = ex.Message.ToString();
                            if (r.IndexOf('\r') > 0)
                                r = r.Substring(0, r.IndexOf('\r'));
                            return r;
                        }
                        s += 1;
                    }
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqle)
                    {
                        r = sqle.Message.ToString();
                        if (r.IndexOf('\r') > 0)
                            r = r.Substring(0, r.IndexOf('\r'));
                    }
                }
                conn.Close();
                conn.Dispose();
            }
            return r.Replace("'", "");
        }

        public static string ExecuteSP(string SPName)
        {
            string r = "";
            string cstr = "Data Source=" + Var.SERVERIP + ";User ID=sa;Password=" + Var.SERVERPASSWORD + ";Initial Catalog=" + Var.DATABASE + ";Connection Timeout=1800;";
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                conn.Open();
                string sqlStr = SPName;
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqle)
                    {
                        r = sqle.Message.ToString();
                        if (r.IndexOf('\r') > 0)
                            r = r.Substring(0, r.IndexOf('\r'));
                    }
                }
                conn.Close();
                conn.Dispose();
            }
            return r.Replace("'", "");
        }
    }
}