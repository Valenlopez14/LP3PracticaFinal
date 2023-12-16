using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PracticaFinal.LP3
{
    public partial class Form1 : Form
    {

        clsAlumnos objAlumnos;
        clsBarrios objBarrios;
        clsFrutas objFrutas;
        clsLeGustan objLeGustan;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                objAlumnos = new clsAlumnos();
                objBarrios = new clsBarrios();
                objFrutas = new clsFrutas();
                objLeGustan = new clsLeGustan();

            }
            catch (Exception)
            {

                MessageBox.Show("Problemas ");
            }

            DataTable TablaAlumnos = new DataTable();
            DataTable TablaBarrios = new DataTable();
            DataTable TablaFrutas = new DataTable();
            DataTable TablaLeGustan = new DataTable();

            TablaBarrios = objBarrios.GetDataBarrios();
            TablaFrutas = objFrutas.GetDataFrutas();
            TablaLeGustan = objLeGustan.GetDataLeGustan();
            TablaAlumnos = objAlumnos.GetDataAlumnos();

            TreeNode abuelo;
            TreeNode padre;
            TreeNode hijo = null;
            TreeNode subhijo;

            abuelo = tv.Nodes.Add("BARRIOS");
            foreach (DataRow fBarrios in TablaBarrios.Rows)
            {
                padre = abuelo.Nodes.Add(fBarrios["nombre"].ToString());
                padre.Tag = fBarrios["barrio"].ToString();
                foreach (DataRow fAlumnos in TablaAlumnos.Rows)
                {
                    if (padre.Tag.ToString() == fAlumnos["barrio"].ToString())
                    {
                        hijo = padre.Nodes.Add(fAlumnos["nombre"].ToString());
                        hijo.Tag = fAlumnos["dni"].ToString();

                        foreach (DataRow fFrutas in TablaFrutas.Rows)
                        {
                            foreach (DataRow fLeGustan in TablaLeGustan.Rows)
                            {
                                if (fLeGustan["dni"].ToString() == fAlumnos["dni"].ToString())
                                {
                                    if (fLeGustan["fruta"].ToString() == fFrutas["fruta"].ToString())
                                    {
                                        subhijo = hijo.Nodes.Add(fFrutas["nombre"].ToString());
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable TablaAlumnos = new DataTable();
            DataTable TablaBarrios = new DataTable();
            DataTable TablaFrutas = new DataTable();
            DataTable TablaLeGustan = new DataTable();

            TablaBarrios = objBarrios.GetDataBarrios();
            TablaFrutas = objFrutas.GetDataFrutas();
            TablaLeGustan = objLeGustan.GetDataLeGustan();
            TablaAlumnos = objAlumnos.GetDataAlumnos();

            Int32 total = 0;
            Int32 f = 0;
            Series serie = new Series();
            chart.Series.Clear();

        
                foreach (DataRow fNombre in TablaAlumnos.Rows)
                {
                    serie = chart.Series.Add(fNombre["nombre"].ToString());
                foreach (DataRow fFrutas in TablaFrutas.Rows)
                {
                    foreach (DataRow fLeGustan in TablaLeGustan.Rows)
                    {
                        if (fNombre["dni"].ToString() == fLeGustan["dni"].ToString())
                        {
                            if (fFrutas["fruta"].ToString() == fLeGustan["fruta"].ToString())
                            {
                                total = total + Convert.ToInt32(fLeGustan["fruta"]);
                                f++;
                            }
                        }
                    }

                    serie.Points.AddXY(fNombre["dni"], total);
                    f = 0;
                }
            }
            

        }

        private void tv_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DataTable TablaAlumnos = new DataTable();
            DataTable TablaBarrios = new DataTable();
            DataTable TablaFrutas = new DataTable();
            DataTable TablaLeGustan = new DataTable();

            TablaBarrios = objBarrios.GetDataBarrios();
            TablaFrutas = objFrutas.GetDataFrutas();
            TablaLeGustan = objLeGustan.GetDataLeGustan();
            TablaAlumnos = objAlumnos.GetDataAlumnos();

            dgvGrilla.Rows.Clear();
       

            if (e.Node.Level == 1)
            {
                string barrioSeleccionado = e.Node.Tag.ToString();

                foreach (DataRow fBarrio in TablaBarrios.Rows)
                {
                        foreach (DataRow fAlumnos in TablaAlumnos.Rows)
                        {
                            if (barrioSeleccionado == fAlumnos["barrio"].ToString() && barrioSeleccionado == fBarrio["barrio"].ToString()  )
                            {
                                dgvGrilla.Rows.Add(fAlumnos["nombre"].ToString(), fBarrio["nombre"].ToString());
                            }
                        }
                }


            }

            
        }

        private void tv_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DataTable TablaAlumnos = new DataTable();
            DataTable TablaBarrios = new DataTable();
            DataTable TablaFrutas = new DataTable();
            DataTable TablaLeGustan = new DataTable();

            TablaBarrios = objBarrios.GetDataBarrios();
            TablaFrutas = objFrutas.GetDataFrutas();
            TablaLeGustan = objLeGustan.GetDataLeGustan();
            TablaAlumnos = objAlumnos.GetDataAlumnos();

            dgvGrilla.Rows.Clear();


            if (e.Node.Level == 2)
            {
                string alumnoSeleccionado = e.Node.Tag.ToString();

                // Buscar el nombre del alumno correspondiente al código de estudiante
                string nombreAlumno = string.Empty;
                foreach (DataRow fAlumno in TablaAlumnos.Rows)
                {
                    if (alumnoSeleccionado == fAlumno["dni"].ToString())
                    {
                        nombreAlumno = fAlumno["nombre"].ToString();
                        break; // Se encontró el nombre, salir del bucle
                    }
                }

                // Crear una fila para cada alumno
                DataGridViewRow filaAlumno = new DataGridViewRow();

                int frutas = 0; // Reinicializar la cantidad

                foreach (DataRow fFrutas in TablaFrutas.Rows)
                {
                    foreach (DataRow fLeGustan in TablaLeGustan.Rows)
                    {
                        if (alumnoSeleccionado == fLeGustan["dni"].ToString() && fLeGustan["fruta"].ToString() == fFrutas["fruta"].ToString())
                        {
                            frutas = frutas + 1;
                        }
                    }
                }

                // Agregar la información del alumno y la cantidad total de frutas a la fila
                filaAlumno.CreateCells(dgvGrilla, nombreAlumno, "----" , frutas);

                // Agregar la fila al DataGridView
                dgvGrilla.Rows.Add(filaAlumno);
            }





        }
    }
}
