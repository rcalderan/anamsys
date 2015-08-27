using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnamSys
{
    class Paciente
    {
        private int id;
        private string nome;
        private string rg;
        private string cpf;
        private string endereco;
        private string bairro;
        private string cidade;
        private string uf;
        private string[] obs;
        private string plano;
        private DateTime hoje, nascimento;

        public int get_id() { return this.id; }
        public string get_nome() { return this.nome; }
        public void set_nome(string nome) { this.nome = nome; }
        public string get_rg() { return this.rg; }
        public void set_rg(string rg) { this.rg = rg; }
        public string get_cpf() { return this.cpf; }
        public void set_cpf(string cpf) { this.cpf = cpf; }
        public string get_endereco() { return this.endereco; }
        public void set_endereco(string endereco) { this.endereco = endereco; }
        public string get_bairro() { return this.bairro; }
        public void set_bairro(string bairro) { this.bairro = bairro; }
        public string get_cidade() { return this.cidade; }
        public void set_cidade(string cidade) { this.cidade = cidade; }
        public string get_uf() { return this.uf; }
        public void set_uf(string uf) { this.uf = uf; }
        public DateTime get_nascimento() { return this.nascimento; }
        public void set_nascimento(DateTime nascimento) { this.nascimento = nascimento; }
        public string[] get_obs() { return this.obs; }
        public void set_obs(string[] obs) { this.obs = obs; }
        public DateTime get_hoje() { return this.hoje; }
        public string get_plano() { return this.plano; }
        public void set_plano(string plano) { this.plano = plano; }
        
        public Paciente( int id)
        {
            Database con = new Database();
            System.Data.DataTable dt = con.Query("select * from paciente where id=" + id.ToString());
            if (dt!=null)
            {
                DateTime aux;
                this.id = id;
                this.nome = dt.Rows[0]["nome"].ToString();
                this.rg = dt.Rows[0]["rg"].ToString();
                this.cpf = dt.Rows[0]["cpf"].ToString();
                this.endereco = dt.Rows[0]["endereco"].ToString();
                this.bairro = dt.Rows[0]["bairro"].ToString();
                this.cidade = dt.Rows[0]["cidade"].ToString();
                this.uf = dt.Rows[0]["uf"].ToString();
                this.plano = dt.Rows[0]["plano"].ToString();
                this.obs = Uteis.Split(dt.Rows[0]["obs"].ToString(),"|");
                if (DateTime.TryParse(dt.Rows[0]["hoje"].ToString(), out aux))
                    this.hoje = aux;
                else
                    this.hoje = new DateTime();

                if (DateTime.TryParse(dt.Rows[0]["nascimento"].ToString(), out aux))
                    this.nascimento = aux;
                else
                    this.nascimento = new DateTime();
            }

        }

        public static bool New(string nome, string rg,string cpf,string endereco, string bairro,string cidade,
            string uf,DateTime nascimento,string[] obs,string plano)
        {
            try
            {
                Database db = new Database();
                if (db.getLastState())
                {
                    string insert = "INSERT INTO paciente ('id','nome','rg','cpf','endereco','bairro','cidade','uf',"+
                        "'nascimento','obs','hoje','plano','avatar') VALUES (NULL,"+
                        "'"+nome+"','"+rg+"','"+cpf+"','"+endereco+"','"+bairro+"','"+cidade+"','"+uf+"','"+
                        nascimento.ToString("yyyy-MM-dd HH:mm:ss")+"','"+Uteis.Join(obs,"|")+"','1111-11-11','k','avatar.png')";
                    if (db.Comando(insert) == "")
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch { return false; }

        }

        public static string[] PacientesToAutocompleteSource()
        {
            try
            {
                Database db = new Database();
                if (db.getLastState())
                {
                    System.Data.DataTable dt = db.Query("select id,nome,nascimento from paciente");
                    if (dt != null)
                    {
                        List<string> ret = new List<string>();
                        foreach(System.Data.DataRow dr in dt.Rows)
                        {
                            ret.Add(dr["id"].ToString() + " - " + dr["nome"].ToString() + " (" + dr["nascimento"].ToString() + ")");
                        }
                        return ret.ToArray();
                    }
                    else
                        return new string[0];
                }
                else
                    return new string[0];
            }
            catch
            {
                return new string[0];
            }
        }

        public static Paciente Load(int id)
        {
            try
            {
                Paciente p = new Paciente(id);
                if (p.id != null)
                {
                    return p;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool Delete(int ID)
        {
            try
            {
                Database db = new Database();
                if (db.getLastState())
                {
                    System.Data.DataTable dt = db.Query("select id,nome from paciente where id=" + ID.ToString());
                    if (dt != null)
                    {
                        if ("" != db.Comando("DELETE from paciente where id=" + ID))
                            return false;
                        else
                            return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            { return false; }
        }

        public bool Update(Paciente newP)
        {
            try
            {
                if (this.id != newP.get_id())
                    return false;
                string query = "update paciente set ";
                if (this.nome != newP.get_nome())
                    query += "nome='" + newP.get_nome() + "',";
                if (this.rg != newP.get_rg())
                    query += "rg='" + newP.get_rg() + "'";
                if (this.cpf != newP.get_cpf())
                    query += "cpf='" + newP.get_cpf() + "'";
                if (this.endereco != newP.get_endereco())
                    query += "endereco='" + newP.get_endereco() + "',";
                if (this.bairro != newP.get_bairro())
                    query += "bairro='" + newP.get_bairro() + "',";
                if (this.cidade != newP.get_cidade())
                    query += "cidade='" + newP.get_cidade() + "',";
                if (this.uf != newP.get_uf())
                    query += "uf='" + newP.get_uf() + "',";
                if (this.nascimento != newP.get_nascimento())
                    query += "nascimento='" + newP.get_nascimento().ToString("yyyy-MM-dd HH:mm:ss") + "',";
                if (this.plano != newP.get_plano())
                    query += "plano='" + newP.get_plano() + "',";
                if (this.obs != newP.get_obs())
                    query += "obs='" + Uteis.Join(newP.get_obs(),"|") + "',";
                if (',' == query[query.Length - 1])
                    query = query.Substring(0, query.Length - 1);
                query += " WHERE id=" + newP.get_id().ToString();
                Database con = new Database();
                if ("" == con.Comando(query))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
