using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prueba.Modelos;


namespace Prueba
{
    public partial class Form1 : Form
    {

        List<estanque> _estanques = new List<estanque>();
        //Variable ruta Json
        string serverPathEstanques = Path.Combine("..\\..\\..\\Prueba\\", "json", "estanques.json");
        string serverPathLog = Path.Combine("..\\..\\..\\Prueba\\", "json", "log.json");

        int idEstanque;


        public Form1()
        {
            InitializeComponent();
            ActualizarListas();
            ActualizarListView();
          //  cargarDataGrid();
        }
        public void ActualizarListas()
        {
            listBox2.Items.Clear();
            var jsonTexto = File.ReadAllText(serverPathEstanques);
            _estanques = JsonConvert.DeserializeObject<List<estanque>>(jsonTexto);
            if (_estanques == null)
                _estanques = new List<estanque>();

            _estanques.ForEach(a => {
                listBox2.Items.Add(a.Nombre);
            });
        }

        public void ActualizarListView()
        {
            listView1.Items.Clear();
            var jsonTexto = File.ReadAllText(serverPathEstanques);
            _estanques = JsonConvert.DeserializeObject<List<estanque>>(jsonTexto);
            if (_estanques == null)
                _estanques = new List<estanque>();

            _estanques.ForEach(a =>{
                ListViewItem lista = new ListViewItem(a.Nombre);
                lista.SubItems.Add(a.capacidad.ToString());
                lista.SubItems.Add(a.Peces.ToString());
                listView1.Items.Add(lista);

            }); 
        }


        private void button1_Click_1(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {

                MessageBox.Show("Debe completar la informacion", "Campo de Texto Vacio");

                return;
            }

                var estanque = new estanque() { Id = Guid.NewGuid().ToString(), Nombre = textBox2.Text, capacidad = int.Parse(textBox3.Text), Peces = 0 };
            var sIniFile = File.ReadAllText(serverPathEstanques);
            var estanquesDes = JsonConvert.DeserializeObject<List<estanque>>(sIniFile);

            if (estanquesDes == null)
                estanquesDes = new List<estanque>();

            estanquesDes.Add(estanque);

            string estanqusJson = JsonConvert.SerializeObject(estanquesDes);

            File.WriteAllText(serverPathEstanques, estanqusJson);
            MessageBox.Show("Estanque '"+ textBox2.Text + "' Creado", "Estanque Creado Correctamente" );
            limpiarEstanqueCreado();
            ActualizarListas();
            ActualizarListView();
            
        }

        private void limpiarEstanqueCreado() 
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox2.Focus();
        }
        private void limpiarPeces() 
        {
            txtCantidadPecesAgregar.Clear();
            txtCantidadPecesAgregar.Focus();
        }
        private void cargarDataGrid(string index)
        {
            var json = File.ReadAllText(serverPathLog);
            var m = JsonConvert.DeserializeObject<List<log>>(json);
            var a = m.Where(z => z.idEstanque == index).Select(y => new ListaLog(){
                fecha=y.fecha,
                cantidad=y.cantidad,
                operacion=(y.operacion)?"Agregar":"Quitar"}).ToList();

            dataGridView1.DataSource = a;

        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCantidadPecesAgregar.Text))
            {

                MessageBox.Show("Texto Vacio, completa la informacion de la cantidad de peces.", "Texto Vacio");

                return;
            }
            var sIniFile = File.ReadAllText(serverPathEstanques);
            var estanqueDes = JsonConvert.DeserializeObject<List<estanque>>(sIniFile);
            var estanque = estanqueDes[idEstanque];
            var validacion = estanque.Peces + int.Parse(txtCantidadPecesAgregar.Text);

            if (validacion > estanque.capacidad)
            {
                MessageBox.Show("La capacidad maxima del estanque '" + estanque.Nombre+ "' es de " + estanque.capacidad.ToString(), "Estanque Lleno", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               
            }
            else 
            {
                estanque.Peces = validacion;
                string pecesAgregadosJson = JsonConvert.SerializeObject(estanqueDes);
                File.WriteAllText(serverPathEstanques, pecesAgregadosJson);
                MessageBox.Show( txtCantidadPecesAgregar.Text+" Peces Agregados a estanque '" + txtEstanqueSeleccionado.Text + "' ", "Peces agregados correctamente", MessageBoxButtons.OK);

                //=================LOG=================================
                
                var slogfile = File.ReadAllText(serverPathLog);
                var logDes = JsonConvert.DeserializeObject<List<log>>(slogfile);

                if (logDes == null)
                {
                    logDes = new List<log>();    
                }

                log log = new log
                {
                    id = Guid.NewGuid().ToString(),
                    idEstanque = estanque.Id,
                    fecha = DateTime.Now,
                    operacion = true,
                    cantidad = int.Parse(txtCantidadPecesAgregar.Text)

                } ;
                logDes.Add(log);

                string logJSON = JsonConvert.SerializeObject(logDes);

                File.WriteAllText(serverPathLog, logJSON);

                //=================LOG=================================

                limpiarPeces();
                ActualizarListView();
                ActualizarListas();
                txtCantidadActualEstanque.Text = estanque.Peces.ToString();

            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCantidadPecesAgregar.Text))
            {

                MessageBox.Show("Texto Vacio, completa la informacion de la cantidad de peces.", "Texto Vacio");

                return;
            }

            var sIniFile = File.ReadAllText(serverPathEstanques);
            var estanqueDes = JsonConvert.DeserializeObject<List<estanque>>(sIniFile);
            var estanque = estanqueDes[idEstanque];
            var validacion = estanque.Peces - int.Parse(txtCantidadPecesAgregar.Text);
           
            if (validacion < estanque.capacidad && validacion < 0 )
            {
                MessageBox.Show("El estanque '"+ txtEstanqueSeleccionado.Text+"' se encuentra vacio." , "Estanque Vacio.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            else
            {
                estanque.Peces = validacion;
                string pecesAgregadosJson = JsonConvert.SerializeObject(estanqueDes);
                File.WriteAllText(serverPathEstanques, pecesAgregadosJson);
                MessageBox.Show(txtCantidadPecesAgregar.Text + " Peces Quitados a estanque '" + txtEstanqueSeleccionado.Text + "' ", "Peces Quitados Correctamente", MessageBoxButtons.OK);

                //=================LOG=================================

                var slogfile = File.ReadAllText(serverPathLog);
                var logDes = JsonConvert.DeserializeObject<List<log>>(slogfile);

                if (logDes == null)
                {
                    logDes = new List<log>();
                }

                log log = new log
                {
                    id = Guid.NewGuid().ToString(),
                    idEstanque = estanque.Id,
                    fecha = DateTime.Now,
                    operacion = false,
                    cantidad = int.Parse(txtCantidadPecesAgregar.Text)

                };
                logDes.Add(log);

                string logJSON = JsonConvert.SerializeObject(logDes);

                File.WriteAllText(serverPathLog, logJSON);

                //=================LOG=================================

                limpiarPeces();
                ActualizarListas();
                ActualizarListView();
                txtCantidadActualEstanque.Text = estanque.Peces.ToString();

            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sIniFile = File.ReadAllText(serverPathEstanques);
            var estanqueDes = JsonConvert.DeserializeObject<List<estanque>>(sIniFile);

            var index = listBox2.SelectedIndex;
            idEstanque = index;
            cargarDataGrid(estanqueDes[index].Id);

        }
        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 ){ 
            
           var index = listView1.SelectedIndices[0];
           idEstanque = index;
            }

            ListView.SelectedListViewItemCollection listIndex = this.listView1.SelectedItems;


            foreach (ListViewItem item in listIndex)
            {
                
                txtEstanqueSeleccionado.Text = item.SubItems[0].Text;
                txtCapacidad.Text = item.SubItems[1].Text;
                txtCantidadActualEstanque.Text = item.SubItems[2].Text;

            }
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
