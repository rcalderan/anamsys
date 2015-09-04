using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace AnamSys
{
    class Database
    {
        private string conectString = @"Data Source=anamsys;Version=3;";

        private SQLiteConnection conexao;

        private bool state = false;

        public bool getLastState()
        {
            return state;
        }

        public Database ()
        {	
            try
            {
                this.conexao = new SQLiteConnection(conectString);
                state = DataBaseCheck.Check(conexao);
            }
            catch
            {
                state = false;
            }
        }

        public System.Data.DataTable Query(string query)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                this.conexao = new SQLiteConnection(conectString);
                conexao.Open();
                SQLiteDataAdapter adap = new SQLiteDataAdapter(query, this.conexao);
                conexao.Close();
                adap.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    state = true;
                    return dt;
                }
                else
                {
                    state = true;
                    return null;
                }
            }
            catch(Exception e)
            {
                string msg = e.Message;
                state = false;
                return null;
            }
        }
        public string Comando(string query)
        {
            try
            {
                if (conexao == null)
                    this.conexao = new SQLiteConnection(conectString);
                conexao.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, this.conexao);
                cmd.ExecuteNonQuery();
                conexao.Close();
                state = true;
                return "";
            }
            catch(Exception e)
            {
                state = false;
                return e.Message;
            }

        }

        public string proximo(string tabela, string primaryKey)
        {
            
            try
            {
                System.Data.DataTable dt = this.Query("select " + primaryKey + " from " + tabela);
                if (dt != null)
                {
                    int aux,ret=1;
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        if (int.TryParse(dr[primaryKey].ToString(), out aux))
                        {
                            if (aux != ret)
                                return ret.ToString();
                            else
                                ret++;
                        }
                    }
                    return ret.ToString();
                }
                else
                    return "1";
            }
            catch
            {
                return "1";
            }
            
        }
    }
    class DataBaseCheck : Database
    {
        private static Dictionary<string, string> sqlite_tables = new Dictionary<string, string>(){
            {"consulta","CREATE TABLE 'consulta' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'paciente' INTEGER NOT NULL,'hoje' DATETIME NOT NULL DEFAULT CURRENT_DATE,'detalhes' TEXT,'plano' TEXT, 'anamnese' INTEGER, 'ativa' INTEGER NOT NULL DEFAULT 1,'data' DATETIME)"},
            {"paciente","CREATE TABLE 'paciente' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'nome' TEXT, 'rg' TEXT, 'cpf' TEXT, 'endereco' TEXT, 'bairro' TEXT, 'cidade' TEXT, 'uf' TEXT,'nascimento' DATETIME,'obs' TEXT, 'hoje' DATETIME, 'plano' TEXT DEFAULT '', 'avatar' TEXT DEFAULT 'avatar.png')"},
            {"fatura","CREATE TABLE 'fatura' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,'consulta' INTEGER,'data' DATETIME, 'valor' REAL, 'forma' NUMERIC, 'parcela' INTEGER, 'pendencia' BOOLEAN, 'hoje' DATETIME)"}
        };

        private static Dictionary<string, string> sqlite_FirstInsert = new Dictionary<string, string>(){
            {"consulta",""},
            {"paciente",""},
            {"fatura",""}
        };

        public static bool Check(SQLiteConnection con)
        {
            try
            {
                con.Open();
                SQLiteDataAdapter adap = new SQLiteDataAdapter("SELECT name FROM sqlite_master WHERE type='table'", con);
                SQLiteCommand cmd;
                System.Data.DataTable dt = new System.Data.DataTable();
                adap.Fill(dt);
                if (dt.Rows.Count>0)
                {
                    System.Data.DataColumn[] key = { dt.Columns["name"] };
                    dt.PrimaryKey = key;
                    foreach (KeyValuePair<string, string> pair in DataBaseCheck.sqlite_tables)
                    {
                        if (!dt.Rows.Contains(pair.Key))
                        {
                            cmd = new SQLiteCommand(pair.Value, con);
                            cmd.ExecuteNonQuery();
                            if (DataBaseCheck.sqlite_FirstInsert[pair.Key] != "")
                            {
                                cmd = new SQLiteCommand(DataBaseCheck.sqlite_FirstInsert[pair.Key], con);
                                cmd.ExecuteNonQuery();
                            }

                        }
                    }
                }
                else
                    foreach (KeyValuePair<string, string> pair in DataBaseCheck.sqlite_tables)
                    {
                        cmd = new SQLiteCommand(pair.Value, con);
                        cmd.ExecuteNonQuery();
                        if (DataBaseCheck.sqlite_FirstInsert[pair.Key] != "")
                        {
                            cmd = new SQLiteCommand(DataBaseCheck.sqlite_FirstInsert[pair.Key], con);
                            cmd.ExecuteNonQuery();
                        }
                    }
                con.Close();
                return true;
            }
            catch
            {
                con.Close(); 
                return false;
            }
        }
    }

}
