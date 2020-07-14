using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    abstract class Materia
    {
        //declaracion de variables
        private string codigo;
        private string nombre;
        private int numMatricula;
        private int semestre;




        //Getters y setters
        protected string Codigo { get => codigo; set => codigo = value; }
        protected string Nombre { get => nombre; set => nombre = value; }
        protected int NumMatricula { get => numMatricula; set => numMatricula = value; }
        protected int Semestre { get => semestre; set => semestre = value; }
    }
}
