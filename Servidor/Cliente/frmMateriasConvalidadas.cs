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
    public partial class frmMateriasConvalidadas : Form
    {

        //referencia al frmLogin
        frmLogin frmPadre;
        public frmMateriasConvalidadas(frmLogin frm)
        {
            frmPadre = frm;
            InitializeComponent();
        }

        private void frmMateriasConvalidadas_Load(object sender, EventArgs e)
        {
            ActualizarMatConv();
        }


        public void ActualizarMatConv()
        {
            listFinal.Items.Clear();
            foreach (Materia i in frmPadre.estudiante.MateriasConvalidadas.List)
            {
                listFinal.Items.Add(i);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
