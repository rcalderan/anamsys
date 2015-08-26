using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnamSys
{
    class Consulta
    {
        private int id;
        private int paciente;
        private int anamnese;
        private DateTime hoje;
        private DateTime data;
        private string detalhes;
        private string plano;
        private bool ativa;
        
        public int get_Id() { return this.id; }
        public int get_Paciente() { return this.paciente; }
        public void set_Paciente(int paciente) { this.paciente = paciente; }
        public int get_Anamnese() { return this.anamnese; }
        public void set_Anamnese(int Anamnese) { this.anamnese = Anamnese; }
        public DateTime get_Hoje() { return this.hoje; }
        public void set_Hoje(DateTime Hoje) { this.hoje = Hoje; }
        public DateTime get_Data() { return this.data; }
        public void set_Data(DateTime Data) { this.data = Data; }
        public string get_Detalhes() { return this.detalhes; }
        public void set_Detalhes(string detalhes) { this.detalhes = detalhes; }
        public string get_Plano() { return this.plano; }
        public void set_Plano(string plano) { this.plano = plano; }
        public bool get_Estado() { return this.ativa; }
        public void set_Estado(bool Estado) { this.ativa = Estado; }

        public static string listToDatabaseFormat(List<int> list)
        {
            string dt = "";
            foreach (int i in list)
                dt += i.ToString() + "";
            return dt;
        }

        public Consulta()
        {
        }

        public static Consulta Load(int id)
        {
            try
            {
                Database db = new Database();
                if (db.getLastState())
                {
                    Consulta c = new Consulta();
                    System.Data.DataTable dt = db.Query("select * from consulta where id=" + id);
                    if (dt != null)
                    {
                        c.id = Convert.ToInt32(id);
                        c.set_Anamnese(Convert.ToInt32(dt.Rows[0]["anamnese"]));
                        c.set_Paciente(Convert.ToInt32(dt.Rows[0]["paciente"]));
                        c.set_Detalhes(dt.Rows[0]["detalhes"].ToString());
                        c.set_Plano(dt.Rows[0]["plano"].ToString());
                        c.set_Hoje(Convert.ToDateTime(dt.Rows[0]["hoje"]));
                        c.set_Data(Convert.ToDateTime(dt.Rows[0]["data"]));
                        c.set_Estado(bool.Parse(dt.Rows[0]["ativa"].ToString()));
                        return c;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch { return null; }

        }

        public Consulta(int paciente, int anamnese, string detalhes, string plano, DateTime hoje,DateTime data,bool estadoDaConsulta)
        {
            Database conexao = new Database();
            int idd;
            if (paciente != 0)
                if (null== conexao.Query("select id from paciente where id=" + paciente.ToString()))
                    paciente = 0;
            if (anamnese != 0)
                if (null == conexao.Query("select id from anamnese where id=" + anamnese.ToString()))
                    anamnese = 0;
            if (!int.TryParse(conexao.proximo("consulta", "id"), out idd))
                id = 0;
            else
                this.id = idd;
            this.anamnese = anamnese;
            this.paciente = paciente;
            this.detalhes = detalhes;
            this.plano = plano;
            this.hoje = hoje;
            this.data = data;
            this.ativa = estadoDaConsulta;
        }

        public static bool New(int paciente, int anamnese, string detalhes, string plano, DateTime data, bool estadoDaConsulta)
        {
            try
            {
                Database conexao = new Database();
                if (conexao.getLastState())
                {
                    string ativa= "0";
                    if (estadoDaConsulta)
                        ativa = "1";
                    string query = "INSERT INTO consulta ('id','paciente','detalhes','plano','anamnese','data','hoje','ativa') VALUES (NULL," + paciente + ",'" + detalhes + "','" + plano + "'," + anamnese + ",'" + data.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"+ativa+")";
                    if ("" != conexao.Comando(query))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch { return false; }

        }

        public string save()
        {
            try 
            {
                Database conexao = new Database();
                System.Data.DataTable dt = conexao.Query("select id from consulta where id=" + id.ToString());
                if (dt==null)
                {//novo
                    int idd = this.id;
                    if (!int.TryParse(conexao.proximo("consulta", "id"), out idd))
                        id = 0;
                    else
                        id = idd;
                    string query = "INSERT INTO consulta VALUES("+id.ToString()+"," +
                            paciente + ",'" +
                            hoje.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                            detalhes + "','" +
                            plano + "'," +
                            anamnese + ",1,'" +
                            data.ToString("yyyy-MM-dd HH:mm:ss") + "')", erro = conexao.Comando(query);
                    return erro;
                }
                else
                {//atualiza
                    string ok = "0";
                    if (this.ativa)
                        ok = "1";
                    string query = "UPDATE consulta set paciente="+ paciente + 
                        ",hoje='" + hoje.ToString("yyyy-MM-dd HH:mm:ss") +
                        "',detalhes='" + detalhes + 
                        "',plano='" + plano + 
                        "',anamnese=" + anamnese + 
                        ",ativa="+ok+
                        ",data='" + data.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "' WHERE id="+this.id, erro = conexao.Comando(query);
                    return erro;
                }
            }
            catch(Exception erro)
            {
                return erro.Message;
            }

        }

        public static List<DateTime> todasDatas()
        {
            try
            {
                Database db = new Database();
                List<DateTime> datas= new List<DateTime>();
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = db.Query("Select data from consulta");
                if (dt == null)
                    return datas;
                else
                {
                    DateTime aux=new DateTime();
                    foreach (System.Data.DataRow r in dt.Rows)
                        if ((DateTime.TryParse(r["data"].ToString(), out aux)) && (!datas.Contains(aux)))
                            datas.Add(aux);
                    return datas;
                }
            }
            catch
            {
                return new List<DateTime>();
            }
        }

    }
}
