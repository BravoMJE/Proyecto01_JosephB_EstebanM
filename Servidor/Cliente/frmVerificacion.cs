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
                Application.Exit();
            }
        }
    }
}
