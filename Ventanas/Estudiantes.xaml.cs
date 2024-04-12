﻿using Examen.Clases;
using Examen.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string err;
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
            //GuardarCambios();
            RegistrarEstudiante();
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
            } else
            {
                File.Create(path).Dispose();
            }

            if (listaEstudiante == null || listaEstudiante.Count == 0)
            {
                MessageBox.Show("Favor agregue un docente como mínimo para poder importar.");

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
                MessageBox.Show("Favor agregue un usuario como mínimo para poder exportar.");
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

                SaveUsuarioToJson(listaEstudiante, path);

                MessageBox.Show("Estudiante agregado exitosamente.");
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

        static void SaveUsuarioToJson(List<clsEstudiante> listaDocentes, string filePath)
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

                MessageBox.Show("Estudiante modificado exitosamente.");
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
                importarEstudiantes();
            }
            else
            {
                MessageBox.Show("No existe ese ID");
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
                var respuesta = MessageBox.Show("Deseas eliminar este estudiante?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Question);
                {
                    if (respuesta == MessageBoxResult.Yes)
                    {
                        EliminarUsuarioPorId(Convert.ToInt32(txtID.Text));
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Debe seleccionar un estudiante.");
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            ListaEstudiantes(txtBuscar?.Text);
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            ExportarEstudiantes();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (txtID.Text != null)
                {
                    /*string filePath = @"C:\Examen\Docentes.json";

          // Leer el contenido del archivo JSON
          string jsonContent = File.ReadAllText(filePath);

          // Parsear el JSON a una lista de objetos dinámicos
          JArray jsonArray = JArray.Parse(jsonContent);

          // Buscar el usuario que deseas editar (en este caso, el primero)
          JObject usuario = jsonArray[0] as JObject;

          // Realizar los cambios necesarios
          usuario["Nombre"] = "NuevoNombre";

          // Convertir la lista de objetos dinámicos de vuelta a una cadena JSON
          string updatedJson = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

          // Escribir la cadena JSON actualizada de vuelta al archivo
          File.WriteAllText(filePath, updatedJson);*/
                    int id = Convert.ToInt32(txtID.Text);
                    List<clsEstudiante> listaEditar = LoadUsuarioFromJson(path);

                    EditDocente(listaEditar, id, new clsEstudiante
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

                    SaveUsuarioToJson(listaEditar,path);
                    MessageBox.Show("Se edito el Usuario, ID: "+id.ToString());
                    importarEstudiantes();

                }
                else
                {
                    MessageBox.Show("Ingresa la dentificacion del usuario");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error a realizar la operacion");
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

        static void EditDocente(List<clsEstudiante> listaDocentes, int idDocente, clsEstudiante nuevosDatos)
        {
            // Buscar el docente con el ID especificado
            clsEstudiante docente = listaDocentes.Find(d => d.IdEstudiante == idDocente);

            if (docente != null)
            {
                // Aplicar los nuevos datos al docente encontrado
                if (!string.IsNullOrEmpty(nuevosDatos.Nombre))
                    docente.Nombre = nuevosDatos.Nombre;

                if (!string.IsNullOrEmpty(nuevosDatos.Apellidos))
                    docente.Apellidos = nuevosDatos.Apellidos;

                if (!string.IsNullOrEmpty(nuevosDatos.Direccion))
                    docente.Direccion = nuevosDatos.Direccion;

                if (!string.IsNullOrEmpty(nuevosDatos.Correo))
                    docente.Correo = nuevosDatos.Correo;

                if (!string.IsNullOrEmpty(nuevosDatos.Identificacion))
                    docente.Identificacion = nuevosDatos.Identificacion;

                if (!string.IsNullOrEmpty(nuevosDatos.Celular))
                    docente.Celular = nuevosDatos.Celular;

                if (!string.IsNullOrEmpty(nuevosDatos.Genero))
                    docente.Genero = nuevosDatos.Genero;

                if (!string.IsNullOrEmpty(nuevosDatos.Nivel))
                    docente.Nivel = nuevosDatos.Nivel;

                if (!string.IsNullOrEmpty(nuevosDatos.FechaNacimiento))
                    docente.FechaNacimiento = nuevosDatos.FechaNacimiento;



                // Puedes agregar más campos según sea necesario
            }
            else
            {
                throw new Exception($"No se encontró ningún usuario con el ID {idDocente}");
            }
        }

    }
}
