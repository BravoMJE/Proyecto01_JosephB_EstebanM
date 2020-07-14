using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    abstract class Materia
    {
        //declaracion de variables
        protected string codigo;
        protected string nombre;
        protected int numMatricula;
        protected int semestre;




        //Getters y setters
        public string Codigo { get => codigo; set => codigo = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public int NumMatricula { get => numMatricula; set => numMatricula = value; }
        public int Semestre { get => semestre; set => semestre = value; }


        //Constructor
        protected Materia(string codigo, string nombre, int numMatricula, int semestre)
        {
            this.codigo = codigo;
            this.nombre = nombre;
            this.numMatricula = numMatricula;
            this.semestre = semestre;
        }


        

    }
}
