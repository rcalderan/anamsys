using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnamSys.Classes
{
    class Financeiro
    {
    }
    
    class Fatura: Financeiro
    {
        private int id;
        private int consulta;
        private DateTime data;
        private DateTime hoje;
        private double valor;
        private int forma;
        private int parcela;
        private bool pendencia;

        public int get_Id() { return this.id; }
        public int get_Consulta() { return this.consulta; }
        public int get_Forma() { return this.forma; }
        public int get_Parcela() { return this.parcela; }
        public double get_Valor() { return this.valor; }

        public DateTime get_Data() { return this.data; }
        public DateTime get_Hoje() { return this.hoje; }
        public bool get_Pendencia() { return this.pendencia; }


        public Fatura(int Consulta, int Parcela, DateTime Data, double Valor, int Forma, bool Pendencia)
        {
            try
            {//new Classes.Fatura(novaConsulta.get_Id(), 0, novaConsulta.get_Data(), val, conDetFormaCb.SelectedIndex, conDetParCh.Checked);
                Database conexao = new Database();
                System.Data.DataTable dt = conexao.query("select id from fatura where consulta=" + Consulta.ToString()+" and parcela="+Parcela.ToString());
                if (dt == null)
                {//novo
                    DateTime hj = DateTime.Now;
                    string novoId = conexao.proximo("fatura", "id");
                    string query = "INSERT INTO fatura VALUES(" + novoId + "," +
                             Consulta.ToString() + ",'" +
                             Data.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                             Valor + "'," +
                             Forma.ToString() + "," +
                             Parcela.ToString() + ",'" +
                             Pendencia.ToString() + "','" +
                             hj.ToString("yyyy-MM-dd HH:mm:ss") + "')", erro = conexao.comando(query);
                    if (erro != "")
                        this.id = -1;
                    else
                    {
                        this.id = int.Parse(novoId);
                        this.consulta = Consulta;
                        this.parcela = Parcela;
                        this.data = Data;
                        this.valor = Valor;
                        this.forma = Forma;
                        this.pendencia = Pendencia;
                        this.hoje = hj;
                    }
                }
            }
            catch
            {
                this.id = -1;
            }
        }

        public Fatura(string Id)
        {
            try
            {
                Database conexao = new Database();
                System.Data.DataTable dt = conexao.query("select * from fatura where id="+Id);
                if (dt != null)
                {
                    this.id = int.Parse(dt.Rows[0]["id"].ToString());

                    this.consulta = int.Parse(dt.Rows[0]["consulta"].ToString());
                    this.data = DateTime.Parse(dt.Rows[0]["data"].ToString());
                    this.valor = double.Parse(dt.Rows[0]["valor"].ToString());

                    this.forma = int.Parse(dt.Rows[0]["forma"].ToString());
                    this.parcela = int.Parse(dt.Rows[0]["parcela"].ToString());
                    this.pendencia = bool.Parse(dt.Rows[0]["pendendia"].ToString());
                    this.hoje = DateTime.Parse(dt.Rows[0]["hoje"].ToString());
                }
                else
                    this.id = -1;
            }
            catch
            {
                this.id = -1;
            }
        }

        public bool load(int id)
        {
            try
            {
                Database conexao = new Database();
                System.Data.DataTable dt = conexao.query("select * from fatura where id=" + id.ToString());
                if (dt == null)
                    return false;
                else
                {
                    this.id = int.Parse(dt.Rows[0]["id"].ToString());

                    this.consulta = int.Parse(dt.Rows[0]["consulta"].ToString());
                    this.data = DateTime.Parse(dt.Rows[0]["data"].ToString());
                    this.valor = double.Parse(dt.Rows[0]["valor"].ToString());

                    this.forma = int.Parse(dt.Rows[0]["forma"].ToString());
                    this.parcela = int.Parse(dt.Rows[0]["parcela"].ToString());
                    this.pendencia = bool.Parse(dt.Rows[0]["pendendia"].ToString());
                    this.hoje = DateTime.Parse(dt.Rows[0]["hoje"].ToString());
                    return true;
                }

            }
            catch { return false; }
        }
        public static double[] stringStringToDoubleArray(string taxas)
        {
            string[] l = taxas.Split(' ');
            double[] taxes = new double[l.Length];

            double aux = 0;
            for (int i = 0; i < l.Length;i++ )
            {
                if (double.TryParse(l[i], out aux))
                    taxes[i] = aux;
                else
                    taxes[i] = 0;
            }
            return taxes;
        }

        public bool update(Fatura newF)
        {
            try
            {
                if (this.id != newF.get_Id())
                    return false;
                string query = "update fatura set ";
                if (this.consulta != newF.get_Consulta())
                    query += "consulta=" + newF.get_Consulta() + ",";
                if (this.data.CompareTo(newF.get_Data())!=0)
                    query += "data='" + newF.get_Data().ToString("yyyy-MM-dd HH:mm:ss") + "',";
                if (this.valor != newF.get_Valor())
                    query += "valor='" + newF.get_Valor() + "',";
                if (this.forma != newF.get_Forma())
                    query += "forma=" + newF.get_Forma() + ",";
                if (this.parcela != newF.get_Parcela())
                    query += "parcela=" + newF.get_Valor() + ",";
                if (this.pendencia != newF.get_Pendencia())
                {
                    if (newF.get_Pendencia())
                        query += "valor=1,";
                    else
                        query += "valor=0,";
                }
                if (',' == query[query.Length - 1])
                    query = query.Substring(0, query.Length - 1);
                query += " WHERE id=" + newF.get_Id().ToString();
                Database con = new Database();
                if ("" == con.comando(query))
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }
        public string save()
        {
            try
            {
                if (this.id == -1)
                    return "Erro ao faturar: id= -1";
                else
                {
                    Database conexao = new Database();
                    System.Data.DataTable dt = conexao.query("select id from fatura where id=" + this.id.ToString());
                    if (dt == null)
                    {//novo
                       string query = "INSERT INTO fatura VALUES(" + this.id.ToString() + "," +
                                this.consulta + ",'" +
                                this.data.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                                this.valor + "'," +
                                this.forma.ToString() + "," +
                                this.parcela.ToString() + ",'" +
                                this.pendencia.ToString() + "','" +
                                this.hoje.ToString("yyyy-MM-dd HH:mm:ss") + "')", erro = conexao.comando(query);
                        return erro;
                    }
                    return "";
                }
            }
            catch(Exception e)
            {
                return "Erro ao faturar: "+e.Message;
            }
        }
    }

    struct FormaDePagamento
    {
        private static Dictionary<int, string> formas = new Dictionary<int, string>()
        {
            {0,"Dinheiro"},
            {1,"Debito"},
            {2,"Cheque"},
            {3,"CreditoAVista"},
            {4,"CreditoParcelado"}
        };
        static string Dinheiro = formas[0];
        static string Debito = formas[1];
        static string Cheque = formas[2];
        static string CreditoAVista = formas[3];
        static string CreditoParcelado = formas[4];

        static int getFormaCode(string forma)
        {
            try
            {
                foreach (var pair in formas)
                    if (pair.Value == forma)
                        return pair.Key;
                return -1;
            }
            catch { return -1; }

        }
    }

    /*
     
    class Forma : Financeiro
    {
        private int id;
        private float taxa;
        public int get_id() { return this.id; }
        public float get_taxa() { return this.taxa; }
       
        public Forma(int id)
        {
            try
            {
                Database db = new Database();
                System.Data.DataTable dt = db.query("Select * from forma where id=" + id);
                if (dt == null)
                    this.id = -1;
                else
                {
                    this.id = int.Parse(dt.Rows[0]["id"].ToString());
                    this.taxa = float.Parse(dt.Rows[0]["taxa"].ToString());
                }
            }
            catch
            {
                this.id = -1;
            }
        }

        public Forma(float taxa)
        {
            try
            {
                Database db = new Database();
                System.Data.DataTable dt = db.query("Select * from forma where id=" + id);
                if (dt == null)
                    this.id = -1;
                else
                {
                    this.id = int.Parse(dt.Rows[0]["id"].ToString());
                    this.taxa = float.Parse(dt.Rows[0]["taxa"].ToString());
                }
            }
            catch
            {
                this.id = -1;
            }
        }

        
    }
     * */
}
