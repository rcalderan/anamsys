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
        /*
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
        */
        public Consulta()
        {
            Database conexao = new Database(); 
            int idd;
            if (!int.TryParse(conexao.proximo("consulta", "id"), out idd))
                this.id = 0;
            else
                this.id = idd;

        }

        public Consulta(string id)
        {
            Database conexao = new Database();
            int idd;
            System.Data.DataTable dt = conexao.query("select * from consulta where id=" + id);
            if (dt==null)
                if (!int.TryParse(conexao.proximo("consulta", "id"), out idd))
                    this.id = 0;
                else
                    this.id = idd;
            else
            {
                this.id = Convert.ToInt32(id);
                this.anamnese = Convert.ToInt32(dt.Rows[0]["anamnese"]);
                this.paciente = Convert.ToInt32(dt.Rows[0]["paciente"]);
                this.detalhes = dt.Rows[0]["detalhes"].ToString();
                this.plano = dt.Rows[0]["plano"].ToString();
                this.hoje = Convert.ToDateTime(dt.Rows[0]["hoje"]);
                this.data = Convert.ToDateTime(dt.Rows[0]["data"]);
                this.ativa = true;
            }

        }

        public Consulta(int paciente, int anamnese, string detalhes, string plano, DateTime hoje,DateTime data,bool estadoDaConsulta)
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
            this.detalhes = detalhes;
            this.plano = plano;
            this.hoje = hoje;
            this.data = data;
            this.ativa = estadoDaConsulta;
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
                            plano + "'," +
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
                        "',plano='" + plano + 
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
        /*
        public bool update(Consulta newC)
        {
            try
            {
                bool updateRequired = false;
                if (this.id != newC.get_Id())
                    return false;
                string query = "update consulta set ";
                /*
                this.faturas = faturas;

                if (this.anamnese != newC.get_Anamnese())
                {
                    updateRequired = true;
                    query += "anamnese=" + newC.get_Anamnese() + ",";
                }
                if (this.data.CompareTo(newC.get_Data()) != 0)
                {
                    updateRequired = true;
                    query += "data='" + newC.get_Data().ToString("yyyy-MM-dd HH:mm:ss") + "',";
                }
                if (this.hoje.CompareTo(newC.get_Hoje()) != 0)
                {
                    updateRequired = true;
                    query += "hoje='" + newC.get_Hoje().ToString("yyyy-MM-dd HH:mm:ss") + "',";
                }
                if (this.paciente != newC.get_Paciente())
                {
                    updateRequired = true;
                    query += "paciente='" + newC.get_Paciente() + "',";
                }
                if (this.plano != newC.get_Plano())
                {
                    updateRequired = true;
                    query += "plano='" + newC.get_Plano() + "',";
                }
                if (this.detalhes != newC.get_Detalhes())
                {
                    updateRequired = true;
                    query += "detalhes='" + newC.get_Detalhes() + "',";
                }
                if (this.faturas.Equals(newC.faturas))
                {
                    updateRequired = true;
                    query += "faturas='" + newC.get_Faturas() + "',";
                }
                if (this.ativa != newC.get_Estado())
                {
                    updateRequired = true;
                    if (newC.get_Estado())
                        query += "ativar=1,";
                    else
                        query += "ativa=0,";
                }
                if (updateRequired)
                {
                    if (',' == query[query.Length - 1])
                        query = query.Substring(0, query.Length - 1);
                    query += " WHERE id=" + newC.get_Id().ToString();
                    Database con = new Database();
                    string res = con.comando(query);
                    if ("" == res)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
            catch { return false; }
        }
*/

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
