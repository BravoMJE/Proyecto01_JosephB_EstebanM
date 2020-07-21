// ************************************************************************
// Proyecto 01
// Joseph Bravo, Esteban Machado
// Fecha de realizacion: 21/07/2020
// Fecha de entrega: 22/07/2020
// Resultados:
// * La siguiente solucion nos permite hacer la convalidacion de materias
//   del pensum 2015 hacia el pensum 2020 mediante el uso de sockets, 
//   ademas se usa hilos para manejar a varios clientes, como base de datos se
//   uso una base de datos no relacional en linea de donde se recuperaran 
//   los estudiantes y las materias de los dos pensum.
//   Para el intercambio de datos entre servidor y cliente se uso la 
//   serializacion de objetos 
// Conclusiones:
// * Se verifico la importancia del uso de hilos para la asignacion 
//   de diferentes funcionalidades
// * El comprobo que LOCK es importante ya que permite evitar conflictos
//   cuando se pueden ejecutar varios hilos y estos manejan recursos comportidos
// * La serializacion de objetos ayuda a que estos sean enviables a travez de TCP
//   en este caso.
// Recomendaciones:
//*Verificar la disponibilidad de los puertos a usar y cerrarlos al momento de 
//terminar el uso
// ************************************************************************

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
        static ListaMaterias matConv;


        static void Main(string[] args)
        {
            //Inicio de conexion con la base de datos
            ConectarDB();
            Console.WriteLine("Conexion a la base de datos exitosa.");

            //Recuperacion desde la base de datos de todos los datos
            //necesarios

            Console.WriteLine("Recuperando informacion de la base de datos, espere por favor.");
            RecuperarEstudiantes();
            RecuperarMaterias15();
            RecuperarMaterias20();
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


            //se instancia un hilo que estara a la escucha 
            //de nuevos clientes
            Thread hiloEscucha = new Thread(new ThreadStart(Escuchar));
            hiloEscucha.IsBackground = true;
            hiloEscucha.Start();



            
            
        }



        public void Escuchar()
        {
            //bucle que nos permite aceptar multiples usuarios
            while (true)
            {
                //Metodo del hilo que esta a la escucha de nuevos clientes
                s_servidor.Listen(-1);

                //Inializamos el socket cliente
                s_cliente = s_servidor.Accept();

                if (s_cliente.Connected)
                {

                    //Se instancia un hilo por cada cliente que se conecta
                    //pasandole el delegado con el metodo recibir
                    Thread hiloCliente = new Thread(new ThreadStart(Recibir));

                    //Este hilo se ejecuta en segundo plano
                    hiloCliente.IsBackground = true;
                    hiloCliente.Start();
                }


            }
        }


        //Metodo cliente
        public void Recibir()
        {
            //Se asigna un socket para el cliente
            Socket sCliente  = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //instancia de un objeto para recibir desde el cliente
            Object obj;
            
            
            //se bloquea el cliente para que no existan conflictos
            lock (this)
            {
                sCliente = s_cliente;
            }


            //bucle que recibe varios mensajes del cliente 
            while (true)
            {
                try
                {

                    //declarion del buffer que nos permite almacenar lo enviado 
                    //por el cliente
                    byte[] bufferRX = new byte[10024];
                    
                    //llamada el metodo recibe
                    s_cliente.Receive(bufferRX);

                    //Deserializacion del objeto enviado por el cliente
                    obj = (Object)BinarySerialization.Deserializate(bufferRX);
                    
                    //verificacion del tipo de objeto que ha enviado el cliente
                    if (typeof(Estudiante).IsInstanceOfType(obj))
                    {
                        //Si es un estudiante se redirige al metodo validar
                        //estudiante
                        if (((Estudiante)obj).Convalidado)
                        {
                            //Si el estudiante setea convalidado
                            //se procede a llamar al metodo actualizar informacion
                            //del estudiante en la base de datos
                            ActualizarInforEstudiante((Estudiante)obj);
                        }
                        else
                        {
                            //si el atributo convalidado del estudiante es falso
                            //se procede a validar las credenciales del estudiante
                            validarEstudiante((Estudiante)obj);
                        }
                        
                    }
                    if (typeof(ListaMaterias).IsInstanceOfType(obj))
                    {
                        //si es un objeto ListaMaterias se redirige al metodo
                        //validar materias
                        ValidarMaterias((ListaMaterias)obj);
                    }
 
                }
                catch (Exception)
                {

                    Console.WriteLine("Cliente ha salido");
                    
                }

                if (!sCliente.Connected)
                    break;
            }

            //sCliente.Close();

        }

        public void ActualizarInforEstudiante(Estudiante es)
        {
            //Se envia la informacion actualiza a la base de datos
            var set = clienteDB.Set(@"Estudiante/" + es.IdEstudiante, es);
        }


        public void ValidarMaterias(ListaMaterias lst)
        {
            matAConv = lst;
            Console.WriteLine("Materias a convalidar");
            matConv = new ListaMaterias();

            //////////////////////////////////////////////////
            ///   PRIMER SEMESTRE
            /////////////////////////

            //Algebra cod20 MATD113 cod15 MATR114
            if ((matAConv.BuscarCod("MATR114")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("MATD113"));
                matConv.BuscarCod("MATD113").NumMatricula = matAConv.BuscarCod("MATR114").NumMatricula;
            }

            //Calculo cod20 MATD123 cod15 MATR124
            if ((matAConv.BuscarCod("MATR124")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("MATD123"));
                matConv.BuscarCod("MATD123").NumMatricula = matAConv.BuscarCod("MATR124").NumMatricula;
            }

            //Quimica cod20 QUID143 cod15 QUIR114
            if ((matAConv.BuscarCod("QUIR114")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("QUID143"));
                matConv.BuscarCod("QUID143").NumMatricula = matAConv.BuscarCod("QUIR114").NumMatricula;
            }

            //Mecanica cod20 FISD134 cod15 FISR124
            if ((matAConv.BuscarCod("FISR124")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("FISD134"));
                matConv.BuscarCod("FISD134").NumMatricula = matAConv.BuscarCod("FISR124").NumMatricula;
            }

            //Comunicacion Oral y Escrita cod20 CSHD111 cod15 CSHR112
            if ((matAConv.BuscarCod("CSHR112")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("CSHD111"));
                matConv.BuscarCod("CSHD111").NumMatricula = matAConv.BuscarCod("CSHR112").NumMatricula;
            }

            //Asignatura de comunicaciones cod20 CSHD500 Comunicacion Oral y Escrita  cod15 CSHR112
            if ((matAConv.BuscarCod("CSHR112")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("CSHD500"));
                matConv.BuscarCod("CSHD500").NumMatricula = matAConv.BuscarCod("CSHR112").NumMatricula;
            }
            //////////////////////////////////////////////////
            ///   SEGUNDO SEMESTRE
            /////////////////////////

            // HERRAMIENTAS INFORMATICAS cod20 ICOD111 Ofimatica cod15 ICOR111
            if ((matAConv.BuscarCod("ICOR111")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ICOD111"));
                matConv.BuscarCod("ICOD111").NumMatricula = matAConv.BuscarCod("ICOR111").NumMatricula;
            }

            //ANALISIS SOCIOECONOMICO Y POLITICO DEL ECUADOR cod20 CSHD211 cod15 CSHR212
            if ((matAConv.BuscarCod("CSHR212")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("CSHD211"));
                matConv.BuscarCod("CSHD211").NumMatricula = matAConv.BuscarCod("CSHR212").NumMatricula;
            }

            //METODOLOGIAS DE ESTUDIO cod20 MATD113 INTRODUCCION A LAS TECNOLOGÍAS DE INFORMACION cod15 MATR114
            if ((matAConv.BuscarCod("ITIR113")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED211"));
                matConv.BuscarCod("IEED211").NumMatricula = matAConv.BuscarCod("ITIR113").NumMatricula;
            }

            //CALCULO VECTORIAL cod20 IEED232 cod15 MATR224
            if ((matAConv.BuscarCod("MATR224")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED232"));
                matConv.BuscarCod("IEED232").NumMatricula = matAConv.BuscarCod("MATR224").NumMatricula;
            }

            //ECUACIONES DIFERENCIALES ORDINARIAS cod20 MATD213 cod15 MATR214
            if ((matAConv.BuscarCod("MATR214")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("MATD213"));
                matConv.BuscarCod("MATD213").NumMatricula = matAConv.BuscarCod("MATR214").NumMatricula;
            }

            //PROBABILIDAD Y ESTADISTICA cod20 MATD113 cod15 MATR234
            if ((matAConv.BuscarCod("MATR234")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("MATD223"));
                matConv.BuscarCod("MATD223").NumMatricula = matAConv.BuscarCod("MATR234").NumMatricula;
            }

            //FUNDAMENTOS DE ELECTROMAGNETISMO cod20 MATD123 ELECTRICIDAD Y MAGNETISMO cod15 FISR214
            if ((matAConv.BuscarCod("FISR214")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED242"));
                matConv.BuscarCod("IEED242").NumMatricula = matAConv.BuscarCod("FISR214").NumMatricula;
            }

            //ELECTROTECNIA  cod20 QUID143  FUNDAMENTOS DE CIRCUITOS ELECTRICOS cod15 QUIR114
            if ((matAConv.BuscarCod("IEER434")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED272"));
                matConv.BuscarCod("IEED272").NumMatricula = matAConv.BuscarCod("IEER434").NumMatricula;
            }

            //PROGRAMACION cod20 MATD113 cod15 ICOR312
            if ((matAConv.BuscarCod("ICOR312")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED252"));
                matConv.BuscarCod("IEED252").NumMatricula = matAConv.BuscarCod("ICOR312").NumMatricula;
            }

            //////////////////////////////////////////////////
            ///   TERCER SEMESTRE
            /////////////////////////

            //FUNDAMENTOS DE CIRCUITOS ELECTRICOS cod20 MATD123 cod15 IEER434
            if ((matAConv.BuscarCod("IEER434")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED353"));
                matConv.BuscarCod("IEED353").NumMatricula = matAConv.BuscarCod("IEER434").NumMatricula;
            }

            //Quimica cod20 QUID143  ANALISIS DE FOURIER cod15 QUIR114
            if ((matAConv.BuscarCod("QUIR114")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("QUID143"));
                matConv.BuscarCod("QUID143").NumMatricula = matAConv.BuscarCod("QUIR114").NumMatricula;
            }

            //MATEMATICA AVANZADA cod20 MATD113 cod15 MATR314
            if ((matAConv.BuscarCod("MATR314")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED312"));
                matConv.BuscarCod("IEED312").NumMatricula = matAConv.BuscarCod("MATR314").NumMatricula;
            }

            //TEORIA ELECTROMAGNETICA cod20 MATD123 ELECTRICIDAD Y MAGNETISMO cod15 FISR214
            if ((matAConv.BuscarCod("FISR214")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED342"));
                matConv.BuscarCod("IEED342").NumMatricula = matAConv.BuscarCod("FISR214").NumMatricula;
            }

            //DISPOSITIVOS ELECTRONICOS cod20 IEED333 cod15 IEER434
            if ((matAConv.BuscarCod("IEER434")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED333"));
                matConv.BuscarCod("IEED333").NumMatricula = matAConv.BuscarCod("IEER434").NumMatricula;
            }

            //SISTEMAS DIGITALES cod20 MATD113 cod15 IEER524
            if ((matAConv.BuscarCod("IEER524")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED323"));
                matConv.BuscarCod("IEED323").NumMatricula = matAConv.BuscarCod("IEER524").NumMatricula;
            }

            //MATEMATICA DISCRETA cod20 IEED371  SISTEMAS DIGITALES cod15 IEER524
            if ((matAConv.BuscarCod("IEER524")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED371"));
                matConv.BuscarCod("IEED371").NumMatricula = matAConv.BuscarCod("IEER524").NumMatricula;
            }

            //ASIGNATURA DE ARTES Y HUMANIDADES cod20 CSHD300 cod15 CSHR300
            if ((matAConv.BuscarCod("CSHR300")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("CSHD300"));
                matConv.BuscarCod("CSHD300").NumMatricula = matAConv.BuscarCod("CSHR300").NumMatricula;
            }

            //CLUBES IEED210 MATD113 ASIGNATURA DE ARTES Y HUMANIDADES cod15 CSHR300
            if ((matAConv.BuscarCod("CSHR300")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED210"));
                matConv.BuscarCod("IEED210").NumMatricula = matAConv.BuscarCod("CSHR300").NumMatricula;
            }

            //////////////////////////////////////////////////
            ///   CUARTO SEMESTRE
            /////////////////////////

            //ANALISIS DE SENIALES cod20 TELD423 cod15 TELR333
            if ((matAConv.BuscarCod("TELR333")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("TELD423"));
                matConv.BuscarCod("TELD423").NumMatricula = matAConv.BuscarCod("TELR333").NumMatricula;
            }

            //SISTEMAS OPERATIVOS cod20 ITID452 cod15 ITIR523
            if ((matAConv.BuscarCod("ITIR523")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID452"));
                matConv.BuscarCod("ITID452").NumMatricula = matAConv.BuscarCod("ITIR523").NumMatricula;
            }

            //BASE DE DATOS cod20 ITID413 cod15 ITIR453
            if ((matAConv.BuscarCod("ITIR453")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID413"));
                matConv.BuscarCod("ITID413").NumMatricula = matAConv.BuscarCod("MATITIR453R114").NumMatricula;
            }

            //PROGRAMACION AVANZADA cod20 ITID433 cod15 ITIR354
            if ((matAConv.BuscarCod("ITIR354")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID433"));
                matConv.BuscarCod("ITID433").NumMatricula = matAConv.BuscarCod("ITIR354").NumMatricula;
            }

            //INSTALACIONES ELECTRICAS cod20 IEED413 cod15 IEER553
            if ((matAConv.BuscarCod("IEER553")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("IEED413"));
                matConv.BuscarCod("IEED413").NumMatricula = matAConv.BuscarCod("IEER553").NumMatricula;
            }

            //ASIGNATURA DE ECONOMIA Y SOCIEDAD cod20 CSHD400 cod15 CSHR400
            if ((matAConv.BuscarCod("CSHR400")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("CSHD400"));
                matConv.BuscarCod("CSHD400").NumMatricula = matAConv.BuscarCod("CSHR400").NumMatricula;
            }

            //////////////////////////////////////////////////
            ///   QUINTO SEMESTRE
            /////////////////////////


            //SISTEMAS DE CABLEADO ESTRUCTURADO cod20 ITID512 CABLEADO ESTRUCTURADO AVANZADO  cod15 ITIR513
            if ((matAConv.BuscarCod("ITIR513")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID512"));
                matConv.BuscarCod("ITID512").NumMatricula = matAConv.BuscarCod("ITIR513").NumMatricula;
            }

            //TRANSMISION DIGITAL cod20 ITID524 FUNDAMENTOS DE COMUNICACIONES cod15 TELR425
            if ((matAConv.BuscarCod("TELR425")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID524"));
                matConv.BuscarCod("ITID524").NumMatricula = matAConv.BuscarCod("TELR425").NumMatricula;
            }

            //SISTEMAS EMBEBIDOS cod20 ITID553 cod15 TELR643
            if ((matAConv.BuscarCod("TELR643")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID553"));
                matConv.BuscarCod("ITID553").NumMatricula = matAConv.BuscarCod("TELR643").NumMatricula;
            }

            //DISENIO Y PROGRAMACION DE SOFTWARE cod20 ITID543 PROGRAMACION BASADA EN EVENTOS cod15 ITIR554
            if ((matAConv.BuscarCod("ITIR554")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID543"));
                matConv.BuscarCod("ITID543").NumMatricula = matAConv.BuscarCod("ITIR554").NumMatricula;
            }

            //TEORIA DE INFORMACION Y CODIFICACION cod20 TELD522 cod15 TELR433
            if ((matAConv.BuscarCod("TELR433")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("TELD522"));
                matConv.BuscarCod("TELD522").NumMatricula = matAConv.BuscarCod("TELR433").NumMatricula;
            }

            //GESTION ORGANIZACIONALcod20 TELD522 Comportamiento Humano cod15 ADMR742
            if ((matAConv.BuscarCod("ADMR742")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ADMD511"));
                matConv.BuscarCod("ADMD511").NumMatricula = matAConv.BuscarCod("ADMR742").NumMatricula;
            }

            //CABLEADO ESTRUCTURADO AVANZADO cod20 ITID612 cod15 ITIR513
            if ((matAConv.BuscarCod("ITIR513")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID612"));
                matConv.BuscarCod("ITID612").NumMatricula = matAConv.BuscarCod("ITIR513").NumMatricula;
            }

            //GESTION DE PROCESOS Y CALIDAD cod20 TELD522 PENSAMIENTO CREATIVO E INNOVACIÓN cod15 ADMR732 
            if ((matAConv.BuscarCod("ADMR732")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ADMD611"));
                matConv.BuscarCod("ADMD611").NumMatricula = matAConv.BuscarCod("ADMR732").NumMatricula;
            }

            //ECOLOGÍA Y AMBIENTE cod20 AMBD900 cod15 AMBR512
            if ((matAConv.BuscarCod("AMBR512")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("AMBD900"));
                matConv.BuscarCod("AMBD900").NumMatricula = matAConv.BuscarCod("AMBR512").NumMatricula;
            }

            //TEORIA DE INFORMACION Y CODIFICACION cod20 TELD522 cod15 TELR433
            if ((matAConv.BuscarCod("TELR433")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("TELD522"));
                matConv.BuscarCod("TELD522").NumMatricula = matAConv.BuscarCod("TELR433").NumMatricula;
            }

            // ENRUTAMIENTO cod20 ITID633 cod15 REDES DE AREA LOCAL ITIR534
            if ((matAConv.BuscarCod("ITIR534")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID633"));
                matConv.BuscarCod("ITID633").NumMatricula = matAConv.BuscarCod("ITIR534").NumMatricula;
            }

            //REDES DE AREA LOCAL cod20 ITID623 cod15 TELR433
            if ((matAConv.BuscarCod("ITIR534")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID623"));
                matConv.BuscarCod("ITID623").NumMatricula = matAConv.BuscarCod("ITIR534").NumMatricula;
            }

            //COMUNICACIONES INALAMBRICAS cod20 TELD522 cod15 TELR744
            if ((matAConv.BuscarCod("TELR744")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID683"));
                matConv.BuscarCod("ITID683").NumMatricula = matAConv.BuscarCod("TELR744").NumMatricula;
            }

            //PROCESAMIENTO DE DATOS cod20 TELD522 cod15 ITIR614  ************************************************
            if ((matAConv.BuscarCod("ITIR614")) != null)
            {
                //PROCESAMIENTO DE DATOS cod15 ITIR624
                if ((matAConv.BuscarCod("ITIR624")) != null)
                {
                    matConv.AggMaterias(materias20.BuscarCod("ITID643"));
                    matConv.BuscarCod("ITID643").NumMatricula = matAConv.BuscarCod("ITIR624").NumMatricula + matAConv.BuscarCod("ITIR614").NumMatricula;
                }
            }

            //////////////////////////////////////////////////
            ///   SEXTO SEMESTRE
            /////////////////////////


            //APLICACIONES DISTRIBUIDAS cod20 ITID713 cod15 ITIR654
            if ((matAConv.BuscarCod("ITIR654")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID713"));
                matConv.BuscarCod("ITID713").NumMatricula = matAConv.BuscarCod("ITIR654").NumMatricula;
            }

            //INGENIERIA FINANCIERA cod20 ADMD711 ADMINISTRACION FINANCIERA cod15 AMDR722
            if ((matAConv.BuscarCod("AMDR722")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ADMD711"));
                matConv.BuscarCod("ADMD711").NumMatricula = matAConv.BuscarCod("AMDR722").NumMatricula;
            }

            //REDES DE AREA EXTENDIDAcod20 TELD522 cod15 TELR433
            if ((matAConv.BuscarCod("ITIR654")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID723"));
                matConv.BuscarCod("ITID723").NumMatricula = matAConv.BuscarCod("ITIR654").NumMatricula;
            }

            //SEGURIDAD EN REDES cod20 TELD522 cod15 ITIR734
            if ((matAConv.BuscarCod("ITIR734")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID733"));
                matConv.BuscarCod("ITID733").NumMatricula = matAConv.BuscarCod("ITIR734").NumMatricula;
            }

            //REDES E INTRANETS cod20 TELD522 cod15 ITIR844
            if ((matAConv.BuscarCod("ITIR844")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID742"));
                matConv.BuscarCod("ITID742").NumMatricula = matAConv.BuscarCod("ITIR844").NumMatricula;
            }

            //APLICACIONES WEB Y MOVILES cod20 TELD522 cod15 ITIR754
            if ((matAConv.BuscarCod("ITIR754")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ITID753"));
                matConv.BuscarCod("ITID753").NumMatricula = matAConv.BuscarCod("ITIR754").NumMatricula;
            }

            //FORMULACION Y EVALUACION DE PROYECTOS cod20 TELD522 cod15 TITR622
            if ((matAConv.BuscarCod("TITR622")) != null)
            {
                matConv.AggMaterias(materias20.BuscarCod("ADMD800"));
                matConv.BuscarCod("ADMD800").NumMatricula = matAConv.BuscarCod("TITR622").NumMatricula;
            }



            


            //Serializacion de lista de materias convalidadas y envio hacia el cliente

            s_cliente.Send(BinarySerialization.Serializate(matConv));
            

        }


        //validacion de credenciales del estudiante
        public void validarEstudiante(Estudiante es)
        {
            //Con los estudiantes de la base de datos hace la relacion 
            //para ver si las credenciales enviadas por el cliente coinciden con algunas
            //de la base de datos
            Boolean vali = false;
            foreach (Estudiante estudiante in estudiantes)
            {

                if (es.Correo.Equals(estudiante.Correo) && es.Contrasenia.Equals(estudiante.Contrasenia))
                {
                    vali = true;
                    //Si las credenciales coinciden se envia una respuesta positiva al cliente
                    byte[] response = Encoding.ASCII.GetBytes("true");
                    s_cliente.Send(response);

                    //Y se envian las materias del 2015 al formulario login 
                    EnviarMaterias();
                    EnviarEstudiante(estudiante);
                    break;
                }
               
            }


            //Si las credenciales no coinciden se envia una respuesta negativa al cliente
            if (vali == false)
            {
                byte[] response1 = Encoding.ASCII.GetBytes("false");
                s_cliente.Send(response1);
            }
            


        }

        //metodo para enviar materias
        public void EnviarMaterias()
        {
            s_cliente.Send(BinarySerialization.Serializate(materias15));
        }


        //Metodo para enviar estudiante
        public void EnviarEstudiante(Estudiante e)
        {
            s_cliente.Send(BinarySerialization.Serializate(e));
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
