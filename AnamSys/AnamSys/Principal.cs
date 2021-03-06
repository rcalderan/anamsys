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
            Thread.CurrentThread.CurrentCulture = pt_Br;
            InitializeComponent();
            Control[] paineis = { consultasPn, cad1Pn, finPn, planoPn, avalPn, conDetailsPn, EvoPn };
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
                cboMes.Text = mesAtual.ToString();
                txtAno.Text = anoAtual.ToString();
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
                    if (av * (-1) < 0)
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
                    else
                    {
                        //
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

                DataTable marcadas = db.Query(query);
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
                DataTable dt = db.Query("Select c.id, c.ativa,c.data, p.nome from consulta as c inner join paciente as p where c.paciente=p.id and ativa=1 and c.data>='" + DateTime.Today.ToString("yyyy-MM-dd") + "'");
                if (dt != null)
                {
                    conLv.Items.Clear();
                    ListViewItem aux;
                    string[] str = new string[5];
                    DateTime dtAux = new DateTime();
                    int compare;
                    DataTable fin;
                    foreach (DataRow r in dt.Rows)
                    {
                        fin = db.Query("select * from fatura where consulta=" + r["id"].ToString());
                        str[0] = r["id"].ToString();
                        str[1] = r["nome"].ToString();
                        if (!DateTime.TryParse(r["data"].ToString(), out dtAux))
                            str[2] = "Data Não Definida";
                        else
                            str[2] = dtAux.ToString("dd/MM/yyyy HH:mm");
                        aux = new ListViewItem(str);
                        if (fin!=null)
                            if (fin.Rows[0]["pg"].ToString() == "1")
                        {
                            aux.Checked = true;
                            str[4] = "OK";
                        }
                        else
                        {
                            aux.Checked = false;
                            str[4] = "PEND";
                        }

                        conLv.Items.Add(aux);
                    }
                    //dt = db.query("select * from fatura");
                    foreach (ListViewItem i in conLv.Items)
                    {
                        if (DateTime.TryParse(i.SubItems[2].Text, out dtAux))
                        {
                            compare = dtAux.CompareTo(DateTime.Today);
                            if (compare < 0)
                            {
                                if (i.SubItems[4].Text == "OK")
                                    i.BackColor = Color.PaleGoldenrod;
                                else
                                    if (i.SubItems[4].Text == "PEND")
                                        i.BackColor = Color.Red;
                            }
                            else
                                if (compare >= 0)
                                    i.BackColor = Color.LightSeaGreen;
                        }
                        else
                            i.BackColor = Color.White;
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void listaConsultas(string IDs)
        {
            try
            {
                string query = "Select c.id, c.ativa,c.data, p.nome from consulta as c inner join paciente as p where c.paciente=p.id and c.ativa=1 and data>='" + DateTime.Today.ToString("yyyy-MM-dd") + "' and (";
                string[] cods = IDs.Split(' ');
                foreach (string s in cods)
                {
                    query += "c.id="+s+" or ";
                }
                query = query.Substring(0,query.Length - 4)+")";
                DataTable dt = db.Query(query);
                if (dt != null)
                {
                    conLv.Items.Clear();
                    ListViewItem aux;
                    string[] str = new string[5];
                    DateTime dtAux = new DateTime();
                    int compare;
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
                        {
                            aux.Checked = true;
                            str[4] = "OK";
                        }
                        else
                        {
                            aux.Checked = false;
                            str[4] = "PEND";
                        }

                        conLv.Items.Add(aux);
                    }
                    //dt = db.query("select * from fatura");
                    foreach (ListViewItem i in conLv.Items)
                    {
                        if (DateTime.TryParse(i.SubItems[2].Text, out dtAux))
                        {
                            compare = dtAux.CompareTo(DateTime.Today);
                            if (compare < 0)
                            {
                                if (i.SubItems[4].Text == "OK")
                                    i.BackColor = Color.PaleGoldenrod;
                                else
                                    if (i.SubItems[4].Text == "PEND")
                                        i.BackColor = Color.Red;
                            }
                            else
                                if (compare >= 0)
                                    i.BackColor = Color.LightSeaGreen;
                        }
                        else
                            i.BackColor = Color.White;
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
                cad1IdLb.Text = db.proximo("paciente", "id");
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

                DataTable marcadas = db.Query(query);
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

                    auxDt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, i);
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
                //return all values to default
                x = 0;
                ndayz = 0;
                y = 0;

                /*
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
                 */
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
                cad1IdLb.Text = db.proximo("paciente", "id");
                cad1NomeTb.Clear();
                cad1EnderecoTb.Clear(); 
                cad1CpfMtb.Clear();
                cad1RgMtb.Clear();
                cad1BairroTb.Clear();
                cad1CidadeTb.Clear();
                cad1UfTb.Clear();
                cad1ObsTb.Clear();
                conDataDtp.Value = DateTime.Today;
                cad1NascDtp.Value = DateTime.Today;
                conAvalTb.Clear();
                conPlanoTb.Clear();;
                conPacienteTb.AutoCompleteCustomSource.AddRange(Paciente.PacientesToAutocompleteSource(false));
                Control[] cs = cad1Pn.Controls.Find("cad1OutrosPn", false);
                if (cs.Length != 0)
                    cs[0].Dispose();
                avalNomeLb.Text = "Nome";
                planoNomeLb.Text = "Nome";
                listaConsultas();
                cad1CpfMtb.BackColor = Color.White;
                cad1CaleLb.ForeColor = Color.Black;
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
                    Paciente estePaciente = Paciente.Load(int.Parse(cad1IdLb.Text));
                    Paciente newPaciente = Paciente.Load(int.Parse(cad1IdLb.Text));
                    if ((estePaciente != null) && (newPaciente != null))
                    {
                        newPaciente.set_nome(cad1NomeTb.Text);
                        newPaciente.set_cpf(cad1CpfMtb.Text);
                        newPaciente.set_rg(cad1RgMtb.Text);
                        newPaciente.set_endereco(cad1EnderecoTb.Text);
                        newPaciente.set_cidade(cad1CidadeTb.Text);
                        newPaciente.set_bairro(cad1BairroTb.Text);
                        newPaciente.set_uf(cad1UfTb.Text);
                        newPaciente.set_obs(cad1ObsTb.Lines);
                        newPaciente.set_nascimento(cad1NascDtp.Value);
                        newPaciente.set_plano(conPlanoTb.Text);
                        if (estePaciente.Update(newPaciente))
                            MessageBox.Show("Os dados cadastrais do paciente " + cad1NomeTb.Text + " foram atualizados com sucesso!");
                        else
                            MessageBox.Show("Não foi possíver atualizar os dados do paciente...");
                    }
                    else
                        MessageBox.Show("Não foi possíver atualizar os dados do paciente...");
                    carregaPaciente(int.Parse(cad1IdLb.Text));
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
                    if (Paciente.Delete(int.Parse(cad1IdLb.Text)))
                        MessageBox.Show("Paciente DELETADO com sucesso!");
                    else
                        MessageBox.Show("Não foi possível deletar paciente!");
                    limpaPaciente();
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
                if (!Uteis.ValidaCPF(cad1CpfMtb.Text))
                {
                    cad1CpfMtb.BackColor = Color.Yellow;
                    if (DialogResult.No== MessageBox.Show("CPF está Incorreto! Deseja continuar?","CPF Inválido",MessageBoxButtons.YesNo))
                        return;
                }
                else
                    cad1CpfMtb.BackColor = Color.White;
                if (cad1NascDtp.Value == DateTime.Today)
                {
                    cad1CaleLb.ForeColor = Color.Red;
                    MessageBox.Show("Informe a DATA de nascimento do paciente.");
                    return;
                }
                else
                    cad1CaleLb.ForeColor = Color.Black;

                if (MessageBox.Show("Deseja Salvar?", "Salvar?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataTable dt = db.Query("select id from paciente where cpf='" + cad1CpfMtb.Text + "'");
                    if (dt != null)
                        MessageBox.Show("Para Atualizar o cadastro, utize o botão \"Atualizar\"");
                    else
                    {
                        if (pictureBox1.ImageLocation == "")
                            pictureBox1.ImageLocation = "0.png";
                        if (Paciente.New(cad1NomeTb.Text, cad1RgMtb.Text, cad1CpfMtb.Text, cad1EnderecoTb.Text,
                            cad1BairroTb.Text, cad1CidadeTb.Text, cad1UfTb.Text, cad1NascDtp.Value, cad1ObsTb.Lines,
                            conPlanoTb.Text))
                            MessageBox.Show("O paciente " + cad1NomeTb.Text + " foi cadastrado com sucesso! ");
                        else
                            MessageBox.Show("Não foi possível salvar paciente.");
                        limpaPaciente();
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
                    string erro = carregaPaciente(int.Parse(cad1IdLb.Text));
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

        private string carregaPaciente(int id)
        {
            try
            {
                DataTable dt = db.Query("Select * from paciente where id='" + id.ToString() + "'");
                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    DateTime nas;
                    cad1NomeTb.Text = dr["nome"].ToString();
                    cad1IdLb.Text = id.ToString();
                    avalNomeLb.Text = dr["nome"].ToString();
                    planoNomeLb.Text = dr["nome"].ToString();
                    cad1RgMtb.Text = dr["rg"].ToString();
                    cad1CpfMtb.Text = dr["cpf"].ToString();
                    cad1EnderecoTb.Text = dr["endereco"].ToString();
                    cad1BairroTb.Text = dr["bairro"].ToString();
                    cad1CidadeTb.Text = dr["cidade"].ToString();
                    cad1UfTb.Text = dr["uf"].ToString();
                    if (DateTime.TryParse(dr["nascimento"].ToString(), out nas))
                        cad1NascDtp.Value = nas;
                    else
                        cad1NascDtp.Value = DateTime.Today;
                    cad1ObsTb.Lines = Uteis.Split(dr["obs"].ToString(),"|");
                    conPlanoTb.Text = dr["plano"].ToString();
                    conPacienteTb.AutoCompleteCustomSource.AddRange(Paciente.PacientesToAutocompleteSource(false));
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

        private string carregaPaciente(string cpf)
        {
            try
            {
                DataTable dt = db.Query("Select * from paciente where cpf='" + cpf + "'");
                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    DateTime nas;
                    cad1NomeTb.Text = dr["nome"].ToString();
                    cad1IdLb.Text = dr["id"].ToString();
                    avalNomeLb.Text = dr["nome"].ToString();
                    planoNomeLb.Text = dr["nome"].ToString();
                    cad1RgMtb.Text = dr["rg"].ToString();
                    cad1CpfMtb.Text = cpf;
                    cad1EnderecoTb.Text = dr["endereco"].ToString();
                    cad1BairroTb.Text = dr["bairro"].ToString();
                    cad1CidadeTb.Text = dr["cidade"].ToString();
                    cad1UfTb.Text = dr["uf"].ToString();
                    if (DateTime.TryParse(dr["nascimento"].ToString(), out nas))
                        cad1NascDtp.Value = nas;
                    else
                        cad1NascDtp.Value = DateTime.Today;
                    cad1ObsTb.Lines = Uteis.Split(dr["obs"].ToString(),"|");
                    conPlanoTb.Text = dr["plano"].ToString();
                    conPacienteTb.AutoCompleteCustomSource.AddRange(Paciente.PacientesToAutocompleteSource(false));
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
                float y = 0;
                string[] imgs = System.IO.Directory.GetFiles(path,"*.jpg"); 
                foreach(string str in imgs)
                {
                    im = Image.FromFile(str);
                    e.Graphics.DrawImage(im, 0, y);
                    y += im.Height;
                }
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
                if (conPacienteIdLb.Text =="0")
                {
                    MessageBox.Show("Informe o paciente! (ID = 0)");
                    return;
                }
                foreach(DataGridViewRow r in conDetFinDg.Rows)
                {
                    if (Convert.ToBoolean(r.Cells[4].Value)==true)
                    {
                        MessageBox.Show(r.Cells.Count.ToString());
                        if (string.IsNullOrEmpty(r.Cells[1].Value.ToString()))
                        {
                            MessageBox.Show("Informe a forma de pagamento primeiro.");
                            return;
                        }
                        else
                             if (r.Cells[1].Value.ToString() == "N/A")
                             {
                                 MessageBox.Show("Informe a forma de pagamento primeiro.");
                                 return;
                             }
                    }
                }
                DataTable paciTb = db.Query("select id,nome from paciente where id=" + conPacienteIdLb.Text);
                if (null != paciTb)
                {
                    DataTable dt = db.Query("select id from consulta where id=" + conDetIdLb.Text);
                    if (null == dt)
                    {
                        double val,sum=0;
                        DateTime data = new DateTime(conDataDtp.Value.Year, conDataDtp.Value.Month, conDataDtp.Value.Day, int.Parse(conHoraCb.Text), int.Parse(conMinCb.Text), 0);
                        
                        DataGridViewCheckBoxCell chk;// = roow.Cells[0] as DataGridViewCheckBoxCell;
                        DataGridViewComboBoxCell cb;
                        foreach (DataGridViewRow r in conDetFinDg.Rows)
                        {
                            chk = r.Cells[4] as DataGridViewCheckBoxCell;
                            cb = r.Cells[1] as DataGridViewComboBoxCell;
                            if (Convert.ToBoolean(r.Cells[4].Value) == true)
                            {
                                if (double.TryParse(r.Cells[3].Value.ToString(), out val))
                                    sum += val;
                                else
                                    r.Cells[3].Value = "0.00";
                            }
                            MessageBox.Show(cb.Value.ToString());
                        }
                        /*Consulta last = Consulta.New(int.Parse(conPacienteIdLb.Text),
                            0,
                            conDetDetTb.Text,
                            conPlanoTb.Text,
                            data,
                            true);
                        if (last!=null)
                        {

                            MessageBox.Show("A consulta de " + paciTb.Rows[0]["nome"].ToString() + " foi marcada para " + data.ToShortDateString() + " às " + conHoraCb.Text + ":" + conMinCb.Text);
                            conDetailsPn.Hide();
                            listaConsultas();
                        }
                        else
                        {
                            MessageBox.Show("Não Foi possível registrar nova consulta...");
                            return;
                        }
                        listaConsultas();
                         * */
                    }
                    else
                    {//atualizar consulta
                        /*DateTime data = new DateTime(conDataDtp.Value.Year, conDataDtp.Value.Month, conDataDtp.Value.Day, int.Parse(conHoraCb.Text), int.Parse(conMinCb.Text), 0);
                        Consulta novaConsulta = Consulta.Load(int.Parse(dt.Rows[0]["id"].ToString())), old = Consulta.Load(int.Parse(dt.Rows[0]["id"].ToString()));

                        novaConsulta.set_Data(data);
                        novaConsulta.set_Detalhes(conDetDetTb.Text);
                        string erro=novaConsulta.save();
                        if (erro != "")
                        {
                            MessageBox.Show("erro ao atualizar dados da consulta: \""+erro+"\"");
                        }
                        else
                        {
                            Fatura fatura = new Fatura(novaConsulta.get_Id(), 0, data, Double.Parse(conDetValorTb.Text), Financeiro.FormaDePagamento.GetFromId(conDetFormaCb.SelectedIndex), conDetPgCh.Checked);
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
                         * */
                    }
                    avancaMes(0);
                }
                else
                {
                    conPacienteIdLb.Text = "0";
                    MessageBox.Show("Paciente não encontrado!");
                    return;
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
            finPn.Show();
        }

        private void fichaEvolutivaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try 
            {
                mostraGb(EvoPn);
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void agendaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostraGb(consultasPn);
            limpaDetConsulta();
            listaConsultas();
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
            listaConsultas();
            //limpaPaciente();
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
                    if (snd.Text.IndexOf(' ')!=-1)
                        snd.Text = snd.Text.Trim().Remove(' ');
                    double val =0;
                    if (!double.TryParse(snd.Text, out val))
                    {
                        MessageBox.Show("Formato errado. Selecione o valor da taxa corretamente.");
                    }
                    else
                    {
                        snd.Text = val.ToString("F2");
                        salvaOperadora();
                    }
                }
            }
            catch (Exception erro)
            {
                if (debug_mode)
                    MessageBox.Show("Caugth: " + erro.Message);
            }
        }

        private void salvaOperadora()
        {
            try
            {
                Financeiro a = new Financeiro();
                Operadora newOp = new Operadora(), oldOp = new Operadora();
                newOp = Operadora.Find(conTaxName2Lb.Text);
                if (newOp != null)
                {
                    oldOp = Operadora.Find(conTaxName2Lb.Text);
                    Control[] c;
                    string auxTax = "";

                    newOp.set_Id(oldOp.get_Id());
                    newOp.set_Nome(conTaxName2Lb.Text);
                    newOp.set_Padrao(conTaxEsteCh.Checked);
                    for (int i = 0; i < 13; i++)
                    {
                        c = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                        if (c.Length != 0)
                        {
                            auxTax += c[0].Text + " ";
                        }
                        newOp.set_Taxas(auxTax);
                    }
                    string erro = oldOp.Update(newOp);
                    if (erro != "")
                        MessageBox.Show(erro);
                    else
                        MessageBox.Show("Alteração salva!");
                }
                else
                    MessageBox.Show("Selecione uma operadora...");
                carregaConfig();

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
                DataTable dt = db.Query("Select * from operadora where nome='" + conTaxNameCb.Text + "'");
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
                            if (taxesTb.Length == 0)
                                MessageBox.Show("index nao encontrado "+i.ToString());
                            aux = (TextBox)taxesTb[0];
                            aux.Text = aux.Text.Replace(',', '.');
                            taxes[i] = aux.Text;
                            if (i < 12)
                                taxas += aux.Text + " ";
                            else
                                taxas += aux.Text;

                        }
                        query += taxas + "')";
                        erro = db.Comando(query);
                        if (erro != "")
                            MessageBox.Show("Falha ao salvar: " + erro);
                        else
                            if ((padrao == "1") && (db.Comando("update operadora set ativo=0 where id!=" + id) != ""))
                                MessageBox.Show("Escolha uma operadora como padrão e salve!");
                    }
                    else
                        carregaConfig();
                }
                else
                {
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
                Operadora[] ops = Operadora.findAll();
                if (ops!=null)
                {
                    conFinCartRb.Enabled = true;
                    conFinDebRb.Enabled = true;
                    conFinCredVistaRb.Enabled = true;
                    conFinParRb.Enabled = true;
                    conFinVezesNup.Enabled = true;

                    double[] taxasVal;
                    Control[] taxesTb;
                    TextBox aux;
                    conTaxNameCb.Items.Clear();
                    foreach (Operadora op in ops)
                    {
                        conTaxNameLb.Text = op.get_Nome();
                        conTaxNameCb.Items.Add(conTaxNameLb.Text);
                        if (op.get_Padrao())
                        {
                            conTaxName2Lb.Text = op.get_Nome();
                            taxasVal = Operadora.TaxesToArray(op.get_Taxas());
                            if (taxasVal!=null)
                                for (int i = 0; i < 13; i++)
                                {
                                    taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                                    aux = (TextBox)taxesTb[0];
                                    if (i < taxasVal.Length)
                                        aux.Text = taxasVal[i].ToString("F2");
                                    else
                                        aux.Text = "0";
                                }
                            else
                                for (int i = 0; i < 13; i++)
                                {
                                    taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                                    aux = (TextBox)taxesTb[0];
                                    aux.Text = "0";
                                }

                        }
                        
                    }
                    //conTaxNameCb.Text = conTaxNameLb.Text;
                }
                else
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
                Operadora op = Operadora.Find(operadora);
                if (op != null)
                {
                    conFinCartRb.Enabled = true;
                    conFinDebRb.Enabled = true;
                    conFinCredVistaRb.Enabled = true;
                    conFinParRb.Enabled = true;
                    conFinVezesNup.Enabled = true;

                    double[] taxasVal;
                    Control[] taxesTb;
                    TextBox aux;
                    //conTaxNameCb.Items.Clear();
                    conTaxName2Lb.Text = operadora;
                    taxasVal = Operadora.TaxesToArray(op.get_Taxas());
                    if (taxasVal != null)
                        for (int i = 0; i < 13; i++)
                        {
                            taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                            aux = (TextBox)taxesTb[0];
                            if (i < taxasVal.Length)
                                aux.Text = taxasVal[i].ToString("F2");
                            else
                                aux.Text = "0";
                        }
                    else
                        for (int i = 0; i < 13; i++)
                        {
                            taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                            aux = (TextBox)taxesTb[0];
                            aux.Text = "0";
                        }
                }
                else
                {
                    MessageBox.Show("Operadora não encontrada!");
                    carregaConfig();
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
                DataTable dt = db.Query("select id from paciente where id=" + cad1IdLb.Text);
                if (dt != null)
                {
                    Paciente oldP = new Paciente(int.Parse(cad1IdLb.Text));
                    string oldPlano = oldP.get_plano();
                    if (oldPlano != conPlanoTb.Text)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Deseja salvar alterações?", "Salvar?", MessageBoxButtons.YesNo))
                        {
                            Paciente newP = new Paciente(int.Parse(cad1IdLb.Text));
                            newP.set_plano(conPlanoTb.Text);
                            if (!oldP.Update(newP))
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
                DataTable dt = db.Query("select id from paciente where id=" + cad1IdLb.Text);
                if (dt != null)
                {
                    Paciente oldP = new Paciente(int.Parse(cad1IdLb.Text));
                    string oldPlano = oldP.get_plano();
                    if (oldPlano != conPlanoTb.Text)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Deseja salvar alterações?", "Salvar?", MessageBoxButtons.YesNo))
                        {
                            Paciente newP = new Paciente(int.Parse(cad1IdLb.Text));
                            newP.set_plano(conPlanoTb.Text);
                            if (!oldP.Update(newP))
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

        private void limpaDetConsulta()
        {

            conDataDtp.Value = DateTime.Now;
            conHoraCb.Text = "12";
            conMinCb.Text = "00";
            conDetDetTb.Clear();
            conDetIdLb.Text = db.proximo("consulta", "id");
            conDetFinDg.Rows.Clear();
            conDetFinDg.Rows[0].Cells[0].Value = 1;
            conDetFinDg.Rows[0].Cells[2].Value = DateTime.Today.ToString("dd/MM/yyyy");
            conDetFinDg.Rows[0].Cells[3].Value = "0,00";
        }

        private void conDetLpLb_Click(object sender, EventArgs e)
        {
            try
            {
                limpaDetConsulta();
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
                limpaDetConsulta();
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
                    DataTable dt = db.Query("select * from consulta where id=" + i.SubItems[0].Text),paciTb;
                    if (dt != null)
                    {
                        //limpaConsulta();
                        /*if (carregaPaciente(int.Parse(dt.Rows[0]["paciente"].ToString())) != "")
                        {
                            MessageBox.Show("Erro. Paciente (" + i.SubItems[1].Text + ") " + i.SubItems[1].Text + " NÃO não encontrado, favor cadastre novamente!");
                            mostraGb(cad1Pn);
                            return;
                        }*/
                        paciTb = db.Query("select * from paciente where id="+dt.Rows[0]["paciente"].ToString());
                        if (paciTb!=null)
                        {
                            conPacienteIdLb.Text = dt.Rows[0]["paciente"].ToString();
                            conPacienteTb.Text = i.SubItems[1].Text;
                        }
                        else
                        {
                            conPacienteIdLb.Text = "0";
                            conPacienteTb.Text = "";
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
                        dt = db.Query("select * from fatura where consulta=" + i.SubItems[0].Text + " order by parcela asc");
                        if (dt != null)
                        {
                            Double aux_Valor;
                            int index;
                            bool pg;
                            string test = dt.Rows[0]["valor"].ToString();
                            if (!double.TryParse(dt.Rows[0]["valor"].ToString(), out aux_Valor))
                                aux_Valor = 0;
                            if (!int.TryParse(dt.Rows[0]["forma"].ToString(), out index))
                                index = 0;
                            if (!bool.TryParse(dt.Rows[0]["pg"].ToString(), out pg))
                                pg = false;
                        }
                        else
                        {
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

       
        private void cad1ProxLv_Click(object sender, EventArgs e)
        {
            try
            {
                int prox = Convert.ToInt32(db.proximo("paciente", "id")),atual = Convert.ToInt32(cad1IdLb.Text);
                atual++;
                if (atual< prox)
                {
                    cad1IdLb.Text = atual.ToString();
                    carregaPaciente(atual);
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
                int prox = Convert.ToInt32(db.proximo("paciente", "id")), atual = Convert.ToInt32(cad1IdLb.Text);
                atual--;
                if (0 != atual)
                {

                    cad1IdLb.Text = atual.ToString();
                    carregaPaciente(atual);
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
                    Uteis.centralizaControl(cad1Pn);
                    cad1NomeTb.AutoCompleteCustomSource.AddRange(Paciente.PacientesToAutocompleteSource(false));
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
                if (carregaPaciente(int.Parse(cad1IdLb.Text)) == "")
                {
                    mostraGb(consultasPn);
                    listaConsultas(cad1IdLb.Text);
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
                    Control parent = c.Parent, lastParent = c.Parent;
                    Point lastLocation = c.Location, targetLocation;
                    int x=y=0;
                    //bool root = boxes.Contains<Control>(parent);
                    while(parent!=null)//&&(!root))
                    {
                        x += c.Parent.Location.X;
                        y += c.Parent.Location.Y;
                        parent = parent.Parent;
                        if (parent != null)
                            lastParent = parent;
                    }
                    targetLocation = new Point(MousePosition.X - x - hoverCursorLocation.X-8, MousePosition.Y - y - hoverCursorLocation.Y-30);

                    if ((targetLocation.Y < 0)||(targetLocation.Y+c.Height>lastParent.Height))
                        targetLocation.Y = lastLocation.Y;
                    if ((targetLocation.X < 0) || (targetLocation.X + c.Width > lastParent.Width))
                        targetLocation.X = lastLocation.X;
                    c.Location = targetLocation;
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
                    conPacienteTb.AutoCompleteCustomSource.AddRange(Paciente.PacientesToAutocompleteSource(false));
                    listaConsultas();
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

        private void evoXLb_Click(object sender, EventArgs e)
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

        private void EvoPn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (EvoPn.Visible)
                {
                    string query = "SELECT id,nome FROM paciente";
                    string[] data;
                    DataTable pts = db.Query(query);
                    if (pts!=null)
                    {
                        data = new string[pts.Rows.Count];
                        var source = new AutoCompleteStringCollection();
                        List<string> l = new List<string>();
                        foreach(DataRow r in pts.Rows)
                            l.Add(r["nome"].ToString());
                        source.AddRange(l.ToArray());
                        evoNomeTb.AutoCompleteCustomSource = source;
                    }
                    pts = db.Query("select data from evolucao group by data asc");
                    if (pts != null)
                    {
                        data = new string[pts.Rows.Count];
                        DateTime aux;
                        for (int i = 0; i < pts.Rows.Count; i++)
                        {
                            if (DateTime.TryParse(pts.Rows[i]["data"].ToString(), out aux))
                                data[i] = aux.ToString("dd/MM/yyyy");
                        }
                        evoDataCb.Items.Clear();
                        evoDataCb.Items.AddRange(data);
                    }
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoNomeTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string erro = carregaEvolucao(evoNomeTb.Text);
                    if (""!=erro)
                    {
                        MessageBox.Show(erro);
                    }
                    else
                        loadChart();
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private string carregaEvolucao(int idPaciente)
        {
            try
            {
                string query = "SELECT * FROM paciente WHERE id='" + idPaciente + "'";
                DataTable pts = db.Query(query);
                if (pts != null)
                {
                    evoPacienteMtb.Text = idPaciente.ToString();
                    evoNomeTb.Text = pts.Rows[0]["nome"].ToString();
                    pts = db.Query("select id from teste where paciente='" + evoPacienteMtb.Text + "' order by id");
                    if (pts != null)
                    {
                        evoTesteCb.Items.Clear();
                        foreach (DataRow r in pts.Rows)
                        {
                            evoTesteCb.Items.Add(r["id"].ToString());
                        }
                        evoTesteCb.SelectedIndex = 0;
                    }
                    return "";
                }
                else
                    return "Paciente não encontrado!";

            }
            catch (Exception err)
            {
                if (debug_mode)
                    return err.Message;
                else
                    return "";
            }

        }
        private string carregaEvolucao(string nomePacinete)
        {
            try
            {
                string query = "SELECT * FROM paciente WHERE nome='" + nomePacinete + "'";
                DataTable pts = db.Query(query);
                if (pts != null)
                {
                    evoPacienteMtb.Text = pts.Rows[0]["id"].ToString();
                    pts = db.Query("select id from teste where paciente='" + evoPacienteMtb.Text+"' order by id");
                    if (pts != null)
                    {
                        evoTesteCb.Items.Clear();
                        foreach(DataRow r in pts.Rows)
                        {
                            evoTesteCb.Items.Add(r["id"].ToString());
                        }
                        evoTesteCb.SelectedIndex = 0;
                    }
                    /*
                    pts = db.query("select id from teste where paciente='" + evoPacienteMtb.Text + "' order by id");
                    if (pts!=null)
                    {

                        string[] data = new string[pts.Rows.Count];
                        DateTime aux;
                        for (int i = 0; i < pts.Rows.Count; i++)
                        {
                            if (DateTime.TryParse(pts.Rows[i]["data"].ToString(), out aux))
                                data[i] = aux.ToString("dd/MM/yyyy");
                        }
                        evoDataCb.Items.Clear();
                        evoDataCb.Items.AddRange(data);
                    }*/

                    return "";
                }
                else
                    return "Paciente não encontrado!";

            }
            catch (Exception err)
            {
                if (debug_mode)
                    return err.Message;
                else
                    return "";
            }

        }

        private void evoPacienteMtb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int p;
                    if (int.TryParse(evoPacienteMtb.Text, out p))
                    {
                        string erro = carregaEvolucao(p);
                        if (erro != "")
                        {
                            MessageBox.Show("Paciente não encontrado...");
                        }
                        else
                            loadChart();
                    }
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void limpaEvolucao()
        {
            try
            {
                evoPacienteMtb.Clear();
                evoNomeTb.Clear();
                evoInsUnidadeUd.Value = 0;
                evoInsDataDtp.Value = DateTime.Today;
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoLpLb_Click(object sender, EventArgs e)
        {
            try
            {
                limpaEvolucao();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoInserirBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (db.Query("select id from paciente where id='"+evoPacienteMtb.Text+"'")==null)
                {
                    MessageBox.Show("Antes disto, adicione um paciente...");
                    return;
                }
                DataTable dt = db.Query("select id from teste where paciente='"+evoPacienteMtb.Text+"' and tipo='"+evoInsTipo.SelectedIndex.ToString()+"'");
                string prox = db.proximo("teste", "id");
                if (dt==null)
                {
                    string erro = db.Comando("insert into teste values("+prox+",'" + evoPacienteMtb.Text + "','" + evoInsTipo.SelectedIndex.ToString() + "')");
                    if ("" == erro)
                    {

                    }
                    else
                    {
                        MessageBox.Show("Erro ao registrar novo teste: " + erro);
                        return;
                    }
                }
                else
                {
                    prox = dt.Rows[0]["id"].ToString();
                }
                dt = db.Query("select id from evolucao where data='" + evoInsDataDtp.Value.ToString("yyyy-MM-dd") + "' and teste='"+ prox+"'");
                if (dt!=null)
                {
                    if (MessageBox.Show("Este valor já foi definido. Deseja atualizar os dados?","Atualizar",MessageBoxButtons.YesNo)== DialogResult.Yes)
                    {
                        string query = "update evolucao set data='" + evoInsDataDtp.Value.ToString("yyyy-MM-dd") + "', unidade='" + evoInsUnidadeUd.Value.ToString() + 
                            "' where id='"+dt.Rows[0]["id"].ToString()+"'",
                           erro = db.Comando(query);
                        if (erro != "")
                            MessageBox.Show("Erro ao atualizar teste: " + erro);
                        else
                        {
                            MessageBox.Show("Dado inserido no teste com sucesso!");
                            carregaEvolucao(evoPacienteMtb.Text);
                        }
                    }
                }
                else
                {
                    string query = "INSERT INTO evolucao VALUES('"+db.proximo("evolucao","id")+"','"+
                        evoPacienteMtb.Text + "','" + 
                        prox+"','"+evoInsDataDtp.Value.ToString("yyyy-MM-dd")+"','"+evoInsUnidadeUd.Value.ToString()+"')",
                        erro=db.Comando(query);
                    if (erro != "")
                        MessageBox.Show(erro);
                    else
                    {
                        MessageBox.Show("Dado inserido no teste com sucesso!");
                        carregaEvolucao(evoPacienteMtb.Text);
                    }
                    
                }
                evoInsPn.Hide();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoInsDadoTb_Click(object sender, EventArgs e)
        {
            try
            {
                evoInsPn.BringToFront();
                if (!evoInsPn.Visible)
                    evoInsPn.Show();
                else
                    evoInsPn.Hide();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoTesteCb_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                loadChart();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void loadChart()
        {
            try
            {
                string query = "select e.data,e.unidade, t.tipo from evolucao as e inner join teste as t where e.teste=t.id and and t.tipo= e.paciente='" + evoPacienteMtb.Text + "' and e.data>='" + evoDeDtp.Value.ToString("yyyy-MM-dd") + "' and e.data<='" + evoAteDtp.Value.ToString("yyyy-MM-dd") + "' order by e.data asc";
                DataTable dt = db.Query(query);
                if (dt != null)
                {
                    evoChart.Show();
                    evoInsTipo.SelectedIndex = Convert.ToInt32(dt.Rows[0]["tipo"]);
                    DateTime auxDt;
                    evoChart.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM\nyyyy";
                    evoChart.Series["Dados"].Points.Clear();
                    evoChart.Series["Dados"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                    evoChart.Series["Desempenho"].Points.Clear();
                    evoChart.Series["Desempenho"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                    
                    if (DateTime.TryParse(dt.Rows[0]["data"].ToString(), out auxDt))
                        evoDeDtp.Value = auxDt;
                    if (DateTime.TryParse(dt.Rows[dt.Rows.Count-1]["data"].ToString(), out auxDt))
                        evoAteDtp.Value = auxDt;

                    System.Windows.Forms.DataVisualization.Charting.DataPoint pt = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                    pt.SetValueXY(dt.Rows[0]["data"], dt.Rows[0]["unidade"]);
                    evoChart.Series["Desempenho"].Points.Add(pt);
                    pt = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                    pt.SetValueXY(dt.Rows[dt.Rows.Count-1]["data"], dt.Rows[dt.Rows.Count-1]["unidade"]);
                    evoChart.Series["Desempenho"].Points.Add(pt);
                    //System.Windows.Forms.DataVisualization.Charting.DataPoint pt = new System.Windows.Forms.DataVisualization.Charting.DataPoint(evoChart.Series["Dados"]);
                  
                    //IEnumerable<DateTime> dtm = from value in Enumerable.Range(evoDeDtp.Value,evoDeDtp.Value) select value;
                    //IEnumerable<DateTime> dtm = from val in dt.Rows
                    
                    foreach(DataRow r in dt.Rows)
                    {
                        pt = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                        pt.SetValueXY(r["data"], r["unidade"]);
                        evoChart.Series["Dados"].Points.Add(pt);
                        //evoChart.Series["Desempenho"].Points.Add(pt);
                        //DateTime.TryParse(dt.Rows[i]["data"].ToString(), out auxDt);
                        //evoChart.Series["Dados"].Points.AddY(Convert.ToDouble(dt.Rows[i]["unidade"]));
                        //evoChart.Series["Dados"].Points.DataBindXY(r["data"],"data", r["unidade"], "unidade");
                        //evoChart.Series["Desempenho"].Points.AddY(Convert.ToDouble(dt.Rows[i]["unidade"]));
                    }

                }
                else
                    evoChart.Hide();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoDeDtp_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                loadChart();

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void evoInsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = db.Query("select id from teste where paciente='"+evoPacienteMtb.Text+"' and tipo='"+evoInsTipo.SelectedIndex.ToString()+"'");
                if (dt!=null)
                {

                }
                else
                {
                    if (DialogResult.Yes==MessageBox.Show("Inserir novo tipo de teste com unidade \""+evoInsTipo.Text+"\"? Lembrando que não é permitido criar mais de um teste com a mesma unidade para o mesmo paciente.","Novo",MessageBoxButtons.YesNo))
                    {
                        string prox = db.proximo("teste", "id"),
                            erro = db.Comando("insert into teste values(" + prox + "," + evoPacienteMtb.Text + "," + evoInsTipo.SelectedIndex.ToString() + ")");
                        if (erro == "")
                        {
                            loadChart();
                            MessageBox.Show("Novo teste criado com sucesso! Você já pode inserir os novos dados!");
                            evoInsPn.Show();

                            evoInsPn.BringToFront();
                        }
                        else
                            MessageBox.Show("Erro ao criar teste: " + erro);                        
                    }
                }
                evoInsUniLb.Text = evoInsTipo.Text;
                EvoNovoPn.Hide();
                loadChart();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void label65_Click(object sender, EventArgs e)
        {
            try
            {
                EvoNovoPn.BringToFront();
                if (EvoNovoPn.Visible)
                    EvoNovoPn.Hide();
                else
                    EvoNovoPn.Show();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void label66_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Recurso não implementado.");
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conPesqTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string query = "Select * from consulta as c inner join paciente as p where c.paciente = p.id and p.nome like '%" + conPesqTb.Text + "%'";
                    DataTable dt = db.Query(query);
                    conLv.Items.Clear();
                    conFinPacienteLb.Text = conPacienteTb.Text;
                    if (dt != null)
                    {
                        string codigos="";
                        foreach (DataRow r in dt.Rows)
                            codigos += r["id"].ToString() + " ";
                        if (codigos != "")
                            codigos = codigos.Substring(0, codigos.Length - 1);
                        listaConsultas(codigos);
                    }
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1NomeTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DataTable dt = db.Query("select id,nome,nascimento from paciente where nome='"+cad1NomeTb.Text+"'");
                    if (dt != null)
                    {
                        if (dt.Rows.Count==1)
                            carregaPaciente(int.Parse(dt.Rows[0]["id"].ToString()));
                        else
                        {
                            TableLayoutPanel pn = new TableLayoutPanel();
                            pn.ColumnCount = 1;
                            pn.RowCount = dt.Rows.Count;
                            RowStyle rs = new RowStyle(SizeType.Percent, 40);
                            List<Control> adds = new List<Control>();
                            pn.Name = "cad1OutrosPn";
                            pn.Width = 500;
                            Button[] chs = new Button[dt.Rows.Count];
                            DateTime nasc;
                            int he = 0;
                            for (int i = 0; i < dt.Rows.Count; i++ )
                            {
                                if (!DateTime.TryParse(dt.Rows[i]["nascimento"].ToString(),out nasc))
                                    nasc = DateTime.Today;
                                chs[i] = new Button();
                                chs[i].Height = 30;
                                he += chs[i].Height+6;
                                chs[i].Text = dt.Rows[i]["id"].ToString()+" - "+dt.Rows[i]["nome"].ToString()+" ("+nasc.ToString("dd/MM/yyyy")+")";
                                chs[i].Dock = DockStyle.Fill;
                                chs[i].Click += principalForm_Click;
                                pn.Controls.Add(chs[i]);
                                //pn.SetRow(chs[i], i);
                                adds.Add(chs[i]);
                            }
                            pn.Height = he;
                            cad1Pn.Controls.Add(pn);
                            Uteis.centralizaControl(pn);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        void principalForm_Click(object sender, EventArgs e)
        {
            Button snd = (Button)sender;
            carregaPaciente(Uteis.getIdFromString(snd.Text));
            snd.Parent.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                /*
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Filter = "Image Files (JPG,PNG,GIF)|*.JPG;*.PNG;*.GIF";
                dialog.InitialDirectory = Application.StartupPath + "\\images";
                if (!System.IO.Directory.Exists(dialog.InitialDirectory))
                    System.IO.Directory.CreateDirectory(dialog.InitialDirectory);
                if (DialogResult.Cancel != dialog.ShowDialog())
                {
                    if (System.IO.File.Exists(dialog.FileName))
                    {
                        //string file = dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\') + 1, dialog.FileName.Length-1 - dialog.FileName.LastIndexOf('\\'));
                        //System.IO.File.Copy(dialog.FileName,dialog.InitialDirectory+"\\"+file,true);
                        pictureBox1.Load(dialog.FileName);
                        pictureBox1.Refresh();
                        //pictureBox1.ImageLocation = dialog.InitialDirectory + "\\avatar.png";
                        //MessageBox.Show(pictureBox1.ImageLocation);
                    }
                }*/
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void finPn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (finPn.Visible)
                {
                    mostraFatura();
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        
        private void mostraFatura()
        {
            try
            {
                string query = "select * from fatura";
                DataTable dt = db.Query(query);
                if (dt!=null)
                {
                    List<ListViewItem> itens = new List<ListViewItem>();
                    ListViewItem itemAux;
                    string[] subitens;
                    object[] ar;
                    DateTime auxDt;
                    foreach (DataRow i in dt.Rows)
                    {
                        ar = i.ItemArray;
                        subitens = new string[ar.Length];
                        for (int x = 0; x < subitens.Length; x++)
                        {
                            if (x==2 || x==7)
                            {
                                if (!DateTime.TryParse(ar[x].ToString(), out auxDt))
                                    auxDt = new DateTime(1111, 11, 11);
                                subitens[x] = auxDt.ToString("yyyy/MM/dd") +" às " +auxDt.ToString("t");
                            }
                            else
                                subitens[x] = ar[x].ToString();
                        }
                        itemAux = new ListViewItem(subitens);
                        itens.Add(itemAux);
                    }
                    conFinLv.Items.Clear();
                    conFinLv.Items.AddRange(itens.ToArray());
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void label20_Click(object sender, EventArgs e)
        {
            try
            {
                mostraFatura();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conOperNovoLb_Click(object sender, EventArgs e)
        {
            try
            {
                if (conTaxNameCb.Text != "")
                {

                    if (DialogResult.Yes == MessageBox.Show("Deseja mesmo Adicionar uma operadora de cartões chamada \"" + conTaxNameCb.Text + "\"??", "Salvando...", MessageBoxButtons.YesNo))
                    {
                        DataTable dt = db.Query("Select * from operadora where nome='" + conTaxNameCb.Text + "'");
                        if (dt == null)
                        {//salvar
                            string id, query, padrao, taxas = "", erro;
                            string[] taxes = new string[13];
                            Control[] taxesTb = conTaxPn.Controls.Find("conTx0Tb", false);
                            TextBox aux;
                            if (conTaxEsteCh.Checked)
                            {
                                if (db.Comando("update operadora set padrao=0") == "")
                                    padrao = "1";
                                else
                                {
                                    conTaxEsteCh.Checked = false;
                                    padrao = "0";
                                }
                            }
                            else
                                padrao = "0";
                            id = db.proximo("operadora", "id");
                            query = "INSERT INTO operadora VALUES(" + id + ",'" +
                                   conTaxNameCb.Text + "',1,'";
                            for (int i = 0; i < 13; i++)
                            {
                                taxesTb = conTaxPn.Controls.Find("conTx" + i.ToString() + "Tb", false);
                                if (taxesTb.Length == 0)
                                    MessageBox.Show("index nao encontrado " + i.ToString());
                                aux = (TextBox)taxesTb[0];
                                aux.Text = aux.Text.Replace(',', '.');
                                taxes[i] = aux.Text;
                                if (i < 12)
                                    taxas += aux.Text + " ";
                                else
                                    taxas += aux.Text;

                            }
                            query += taxas + "',"+padrao+")";
                            erro = db.Comando(query);
                            if (erro != "")
                                MessageBox.Show("Falha ao salvar: " + erro);
                            else
                                MessageBox.Show("Operadora ADICIONADA com sucesso!");
                        }
                        else
                            MessageBox.Show("Esta operadora já está registrada!");
                    }
                }
                else
                    MessageBox.Show("Digite o nome da operadora de cartões!");
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conOperDesativarLb_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = db.Query("Select * from operadora where nome='" + conTaxNameCb.Text + "'");
                if ((conTaxNameCb.Text == "") || (dt == null))
                    MessageBox.Show("Nada a ser excluido!");
                else
                {
                    if (DialogResult.Yes == MessageBox.Show("Deseja mesmo DESATIVAR a operadora \"" + conTaxNameCb.Text + "\" do banco de dados??", "Excluir", MessageBoxButtons.YesNo))
                    {
                        string erro = db.Comando("upadte operadora set ativo=0 where nome='" + conTaxNameCb.Text + "'");
                        if (erro != "")
                            MessageBox.Show("Não foi possível desativar operadora: erro\"" + erro + "\"");
                        else
                        {
                            MessageBox.Show("Operadora foi DESATIVADA com sucesso!");
                            carregaConfig();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conTaxEsteCh_Click(object sender, EventArgs e)
        {
            try
            {
                salvaOperadora();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conFinLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                double sum = 0,aux;
                foreach(ListViewItem i in conFinLv.SelectedItems)
                {
                    if (!double.TryParse(i.SubItems[3].Text, out aux))
                        aux = 0;
                    sum += aux;
                }
                FinTotLb.Text = "R$ " + sum.ToString("F2");
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(Application.StartupPath);
                entry.AuthenticationType = System.DirectoryServices.AuthenticationTypes.None;
                FolderBrowserDialog folder = new FolderBrowserDialog();
                folder.SelectedPath = Application.StartupPath + @"\Backup";
                if (folder.ShowDialog()== DialogResult.OK)
                {
                    if (!System.IO.Directory.Exists(folder.SelectedPath))
                        System.IO.Directory.CreateDirectory(folder.SelectedPath);
                    System.IO.File.Copy(Application.StartupPath+@"\"+"anamsys",folder.SelectedPath+@"\anamsys",true);
                    MessageBox.Show("Backup realizado com sucesso.");
                }

            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void cad1CpfMtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                if (!Uteis.ValidaCPF(cad1CpfMtb.Text))
                {
                    cad1CpfMtb.BackColor = Color.Yellow;
                }
                else
                {
                    cad1CpfMtb.BackColor = Color.White;
                    carregaPaciente(cad1CpfMtb.Text);
                }
            }
        }

        private void conPacienteTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode ==Keys.Enter)
            {
                DataTable dt = db.Query("select * from paciente where nome like '%" + conPacienteTb.Text + "%'");
                if (dt != null)
                {
                    if (dt.Rows.Count > 1)
                    {
                        TableLayoutPanel pn = new TableLayoutPanel();
                        pn.ColumnCount = 1;
                        pn.RowCount = dt.Rows.Count;
                        RowStyle rs = new RowStyle(SizeType.Percent, 40);
                        List<Control> adds = new List<Control>();
                        pn.Name = "conOutrosPn";
                        pn.Width = 500;
                        Button[] chs = new Button[dt.Rows.Count];
                        DateTime nasc;
                        int he = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (!DateTime.TryParse(dt.Rows[i]["nascimento"].ToString(), out nasc))
                                nasc = DateTime.Today;
                            chs[i] = new Button();
                            chs[i].Height = 30;
                            he += chs[i].Height + 6;
                            chs[i].Text = dt.Rows[i]["id"].ToString() + " - " + dt.Rows[i]["nome"].ToString() + " (" + nasc.ToString("dd/MM/yyyy") + ")";
                            chs[i].Dock = DockStyle.Fill;
                            chs[i].Click +=consultasPaciente_Click;
                            pn.Controls.Add(chs[i]);
                            adds.Add(chs[i]);
                        }
                        pn.Height = he;
                        conDetailsPn.Controls.Add(pn);
                        Uteis.centralizaControl(pn);
                    }
                    else
                    {
                        conPacienteIdLb.Text = dt.Rows[0]["id"].ToString();
                    }
                }
                else
                {
                    conPacienteIdLb.Text = "0";
                    if (DialogResult.Yes == MessageBox.Show("Nome não encontrado. Deseja cadastrar novo paciente?","Cadastrar Novo",MessageBoxButtons.YesNo))
                    {
                        limpaPaciente();
                        mostraGb(cad1Pn);
                    }
                }

            }
        }

        void consultasPaciente_Click(object sender, EventArgs e)
        {
            Button snd = (Button)sender;
            conPacienteIdLb.Text = Uteis.getIdFromString(snd.Text).ToString();
            snd.Parent.Dispose();
        }

        private void conMinCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox snd = (ComboBox)sender;
            int aux;
            if (!int.TryParse(snd.Text,out aux))
            {
                MessageBox.Show("Formato incorreto...");
                snd.Text = "00";
            }
        }

        private void conMinCb_Leave(object sender, EventArgs e)
        {
            ComboBox snd = (ComboBox)sender;
            int aux;
            if (!int.TryParse(snd.Text, out aux))
            {
                MessageBox.Show("Formato incorreto...");
                snd.Text = "00";
            }
        }

        private void conDetFinDg_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                conDetFinDg.Rows[e.RowIndex].Cells[0].Value = e.RowIndex+1;
                switch(e.ColumnIndex)
                {
                    case 2:
                        DateTime d;
                        if (DateTime.TryParse(conDetFinDg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out d))
                            conDetFinDg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = d.ToString("dd/MM/yyyy");
                        else
                            conDetFinDg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                        break;
                    case 3:
                        float f;
                        if (float.TryParse(conDetFinDg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out f))
                            conDetFinDg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = f.ToString("F2");
                        else
                            conDetFinDg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0,00";
                        break;
                }
                calculaTotais();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void conDetFinDg_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                conDetFinDg.Rows[e.RowIndex].Cells[0].Value = (conDetFinDg.Rows.Count).ToString();
                DataGridViewComboBoxCell cb = conDetFinDg.Rows[e.RowIndex].Cells[3].Cells[1] as DataGridViewComboBoxCell;
                conDetFinDg.Rows[e.RowIndex].Cells[3].Value = DateTime.Today.ToString("dd/MM/yyyy");
                conDetFinDg.Rows[e.RowIndex].Cells[3].Value = "0,00";
                calculaTotais();
            }
            catch (Exception err)
            {
                if (debug_mode)
                    MessageBox.Show(err.Message);
            }
        }

        private void calculaTotais()
        {
            try
            {
                float sub=0, total=0, aux;
                bool auxB;
                foreach(DataGridViewRow r in conDetFinDg.Rows)
                {
                    if (Convert.ToBoolean(r.Cells[4].Value) == true)
                    {
                        if (float.TryParse(r.Cells[3].Value.ToString(), out aux))
                            sub += aux;
                        else
                            r.Cells[3].Value = "0,00";
                    }
                    if (float.TryParse(r.Cells[3].Value.ToString(), out aux))
                        total += aux;
                    else
                        r.Cells[3].Value = "0,00";
                }
                conDetSubtotalLb.Text = sub.ToString("F2");
                conDetTotalLb.Text = total.ToString("F2");
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

    class Uteis
    {
        public static bool ValidaCPF(string cpf)
        {
            try
            {
                /*Validar cpf*/
                int c1 = int.Parse(cpf.Substring(0, 1)),
                    c2 = int.Parse(cpf.Substring(1, 1)),
                    c3 = int.Parse(cpf.Substring(2, 1)),
                    c4 = int.Parse(cpf.Substring(4, 1)),
                    c5 = int.Parse(cpf.Substring(5, 1)),
                    c6 = int.Parse(cpf.Substring(6, 1)),
                    c7 = int.Parse(cpf.Substring(8, 1)),
                    c8 = int.Parse(cpf.Substring(9, 1)),
                    c9 = int.Parse(cpf.Substring(10, 1)),
                    c10 = int.Parse(cpf.Substring(12, 1)),
                    c11 = int.Parse(cpf.Substring(13, 1)),
                    soma1 = c1 * 10 + c2 * 9 + c3 * 8 + c4 * 7 + c5 * 6 + c6 * 5 + c7 * 4 + c8 * 3 + c9 * 2,
                    resto1 = soma1 % 11,
                    soma2 = c1 * 11 + c2 * 10 + c3 * 9 + c4 * 8 + c5 * 7 + c6 * 6 + c7 * 5 + c8 * 4 + c9 * 3 + c10 * 2,
                    resto2 = soma2 % 11;
                if (resto1 < 2)
                    resto1 = 0;
                else
                    resto1 = 11 - resto1;
                if (resto2 < 2)
                    resto2 = 0;
                else
                    resto2 = 11 - resto2;
                if ((resto1 != c10) ||
                    (resto2 != c11) ||
                    (cpf == "000.000.000-00") ||
                    (cpf == "111.111.111-11") ||
                    (cpf == "222.222.222-22") ||
                    (cpf == "333.333.333-33") ||
                    (cpf == "444.444.444-44") ||
                    (cpf == "555.555.555-55") ||
                    (cpf == "666.666.666-66") ||
                    (cpf == "777.777.777-77") ||
                    (cpf == "888.888.888-88") ||
                    (cpf == "999.999.999-99"))
                {
                    return false;
                }
                else
                    return true;
            }
            catch
            { return false; }
        }
        public static void centralizaControl(Control control)
        {
            control.Location = new Point(control.Parent.Width / 2 - control.Width / 2, control.Parent.Height / 2 - control.Height / 2);
            control.BringToFront();
        }

        public static string getNameFromString(string s)
        {
            try
            {
                int index = s.IndexOf(" - ");
                if (index == -1)
                    return "";
                else
                    return s.Substring(index + 3, s.Length - (index + 3));
            }
            catch
            {
                return "";
            }
        }

        public static int getIdFromString(string s)
        {
            try
            {
                int aux, index = s.IndexOf(" - ");
                if (index == -1)
                    return -1;
                else
                {
                    if (int.TryParse(s.Substring(0, index), out aux))
                        return aux;
                    else
                        return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        public static string[] Split(string str, string chars)
        {
            List<string> splited = new List<string>();
            int i = str.IndexOf(chars);
            string piece = str;
            if (str.Length > 0)
            {
                if (i == -1)
                    splited.Add(str);
                while (i != -1)
                {
                    piece = str.Substring(0, i);
                    splited.Add(piece);
                    if (str.Length > 3)
                    {
                        str = str.Substring(i + chars.Length, str.Length - (piece.Length + chars.Length));
                        i = str.IndexOf(chars);
                        if (i == -1)
                            splited.Add(str);
                    }
                    else
                        i = -1;
                }
                return splited.ToArray();
            }
            else
                return new string[0];
        }
        public static string Join(string[] array, string chars)
        {
            string res = "";
            if (array.Length > 0)
            {
                res = array[0];
                for (int i = 1; i < array.Length; i++)
                    res += chars + array[i];
            }
            return res;
        }

        public static string SerializeItens(System.Windows.Forms.ListViewItem[] items)
        {
            try
            {
                if (items.Length > 0)
                {
                    string serialized = "", scaped;
                    for (int i = 0; i < items.Length; i++)
                    {
                        serialized += "[";
                        for (int j = 0; j < items[i].SubItems.Count; j++)
                        {
                            scaped = items[i].SubItems[j].Text.Replace('[', '|');
                            scaped = scaped.Replace(']', '&');
                            scaped = scaped.Replace(',', ';');
                            serialized += "[" + scaped + "],";
                        }
                        serialized = serialized.Substring(0, serialized.Length - 1);
                        serialized += "],";
                    }
                    serialized = serialized.Substring(0, serialized.Length - 1);
                    return serialized;
                }
                else
                    return "";
            }
            catch
            { return ""; }
        }

        public static System.Windows.Forms.ListViewItem[] UnserializeItens(string serialized)
        {
            try
            {
                List<System.Windows.Forms.ListViewItem> list = new List<System.Windows.Forms.ListViewItem>();
                if (serialized.Length > 0)
                {/*   
                      *   [[x],[y]], <-lv1 2subs
                      *   [[e],[d],[g]], <-lv2 3 subs
                      *   [[a]] <lv3 1 sub 
                      *  
                      */
                    string auxS = serialized, unScape;
                    System.Windows.Forms.ListViewItem auxLi;
                    List<string> subs = new List<string>();
                    int indexAbre = auxS.IndexOf("["), indexFecha;
                    while (auxS.IndexOf("[") != -1)
                    {
                        if (auxS[indexAbre + 1] == '[')
                        {//subs
                            indexAbre = indexAbre + 2;
                        }
                        else
                            indexAbre++;
                        indexFecha = auxS.IndexOf("]");
                        unScape = auxS.Substring(indexAbre, indexFecha - indexAbre);
                        subs.Add(unScape);
                        if (auxS[indexFecha + 1] == ',')
                        {
                            auxS = auxS.Substring(indexFecha + 1, auxS.Length - (indexFecha + 1));
                        }
                        else
                        {
                            //subs.RemoveAt(subs.Count - 1);
                            foreach (string str in subs)
                            {
                                unScape = str.Replace('|', '[');
                                unScape = unScape.Replace('&', ']');
                                unScape = unScape.Replace(';', ',');
                            }
                            auxLi = new System.Windows.Forms.ListViewItem(subs.ToArray());
                            subs.Clear();
                            list.Add(auxLi);
                            auxS = auxS.Substring(indexFecha + 1, auxS.Length - (indexFecha + 1));
                            indexAbre = auxS.IndexOf(',');
                            if (indexAbre != -1)
                                auxS = auxS.Substring(indexAbre + 1, auxS.Length - (indexAbre + 1));
                        }
                        indexAbre = auxS.IndexOf("[");
                    }
                    System.Windows.Forms.ListViewItem[] ar = list.ToArray();
                    if (ar == null)
                        ar = new System.Windows.Forms.ListViewItem[0];
                    return ar;
                }
                else
                    return new System.Windows.Forms.ListViewItem[0];
            }
            catch
            { return null; }
        }
    }
}
