using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlibliotecaCambioMalla
{
    //Hacemos nuestra clase serializable
    [Serializable]
    public class Estudiante
    {
        //Atributos
        private int idEstudiante;
        private String nombre;
        private String cedula;
        private String correo;
        private String contrasenia;
        private ListaMaterias materiasConvalidadas;
        private Boolean convalidado;


        //getters y setters
        public int IdEstudiante { get => idEstudiante; set => idEstudiante = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Cedula { get => cedula; set => cedula = value; }
        public string Correo { get => correo; set => correo = value; }
        public string Contrasenia { get => contrasenia; set => contrasenia = value; }
        public ListaMaterias MateriasConvalidadas { get => materiasConvalidadas; set => materiasConvalidadas = value; }
        public bool Convalidado { get => convalidado; set => convalidado = value; }

        public Estudiante(int idEstudiante, string nombre, string cedula, string correo, string contrasenia, ListaMaterias materiasConvalidadas, Boolean convalidado)
        {
            this.idEstudiante = idEstudiante;
            this.nombre = nombre;
            this.cedula = cedula;
            this.correo = correo;
            this.contrasenia = contrasenia;
            this.materiasConvalidadas = materiasConvalidadas;
            this.convalidado = convalidado;
        }

        public Estudiante()
        {
        }

        public override string ToString()
        {
            return "Estudiante: " + this.correo + "  " + this.contrasenia;
        }

    }
}
