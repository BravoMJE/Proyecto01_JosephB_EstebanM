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
                this.Close();
            }
        }

        private void frmVerificacion_Load(object sender, EventArgs e)
        {
            ActualizarMatAproV();
        }


        public void ActualizarMatAproV()
        {
            listFinal.Items.Clear();
            foreach (Materia i in frmPadreObj.matApro)
            {
                listFinal.Items.Add(i);
            }
        }

    }
}
