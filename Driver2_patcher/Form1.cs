using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Driver2_patcher
{
    public partial class Form1 : Form
    {
        string fichero = "";

        //SLES_02994
        string refEU = "SLES_029.9";
        string ValorEU = "SLES_129.9*";
        int PosicionEU = 154503145;

        string refUS = "SLUS_011.61";
        string ValorUS = "SLUS_013.18";            
        int PosicionUS = 155667209;
        //NTSC true, PAL false
        bool version = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void btnCargarImagen_Click(object sender, EventArgs e)
        {
            btnParchear.Enabled = false;
            toolStripProgressBar1.Value = 0;
            lblDetectado.Text = "";
            lblFichero.Text = "";
            openFileDialog1.ShowDialog();

        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
           fichero = openFileDialog1.FileName;
            if (fichero != null)
            {
                // Comprobamos que el fichero es parcheable buscando las cadenas a editar
                CargarCompatibilidad(fichero);

            }
        }
        private void CargarCompatibilidad(string fichero)
        {

            try
            {
                using (FileStream fs = File.OpenRead(fichero))
                {
                    lblFichero.Text = openFileDialog1.SafeFileName + " cargado";
                    fs.Seek(PosicionEU, SeekOrigin.Begin);
                    byte[] b = new byte[11];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    fs.Read(b, 0, 11);
                    if (temp.GetString(b).Contains(refEU))
                    {
                        lblDetectado.Text = "Versión PAL detectada";
                        btnParchear.Enabled = true;
                    }
                    else
                    {
                        fs.Seek(PosicionUS, SeekOrigin.Begin);
                        fs.Read(b, 0, 11);
                        if (temp.GetString(b).Contains(refUS))
                        {
                            lblDetectado.Text = "Versión NTSC detectada";
                            btnParchear.Enabled = true;
                            version = true;
                        }
                        else
                            MessageBox.Show("El archivo proporcionado no es una imagen válida", "Imagen inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } catch (IOException e)
            {
                MessageBox.Show("Error al acceder al archivo. Otra aplicación la está utilizando.", "Error al leer", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception e)
            {

                MessageBox.Show("Error al acceder al archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }




        }

        private void btnParchear_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Visible = true;
            
            string ficherofinal = fichero.Insert(fichero.Length-4,"-NEW");
            int buffer = 100000;
            int contador = 0;
            byte[] b = new byte[buffer];
            int InicioParcheo;
            byte[] Parcheo;
            if (version)
            {
                InicioParcheo = PosicionUS;
                Parcheo = Encoding.ASCII.GetBytes(ValorUS);
            }
            else
            {
                InicioParcheo = PosicionEU;
                Parcheo = Encoding.ASCII.GetBytes(ValorEU);
            }

            try
            {
                using (FileStream origen = File.OpenRead(fichero))
                using (FileStream destino = File.OpenWrite(ficherofinal))
                {
                   lblFichero.Text = "Parcheando";
                   contador = CopiarDatos(buffer, contador, InicioParcheo, b, origen, destino);
                   toolStripProgressBar1.Value = 50;
                    //escribimos el cambio
                    destino.Write(Parcheo, 0, Parcheo.Length);
                    origen.Seek(Parcheo.Length, SeekOrigin.Current);
                    contador += Parcheo.Length;

                    CopiarDatos(buffer, contador, (int)origen.Length, b, origen, destino);
                    toolStripProgressBar1.Value = 100;
                    lblFichero.Text = "Completado";

                }
                MessageBox.Show("Parcheo completado","Completado",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        int CopiarDatos (int buffer, int contador, int final, byte[] b, FileStream origen, FileStream destino)
        {

            //para evitar que el buffer se pase leyendo, cortamos antes del tamaño del buffer
            //como es un int, dividimos entre buffer y multiplicamos por buffer para cepillarnos los valores inferiores a buffer
            while (contador < ((final / buffer) * buffer))
            {
                b.Initialize();
                origen.Read(b, 0, buffer);
                destino.Write(b, 0, buffer);
                contador += buffer;
            }

            b.Initialize();
            origen.Read(b, 0, final % buffer);
            destino.Write(b, 0, final % buffer);
            contador += (final % buffer);

            return contador;

        }

        private void btnAcerca_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Parche y parcheador por larger0o.\r\nMe podéis encontrar en Twitter: @larger0o.", "Driver 2 - Parcheador", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
