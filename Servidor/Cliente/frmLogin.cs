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

        //Variables
        public Estudiante estudiante;

        //Variables para conexion TCP
        IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint endPoint;
        int puerto = 4550;
        Estudiante es;
        

        //Sockets
        public Socket s_cliente;

        public frmLogin()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;

        }


        //Metodo para salir de la aplicacion 
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


        //Boton ingresar con este evento se envia un objeto estudiante 
        //que contiene las credenciales que se validaran en el servidor
        //este devolvera la respuesta
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            //Instancia de un estudiante vacio
            Estudiante es = new Estudiante();

            //Se setea las variables ingresadas en el login al
            //objeto estudiante
            es.Correo = txtUser.Text;
            es.Contrasenia = txtPass.Text;
            
            
            //Llamada al metodo iniciar envio de estudiante
            iniciarEnvio(es);

            /*
             * cambiar buffer por ahora solo con 4 bytes se necesita recortar
            */

            //recepcion de la validacion de las credenciales del estudiante por parte 
            //del servidor
            byte[] bytesVerificacion = new byte[4];
            s_cliente.Receive(bytesVerificacion);
            //se hace el cast a un string para la validacion en la applicacion 
            //del cliente
            string validacion = Encoding.ASCII.GetString(bytesVerificacion);
            Console.WriteLine(validacion);
            if (validacion.Equals("true"))
            {
                //si la validacion es correcta el servidor envia la lista de materias
                //correspondiente al pensum 2015
                byte[] listaMat = new byte[10024];
                s_cliente.Receive(listaMat);

                //cast y asignacion de las materias recibidas desde el cliente
                //se hace la llamada a deserializate para obtener un objeto
                ListaMaterias materias2015 = (ListaMaterias)BinarySerialization.Deserializate(listaMat);


                //si la validacion es correcta el servidor envia el estudiante logueado
                byte[] buffEstudiante = new byte[10024];
                s_cliente.Receive(buffEstudiante);

                //cast y asignacion de las materias recibidas desde el cliente
                //se hace la llamada a deserializate para obtener un objeto
                estudiante = (Estudiante)BinarySerialization.Deserializate(buffEstudiante);

                if (estudiante.Convalidado)
                {
                    frmMateriasConvalidadas frmConva = new frmMateriasConvalidadas(this);
                    this.Hide();
                    frmConva.Show();
                }
                else
                {
                    //llamada al formulario main el cual permite seleccionar las materias al estudiante
                    //se le envia como parametro la lista de materias y el formulario login
                    frmMain frmMain = new frmMain(materias2015, this);
                    this.Hide();
                    frmMain.Show();
                }

                
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Usuario o Password incorrectos", "ERROR", MessageBoxButtons.OK);
            }

        }


        //Metodo que nos permite realizar el envio de un objeto 
        //hacia el servidor
        public void iniciarEnvio(Object obj)
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

            s_cliente.Send(BinarySerialization.Serializate(obj));
           
        }





    }


}
