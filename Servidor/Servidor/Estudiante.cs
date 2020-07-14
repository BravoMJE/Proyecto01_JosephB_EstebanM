using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    class Estudiante
    {
        //Atributos
        private int idEstudiante;
        private String nombre;
        private String cedula;
        private String correo;
        private String contrasenia;
        private List<String> materiasConvalidadas;


        //getters y setters
        public int IdEstudiante { get => idEstudiante; set => idEstudiante = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Cedula { get => cedula; set => cedula = value; }
        public string Correo { get => correo; set => correo = value; }
        public string Contrasenia { get => contrasenia; set => contrasenia = value; }
        public List<string> MateriasConvalidadas { get => materiasConvalidadas; set => materiasConvalidadas = value; }

        public Estudiante(int idEstudiante, string nombre, string cedula, string correo, string contrasenia, List<string> materiasConvalidadas)
        {
            this.idEstudiante = idEstudiante;
            this.nombre = nombre;
            this.cedula = cedula;
            this.correo = correo;
            this.contrasenia = contrasenia;
            this.materiasConvalidadas = materiasConvalidadas;
        }

        public Estudiante()
        {
        }

        public override string ToString()
        {
            return "Estudiante: " + this.nombre;
        }

    }
}
