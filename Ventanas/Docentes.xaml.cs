using Examen.Clases;
using Examen.Utilidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Examen
{
    /// <summary>
    /// Interaction logic for Docentes.xaml
    /// </summary>
    public partial class Docentes : Window
    {
        #region Declaraciones
        List<clsDocente> listaDocentes = new List<clsDocente>();//Lista de docentes
        List<clsDocente> listalistaDocenteTemp = new List<clsDocente>();//Lista de estudiantes temporal
        clsDocente docenteGrilla = new clsDocente();
        private string path = AppDomain.CurrentDomain.BaseDirectory + "Docentes.json";
        public string err;
        public static escribirLog bitacora;
        public static Registros registro;
        #endregion

        public Docentes()
        {
            InitializeComponent();
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            importarDocentes();
        }

        public void importarDocentes()
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            //se abre el archivo Json que se va leer
            using (StreamReader sr = File.OpenText(path))
            {
                try
                {
                    string vsRespuestaJson = string.Empty;
                    while ((vsRespuestaJson = sr.ReadLine()) != null)
                    {
                        listaDocentes = JsonConvert.DeserializeObject<List<clsDocente>>(vsRespuestaJson);//se deserealiza (descompone) el Json del archivo
                        dgDocentes.ItemsSource = null;
                        dgDocentes.ItemsSource = listaDocentes;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (listaDocentes == null || listaDocentes.Count == 0)
            {
                MessageBox.Show("Favor agregue un docente como mínimo para poder importar.");               
                err = "ERROR-No se agrego un docente para ser importado.";
                bitacora = new escribirLog(err, false);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtID.Text) || string.IsNullOrEmpty(txtNom.Text) || string.IsNullOrEmpty(txtApes.Text)
                    || string.IsNullOrEmpty(txtCel.Text) || string.IsNullOrEmpty(txtCorreo.Text) || string.IsNullOrEmpty(txtAsignatura.Text)
                    || string.IsNullOrEmpty(cbNivel.Text) || string.IsNullOrEmpty(dpFNac.Text) || string.IsNullOrEmpty(cbEstadoCivil.Text)
                    || string.IsNullOrEmpty(cbGenero.Text))
                {                    
                    err = "ERROR-No se completaron todos los campos para guardar al docente. ";
                    bitacora = new escribirLog(err, false);
                    throw new Exception("Debe de proporcionar informacion en todos los datos solicitados");
                }
                else
                {
                    listaDocentes.Add(new clsDocente
                    {
                        IdDocente = Convert.ToInt32(txtID.Text),
                        Nombre = txtNom.Text,
                        Apellidos = txtApes.Text,
                        Identificacion = txtID.Text,
                        Celular = txtCel.Text,
                        Correo = txtCorreo.Text,
                        Asignatura = txtAsignatura.Text,
                        Nivel = cbNivel.Text,
                        FechaInicioMEP = dpFNac.Text,
                        EstadoCivil = cbEstadoCivil.Text,
                        Genero = cbGenero.Text,
                    });
                    int id = Convert.ToInt32(txtID.Text);
                    string name = txtNom.Text;
                    string ape = txtApes.Text;
                    SaveDocentesToJson(listaDocentes, path);
                    MessageBox.Show("Se guardo de forma exitosa");
                    err = $"El docente,ID:{id},Nombre:{name},Apellido:{ape}, ha sido registrado.";
                    registro = new Registros(err,false);
                    importarDocentes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // MessageBox.Show(cbNivel.Text);

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (docenteGrilla != null && docenteGrilla.IdDocente != 0)
                {
                    if (txtID.Text != null)
                    {
                        int id = Convert.ToInt32(txtID.Text);
                        List<clsDocente> listaEditar = LoadDocentesFromJson(path);

                        EditDocente(listaEditar, id, new clsDocente
                        {
                            Nombre = txtNom.Text,
                            Apellidos = txtApes.Text,
                            Identificacion = txtID.Text,
                            Celular = txtCel.Text,
                            Correo = txtCorreo.Text,
                            Asignatura = txtAsignatura.Text,
                            Nivel = cbNivel.Text,
                            FechaInicioMEP = dpFNac.Text,
                            EstadoCivil = txtAsignatura.Text,
                            Genero = cbGenero.Text,
                        });                       
                        SaveDocentesToJson(listaEditar, path);
                        MessageBox.Show("Se edito el usuario, ID: " + id.ToString());
                        err = $"El docente,ID:{id} ha sido editado.";
                        registro = new Registros(err, false);
                        importarDocentes();
                    }
                    else
                    {
                        MessageBox.Show("Ingresa la identificacion del usuario");
                        err = "Error-No se completo el campo de identificacion para modificar al usuario.";
                        bitacora = new escribirLog(err,false);
                    }
                }
                else
                {
                    MessageBox.Show("Debe seleccionar al menos un estudiante en la tabla.");
                    err = "Error-No se selecciono un estudiante de la tabla.";
                    bitacora = new escribirLog(err, false);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error a realizar la operacion");
                err = "Error-No se pudo procesar la solicitud de editar.";
                bitacora = new escribirLog(err, false);
            }
        }

        static List<clsDocente> LoadDocentesFromJson(string filePath)
        {
            // Leer el contenido del archivo JSON
            string jsonContent = File.ReadAllText(filePath);

            // Deserializar el JSON a una lista de objetos de clsDocente
            List<clsDocente> listaDocentes = JsonConvert.DeserializeObject<List<clsDocente>>(jsonContent);

            return listaDocentes;
        }

        static void SaveDocentesToJson(List<clsDocente> listaDocentes, string filePath)
        {

            if (!File.Exists(filePath))
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "Docentes.json").Dispose();
            }

            // Serializar la lista de docentes de vuelta a JSON
            //string json = JsonConvert.SerializeObject(listaDocentes, Formatting.Indented);
            string json = JsonConvert.SerializeObject(listaDocentes.ToArray());
            // Escribir el JSON actualizado de vuelta al archivo
            File.WriteAllText(filePath, json);
        }

        static void EditDocente(List<clsDocente> listaDocentes, int idDocente, clsDocente nuevosDatos)
        {
            // Buscar el docente con el ID especificado
            clsDocente docente = listaDocentes.Find(d => d.IdDocente == idDocente);
            string err;
            if (docente != null)
            {
                // Aplicar los nuevos datos al docente encontrado
                if (!string.IsNullOrEmpty(nuevosDatos.Nombre))
                    docente.Nombre = nuevosDatos.Nombre;

                if (!string.IsNullOrEmpty(nuevosDatos.Apellidos))
                    docente.Apellidos = nuevosDatos.Apellidos;

                if (!string.IsNullOrEmpty(nuevosDatos.EstadoCivil))
                    docente.EstadoCivil = nuevosDatos.EstadoCivil;

                if (!string.IsNullOrEmpty(nuevosDatos.Correo))
                    docente.Correo = nuevosDatos.Correo;

                if (!string.IsNullOrEmpty(nuevosDatos.Identificacion))
                    docente.Identificacion = nuevosDatos.Identificacion;

                if (!string.IsNullOrEmpty(nuevosDatos.Celular))
                    docente.Celular = nuevosDatos.Celular;

                if (!string.IsNullOrEmpty(nuevosDatos.Asignatura))
                    docente.Asignatura = nuevosDatos.Asignatura;

                if (!string.IsNullOrEmpty(nuevosDatos.Nivel))
                    docente.Nivel = nuevosDatos.Nivel;

                if (!string.IsNullOrEmpty(nuevosDatos.FechaInicioMEP))
                    docente.FechaInicioMEP = nuevosDatos.FechaInicioMEP;

                if (!string.IsNullOrEmpty(nuevosDatos.Genero))
                    docente.Genero = nuevosDatos.Genero;


                // Puedes agregar más campos según sea necesario
            }
            else
            {
                err = $"Error-El ID:{idDocente}, no esta registrado.";
                bitacora = new escribirLog(err, false);
                throw new Exception($"No se encontró ningún docente con el ID {idDocente}");
            }
        }

        public void EliminarDocentePorId(int idDocente)
        {
            List<clsDocente> docentes = LeerDocentesDesdeArchivo(path);

            // Buscar y eliminar el docente con el IdDocente dado
            clsDocente docenteAEliminar = docentes.Find(d => d.IdDocente == idDocente);
            if (docenteAEliminar != null)
            {
                int id = idDocente;
                docentes.Remove(docenteAEliminar);
                GuardarDocentesEnArchivo(docentes, path);
                err = $"El docente,ID: {id}, ha sido eliminado.";
                registro = new Registros(err, false);
                MessageBox.Show("Se elimino de forma correcta");
                importarDocentes();
            }
            else
            {
                MessageBox.Show($"No existe un docente registrado bajo el ID: {idDocente}");
                err = $"Error-No existe un docente registrado bajo el ID: {idDocente}.";
                bitacora = new escribirLog(err, false);
            }
        }

        private List<clsDocente> LeerDocentesDesdeArchivo(string filePath)
        {
            List<clsDocente> docentes;
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                docentes = (List<clsDocente>)serializer.Deserialize(file, typeof(List<clsDocente>));
            }
            return docentes;
        }

        private void GuardarDocentesEnArchivo(List<clsDocente> docentes, string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            try
            {
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, docentes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer el archivo docentes");
                err = "Error al leer el archivo docentes.";
                bitacora = new escribirLog(err, false);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (docenteGrilla != null && docenteGrilla.IdDocente != 0)
            {
                EliminarDocentePorId(Convert.ToInt32(txtID.Text));
            }
            else
            {
                MessageBox.Show("Debe seleccionar al menos un estudiante en la tabla.");
                err = "Error-No se ha seleccionado un estudiante de la tabla.";
                bitacora = new escribirLog(err, false);
            }
        }

        private void dgDocentes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            docenteGrilla = ((System.Windows.Controls.DataGrid)sender).SelectedItem as clsDocente;

            txtID.Text = docenteGrilla?.Identificacion;
            txtNom.Text = docenteGrilla?.Nombre;
            txtApes.Text = docenteGrilla?.Apellidos;
            txtCorreo.Text = docenteGrilla?.Correo;
            txtCel.Text = docenteGrilla?.Celular;
            txtAsignatura.Text = docenteGrilla?.Asignatura;

            if (docenteGrilla?.Genero != null)
            {
                if ((bool)(docenteGrilla?.Genero.Equals("Femenino")))
                {
                    cbGenero.SelectedItem = cbGenero.Items.GetItemAt(0);
                }
                else
                {
                    cbGenero.SelectedItem = cbGenero.Items.GetItemAt(1);

                }
            }

            if (docenteGrilla?.Nivel != null)
            {
                if ((bool)(docenteGrilla?.Nivel.Equals("Setimo")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(0);
                }
                else if ((bool)(docenteGrilla?.Nivel.Equals("Octavo")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(1);

                }
                else if ((bool)(docenteGrilla?.Nivel.Equals("Noveno")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(2);

                }
                else if ((bool)(docenteGrilla?.Nivel.Equals("Decimo")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(3);

                }
                else
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(4);

                }
            }

        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                MessageBox.Show("Debe proporcionar un numero de identificacion para realizar la busqueda unica");
                err = "Error-No se ha completado el campo de identificacion para realizar la busqueda unica.";
                bitacora = new escribirLog(err, false);
            }
            else
            {
                ListaDocente(txtBuscar?.Text);
            }
        }

        public void ListaDocente(string pDato)
        {
            if (listaDocentes != null && listaDocentes.Count > 0)
            {
                if (!string.IsNullOrEmpty(pDato) && pDato != null && pDato != "")
                {
                    listalistaDocenteTemp = new List<clsDocente>();
                    listalistaDocenteTemp = listaDocentes.FindAll(u => u.Nombre.Contains(pDato) || u.Identificacion.Contains(pDato) || u.Nivel.Contains(pDato));

                    dgDocentes.ItemsSource = null;
                    dgDocentes.ItemsSource = listalistaDocenteTemp;
                }
                else
                {
                    dgDocentes.ItemsSource = null;
                    dgDocentes.ItemsSource = listaDocentes;
                }
            }
            else
            {
                dgDocentes.ItemsSource = null;
                dgDocentes.ItemsSource = listaDocentes;
            }
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            ExportarDocentes();
        }

        public void ExportarDocentes()
        {
            string vsUsuarioJson = string.Empty;

            if (listaDocentes != null && listaDocentes.Count > 0)
            {
                vsUsuarioJson = JsonConvert.SerializeObject(listaDocentes);//se serializa (transforma) el Json para el archivo
                EscribirArchivo(vsUsuarioJson);
                MessageBox.Show("Datos exportados con éxito.");
                err = $"Se han exportado los datos,Archivo: {vsUsuarioJson}.";
                registro = new Registros(err, false);
            }
            else
            {
                MessageBox.Show("Favor agregue un estudiante como mínimo para poder exportar.");
                err = "Error-No se han añadido estudiantes para exportar.";
                bitacora = new escribirLog(err, false);
            }
        }

        public void EscribirArchivo(string pDatos)
        {
            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);//Siempre borro del archivo antes de escribir

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(pDatos);
                }
            }
        }
    }
}
