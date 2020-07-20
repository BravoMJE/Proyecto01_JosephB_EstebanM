using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlibliotecaCambioMalla;

namespace Cliente
{
    public partial class frmMain : Form
    {

        //Instancia de objeto listamaterias 
        ListaMaterias materias2015 = new ListaMaterias();
        List<Materia> mat15S1 = new List<Materia>();
        List<Materia> mat15S2 = new List<Materia>();
        List<Materia> mat15S3 = new List<Materia>();
        List<Materia> mat15S4 = new List<Materia>();
        List<Materia> mat15S5 = new List<Materia>();
        List<Materia> mat15S6 = new List<Materia>();
        public List<Materia> matApro = new List<Materia>();
        public List<Materia> matCon = new List<Materia>();
        public frmLogin frmLogPadre;


        public frmMain(ListaMaterias materias, frmLogin padre)
        {
            frmLogPadre = padre;
            materias2015 = materias;
            InitializeComponent();
        }



        private void frmMain_Load(object sender, EventArgs e)
        {

            mat15S1 = materias2015.BuscarPorSemestre(1).List;
            mat15S2 = materias2015.BuscarPorSemestre(2).List;
            mat15S3 = materias2015.BuscarPorSemestre(3).List;
            mat15S4 = materias2015.BuscarPorSemestre(4).List;
            mat15S5 = materias2015.BuscarPorSemestre(5).List;
            mat15S6 = materias2015.BuscarPorSemestre(6).List;

            

            LlenarCombo();
            cbxMatricula.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSemestre.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LlenarCombo()
        {
            
            String cbx1 = "1";
            String cbx2 = "2";
            String cbx3 = "3";

            String cbx4 = "Primer Semestre";
            String cbx5 = "Segundo Semestre";
            String cbx6 = "Tercer Semestre";
            String cbx7 = "Cuarto Semestre";
            String cbx8 = "Quinto Semestre";
            String cbx9 = "Sexto Semestre";


            cbxMatricula.Items.Add(cbx1);
            cbxMatricula.Items.Add(cbx2);
            cbxMatricula.Items.Add(cbx3);

            cbxSemestre.Items.Add(cbx4);
            cbxSemestre.Items.Add(cbx5);
            cbxSemestre.Items.Add(cbx6);
            cbxSemestre.Items.Add(cbx7);
            cbxSemestre.Items.Add(cbx8);
            cbxSemestre.Items.Add(cbx9);

            cbxMatricula.SelectedIndex = 0;
            cbxSemestre.SelectedIndex = 0;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

            

            
            

            ListaMaterias matVerificar = new ListaMaterias();
            matVerificar.List = matApro;


            frmLogPadre.iniciarEnvio(matVerificar);


            byte[] listaConvalidada = new byte[10024];
            frmLogPadre.s_cliente.Receive(listaConvalidada);

            matCon = ((ListaMaterias)BinarySerialization.Deserializate(listaConvalidada)).List;

            
            frmVerificacion frmVerificacion = new frmVerificacion(this);
            this.Hide();
            frmVerificacion.Show();

            Console.WriteLine(matCon);
        }

        private void cbxSemestre_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxSemestre.SelectedItem.Equals("Primer Semestre"))
            {
                listMaterias.Items.Clear();
                foreach (Materia i in mat15S1)
                {
                    listMaterias.Items.Add(i);
                }
            }
            if (cbxSemestre.SelectedItem.Equals("Segundo Semestre"))
            {
                listMaterias.Items.Clear();
                foreach (Materia i in mat15S2)
                {
                    listMaterias.Items.Add(i);
                }
            }
            if (cbxSemestre.SelectedItem.Equals("Tercer Semestre"))
            {
                listMaterias.Items.Clear();
                foreach (Materia i in mat15S3)
                {
                    listMaterias.Items.Add(i);
                }
            }
            if (cbxSemestre.SelectedItem.Equals("Cuarto Semestre"))
            {
                listMaterias.Items.Clear();
                foreach (Materia i in mat15S4)
                {
                    listMaterias.Items.Add(i);
                }
            }
            if (cbxSemestre.SelectedItem.Equals("Quinto Semestre"))
            {
                listMaterias.Items.Clear();
                foreach (Materia i in mat15S5)
                {
                    listMaterias.Items.Add(i);
                }
            }
            if (cbxSemestre.SelectedItem.Equals("Sexto Semestre"))
            {
                listMaterias.Items.Clear();
                foreach (Materia i in mat15S6)
                {
                    listMaterias.Items.Add(i);
                }
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            foreach (Materia i in listMaterias.SelectedItems)
            {
                
                i.NumMatricula = int.Parse(cbxMatricula.SelectedItem.ToString());
                matApro.Add(i);
            }
            ActualizarMatApro();
        }


        //Actualizar las materias seleccionadas por el estudiante
        public void ActualizarMatApro()
        {
            listMatAprovadas.Items.Clear();
            foreach (Materia i in matApro)
            {
                listMatAprovadas.Items.Add(i);
            }
        }
    }
}
