using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnamSys
{
    class Financeiro
    {
        public struct FormaDePagamento
        {
            private int Id;
            private string Name;

            private FormaDePagamento(int id, string name)
            {
                this.Id = id;
                this.Name = name;
            }

            public int get_Id()
            {
                return this.Id;
            }

            public static FormaDePagamento Dinheiro 
            { 
                get 
                {
                    FormaDePagamento din = new FormaDePagamento(0, "Dinheiro");
                    return din; 
                } 
            }
            public static FormaDePagamento Debito
            {
                get
                {
                    FormaDePagamento deb = new FormaDePagamento(1, "Débito");
                    return deb;
                }
            }
            public static FormaDePagamento Cheque 
            { 
                get
                {
                    FormaDePagamento forma = new FormaDePagamento(2, "Cheque");
                    return forma;
                } 
            }
            public static FormaDePagamento CreditoAVista 
            {
                get
                {
                    FormaDePagamento forma = new FormaDePagamento(3, "Crédito à Vista");
                    return forma;
                } 
            }
            public static FormaDePagamento CreditoParcelado 
            { 
                get
                {
                    FormaDePagamento forma = new FormaDePagamento(4, "Crédito Parcelado");
                    return forma;
                } 
            }

            public static FormaDePagamento None
            {
                get
                {
                    FormaDePagamento forma = new FormaDePagamento(-1, "Forma Desconhecida");
                    return forma;
                }
            }

            public static int GetId(FormaDePagamento forma)
            {
                if (forma.Equals(Dinheiro))
                    return Dinheiro.Id;
                else
                    if (forma.Equals(Debito))
                        return Debito.Id;
                    else
                        if (forma.Equals(Cheque))
                            return Cheque.Id;
                        else
                            if (forma.Equals(CreditoAVista))
                                return CreditoAVista.Id;
                            else
                                if (forma.Equals(CreditoParcelado))
                                    return CreditoParcelado.Id;
                                else
                                    return None.Id;
            }

            public static FormaDePagamento GetFromId(int id)
            {
                switch(id)
                {
                    case 0:
                        return Dinheiro;
                    case 1:
                        return Debito;
                    case 2:
                        return Cheque;
                    case 3:
                        return CreditoAVista;
                    case 4:
                        return CreditoParcelado;
                    default:
                        return None;
                }
            }
        }
    }
    
    class Fatura: Financeiro
    {
        private int id;
        private int consulta;
        private DateTime data;
        private DateTime hoje;
        private double valor;
        private FormaDePagamento forma;
        private int parcela;
        private bool pendencia;

        public int get_Id() { return this.id; }
        public int get_Consulta() { return this.consulta; }
        public FormaDePagamento get_Forma() { return this.forma; }
        public int get_Parcela() { return this.parcela; }
        public double get_Valor() { return this.valor; }

        public DateTime get_Data() { return this.data; }
        public DateTime get_Hoje() { return this.hoje; }
        public bool get_Pendencia() { return this.pendencia; }


        public Fatura(int Consulta, int Parcela, DateTime Data, double Valor, FormaDePagamento Forma, bool Pendencia)
        {
            try
            {//new Classes.Fatura(novaConsulta.get_Id(), 0, novaConsulta.get_Data(), val, conDetFormaCb.SelectedIndex, conDetParCh.Checked);
                Database conexao = new Database();
                System.Data.DataTable dt = conexao.query("select * from fatura where consulta=" + Consulta.ToString()+" and parcela="+Parcela.ToString());
                if (dt == null)
                {//novo
                    DateTime hj = DateTime.Now;
                    string novoId = conexao.proximo("fatura", "id"),pg;
                    if (pendencia)
                        pg = "1";
                    else
                        pg = "0";
                    string query = "INSERT INTO fatura VALUES(" + novoId + "," +
                             Consulta.ToString() + ",'" +
                             Data.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                             Valor + "'," +
                             Forma.get_Id() + "," +
                             Parcela.ToString() + ",'" +
                             pg + "','" +
                             hj.ToString("yyyy-MM-dd HH:mm:ss") + "')", erro = conexao.comando(query);
                    if (erro == "")
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
                    else
                        this.id = -1; 
                }
                else
                {
                    string erro,pend,query = "update fatura set data='" + Data.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "',valor='" + Valor + 
                        "',forma='" + Forma.get_Id() + "'";
                    if (Pendencia)
                        pend =  "1";
                    else
                        pend = "0";
                    query += ", pendencia ='"+pend+"' WHERE id='" + dt.Rows[0]["id"].ToString() + "' and parcela='" + parcela.ToString() + "'";
                    erro = conexao.comando(query);

                    DateTime aux = new DateTime(1111, 11, 11);
                    DateTime.TryParse(dt.Rows[0]["Hoje"].ToString(), out aux);
                    this.id = int.Parse(dt.Rows[0]["id"].ToString());
                    this.consulta = Consulta;
                    this.parcela = Parcela;
                    this.data = Data;
                    this.valor = Valor;
                    this.forma = Forma;
                    this.pendencia = Pendencia;
                    this.hoje = aux;
                    if (erro != "")
                    {
                        System.Windows.Forms.MessageBox.Show("erro ao atualizar fatura... contate o programador...");
                        this.id = -1;
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
                    this.hoje = DateTime.Parse(dt.Rows[0]["hoje"].ToString());
                    this.valor = double.Parse(dt.Rows[0]["valor"].ToString());
                    this.forma = Financeiro.FormaDePagamento.GetFromId(int.Parse(dt.Rows[0]["forma"].ToString()));
                    this.parcela = int.Parse(dt.Rows[0]["parcela"].ToString());
                    this.pendencia = bool.Parse(dt.Rows[0]["pendendia"].ToString());
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

                    this.forma = Financeiro.FormaDePagamento.GetFromId(int.Parse(dt.Rows[0]["forma"].ToString()));
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
        /*
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
        */
        public bool updatefromList(List<int> faturas)
        {
            try
            {
                string query = "select * from faturas where";
                foreach(int f in faturas)
                {
                    query += " id=" + f.ToString()+" or";
                }
                if (query.LastIndexOf("or") != -1)
                    query = query.Substring(0, query.Length - 3);
                Database con = new Database();
                System.Data.DataTable dt = con.query(query);
                if (dt!=null)
                {

                }
                return true;
            }
            catch
            {
                return false;
            }
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
                    System.Data.DataTable dt = conexao.query("select * from fatura where consulta='" + this.consulta+"' and parcela ='"+this.get_Parcela().ToString()+"'");
                    if (dt == null)
                    {//novo
                       string prox_id = conexao.proximo("fatura","id");
                       string query = "INSERT INTO fatura VALUES(" + prox_id + "," +
                                this.consulta + ",'" +
                                this.data.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                                this.valor + "','" +
                                Financeiro.FormaDePagamento.GetId(this.forma) + "'," +
                                this.parcela.ToString() + ",'";
                        if (this.pendencia)
                            query+="1";
                        else
                            query+="0";
                        query+= "','" +this.hoje.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        return conexao.comando(query);
                    }
                    else
                    {
                        string query = "UPDATE fatura SET data='" + this.data.ToString("yyyy-MM-dd HH:mm:ss") +
                            "',valor='" + this.valor +
                            "',forma='" + this.forma.get_Id() +
                            "',consulta='" + this.consulta.ToString() +
                            "',pendencia='";
                        if (this.pendencia)
                            query += "1'";
                        else
                            query += "0'";
                        query+=" WHERE id='" + dt.Rows[0]["id"].ToString() + "'";
                        return conexao.comando(query);
                    }
                }
            }
            catch(Exception e)
            {
                return "Erro ao faturar: "+e.Message;
            }
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
