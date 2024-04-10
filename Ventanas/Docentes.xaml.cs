﻿using Examen.Clases;
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

namespace Examen
{
    /// <summary>
    /// Interaction logic for Docentes.xaml
    /// </summary>
    public partial class Docentes : Window
    {
        #region Declaraciones
        List<clsDocente> listaDocentes = new List<clsDocente>();//Lista de docentes
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
            string path = @"C:\Examen\Docentes.json";

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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (listaDocentes == null || listaDocentes.Count == 0)
            {
                MessageBox.Show("Favor agregue un docente como mínimo para poder importar.");
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    EstadoCivil = txtAsignatura.Text,
                    Genero = cbGenero.Text,
                });

                //string json = JsonConvert.SerializeObject(listaDocentes.ToArray());

                //File.WriteAllText(@"C:\Examen\Docentes.json", json);
                SaveDocentesToJson(listaDocentes, @"C:\Examen\Docentes.json");
                MessageBox.Show("Se agrego al JSON");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
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
                List<clsDocente> listaEditar = LoadDocentesFromJson(@"C:\Examen\Docentes.json");

                EditDocente(listaEditar, 1, new clsDocente
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
                    EstadoCivil = txtAsignatura.Text,
                    Genero = cbGenero.Text,
                });

                SaveDocentesToJson(listaEditar, @"C:\Examen\Docentes.json");
                MessageBox.Show("Se edito al JSON");
            }
            catch(Exception)
            {

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
            // Serializar la lista de docentes de vuelta a JSON
            string json = JsonConvert.SerializeObject(listaDocentes, Formatting.Indented);

            // Escribir el JSON actualizado de vuelta al archivo
            File.WriteAllText(filePath, json);
        }

        static void EditDocente(List<clsDocente> listaDocentes, int idDocente, clsDocente nuevosDatos)
        {
            // Buscar el docente con el ID especificado
            clsDocente docente = listaDocentes.Find(d => d.IdDocente == idDocente);

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
                    docente.Nombre = nuevosDatos.Correo;

                if (!string.IsNullOrEmpty(nuevosDatos.Identificacion))
                    docente.Apellidos = nuevosDatos.Identificacion;

                if (!string.IsNullOrEmpty(nuevosDatos.Celular))
                    docente.EstadoCivil = nuevosDatos.Celular;

                if (!string.IsNullOrEmpty(nuevosDatos.Asignatura))
                    docente.Nombre = nuevosDatos.Asignatura;

                if (!string.IsNullOrEmpty(nuevosDatos.Nivel))
                    docente.Apellidos = nuevosDatos.Nivel;

                if (!string.IsNullOrEmpty(nuevosDatos.FechaInicioMEP))
                    docente.EstadoCivil = nuevosDatos.FechaInicioMEP;  
                
                if (!string.IsNullOrEmpty(nuevosDatos.Genero))
                    docente.EstadoCivil = nuevosDatos.Genero;


                // Puedes agregar más campos según sea necesario
            }
            else
            {
                throw new Exception($"No se encontró ningún docente con el ID {idDocente}");
            }
        }

    }
}