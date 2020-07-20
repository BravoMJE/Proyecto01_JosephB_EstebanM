using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BlibliotecaCambioMalla;

namespace Servidor
{
    class Servidor
    {

        //declaracion de variables usadas
        //variable de conexion hacia la base de datos
        static IFirebaseClient clienteDB;

        //Variables para conexion TCP
        IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint endPoint;
        int puerto = 4550; 

        //Sockets
        Socket s_servidor;
        Socket s_cliente;


        //listas de usuarios y materias
        static List<Estudiante> estudiantes = new List<Estudiante>();
        static ListaMaterias materias15 = new ListaMaterias();
        static ListaMaterias materias20 = new ListaMaterias();
        static ListaMaterias matAConv = new ListaMaterias();
        static ListaMaterias matConv = new ListaMaterias();


        static void Main(string[] args)
        {
            //Inicio de conexion con la base de datos
            ConectarDB();
            RecuperarEstudiantes();
            RecuperarMaterias15();
            RecuperarMaterias20();

            materias20.listar();
            Console.WriteLine("Servidor listo");
            new Servidor();

            Console.ReadLine();
        }



        public Servidor()
        {
            //Obtenemos un hostentry
            host = Dns.GetHostEntry("127.0.0.1");
            //Obtenemos la primera direccion de la lista
            ipAddr = host.AddressList[0];

            //Creamos una variable enpointn con la ip obtenida y el puerto especificado
            endPoint = new IPEndPoint(ipAddr, puerto);

            //Instanciamos un socket servidor con TCP
            s_servidor = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            s_servidor.Bind(endPoint);
            s_servidor.Listen(10);

            while (true)
            {
                

                //Inializamos el socket cliente
                s_cliente = s_servidor.Accept();

                if (s_cliente.Connected)
                {
                    Thread hiloCliente = new Thread(new ThreadStart(Recibir));
                    hiloCliente.IsBackground = true;
                    hiloCliente.Start();
                }

                
            }
            
        }

        public void Recibir()
        {
            Socket sCliente  = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Object es;
            lock (this)
            {
                sCliente = s_cliente;
            }


            while (true)
            {
                try
                {
                    byte[] bufferRX = new byte[1024];
                    s_cliente.Receive(bufferRX);
                    es = (Object)BinarySerialization.Deserializate(bufferRX);
                    
                    if (typeof(Estudiante).IsInstanceOfType(es))
                    {
                        validarEstudiante((Estudiante)es);
                    }
                    if (typeof(ListaMaterias).IsInstanceOfType(es))
                    {
                        ValidarMaterias((ListaMaterias)es);
                    }
 
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error: " + e); 
                }

                if (!sCliente.Connected)
                    break;
            }

        }


        public void ValidarMaterias(ListaMaterias lst)
        {
            matAConv = lst;
            Console.WriteLine("Materias a convalidar");
            Console.WriteLine("Espere ....");



            //Algebra cod20 MATD113 cod15 MATR114
            if ((matAConv.BuscarCod("MATR114")) != null)
            {
                
                matConv.AggMaterias(materias20.BuscarCod("MATD113"));
                matConv.BuscarCod("MATD113").NumMatricula = matAConv.BuscarCod("MATR114").NumMatricula;
            }

            s_cliente.Send(BinarySerialization.Serializate(matConv));
            matConv.listar();

        }


        public void validarEstudiante(Estudiante es)
        {
            foreach (Estudiante estudiante in estudiantes)
            {
                if (es.Correo.Equals(estudiante.Correo) && es.Contrasenia.Equals(estudiante.Contrasenia))
                {
                    Console.WriteLine("correcto");
                    byte[] response = Encoding.ASCII.GetBytes("true");
                    s_cliente.Send(response);
                    EnviarMaterias();
                    break;
                }
                else
                {
                    Console.WriteLine("incorrecto");
                    byte[] response = Encoding.ASCII.GetBytes("false");
                    s_cliente.Send(response);
                    
                }
            }


        }

        //metodo para enviar materias
        public void EnviarMaterias()
        {
            s_cliente.Send(BinarySerialization.Serializate(materias15));
        }



        //Metodos de recuperacion de informacion desde la 
        //base de datos

        //Metodo de configuracion de firebase
        static IFirebaseConfig config = new FirebaseConfig
        {
            //clave secreta firebase
            AuthSecret = "eUu4ME4xQww16dk60reONqttxKUm5pLS3c13BDwF",
            //Url de conexion a firebase
            BasePath = "https://fir-pruebavisual.firebaseio.com/"
        };

        //Metodo de conexion a la base de datos 
        static void ConectarDB()
        {

            try
            {
                //Al cliente firebase agregamos la configuracion del metodo 
                //Anterior
                clienteDB = new FireSharp.FirebaseClient(config);
                
                
                //Si se logra la conexion se notofica por consola
                if (clienteDB != null)
                {
                    Console.WriteLine("Conexion exitosa");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al conectar revise su conexion a internet");
            }


        }


        //Recuperar los estudiantes de la base de datos
        static void RecuperarEstudiantes()
        {
            //instancia de un firebaseresponse
            //almacena lo recuperado de la base pasado el path de
            //lo que se quiere recuperar de la base
            //contEs es una variable de la base de datos que nos indica 
            //cuantos estudiantes hay en la base de datos
            FirebaseResponse resEs = clienteDB.Get(@"ContEs");
            //se hace el cast de lo recibido a un entero
            int counter = int.Parse(resEs.ResultAs<string>());


            //lazo que nos permitira recuperar los estudiantes
            for (int i = 1; i <= counter; i++)
            {
                //Se recupera un esudiante a la vez pasandole
                //al metodo get el path del la estructura que almacena
                //los estudiantes y el id del estudiante
                var resEst = clienteDB.Get(@"Estudiante/" + i);
                
                //Cast del objeto resultante a un estudiante 
                Estudiante e = resEst.ResultAs<Estudiante>();

                //Se añade el estudiante a la lista 
                estudiantes.Add(e);
            }
        }


        //Recuperar las materias 2015 de la base de datos
        static void RecuperarMaterias15()
        {

            //instancia de un firebaseresponse
            //almacena lo recuperado de la base pasado el path de
            //lo que se quiere recuperar de la base
            //ContMa15 es una variable de la base de datos que nos indica 
            //cuantas materias hay en la base de datos

            FirebaseResponse resMa = clienteDB.Get(@"ContMa15");
            int counter = int.Parse(resMa.ResultAs<string>());


            //lazo que nos permitira recuperar los materias
            for (int i = 1; i <= counter; i++)
            {
                //Se recupera un materias a la vez pasandole
                //al metodo get el path del la estructura que almacena
                //los materias y el id del materia
                var resMat = clienteDB.Get(@"Materia15/" + i);

                //Cast del objeto resultante a una materia
                Materia m = resMat.ResultAs<Materia>();

                //Se añade la materia a la lista 
                materias15.AggMaterias(m);
            }
        }


        //el metodo recuperar materias de la misma forma que el metodo anterior
        static void RecuperarMaterias20()
        {
            FirebaseResponse resMa = clienteDB.Get(@"ContMa20");
            int counter = int.Parse(resMa.ResultAs<string>());

            for (int i = 1; i <= counter; i++)
            {
                var resMat = clienteDB.Get(@"Materia20/" + i);
                Materia m = resMat.ResultAs<Materia>();
                materias20.AggMaterias(m);
            }
        }

       


    }
}
