using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlibliotecaCambioMalla;

namespace Cliente
{
    public partial class frmVerificacion : Form
    {
        frmMain frmPadreObj;
        public frmVerificacion(frmMain frmPadre)
        {
            InitializeComponent();
            frmPadreObj = frmPadre;
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
            frmPadreObj.Show();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Seguro que desea Enviar?", "Aviso", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                btnSalir.Visible = true;
                btnVolver.Visible = false;
                btnAceptar.Visible = false;
                lblAcuerdo.Visible = false;
                frmPadreObj.aceptacion = true;

                frmPadreObj.frmLogPadre.estudiante.Convalidado = true;
                ListaMaterias materiasConvalidadas = new ListaMaterias();
                materiasConvalidadas.List = frmPadreObj.matCon;
                frmPadreObj.frmLogPadre.estudiante.MateriasConvalidadas = materiasConvalidadas;
                frmPadreObj.frmLogPadre.iniciarEnvio(frmPadreObj.frmLogPadre.estudiante);
                
            }
        }

        private void frmVerificacion_Load(object sender, EventArgs e)
        {
            btnSalir.Visible = false;
            ActualizarMatAproV();
        }


        public void ActualizarMatAproV()
        {
            listFinal.Items.Clear();
            foreach (Materia i in frmPadreObj.matCon)
            {
                listFinal.Items.Add(i);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //frmPadreObj.frmLogPadre.s_cliente.Close();
            Application.Exit();
        }
    }
}
