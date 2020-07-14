using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LlenarCombo();
            cbxMatricula.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSemestre.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LlenarCombo()
        {
            String cbx1 = "1";
            String cbx2 = "2";
            String cbx3 = "3";

            String cbx4 = "Primer Semestre";
            String cbx5 = "Segundo Semestre";
            String cbx6 = "Tercer Semestre";
            String cbx7 = "Cuarto Semestre";
            String cbx8 = "Quito Semestre";
            String cbx9 = "Sexto Semestre";


            cbxMatricula.Items.Add(cbx1);
            cbxMatricula.Items.Add(cbx2);
            cbxMatricula.Items.Add(cbx3);

            cbxSemestre.Items.Add(cbx4);
            cbxSemestre.Items.Add(cbx5);
            cbxSemestre.Items.Add(cbx6);
            cbxSemestre.Items.Add(cbx7);
            cbxSemestre.Items.Add(cbx8);
            cbxSemestre.Items.Add(cbx9);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {            
            frmVerificacion frmVerificacion = new frmVerificacion(this);
            frmVerificacion.Show();
            this.Hide();
        }
    }
}
