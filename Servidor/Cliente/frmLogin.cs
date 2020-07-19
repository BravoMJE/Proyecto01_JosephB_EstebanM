using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using BlibliotecaCambioMalla;

namespace Cliente
{
    public partial class frmLogin : Form
    {

        //Variables para conexion TCP
        IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint endPoint;
        int puerto = 4550;
        Estudiante es;
        

        //Sockets
        Socket s_servidor;
        Socket s_cliente;

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

            Estudiante es = new Estudiante();
            es.Correo = txtUser.Text;
            es.Contrasenia = txtPass.Text;
            iniciarEnvio(es);

            byte[] bytesVerificacion = new byte[4];
            //s_cliente.Receive(bytesVerificacion);
            s_cliente.Receive(bytesVerificacion);
            string validacion = Encoding.ASCII.GetString(bytesVerificacion);
            Console.WriteLine(validacion);
            if (validacion.Equals("true"))
            {
                this.Hide();
                frmMain frmMain = new frmMain();
                frmMain.Show();
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Usuario o Password incorrectos", "ERROR", MessageBoxButtons.OK);
            }

        }


        public void iniciarEnvio(Estudiante es)
        {
            //Obtenemos un hostentry
            host = Dns.GetHostEntry("127.0.0.1");
            //Obtenemos la primera direccion de la lista
            ipAddr = host.AddressList[0];

            //Creamos una variable enpointn con la ip obtenida y el puerto especificado
            endPoint = new IPEndPoint(ipAddr, puerto);

            //Instanciamos un socket servidor con TCP
            s_cliente = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            s_cliente.Connect(endPoint);

            //creamos un buffer para la lectura desde el cliente
            
            //byte[] buffertx = Encoding.ASCII.GetBytes(msj);

            s_cliente.Send(BinarySerialization.Serializate(es));
           
        }





    }


}
