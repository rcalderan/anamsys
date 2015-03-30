using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace AnamSys
{
     
    public partial class principalForm : Form
    {
        public bool debug_mode = true;
        private Database db = new Database();
        private Control[] boxes;
        private bool isMoving = false;
        private Point hoverCursorLocation;

        System.Globalization.CultureInfo pt_Br = new System.Globalization.CultureInfo("pt-BR");

        Label lblDayz;
        Int32 y = 0;
        Int32 x;
        Int32 ndayz;
        string Dayofweek, CurrentCulture;

        public principalForm()
        {
            InitializeComponent();
            Control[] paineis = { consultasPn, cad1Pn, finPn, planoPn, avalPn, conDetailsPn };
            foreach (Control c in paineis)
            {
                c.MouseDown += control_MouseDown;
                c.MouseUp += control_MouseUp;
                c.MouseMove += control_MouseMove;
                c.MouseMove += control_MouseMove;
                c.MouseMove += control_MouseMove;
            }
            boxes = paineis;
        }


        private void pacienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb(cad1Pn);
            }
            catch(Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void mostraGb()
        {
            foreach (Control g in boxes)
                g.Hide();
        }

        private void mostraGb(Control gb)
        {
            foreach (Control g in boxes)
            {
                if (g == gb)
                    gb.Show();
                else
                    g.Hide();

                if (gb.Visible)
                {
                    if (principalForm.ActiveForm.Width < gb.Width)
                        principalForm.ActiveForm.Width = gb.Width;
                    if (principalForm.ActiveForm.Height < gb.Height)
                        principalForm.ActiveForm.Height = gb.Height;

                    gb.Location = new Point(principalForm.ActiveForm.Width / 2 - gb.Width / 2, principalForm.ActiveForm.Height / 2 - gb.Height / 2);
                }
            }
        }


        private int VerificaDia()
        {
            DateTime time = Convert.ToDateTime(cboMes.Text + "/01/" + txtAno.Text);
            //péga o dia de inicio da semana para data informada
            Dayofweek = Application.CurrentCulture.Calendar.GetDayOfWeek(time).ToString();
            if (Dayofweek == "Sunday")
            {
                x = 0;
            }
            else if (Dayofweek == "Monday")
            {
                x = 0 + 42;
                ndayz = 1;
            }
            else if (Dayofweek == "Tuesday")
            {
                x = 0 + 84;
                ndayz = 2;
            }
            else if (Dayofweek == "Wednesday")
            {
                x = 0 + 84 + 42;
                ndayz = 3;
            }
            else if (Dayofweek == "Thursday")
            {
                x = 0 + 84 + 84;
                ndayz = 4;
            }
            else if (Dayofweek == "Friday")
            {
                x = 0 + 84 + 84 + 42;
                ndayz = 5;
            }
            else if (Dayofweek == "Saturday")
            {
                x = 0 + 84 + 84 + 84;
                ndayz = 6;
            }
            return x;
        }

        private void carregaData()
        {
            try
            {

                int t = -1;
                if ((cboMes.Text == null) || (txtAno.Text == null) || (string.IsNullOrWhiteSpace(txtAno.Text)) || (string.IsNullOrEmpty(txtAno.Text)))
                {
                    //MessageBox.Show("O ano ou o mês estão incorretos");
                }
                else
                {
                    try
                    {
                        if (int.TryParse(txtAno.Text, out t))
                        {
                            if ((t < 0) || (t > 9999))
                                return;
                            //remove todos os controles do painel

                            panel1.Controls.Clear();
                            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(CurrentCulture);
                            //exibe o nome completo do mes selecionado
                            labelMes.Text = Application.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(cboMes.Text));
                            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-br");
                            Int32 Dayz = DateTime.DaysInMonth(Convert.ToInt32(txtAno.Text), Convert.ToInt32(cboMes.Text));
                            VerificaDia();
                            Int32 mon = Convert.ToInt32(cboMes.Text);
                            Int32 year = Convert.ToInt32(txtAno.Text);
                            //Control[] crts;
                            for (Int32 i = 1; i < Dayz + 1; i++)
                            {
                                ndayz += 1;
                                lblDayz = new Label();
                                lblDayz.Name = "conDia" + i.ToString() + "Lb";
                                lblDayz.Text = i.ToString();
                                lblDayz.BorderStyle = BorderStyle.Fixed3D;
                                lblDayz.Click += calDiaClick;

                                if ((i == DateTime.Now.Day) && (mon == DateTime.Now.Month) && (year == DateTime.Now.Year))
                                {
                                    //destaca o dia atual com cor diferente

                                    lblDayz.BackColor = Color.Green;
                                }
                                else if (ndayz == 01)
                                {
                                    lblDayz.BackColor = Color.LightYellow;
                                }
                                else
                                {
                                    //define a cor para outros dias do mes selecionado
                                    lblDayz.BackColor = Color.Aquamarine;
                                }
                                lblDayz.Font = label31.Font;
                                lblDayz.SetBounds(x, y, 37, 27);

                                x += 42;
                                if (ndayz == 7)
                                {
                                    x = 0;
                                    ndayz = 0;
                                    y += 29;
                                }
                                panel1.Controls.Add(lblDayz);
                            }
                            x = 0;
                            ndayz = 0;
                            y = 0;


                            List<DateTime> todasConsultas = Consulta.todasDatas();
                            DateTime aux;
                            foreach (DateTime dt in todasConsultas)
                            {
                                if ((txtAno.Text == dt.Year.ToString()) && (cboMes.Text == dt.Month.ToString()))
                                {
                                    for (int i = 1; i <= Dayz; i++)
                                        if (i == dt.Day)
                                            lblDayz.BackColor = Color.Red;
                                }
                            }
                            Control[] crts;
                            for (int i = 1; i <= Dayz; i++)
                            {
                                aux = new DateTime(year, mon, i);
                                foreach (DateTime dt in todasConsultas)
                                {
                                    if ((aux.Year == dt.Year) && (aux.Month == dt.Month) && (aux.Day == dt.Day))
                                    {
                                        crts = panel1.Controls.Find("conDia" + i.ToString() + "Lb", false);
                                        if (crts.Length > 0)
                                            ((Label)crts[0]).BackColor = Color.Red;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("O valor deve estar entre 0 e 9999");
                            txtAno.Focus();
                        }
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("O ano deve estar entre 0 e 9999");
                        txtAno.Focus();
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }


        private void conCalProx_Click(object sender, EventArgs e)
        {
            avancaMes(1);
        }
        private void conCalPrev_Click(object sender, EventArgs e)
        {
            avancaMes(-1);
        }

        private void avancaMes(int av)
        {
            try
            {
                Int32 mesAtual, anoAtual;
                anoAtual = Convert.ToInt32(txtAno.Text);
                mesAtual = Convert.ToInt32(cboMes.Text);
                if (av * (-1) > 0)
                {
                    if (mesAtual == 1)
                    {
                        anoAtual -= 1;
                        mesAtual = 12;
                        txtAno.Text = anoAtual.ToString();
                        cboMes.Text = mesAtual.ToString();
                    }
                    else
                    {
                        mesAtual -= 1;
                        cboMes.Text = mesAtual.ToString();
                    }
                }
                else
                {
                    if (mesAtual == 12)
                    {
                        anoAtual += 1;
                        mesAtual = 1;
                        txtAno.Text = anoAtual.ToString();
                        cboMes.Text = mesAtual.ToString();
                    }
                    else
                    {
                        mesAtual += 1;
                        cboMes.Text = mesAtual.ToString();
                    }
                }

                //remove all the controls in the panel
                panel1.Controls.Clear();
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(CurrentCulture);
                //display the selected month's fullname
                labelMes.Text = Application.CurrentCulture.DateTimeFormat.GetMonthName(mesAtual);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-za");
                Int32 Dayz = DateTime.DaysInMonth(Convert.ToInt32(txtAno.Text), Convert.ToInt32(cboMes.Text));
                VerificaDia();
                List<DateTime> cList = new List<DateTime>();
                DateTime auxDt;
                string query = "select * from consulta";// where data>='" + anoAtual.ToString() + "-" + mesAtual + "-1' and data<='" + anoAtual.ToString() + "-" + mesAtual + "-31'";

                DataTable marcadas = db.query(query);
                if (marcadas != null)
                {
                    foreach (DataRow dr in marcadas.Rows)
                    {
                        auxDt = Convert.ToDateTime(dr["data"]);
                        auxDt = new DateTime(auxDt.Year, auxDt.Month, auxDt.Day);
                        cList.Add(auxDt);
                    }
                }
                for (Int32 i = 1; i < Dayz + 1; i++)
                {
                    ndayz += 1;
                    auxDt = new DateTime(anoAtual, mesAtual, i);
                    lblDayz = new Label();
                    lblDayz.Click += calDiaClick;
                    lblDayz.Text = i.ToString();
                    lblDayz.Name = "conDia" + i.ToString() + "Lb";
                    lblDayz.BorderStyle = BorderStyle.Fixed3D;

                    Int32 mon = Convert.ToInt32(cboMes.Text);
                    Int32 year = Convert.ToInt32(txtAno.Text);
                    if ((i == DateTime.Now.Day) && (mon == DateTime.Now.Month) && (year == DateTime.Now.Year))
                    {
                        //the current day must be highlighted differently
                        lblDayz.BackColor = Color.Green;
                    }
                    else if (ndayz == 01)
                    {
                        lblDayz.BackColor = Color.LightSalmon;
                    }
                    else
                    {
                        //set this color for other days in the selected month
                        lblDayz.BackColor = Color.Aquamarine;
                    }

                    if (cList.Exists(element => element == auxDt))
                    {
                        lblDayz.BackColor = Color.LightSeaGreen;
                    }
                    lblDayz.Font = label31.Font;
                    lblDayz.SetBounds(x, y, 37, 27);

                    x += 42;
                    if (ndayz == 7)
                    {
                        x = 0;
                        ndayz = 0;
                        y += 29;
                    }
                    panel1.Controls.Add(lblDayz);
                }
                x = 0;
                ndayz = 0;
                y = 0;
            }
            catch (FormatException)
            {
                MessageBox.Show("Data inválida");
                txtAno.Focus();
            }
        }

        private void calDiaClick(object sender,EventArgs e)
        {
            try
            {
                Label label = (Label)sender;
                foreach (Control l in panel1.Controls)
                    if (l is Label)
                    {
                        ((Label)l).BorderStyle = BorderStyle.Fixed3D;
                    }

                label.BorderStyle = BorderStyle.FixedSingle;
                MessageBox.Show(label.Name);
                /*Control[] lbs = panel1.Controls.Find("calSelectedLb", true);
                if (lbs.Length > 0)
                {
                    lbs[0].Location = new Point(label.Location.X - 3, label.Location.Y - 3);
                    lbs[0].Show();

                }
                else
                {
                    Label lb = new Label();
                    lb.Name = "calSelectedLb";
                    panel1.Controls.Add(lb);
                    lblDayz.BorderStyle = BorderStyle.Fixed3D;
                    lb.SetBounds(label.Location.X - 3, label.Location.Y - 3, label.Width + 3, label.Height + 3);
                    lb.BackColor = Color.Red;
                    lb.Show();

                }*/
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void listaConsultas()
        {
            try
            {
                DataTable dt = db.query("Select c.id, c.ativa,c.data, p.nome from consulta as c inner join paciente as p where c.paciente=p.id");
                if (dt != null)
                {
                    conLv.Items.Clear();
                    ListViewItem aux;
                    string[] str = new string[3];
                    DateTime dtAux = new DateTime();
                    foreach (DataRow r in dt.Rows)
                    {

                        str[0] = r["id"].ToString();
                        str[1] = r["nome"].ToString();
                        if (!DateTime.TryParse(r["data"].ToString(), out dtAux))
                            str[2] = "Data Não Definida";
                        else
                            str[2] = dtAux.ToString("dd/MM/yyyy HH:mm");
                        aux = new ListViewItem(str);
                        if (r["ativa"].ToString() == "1")
                            aux.Checked = true;
                        else
                            aux.Checked = false;

                        conLv.Items.Add(aux);
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void listaConsultas(string paciente)
        {
            try
            {
                DataTable dt = db.query("Select c.id, c.ativa,c.data, p.nome from consulta as c inner join paciente as p where c.paciente=p.id and c.paciente="+paciente);
                if (dt != null)
                {
                    conLv.Items.Clear();
                    ListViewItem aux;
                    string[] str = new string[3];
                    DateTime dtAux = new DateTime();
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r["ativa"].ToString() == "1")
                        {
                            str[0] = r["id"].ToString();
                            str[1] = r["nome"].ToString();
                            if (!DateTime.TryParse(r["data"].ToString(), out dtAux))
                                str[2] = "Data Não Definida";
                            else
                                str[2] = dtAux.ToString("dd/MM/yyyy HH:mm:ss");
                            aux = new ListViewItem(str);
                            conLv.Items.Add(aux);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void principalForm_Load(object sender, EventArgs e)
        {
            try
            {
                cad1IdMtb.Text = db.proximo("paciente", "id");
                listaConsultas();
                CurrentCulture = Application.CurrentCulture.Name;
                //exibe o mes atual
                cboMes.Text = DateTime.Now.Month.ToString();
                //exibe o nome completo do mes atual
                labelMes.Text = Application.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(cboMes.Text));
                //altera a cultura para evitar data incorreta
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
                //obtem o nume de dias no mes e ano selecionado
                Int32 Dayz = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                //exibe o ano atual no textbox
                txtAno.Text = DateTime.Now.Year.ToString();
                //chama a função 
                VerificaDia(); 
                List<DateTime> cList = new List<DateTime>();
                DateTime auxDt;
                string query = "select * from consulta";// where data>='" + anoAtual.ToString() + "-" + mesAtual + "-1' and data<='" + anoAtual.ToString() + "-" + mesAtual + "-31'";

                DataTable marcadas = db.query(query);
                if (marcadas != null)
                {
                    foreach (DataRow dr in marcadas.Rows)
                    {
                        auxDt = Convert.ToDateTime(dr["data"]);
                        auxDt = new DateTime(auxDt.Year, auxDt.Month, auxDt.Day);
                        cList.Add(auxDt);
                    }
                }
                for (Int32 i = 1; i < Dayz + 1; i++)
                {
                    ndayz += 1;
                    lblDayz = new Label();
                    lblDayz.Click += calDiaClick;
                    lblDayz.Name = "conDia" + i.ToString() + "Lb";
                    lblDayz.Text = i.ToString();
                    lblDayz.BorderStyle = BorderStyle.Fixed3D;
                    if (i == DateTime.Now.Day)
                    {
                        lblDayz.BackColor = Color.Green;
                    }
                    else if (ndayz == 01)
                    {
                        lblDayz.BackColor = Color.LightSalmon;
                    }
                    else
                    {
                        lblDayz.BackColor = Color.Aquamarine;
                    }
                    lblDayz.Font = label31.Font;
                    lblDayz.SetBounds(x, y, 37, 27);
                    x += 42;
                    if (ndayz == 7)
                    {
                        x = 0;
                        ndayz = 0;
                        y += 29;
                    }
                    panel1.Controls.Add(lblDayz);
                }
                //return all values to default
                x = 0;
                ndayz = 0;
                y = 0;
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }


        private void limpaPaciente()
        {
            try
            {
                cad1IdMtb.Text = db.proximo("paciente", "id");
                cad1NomeTb.Clear();
                cad1EnderecoTb.Clear(); 
                cad1CpfMtb.Clear();
                cad1RgMtb.Clear();
                cad1BairroTb.Clear();
                cad1CidadeTb.Clear();
                cad1UfTb.Clear();
                cad1ObsTb.Clear();
                conDataDtp.Value = DateTime.Today;
                cadNascDtp.Value = DateTime.Today;
                conAvalTb.Clear();
                conPlanoTb.Clear();
                conPacienteLb.Text = "Nome do Paciente";

                avalNomeLb.Text = "Nome";
                planoNomeLb.Text = "Nome";
                listaConsultas();
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }


        private void cad1AtualizaBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Deseja atualizar dados?", "Atualizar?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Paciente estePaciente = new Paciente(int.Parse(cad1IdMtb.Text));
                    Paciente newPaciente = new Paciente(int.Parse(cad1IdMtb.Text));
                    newPaciente.set_nome(cad1NomeTb.Text);
                    newPaciente.set_cpf(cad1CpfMtb.Text);
                    newPaciente.set_rg(cad1RgMtb.Text);
                    newPaciente.set_endereco(cad1EnderecoTb.Text);
                    newPaciente.set_cidade(cad1CidadeTb.Text);
                    newPaciente.set_bairro(cad1BairroTb.Text);
                    newPaciente.set_uf(cad1UfTb.Text);
                    newPaciente.set_obs(cad1ObsTb.Text);
                    newPaciente.set_nascimento(cadNascDtp.Value);
                    newPaciente.set_plano(conPlanoTb.Text);
                    if (estePaciente.update(newPaciente))
                        MessageBox.Show("Os dados cadastrais do paciente " + cad1NomeTb.Text + " foram atualizados com sucesso!");
                    else
                        MessageBox.Show("Não foi possíver atualizar os dados do paciente...");
                    carregaPaciente(cad1IdMtb.Text);
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void cad1DeletaBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Você tem certeza que deseja proceguir?\nOs dados deste cadastro serão PERDIDOS. Sem volta!", "Deletar?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataTable dt = db.query("select id,nome from paciente where id='" + cad1IdMtb.Text + "'");
                    if (dt != null)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Você está prestes a excluir o paciente (" + dt.Rows[0]["id"].ToString() + ")" + dt.Rows[0]["nome"].ToString() + " do sistema. Deseja mesmo prosseguir?", "Deletar", MessageBoxButtons.YesNo))
                        {
                            string erro = db.comando("DELETE from paciente where id='" + cad1IdMtb.Text + "'");
                            if (erro == "")
                                MessageBox.Show("Paciente DELETADO com sucesso!");
                            else
                                MessageBox.Show("Erro ao tentar deletar: " + erro);
                            limpaPaciente();
                        }
                    }
                }

            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void cad1NovoBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Deseja Salvar?", "Salvar?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string prox = db.proximo("paciente", "id");
                    if (prox == cad1IdMtb.Text)
                    {
                        string query = "INSERT INTO paciente values(" + cad1IdMtb.Text + ",'" +
                            cad1NomeTb.Text + "','" +
                            cad1RgMtb.Text + "','" +
                            cad1CpfMtb.Text + "','" +
                            cad1EnderecoTb.Text + "','" +
                            cad1BairroTb.Text + "','" +
                            cad1CidadeTb.Text + "','" +
                            cad1UfTb.Text + "','" +
                            cadNascDtp.Value.ToString("yyyy-MM-dd") + "','" +
                            cad1ObsTb.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                            conPlanoTb.Text + "')",
                            erro = db.comando(query);
                        if (erro != "")
                            MessageBox.Show(erro);
                        else
                            MessageBox.Show("O paciente " + cad1NomeTb.Text + " foi cadastrado com sucesso. Seu ID é " + cad1IdMtb.Text + ".");
                        limpaPaciente();
                    }
                    else
                    {
                        MessageBox.Show("Para Atualizar o cadastro, utize o botão \"Atualizar\"");
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void cad1IdMtb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string erro = carregaPaciente(cad1IdMtb.Text);
                    if (erro != "")
                        MessageBox.Show(erro);
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private string carregaPaciente(string codigo)
        {
            try
            {
                DataTable dt = db.query("Select * from paciente where id='" + codigo + "'");
                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    DateTime nas;
                    cad1NomeTb.Text = dr["nome"].ToString();
                    cad1IdMtb.Text = codigo;
                    avalNomeLb.Text = dr["nome"].ToString();
                    planoNomeLb.Text = dr["nome"].ToString();
                    cad1RgMtb.Text = dr["rg"].ToString();
                    cad1CpfMtb.Text = dr["cpf"].ToString();
                    cad1EnderecoTb.Text = dr["endereco"].ToString();
                    cad1BairroTb.Text = dr["bairro"].ToString();
                    cad1CidadeTb.Text = dr["cidade"].ToString();
                    cad1UfTb.Text = dr["uf"].ToString();
                    if (DateTime.TryParse(dr["nascimento"].ToString(), out nas))
                        cadNascDtp.Value = nas;
                    else
                        cadNascDtp.Value = DateTime.Today;
                    cad1ObsTb.Text = dr["obs"].ToString();
                    conPlanoTb.Text = dr["plano"].ToString();
                    conPacienteLb.Text = cad1IdMtb.Text + " - " + cad1NomeTb.Text;
                    conDetNomeLb.Text = cad1IdMtb.Text + " - "+ cad1NomeTb.Text;
                    dt = db.query("Select * from consulta where id=" + cad1IdMtb.Text);
                    conLv.Items.Clear();
                    conFinPacienteLb.Text = conPacienteLb.Text;
                    listaConsultas(codigo);
                    return "";
                }
                else
                    return "Paciente não encontrado";

            }
            catch (Exception err)
            {
                return "Não Foi Possível carregar Paciente... " + err.Message;
            }
        }

        private void cad1ObsTb_TextChanged(object sender, EventArgs e)
        {

        }

        private void anamneseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Função ainda não implementada");
            try
            {
                PrintPreviewDialog pd = new PrintPreviewDialog();
                PrintDocument prntDoc = new PrintDocument();
                prntDoc.PrintPage += new PrintPageEventHandler(doc_PrintPage);
                pd.Document = prntDoc;
                pd.ShowDialog();
                if (pd.DialogResult == DialogResult.Yes)
                {
                    prntDoc.Print();
                }
            }
            catch(Exception err)
            {
                if (debug_mode)
                    MessageBox.Show("Caught: " + err.Message);
            }
        }

        void doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            try 
            {
                Image im;
                string path = Application.StartupPath;
                MessageBox.Show(path);
                string[] imgs = System.IO.Directory.GetFiles(path,"*.jpg"); 
                foreach(string str in imgs)
                {
                    MessageBox.Show(str);
                    im = Image.FromFile(str);
                    e.Graphics.DrawImage(im, 0, 0);
                }
                //;
            }
            catch(Exception err)
            {
                if (debug_mode)
                    MessageBox.Show("Caught: " + err.Message);
            }
            //throw new NotImplementedException();
        }

        private void conXLb_Click(object sender, EventArgs e)
        {
            mostraGb();
        }

        private void conLpLb_Click(object sender, EventArgs e)
        {
        }
        

        private void consultarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostraGb(consultasPn);
        }
        
        private void conSalvarBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(conDetValorTb.Text)||(string.IsNullOrEmpty(conDetValorTb.Text)))
                {
                    MessageBox.Show("Valor da Consulta incorreto.");
                    return;
                }
                if (!conDetFormaCb.Items.Contains(conDetFormaCb.Text))
                {
                    conDetFormaCb.Text = "";
                    MessageBox.Show("Selecione a forma de pagamento");
                    return;
                }
                string id="";
                if (-1 != conDetNomeLb.Text.IndexOf(" "))
                {
                    id = conDetNomeLb.Text.Substring(0, conDetNomeLb.Text.IndexOf(" "));
                }
                if (null != db.query("select id from paciente where id=" + id))
                {
                    DataTable dt = db.query("select id from consulta where id=" + conDetIdLb.Text);
                    if (null == dt)
                    {
                        DateTime data = new DateTime(conDataDtp.Value.Year, conDataDtp.Value.Month, conDataDtp.Value.Day, int.Parse(conHoraCb.Text), int.Parse(conMinCb.Text), 0);
                        Consulta novaConsulta = new Consulta(
                            int.Parse(cad1IdMtb.Text),
                            0,
                            conDetDetTb.Text,
                            conPlanoTb.Text,
                            DateTime.Now,
                            data,
                            true);
                        string erro = novaConsulta.save();
                        if (erro != "")
                            MessageBox.Show(erro);
                        else
                        {
                            //agora registra fatura
                            double val;
                            if (!double.TryParse(conDetValorTb.Text, out val))
                                val = 0;
                            Fatura fat = new Fatura(novaConsulta.get_Id(), 0, novaConsulta.get_Data(), val, Financeiro.FormaDePagamento.GetFromId(conDetFormaCb.SelectedIndex), conDetParCh.Checked);

                            if ((debug_mode) && (fat.get_Id() == -1))
                                MessageBox.Show("Erro ao criar nova Fatura: id= -1");
                            else
                            {
                                MessageBox.Show("A consulta de " + cad1NomeTb.Text + " foi marcada para " + data.ToShortDateString() + " às " + conHoraCb.Text + ":" + conMinCb.Text);
                            }
                        }
                        listaConsultas();
                    }
                    else
                    {//atualizar consulta
                        DateTime data = new DateTime(conDataDtp.Value.Year, conDataDtp.Value.Month, conDataDtp.Value.Day, int.Parse(conHoraCb.Text), int.Parse(conMinCb.Text), 0);
                        Consulta novaConsulta = new Consulta(dt.Rows[0]["id"].ToString()), old = new Consulta(dt.Rows[0]["id"].ToString());

                        novaConsulta.set_Data(data);
                        novaConsulta.set_Detalhes(conDetDetTb.Text);
                        string erro=novaConsulta.save();
                        if (erro != "")
                        {
                            MessageBox.Show("erro ao atualizar dados da consulta");
                        }
                        else
                        {
                            Fatura fatura = new Fatura(novaConsulta.get_Id(), 0, data, Double.Parse(conDetValorTb.Text), Financeiro.FormaDePagamento.GetFromId(conDetFormaCb.SelectedIndex), conDetParCh.Checked);
                            if (fatura.get_Id() != -1)
                            {
                                erro = fatura.save();
                                if (erro != "")
                                    MessageBox.Show("Erro ao salvar fatura...");
                                else
                                    MessageBox.Show("Alterações realizadas com sucesso!");
                            }
                            else
                            {
                                MessageBox.Show("erro aou faturar...");
                            }
                        }
                    }
                }
                else
                {
                    mostraGb(cad1Pn);
                }

            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void faturarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limpaPaciente();
        }

        private void fichaEvolutivaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void agendaToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void conConXLb_Click(object sender, EventArgs e)
        {
            mostraGb();
        }

        private void label15_Click(object sender, EventArgs e)
        {
        }

        private void txtAno_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter== e.KeyCode)
            {
                carregaData();
            }
        }

        private void cboMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            carregaData();
        }

        private void txtAno_Leave(object sender, EventArgs e)
        {
            carregaData();
        }


        private void conTodasLb_Click(object sender, EventArgs e)
        {
            limpaPaciente();
        }

        private void conLv_ItemActivate(object sender, EventArgs e)
        {

        }

        private void conFinValorTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode==Keys.Enter)
                {
                    conFinValorTb.Text = conFinValorTb.Text.Trim();
                    float aux=0;
                    if (float.TryParse(conFinValorTb.Text, out aux))
                    {
                        MessageBox.Show("parseou: " + conFinValorTb.Text + " -> " + aux.ToString("f2", pt_Br) + " resto: " + (aux % 2).ToString());
                        conFinValorTb.Text = aux.ToString("F2",pt_Br);
                    }
                    else
                        conFinValorTb.Text = Convert.ToDouble(conFinValorTb.Text).ToString("C", pt_Br);
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }
        
        private void conFinTaxesTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode==Keys.Enter)
                {
                    TextBox snd = (TextBox)sender;
                    snd.Text = snd.Text.Trim().Remove(' ');
                    double val =0;
                    if (!double.TryParse(snd.Text, out val))
                        MessageBox.Show("Formato errado. Selecione o valor da taxa corretamente.");
                    snd.Text = val.ToString("F2");
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void label51_Click(object sender, EventArgs e)
        {
            conOperPn.Hide();
        }

        private void conFinAddOpLb_Click(object sender, EventArgs e)
        {
            try
            {
                carregaConfig();
                conOperPn.BringToFront();
                conOperPn.Show();
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void conTaxSaveBt_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = db.query("Select * from operadora where nome='" + conTaxNameCb.Text + "'");
                if (dt==null)
                {//salvar
                    if (DialogResult.Yes == MessageBox.Show("Deseja mesmo Adicionar uma operadora de cartões chamada \"" + conTaxNameCb.Text + "\"??", "Salvando...", MessageBoxButtons.YesNo))
                    {
                        string id,query,padrao, taxas = "", erro;
                        string[] taxes = new string[13];
                        Control[] taxesTb = conTaxPn.Controls.Find("conTx0Tb", false);
                        TextBox aux;
                        if (conTaxEsteCh.Checked)
                            padrao="1";
                        else
                            padrao ="0";
                        id = db.proximo("operadora", "id");
                        query = "INSERT INTO operadora VALUES(" + id + ",'" +
                               conTaxNameCb.Text + "'," + padrao + ",'";
                        for (int i = 0; i < 13; i++)
                        {
                            taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                            aux = (TextBox)taxesTb[0];
                            aux.Text = aux.Text.Replace(',', '.');
                            taxes[i] = aux.Text;
                            if (i < 12)
                                taxas += aux.Text + " ";
                            else
                                taxas += aux.Text;

                        }
                        query += taxas + "')";
                        erro = db.comando(query);
                        if (erro != "")
                            MessageBox.Show("Falha ao salvar: " + erro);
                        else
                            if ((padrao == "1") && (db.comando("update operadora set ativo=0 where id!=" + id) != ""))
                                MessageBox.Show("Escolha uma operadora como padrão e salve!");
                    }
                    else
                        carregaConfig();
                }
                else
                {//atualiza
                    if (DialogResult.Yes == MessageBox.Show("Deseja atualizar os dados desta operadora de cartões?", "Atualizar",MessageBoxButtons.YesNo))
                    {
                        string padrao,query = "UPDATE operadora set taxas='", taxas = "", erro;
                        string[] taxes = new string[13];
                        Control[] taxesTb = conTaxPn.Controls.Find("conTx0Tb", false);
                        TextBox aux;
                        if (conTaxEsteCh.Checked)
                            padrao = "1";
                        else
                            padrao = "0";
                        for (int i = 0; i < 13; i++)
                        {
                            taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                            aux = (TextBox)taxesTb[0];
                            aux.Text = aux.Text.Replace(',', '.');
                            taxes[i] = aux.Text;
                            taxas += aux.Text + " ";
                        }
                        query += taxas + "',ativo=" + padrao + " where nome='" + conTaxNameCb.Text + "'";
                        erro = db.comando(query);
                        if (erro != "")
                            MessageBox.Show("Falha ao atualizar: " + erro);
                    }
                    else
                        carregaConfig();
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }

        }
        
        private void carregaConfig()
        {
            try
            {
                DataTable dt = db.query("Select * from operadora");
                if (dt == null)
                {
                    conTaxNameLb.Text = "Operadora <- insira uma";
                    conTaxNameCb.Text = "";
                    foreach (Control tb in conTaxPn.Controls)
                        if (tb is TextBox)
                            tb.Text = "0";
                    conFinCartRb.Enabled = false;
                    conFinDebRb.Enabled = false;
                    conFinCredVistaRb.Enabled = false;
                    conFinParRb.Enabled = false;
                    conFinVezesNup.Enabled = false;
                }
                else
                {
                    double[] taxasVal;
                    Control[] taxesTb;
                    string[] itens = new string[dt.Rows.Count];
                    TextBox aux;
                    conTaxNameCb.Items.Clear();
                    int j = 0;
                    foreach(DataRow dr in dt.Rows)
                    {
                        itens[j] = dr["Nome"].ToString();
                        j++;
                        if (bool.Parse(dr["ativo"].ToString()))
                        {
                            conTaxEsteCh.Checked = true;
                            conTaxEsteCh.Enabled = false;
                            conTaxNameLb.Text = dr["nome"].ToString();
                            conTaxName2Tb.Text = dr["Nome"].ToString();
                            taxasVal = Fatura.stringStringToDoubleArray(dr["taxas"].ToString());
                            for (int i = 0; i < 13; i++)
                            {
                                taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                                aux = (TextBox)taxesTb[0];
                                if (i < taxasVal.Length)
                                    aux.Text = taxasVal[i].ToString("F2");
                                else
                                    aux.Text = "0";
                            }
                        }
                    }
                    conTaxNameCb.Items.AddRange(itens);
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }
        private void carregaConfig(string operadora)
        {
            try
            {
                DataTable dt = db.query("Select * from operadora where nome='" + operadora + "'");
                if (dt == null)
                {
                    conTaxEsteCh.Checked = false;
                    conTaxEsteCh.Enabled = true;
                    conTaxName2Tb.Text = "Operadora";
                    foreach (Control tb in conTaxPn.Controls)
                        if (tb is TextBox)
                            tb.Text = "0";
                }
                else
                {
                    double[] taxasVal;
                    Control[] taxesTb;
                    TextBox aux;
                    conTaxNameCb.Items.Clear();
                    DataRow dr = dt.Rows[0];
                    conTaxNameCb.Items.Add(dr["Nome"]);
                    if (bool.Parse(dr["ativo"].ToString()))
                    {
                        conTaxEsteCh.Enabled = false;
                        conTaxEsteCh.Checked = true;
                        conTaxName2Tb.Text = dr["Nome"].ToString();
                    }
                    else
                    {
                        conTaxEsteCh.Checked = false;
                        conTaxEsteCh.Enabled = true;
                    }
                    conTaxName2Tb.Text = dr["Nome"].ToString();
                    conTaxNameLb.Text = dr["nome"].ToString();
                    taxasVal = Fatura.stringStringToDoubleArray(dr["taxas"].ToString());
                    for (int i = 0; i < 13; i++)
                    {
                        taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                        aux = (TextBox)taxesTb[0];
                        if (i < taxasVal.Length)
                            aux.Text = taxasVal[i].ToString("F2");
                        else
                            aux.Text = "0";
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void conTaxNameCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox snd = (ComboBox)sender;
                carregaConfig(snd.Text);
            }
            catch { }
        }

        private void conTaxNameCb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keys.Enter==e.KeyCode)
                {

                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void conFaturaBt_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Ainda não Implementado");
                return;
                if (!string.IsNullOrEmpty(conFinValorTb.Text) && !string.IsNullOrWhiteSpace(conFinValorTb.Text) && (conFinParDtp.Value.CompareTo(DateTime.Today) >= 0) && ((conFinCartRb.Checked && (conFinParRb.Checked || conFinDebRb.Checked || conFinCredVistaRb.Checked)) || conFinChequeRb.Checked || conFinDinRb.Checked))
                {
                    if (MessageBox.Show("Deseja inserir esta fatura na consulta de \""+cad1NomeTb.Text+"\"?","Faturar",MessageBoxButtons.YesNo)==DialogResult.Yes)
                    {

                    }
                }
                else
                    MessageBox.Show("Preencha todos os campos corretamente para realizar o pagamento.");
            }
            catch(Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void conLimparLb_Click(object sender, EventArgs e)
        {
            try 
            {
                conFinValorTb.Clear();
                conParCb.Text = "";
                conFinDinRb.Checked = false;
                conFinCartRb.Checked = false;
                conFinChequeRb.Checked = false;
                conFinDebRb.Checked = false;
                conFinCredVistaRb.Checked = false;
                conFinParRb.Checked = false;
                conFinVezesNup.Value = 2;
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void conFichaTp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ainda não implementado!");
        }

        private void conPlanoTb_Leave(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = db.query("select id from paciente where id=" + cad1IdMtb.Text);
                if (dt != null)
                {
                    Paciente oldP = new Paciente(int.Parse(cad1IdMtb.Text));
                    string oldPlano = oldP.get_plano();
                    if (oldPlano != conPlanoTb.Text)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Deseja salvar alterações?", "Salvar?", MessageBoxButtons.YesNo))
                        {
                            Paciente newP = new Paciente(int.Parse(cad1IdMtb.Text));
                            newP.set_plano(conPlanoTb.Text);
                            if (!oldP.update(newP))
                            {
                                MessageBox.Show("Não foi possível salvar...");
                                conPlanoTb.Text = oldPlano;
                            }
                            else
                                MessageBox.Show("Plano de atendimento atualizado com sucesso!");
                        }
                        else
                            conPlanoTb.Text = oldPlano;
                    }
                }
            }
            catch(Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caught: "+erro.Message);
            }
        }

        private void conPlanoAtualizaBt_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = db.query("select id from paciente where id=" + cad1IdMtb.Text);
                if (dt != null)
                {
                    Paciente oldP = new Paciente(int.Parse(cad1IdMtb.Text));
                    string oldPlano = oldP.get_plano();
                    if (oldPlano != conPlanoTb.Text)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Deseja salvar alterações?", "Salvar?", MessageBoxButtons.YesNo))
                        {
                            Paciente newP = new Paciente(int.Parse(cad1IdMtb.Text));
                            newP.set_plano(conPlanoTb.Text);
                            if (!oldP.update(newP))
                            {
                                MessageBox.Show("Não foi possível salvar...");
                                conPlanoTb.Text = oldPlano;
                            }
                            else
                                MessageBox.Show("Plano de atendimento atualizado com sucesso!");
                        }
                        else
                            conPlanoTb.Text = oldPlano;
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caught: " + erro.Message);
            }
        }

        private void conDetXLb_Click(object sender, EventArgs e)
        {
            try
            {
                conDetailsPn.Hide();
            }
            catch(Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conDetLpLb_Click(object sender, EventArgs e)
        {
            try
            {
                conDataDtp.Value = DateTime.Now;
                conHoraCb.Text = "12";
                conMinCb.Text = "00";
                conDetParTb.Clear();
                conDetParNup.Value = 1;
                conDetParNup.Maximum = 1;
                conDetParLbox.Items.Clear();
                conDetDetTb.Clear();
                conDetParCh.Checked = false;
                conDetValorTb.Text = "0";
                conDetFormaCb.Text = "";
                ConDetPgCh.Checked = false;
                conDetIdLb.Text = db.proximo("consulta", "id");
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void limpaConsulta()
        {
            try
            {
                conDetIdLb.Text = db.proximo("consulta", "id");
                conDataDtp.Value = DateTime.Now;
                conHoraCb.SelectedIndex = conDataDtp.Value.Hour;
                conDetDetTb.Clear();
                conDetFormaCb.Text = "Dinheiro";
                conDetValorTb.Text = "0";
                conDetParCh.Checked = false;

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conNovaBt_Click(object sender, EventArgs e)
        {
            try
            {
                limpaConsulta();
                if (!conDetailsPn.Visible)
                    conDetailsPn.Show();
                else
                    conDetailsPn.Hide();

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conLv_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                ListViewItem i = null;
                foreach (ListViewItem item in conLv.SelectedItems)
                {
                    i = item;
                    break;
                }
                if (i != null)
                {
                    DataTable dt = db.query("select * from consulta where id=" + i.SubItems[0].Text);
                    if (dt != null)
                    {
                        limpaConsulta();
                        if (carregaPaciente(dt.Rows[0]["paciente"].ToString()) != "")
                        {
                            MessageBox.Show("Erro. Paciente (" + i.SubItems[1].Text + ") " + i.SubItems[1].Text + " NÃO não encontrado, favor cadastre novamente!");
                            mostraGb(cad1Pn);
                            return;
                        }
                        conDetIdLb.Text = i.SubItems[0].Text;
                        conDetDetTb.Text = dt.Rows[0]["detalhes"].ToString();
                        DateTime dia;
                        if (DateTime.TryParse(dt.Rows[0]["data"].ToString(), out dia))
                        {
                            conDataDtp.Value = dia;
                            conHoraCb.Text = dia.Hour.ToString();
                            conMinCb.Text = dia.ToString("mm");
                        }
                        dt = db.query("select * from fatura where consulta=" + i.SubItems[0].Text + " order by parcela asc");
                        if (dt != null)
                        {
                            Double aux_Valor;
                            int index;
                            bool pg;
                            /*if (dt.Rows.Count < 12)
                            {
                                conDetParNup.Value = dt.Rows.Count + 1;
                            }
                            else
                            {
                                conDetParNup.Value = 12;
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (!double.TryParse(dr["valor"].ToString(), out aux_Valor))
                                    aux_Valor = 0;
                                conDetParLbox.Items.Add(aux_Valor.ToString("F2"));
                            } */
                            string test = dt.Rows[0]["valor"].ToString();
                            if (!double.TryParse(dt.Rows[0]["valor"].ToString(), out aux_Valor))
                                aux_Valor = 0;
                            if (!int.TryParse(dt.Rows[0]["forma"].ToString(), out index))
                                index = 0;
                            if (!bool.TryParse(dt.Rows[0]["pendencia"].ToString(), out pg))
                                pg = false;
                            conDetValorTb.Text = aux_Valor.ToString("F2");
                            conDetFormaCb.SelectedIndex = index;
                            ConDetPgCh.Checked = pg;
                        }
                        else
                        {
                            conDetParNup.Value = 1;
                            conDetValorTb.Text = "0.0";
                            conDetFormaCb.SelectedIndex = 0;
                            ConDetPgCh.Checked = false;
                        }
                        conDetailsPn.Show();
                    }
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conDetParLbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListBox snd = (ListBox)sender;
                conDetParNup.Value = snd.SelectedIndex+1;
                conDetParTb.Text = snd.GetItemText(snd.SelectedItem);
                double aux,total = 0;
                foreach(object o in snd.Items)
                {
                    if (double.TryParse(o.ToString(),out aux))
                        total+=aux;
                }
                conDetParTotTb.Text = total.ToString("F2");
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conDetParTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    if (!string.IsNullOrEmpty(conDetParTb.Text) && (!string.IsNullOrWhiteSpace(conDetParTb.Text)))
                    {
                        conDetParTb.Text = conDetParTb.Text.Replace(',', '.');
                        double aux,total = 0;
                        if (!double.TryParse(conDetParTb.Text, out aux))
                        {
                            MessageBox.Show("Formato incorreto!");
                            return;
                        }
                        else
                        {
                            conDetParTb.Text = aux.ToString("F2");
                            if (conDetParLbox.Items.Count < conDetParNup.Value)
                            {
                                string txt = conDetParTb.Text;
                                if (conDetParCh.Checked)
                                    txt += " (pago)";
                                conDetParLbox.Items.Add(txt);
                                conDetParNup.Maximum = conDetParLbox.Items.Count+1;
                                conDetParNup.Value++;
                                conDetParTb.Clear();

                            }
                            else
                            {
                                conDetParLbox.SetSelected((int)conDetParNup.Value-1, true);
                                MessageBox.Show("editado");
                            }
                        }
                        foreach (object o in conDetParLbox.Items)
                        {
                            if (double.TryParse(o.ToString(), out aux))
                                total += aux;
                        }
                        conDetParTotTb.Text = total.ToString("F2");
                    }
                    else
                        MessageBox.Show("Formato incorreto!");
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }


        private void conDetParNup_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                conDetParNup.Maximum = conDetParLbox.Items.Count + 1;
                if (conDetParNup.Value == conDetParNup.Maximum)
                    conDetParTb.Clear();
                else
                {
                    conDetParLbox.SetSelected((int)conDetParNup.Value-1, true);
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conDetValorTb_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (conDetValorTb.Text == "")
                        conDetValorTb.Text = "0";
                    if (!string.IsNullOrWhiteSpace(conDetValorTb.Text))
                    {
                        conDetValorTb.Text = conDetValorTb.Text.Replace(',', '.');
                        double aux;
                        if (!double.TryParse(conDetValorTb.Text, out aux))
                        {
                            MessageBox.Show("Formato incorreto!");
                            return;
                        }
                        else
                        {
                            conDetValorTb.Text = aux.ToString("F2");
                        }
                    }
                    else
                        MessageBox.Show("Formato incorreto!");
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1ProxLv_Click(object sender, EventArgs e)
        {
            try
            {
                int prox = Convert.ToInt32(db.proximo("paciente", "id")),atual = Convert.ToInt32(cad1IdMtb.Text);
                atual++;
                if (atual< prox)
                {
                    cad1IdMtb.Text = atual.ToString();
                    carregaPaciente(atual.ToString());
                }
                else
                    limpaPaciente();


            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1AntLb_Click(object sender, EventArgs e)
        {
            try
            {
                int prox = Convert.ToInt32(db.proximo("paciente", "id")), atual = Convert.ToInt32(cad1IdMtb.Text);
                atual--;
                if (0 != atual)
                {
                    
                    cad1IdMtb.Text = atual.ToString();
                    carregaPaciente(atual.ToString());
                }

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1Pn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (cad1Pn.Visible)
                {
                    Point loc = new Point(principalForm.ActiveForm.Width / 2 - cad1Pn.Width / 2, principalForm.ActiveForm.Height / 2 - cad1Pn.Height / 2);
                    cad1Pn.Location = loc;
                }

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void RecizeAppWindow()
        {
            try
            {
                Point loc;
                foreach (Control c in boxes)
                {
                    if (c.Visible)
                    {
                        if (principalForm.ActiveForm.Width < c.Width)
                            principalForm.ActiveForm.Width = c.Width;

                        if (principalForm.ActiveForm.Height < c.Height)
                            principalForm.ActiveForm.Height = c.Height;

                        loc = new Point(principalForm.ActiveForm.Width / 2 - c.Width / 2, principalForm.ActiveForm.Height / 2 - c.Height / 2);
                        c.Location = loc;
                    }
                }

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void principalForm_Resize(object sender, EventArgs e)
        {
            try
            {
                RecizeAppWindow();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1ConsultasBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (carregaPaciente(cad1IdMtb.Text) == "")
                {
                    mostraGb(consultasPn);
                    listaConsultas(cad1IdMtb.Text);
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void anamneseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void conDetailsPn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (conDetailsPn.Visible)
                {
                    string s = carregaPaciente(cad1IdMtb.Text);
                    if ( s!= "")
                    {
                        limpaPaciente();
                        mostraGb(cad1Pn);
                    }
                        
                }

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }
        private void moveControl(Control c)
        {
            try
            {
                if (isMoving)
                {
                    Control parent = c.Parent;
                    int x=y=0;
                    //bool root = boxes.Contains<Control>(parent);
                    while(parent!=null)//&&(!root))
                    {
                        x += c.Parent.Location.X;
                        y += c.Parent.Location.Y;
                        parent = parent.Parent;
                    }
                    c.Location = new Point(MousePosition.X - x - hoverCursorLocation.X-8, MousePosition.Y - y - hoverCursorLocation.Y-30);
                    
                }

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                isMoving = true;
                hoverCursorLocation = e.Location;
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);

                isMoving = false;
            }
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Control snd = (Control)sender;
                moveControl(snd);
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                isMoving = false;
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conTopPn_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void cad1XLb_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb();

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1LimpaLb_Click(object sender, EventArgs e)
        {
            try
            {
                limpaPaciente();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void finXLb_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void finLpLb_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void planoXLb_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void avalXLb_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void planoDeAtendimentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb(planoPn);
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void avaliaçãoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb(avalPn);
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void consultasPn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (consultasPn.Visible)
                {
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void consultasLLb_Click(object sender, EventArgs e)
        {
            try
            {
                limpaConsulta();
                listaConsultas();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void consultasXLb_Click(object sender, EventArgs e)
        {
            try
            {
                mostraGb();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        /*
         * try
            {

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
         */
    }
}
