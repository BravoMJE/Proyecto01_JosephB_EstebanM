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
        static List<Materia> materias = new List<Materia>();

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

            //List<String> materiasAprovadas = new List<String>();
            //materiasAprovadas.Add("ICO11");
            //materiasAprovadas.Add("ADMD611");
            //Estudiante e1 = new Estudiante(1, "Joseph Bravo", "0705760932", "joseph.bravo@epn.edu.ec", "1234",materiasAprovadas);
            //Estudiante e2 = new Estudiante(2, "Mario Bravo", "0705760452", "mario.bravo@epn.edu.ec", "1234",materiasAprovadas);

            //estudiantes.Add(e1);
            //estudiantes.Add(e2);


            //Materia m1 = new Materia(1,"MATR114","Algebra lineal",0,1,2015);
            //Materia m2 = new Materia(1, "MATD113", "Algebra lineal", 0, 1,2020);


            //var set = clienteDB.Set(@"Materia15/" + m1.IdMateria,m1);
            //var set2 = clienteDB.Set(@"Materia20/" + m2.IdMateria,m2);



            

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
            FirebaseResponse resMa = clienteDB.Get(@"ContMa");
            int counter = int.Parse(resMa.ResultAs<string>());

            for (int i = 1; i <= counter; i++)
            {
                var resMat = clienteDB.Get(@"Materia15/" + i);
                Materia m = resMat.ResultAs<Materia>();
                materias.Add(m);
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
