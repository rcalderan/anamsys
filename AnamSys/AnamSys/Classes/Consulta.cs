using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnamSys.Classes
{
    class Consulta
    {
        private int id;
        private int paciente;
        private int anamnese;
        private DateTime hoje;
        private DateTime data;
        private List<int> faturas;
        private string detalhes;
        private string ficha;
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
        public string get_Ficha() { return this.ficha; }
        public void set_Ficha(string Ficha) { this.ficha = Ficha; }
        public bool get_Estado() { return this.ativa; }
        public void set_Estado(bool Estado) { this.ativa = Estado; }

        public static string listToDatabaseFormat(List<int> list)
        {
            string dt = "";
            foreach (int i in list)
                dt += i.ToString() + "";
            return dt;
        }
        public static List<int> stringStringToList(string faturas)
        {
            List<int> l = new List<int>();
            char[] ch = {' '};
            int aux=-1;
            foreach(string s in faturas.Split(ch))
                if (int.TryParse(s,out aux))
                    l.Add(aux);
            return l;
        }

        public Consulta()
        {
            Database conexao = new Database(); 
            int idd;
            if (!int.TryParse(conexao.proximo("consulta", "id"), out idd))
                this.id = 0;
            else
                this.id = idd;

        }
        public Consulta(int paciente, int anamnese, string avaliacao, string ficha, List<int> faturas, DateTime hoje,DateTime data,bool estadoDaConsulta)
        {
            Database conexao = new Database();
            int idd;
            if (paciente != 0)
                if (null== conexao.query("select id from paciente where id=" + paciente.ToString()))
                    paciente = 0;
            if (anamnese != 0)
                if (null == conexao.query("select id from anamnese where id=" + anamnese.ToString()))
                    anamnese = 0;
            if (!int.TryParse(conexao.proximo("consulta", "id"), out idd))
                id = 0;
            else
                this.id = idd;
            this.anamnese = anamnese;
            this.paciente = paciente;
            this.detalhes = avaliacao;
            this.ficha = ficha;
            this.hoje = hoje;
            this.data = data;
            this.ativa = estadoDaConsulta;
            this.faturas = faturas;
        }

        public string save()
        {
            try 
            {
                Database conexao = new Database();
                System.Data.DataTable dt = conexao.query("select id from consulta where id=" + id.ToString());
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
                            ficha + "'," +
                            anamnese + ",1,'" +
                            data.ToString("yyyy-MM-dd HH:mm:ss") + "')", erro = conexao.comando(query);
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
                        "',ficha='" + ficha + 
                        "',anamnese=" + anamnese + 
                        ",ativa="+ok+
                        ",data='" + data.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "' WHERE id="+this.id, erro = conexao.comando(query);
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
                dt = db.query("Select data from consulta");
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
