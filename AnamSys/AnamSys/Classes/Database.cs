using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace AnamSys.Classes
{
    class Database
    {
        /*Estrutura
         * 
         * /////Paciente/////
         * id (pk)
         * nome
         * rg
         * cpf
         * endereco
         * bairro
         * cidade
         * uf
         * cep
         * obs
         * criado (data)
         * 
         * ///////consulta///////
         * 
         * id
         * paciente
         * faturas
         * hoje
         * datar45v ffffffffffffffffffffffffffffffffffffffffffffffffffffffg5ttttttttttttttttttttttttttttttv'1   r'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmbn                           bbb
         * 
         */

        private string conectString = @"Data Source=anamsys;Version=3;";

        private SQLiteConnection conexao;

        public Database ()
        {	
        }

        public System.Data.DataTable query(string query)
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
                    return dt;
                else
                    return null;
            }
            catch (Exception e)
            {
                string s = e.Message;
                return null;
            }
        }
        public string comando(string query)
        {
            try
            {
                if (conexao == null)
                    this.conexao = new SQLiteConnection(conectString);
                conexao.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, this.conexao);
                cmd.ExecuteNonQuery();
                conexao.Close();
                return "";
            }
            catch(Exception e)
            {
                return e.Message;
            }

        }

        public string proximo(string tabela, string primaryKey)
        {
            
            try
            {
                System.Data.DataTable dt = this.query("select " + primaryKey + " from " + tabela);
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
}
