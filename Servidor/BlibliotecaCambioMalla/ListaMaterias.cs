using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlibliotecaCambioMalla
{
    [Serializable]
    //Clase lista de materias que nos permitira 
    //tener un objeto con una lista de materias
    public class ListaMaterias
    {
        private List<Materia> list;


        //Constructor
        public ListaMaterias()
        {
            List = new List<Materia>();
        }

        public List<Materia> List { get => list; set => list = value; }

        //Metodo para agregar materias
        public void AggMaterias(Materia m)
        {
            list.Add(m);
        }

        //Metodo buscar una materia dentro de la lista
        public Materia Buscar(int id)
        {
            foreach (Materia i in list)
            {
                if(i.IdMateria == id)
                {
                    return i;
                }
            }

            return null;
        }


        //Metodo buscar una materia dentro de la lista
        //por codigo
        public Materia BuscarCod(string cod)
        {
            foreach (Materia i in list)
            {
                if (i.Codigo.Equals(cod))
                {
                    return i;
                }
            }

            return null;
        }

        //metodo para eliminar una materia de una lista
        public void Eliminar(int id)
        {
            list.Remove(Buscar(id));
        }


        //Metodo para buscar materias por semestre
        public ListaMaterias BuscarPorSemestre(int sem)
        {
            ListaMaterias matSemestre = new ListaMaterias();
            foreach (Materia i in list)
            {
                if(i.Semestre == sem)
                {
                    matSemestre.AggMaterias(i);
                }
            }
            return matSemestre;
        }


        //Imprimir por consola
        //Metodo para pruebas
        public void listar()
        {
            foreach (Materia i in list)
            {
                Console.WriteLine(i);
            }
        }



    }
}
