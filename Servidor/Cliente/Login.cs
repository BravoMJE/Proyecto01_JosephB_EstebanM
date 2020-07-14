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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
                
        }

        private void btnSalir_Click_1(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("¿Seguro que desea Salir?", "Aviso", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
            
        }

        private void txtUser_Enter(object sender, EventArgs e)
        {
            if(txtUser.Text == "usuario@epn.edu.ec")
            {
                txtUser.Text = "";
            }
        }

        private void txtUser_Leave(object sender, EventArgs e)
        {
            if (txtUser.Text == "")
            {
                txtUser.Text = "usuario@epn.edu.ec";
            }
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            if (txtPass.Text == "password")
            {
                txtPass.Text = "";
            }
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (txtPass.Text == "")
            {
                txtPass.Text = "password";
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (true)
            {
                this.Hide();
                frmMain frmMain = new frmMain();
                frmMain.Show();
            }
            
        }
    }
}
