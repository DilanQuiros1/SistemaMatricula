using Examen.Clases;
using Examen.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Examen
{
    /// <summary>
    /// Interaction logic for Estudiantes.xaml
    /// </summary>
    public partial class Estudiantes : Window
    {
        #region Declaraciones
        List<clsEstudiante> listaEstudiante = new List<clsEstudiante>();//Lista de estudiantes
        List<clsEstudiante> listalistaEstudianteTemp = new List<clsEstudiante>();//Lista de estudiantes temporal
        clsEstudiante estudianteGrilla = new clsEstudiante();//Instancia de un usuario
        string path = @AppDomain.CurrentDomain.BaseDirectory + "Estudiantes.json";
        string accion = "Nuevo";//Variable tipo bandera que almacena la acción que se esta realizando.
        int cont = 1;//Variable que define el ID del estudiante
        public static escribirLog bitacora;
        public static Registros registro;
        public string err;
        public string prueba;
        public string prue;
        #endregion


        public Estudiantes()
        {
            InitializeComponent();
        }

        private void btnMasNiv_Click(object sender, RoutedEventArgs e)
        {
            Docentes ventanaDoc = new Docentes();
            ventanaDoc.ShowDialog();
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            importarEstudiantes();

        }

        private void dgEstudiantes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            accion = "Editar";

            estudianteGrilla = ((System.Windows.Controls.DataGrid)sender).SelectedItem as clsEstudiante;

            txtID.Text = estudianteGrilla?.Identificacion;
            txtNom.Text = estudianteGrilla?.Nombre;
            txtApes.Text = estudianteGrilla?.Apellidos;
            dpFNac.Text = estudianteGrilla?.FechaNacimiento;
            txtCel.Text = estudianteGrilla?.Celular;
            txtCorreo.Text = estudianteGrilla?.Correo;
            if (estudianteGrilla?.Genero != null)
            {
                if ((bool)(estudianteGrilla?.Genero.Equals("F")))
                {
                    cbGenero.SelectedItem = cbGenero.Items.GetItemAt(0);
                }
                else
                {
                    cbGenero.SelectedItem = cbGenero.Items.GetItemAt(1);

                }
            }
            txtDir.Text = estudianteGrilla?.Direccion;
            if (estudianteGrilla?.Nivel != null)
            {
                if ((bool)(estudianteGrilla?.Nivel.Equals("Setimo")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(0);
                }
                else if ((bool)(estudianteGrilla?.Nivel.Equals("Octavo")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(1);

                }
                else if ((bool)(estudianteGrilla?.Nivel.Equals("Noveno")))
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(2);

                }
                else
                {
                    cbNivel.SelectedItem = cbNivel.Items.GetItemAt(3);

                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistrarEstudiante();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void importarEstudiantes()
        {

            if (File.Exists(path))
            {
                //se abre el archivo Json que se va leer
                using (StreamReader sr = File.OpenText(path))
                {
                    string vsRespuestaJson = string.Empty;
                    while ((vsRespuestaJson = sr.ReadLine()) != null)
                    {
                        listaEstudiante = JsonConvert.DeserializeObject<List<clsEstudiante>>(vsRespuestaJson);//se deserealiza (descompone) el Json del archivo
                        dgEstudiantes.ItemsSource = null;
                        dgEstudiantes.ItemsSource = listaEstudiante;
                    }
                }
            }
            else
            {
                File.Create(path).Dispose();
            }

            if (listaEstudiante == null || listaEstudiante.Count == 0)
            {
                MessageBox.Show("Favor agregue un estudiante como mínimo para poder importar.");
                err = "ERROR-No se agrego un estudiante para ser exportado. ";
                bitacora = new escribirLog(err, false);
            }
        }

        public void ExportarEstudiantes()
        {
            string vsUsuarioJson = string.Empty;

            if (listaEstudiante != null && listaEstudiante.Count > 0)
            {
                vsUsuarioJson = JsonConvert.SerializeObject(listaEstudiante);//se serializa (transforma) el Json para el archivo
                EscribirArchivo(vsUsuarioJson);
                MessageBox.Show("Datos exportados con éxito.");
            }
            else
                MessageBox.Show("Favor agregue un estudiante como mínimo para poder exportar.");
            err = "ERROR-No se agrego un estudiante para ser exportado. ";
            bitacora = new escribirLog(err, false);
        }

        public void GuardarCambios()
        {
            if (accion == "Editar")
            {
                EditarEstudiante();
                ListaEstudiantes(null);
                accion = "";
            }
            else
            {
                RegistrarEstudiante();
                ListaEstudiantes(null);
            }
        }

        public void Limpiar()
        {
            txtID.Text = string.Empty;
            txtNom.Text = string.Empty;
            txtApes.Text = string.Empty;
            dpFNac.Text = "01/01/1990";
            txtCel.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            cbGenero.SelectedItem = null;
            txtDir.Text = string.Empty;
            cbNivel.SelectedItem = null;
        }

        public void RegistrarEstudiante()
        {
            if (string.IsNullOrEmpty(txtID.Text) || string.IsNullOrEmpty(txtNom.Text) || string.IsNullOrEmpty(txtApes.Text)
                   || string.IsNullOrEmpty(txtCel.Text) || string.IsNullOrEmpty(txtCorreo.Text) || string.IsNullOrEmpty(cbNivel.Text)
                   || string.IsNullOrEmpty(dpFNac.Text) || string.IsNullOrEmpty(cbGenero.Text))
            {
                throw new Exception("Debe de proporcionar informacion en todos los datos solicitados");
            }
            else
            {
                clsEstudiante estudiante = new clsEstudiante();
                if (txtID.Text.Length > 0)
                {
                    estudiante.IdEstudiante = Convert.ToInt32(txtID.Text);
                    estudiante.Identificacion = txtID.Text;
                    estudiante.Nombre = txtNom.Text;
                    estudiante.Apellidos = txtApes.Text;
                    estudiante.FechaNacimiento = dpFNac.Text;
                    estudiante.Celular = txtCel.Text;
                    estudiante.Correo = txtCorreo.Text;
                    if (cbGenero.Text.ToString() == "Femenino")
                    {
                        estudiante.Genero = "F";
                    }
                    else
                    {
                        estudiante.Genero = "M";

                    }
                    estudiante.Direccion = txtDir.Text;
                    estudiante.Nivel = cbNivel.Text.ToString();
                    listaEstudiante.Add(estudiante);

                    SaveEstudianteToJson(listaEstudiante, path);
                    MessageBox.Show("Estudiante agregado exitosamente.");
                    err = $"El estudiante,ID:{estudiante.IdEstudiante},Nombre:{estudiante.Nombre},Apellidos:{estudiante.Apellidos}, ha sido registrado";
                    registro = new Registros(err, false);
                    importarEstudiantes();
                    Limpiar();
                    cont++;
                }
                else
                {
                    err = "Error no se completo el campo de identificación";
                    MessageBox.Show(err);
                    bitacora = new escribirLog(err, false);
                }
            }
        }

        static void SaveEstudianteToJson(List<clsEstudiante> listaDocentes, string filePath)
        {
            if (File.Exists(filePath))
            {
                // Serializar la lista de vuelta a JSON
                //string json = JsonConvert.SerializeObject(listaDocentes, Formatting.Indented);
                string json = JsonConvert.SerializeObject(listaDocentes.ToArray());
                // Escribir el JSON actualizado de vuelta al archivo
                File.WriteAllText(filePath, json);
            }
        }

        public void EditarEstudiante()
        {
            if (listaEstudiante != null && listaEstudiante.Count > 0)
            {
                foreach (clsEstudiante item in listaEstudiante)
                {
                    if (item.IdEstudiante == estudianteGrilla.IdEstudiante)
                    {
                        item.Nombre = txtNom.Text;
                        item.Identificacion = txtID.Text;
                        item.Apellidos = txtApes.Text;
                        item.FechaNacimiento = dpFNac.Text;
                        item.Celular = txtCel.Text;
                        item.Correo = txtCorreo.Text;
                        if (cbGenero.Text.ToString() == "Femenino")
                        {
                            item.Genero = "F";
                        }
                        else
                        {
                            item.Genero = "M";

                        }
                        item.Nivel = cbNivel.Text.ToString();
                        item.Direccion = txtDir.Text;
                        item.Nivel = cbNivel.Text.ToString();
                    }
                }
                int id = Convert.ToInt32(txtID.Text);
                MessageBox.Show("Estudiante modificado exitosamente.");
                err = $"El estudiante,ID: {id}, ha sido modificado.";
                registro = new Registros(err, false);
            }
        }

        public void ListaEstudiantes(string pDato)
        {
            if (listaEstudiante != null && listaEstudiante.Count > 0)
            {
                if (!string.IsNullOrEmpty(pDato) && pDato != null && pDato != "")
                {
                    listalistaEstudianteTemp = new List<clsEstudiante>();
                    listalistaEstudianteTemp = listaEstudiante.FindAll(u => u.Nombre.Contains(pDato) || u.Identificacion.Contains(pDato) || u.Nivel.Contains(pDato));

                    dgEstudiantes.ItemsSource = null;
                    dgEstudiantes.ItemsSource = listalistaEstudianteTemp;
                }
                else
                {
                    dgEstudiantes.ItemsSource = null;
                    dgEstudiantes.ItemsSource = listaEstudiante;
                }
            }
            else
            {
                dgEstudiantes.ItemsSource = null;
                dgEstudiantes.ItemsSource = listaEstudiante;
            }
        }

        public void EscribirArchivo(string pDatos)
        {
            //string path = @"C:\Examen\Estudiantes.json";

            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);//Siempre borro del archivo antes de escribir

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(pDatos);
                }
            }
        }

        public void EliminarUsuario()
        {
            if (listaEstudiante != null && listaEstudiante.Count > 0)
            {
                //listaEstudiante.RemoveAll(u => u.IdEstudiante == estudianteGrilla.IdEstudiante);
                EliminarUsuarioPorId(Convert.ToInt32(txtID.Text));
            }
        }

        public void EliminarUsuarioPorId(int idUsuario)
        {
            List<clsEstudiante> docentes = LeerUsuariosDesdeArchivo(path);

            // Buscar y eliminar el docente con el IdDocente dado
            clsEstudiante docenteAEliminar = docentes.Find(d => d.IdEstudiante == idUsuario);
            if (docenteAEliminar != null)
            {
                docentes.Remove(docenteAEliminar);
                GuardarUsuariosEnArchivo(docentes, path);
                MessageBox.Show("Se elimino de forma correcta");
                err = $"El usuario:{idUsuario}, ha sido eliminado.";
                importarEstudiantes();
            }
            else
            {
                MessageBox.Show("No existe ese ID");
                err = $"El ID:{idUsuario},no existe";
                bitacora = new escribirLog(err, false);
            }
        }

        private void GuardarUsuariosEnArchivo(List<clsEstudiante> docentes, string filePath)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, docentes);
            }
        }

        private List<clsEstudiante> LeerUsuariosDesdeArchivo(string filePath)
        {
            List<clsEstudiante> docentes;
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                docentes = (List<clsEstudiante>)serializer.Deserialize(file, typeof(List<clsEstudiante>));
            }
            return docentes;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (estudianteGrilla != null && estudianteGrilla.IdEstudiante != 0)
            {
                string ID = txtID.Text;
                var respuesta = MessageBox.Show($"Desea eliminar el estudiante con ID: {ID}?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Question);
                {
                    if (respuesta == MessageBoxResult.Yes)
                    {
                        EliminarUsuarioPorId(Convert.ToInt32(txtID.Text));
                        err = $"El estudiante:{ID}, ha sido eliminado.";
                        registro = new Registros(err, false);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Debe seleccionar al menos un estudiante en la tabla.");
                err = "ERROR-No se seleciono un estudiante de la tabla.";
                bitacora = new escribirLog(err, false);
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                MessageBox.Show("Debe proporcionar un numero de identificacion para realizar la busqueda unica");
                err = "Error-El campo identificacion no se completo para buscar al usuario.";
                bitacora = new escribirLog(err, false);
            }
            else
            {
                ListaEstudiantes(txtBuscar?.Text);
            }
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            ExportarEstudiantes();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (estudianteGrilla != null && estudianteGrilla.IdEstudiante != 0)
                {
                    if (txtID.Text != null)
                    {
                        int id = Convert.ToInt32(txtID.Text);
                        List<clsEstudiante> listaEditar = LoadUsuarioFromJson(path);

                        EditEstudiante(listaEditar, id, new clsEstudiante
                        {
                            Nombre = txtNom.Text,
                            Apellidos = txtApes.Text,
                            Identificacion = txtID.Text,
                            Celular = txtCel.Text,
                            Correo = txtCorreo.Text,
                            Nivel = cbNivel.Text,
                            FechaNacimiento = dpFNac.Text,
                            Direccion = txtDir.Text,
                            Genero = cbGenero.Text,
                        });

                        SaveEstudianteToJson(listaEditar, path);
                        MessageBox.Show("Se edito el Usuario, ID: " + id.ToString());
                        importarEstudiantes();
                        err = $"El Usuario,ID:{id}, ha sido modificado.";
                        registro = new Registros(err, false);
                    }
                    else
                    {
                        MessageBox.Show("Ingresa la dentificacion del usuario");
                        err = "ERROR-No se completo el campo de dentificacion.";
                        bitacora = new escribirLog(err, false);
                    }
                }
                else
                {
                    MessageBox.Show("Debe seleccionar al menos un estudiante en la tabla.");
                    err = "Error-No se ha seleccionado un estudiante de la tabla.";
                    bitacora = new escribirLog(err, false);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error a realizar la operacion");
                err = "Error-La solicitud editar no pudo ser procesada.";
                bitacora = new escribirLog(err, false);
            }

        }

        static List<clsEstudiante> LoadUsuarioFromJson(string filePath)
        {
            // Leer el contenido del archivo JSON
            string jsonContent = File.ReadAllText(filePath);

            // Deserializar el JSON a una lista de objetos de clsDocente
            List<clsEstudiante> listaDocentes = JsonConvert.DeserializeObject<List<clsEstudiante>>(jsonContent);

            return listaDocentes;
        }

        static void EditEstudiante(List<clsEstudiante> listaDocentes, int idDocente, clsEstudiante nuevosDatos)
        {
            string err;
            // Buscar el docente con el ID especificado
            clsEstudiante estudiante = listaDocentes.Find(d => d.IdEstudiante == idDocente);

            if (estudiante != null)
            {
                // Aplicar los nuevos datos al docente encontrado
                if (!string.IsNullOrEmpty(nuevosDatos.Nombre))
                    estudiante.Nombre = nuevosDatos.Nombre;

                if (!string.IsNullOrEmpty(nuevosDatos.Apellidos))
                    estudiante.Apellidos = nuevosDatos.Apellidos;

                if (!string.IsNullOrEmpty(nuevosDatos.Direccion))
                    estudiante.Direccion = nuevosDatos.Direccion;

                if (!string.IsNullOrEmpty(nuevosDatos.Correo))
                    estudiante.Correo = nuevosDatos.Correo;

                if (!string.IsNullOrEmpty(nuevosDatos.Identificacion))
                    estudiante.Identificacion = nuevosDatos.Identificacion;

                if (!string.IsNullOrEmpty(nuevosDatos.Celular))
                    estudiante.Celular = nuevosDatos.Celular;

                if (!string.IsNullOrEmpty(nuevosDatos.Genero))
                    estudiante.Genero = nuevosDatos.Genero;

                if (!string.IsNullOrEmpty(nuevosDatos.Nivel))
                    estudiante.Nivel = nuevosDatos.Nivel;

                if (!string.IsNullOrEmpty(nuevosDatos.FechaNacimiento))
                    estudiante.FechaNacimiento = nuevosDatos.FechaNacimiento;
            }
            else
            {
                err = "ERROR-No se completo el campo de dentificacion.";
                bitacora = new escribirLog(err, false);
                throw new Exception($"No se encontró ningún usuario con el ID {idDocente}");
            }
        }

    }
}
