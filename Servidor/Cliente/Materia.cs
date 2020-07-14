using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class Materia
    {
        //declaracion de variables
        private string codigo;
        private string nombre;
        private int numMatricula;
        private int semestre;
        private int pensum;




        //Getters y setters
        public string Codigo { get => codigo; set => codigo = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public int NumMatricula { get => numMatricula; set => numMatricula = value; }
        public int Semestre { get => semestre; set => semestre = value; }
        public int Pensum { get => pensum; set => pensum = value; }

        //Constructor
        protected Materia(string codigo, string nombre, int numMatricula, int semestre, int pensum)
        {
            this.codigo = codigo;
            this.nombre = nombre;
            this.numMatricula = numMatricula;
            this.semestre = semestre;
            this.pensum = pensum;
        }
    }
