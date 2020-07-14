using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace Servidor
{
    class Servidor
    {

        static IFirebaseClient clienteDB;
        static List<Estudiante> estudiantes = new List<Estudiante>();
        static List<Materia> materias15 = new List<Materia>();
        static List<Materia> materias20 = new List<Materia>();

        static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "eUu4ME4xQww16dk60reONqttxKUm5pLS3c13BDwF",
            BasePath = "https://fir-pruebavisual.firebaseio.com/"
        };

        


        static void Main(string[] args)
        {
            ConectarDB();

            RecuperarEstudiantes();

            foreach (Estudiante i in estudiantes)
            {
                Console.WriteLine(i);
            }
            RecuperarMaterias15();
            RecuperarMaterias20();

            foreach (Materia i in materias15)
            {
                Console.WriteLine(i);
            }

            foreach (Materia i in materias20)
            {
                Console.WriteLine(i);
            }




            Console.ReadLine();
        }



        static void RecuperarEstudiantes()
        {
            FirebaseResponse resEs = clienteDB.Get(@"ContEs");
            int counter = int.Parse(resEs.ResultAs<string>());

            for (int i = 1; i <= counter; i++)
            {
                var resEst = clienteDB.Get(@"Estudiante/" + i);
                Estudiante e = resEst.ResultAs<Estudiante>();
                estudiantes.Add(e);
            }
        }

        static void RecuperarMaterias15()
        {
            FirebaseResponse resMa = clienteDB.Get(@"ContMa15");
            int counter = int.Parse(resMa.ResultAs<string>());

            for (int i = 1; i <= counter; i++)
            {
                var resMat = clienteDB.Get(@"Materia15/" + i);
                Materia m = resMat.ResultAs<Materia>();
                materias15.Add(m);
            }
        }

        static void RecuperarMaterias20()
        {
            FirebaseResponse resMa = clienteDB.Get(@"ContMa20");
            int counter = int.Parse(resMa.ResultAs<string>());

            for (int i = 1; i <= counter; i++)
            {
                var resMat = clienteDB.Get(@"Materia15/" + i);
                Materia m = resMat.ResultAs<Materia>();
                materias20.Add(m);
            }
        }


        static void ConectarDB()
        {

            try
            {
                clienteDB = new FireSharp.FirebaseClient(config);
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


        

    }
}
