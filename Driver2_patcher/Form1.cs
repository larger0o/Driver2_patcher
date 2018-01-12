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
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCargarImagen_Click(object sender, EventArgs e)
        {
            btnParchear.Enabled = false;
            openFileDialog1.ShowDialog();

        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string fichero = openFileDialog1.FileName;
            if (fichero != null)
            {
                // Comprobamos que el fichero es parcheable buscando las cadenas a editar
                CargarCompatibilidad(fichero);

            }
        }
        private void CargarCompatibilidad(string fichero)
        {
            string ValorEU, ValorUS, refEU, refUS;
            int PosicionEU, PosicionUS;

            //SLES_02994
            refEU = "SLES_029.9";
            ValorEU = "SLES_129.9";
            PosicionEU = 154503145;

            refUS = "SLUS_011.61";
            ValorUS = "SLUS_013.18";            
            PosicionUS = 155667209;

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

    }
}
