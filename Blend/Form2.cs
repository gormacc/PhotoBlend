using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blend
{
    public partial class Form2 : Form
    {

        private readonly Form1 _form1;

        public Form2(Form1 form)
        {
            _form1 = form;
            InitializeComponent();
            InitializeContextMenu();
        }

        public void ShowPicture(Bitmap bmp)
        {
            this.ClientSize = new System.Drawing.Size(bmp.Width, bmp.Height);
            this.pictureBox1.Image = bmp;
        }

        private void InitializeContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem save = new MenuItem();
            save.Text = "Save";
            save.Visible = true;
            save.Click += new EventHandler(save_Click);

            MenuItem addToL = new MenuItem();
            addToL.Text = "Add To Library";
            addToL.Click += new EventHandler(add_Click);

            cm.MenuItems.Add(save);
            cm.MenuItems.Add(addToL);

            this.pictureBox1.ContextMenu = cm;
        }
        
        private void save_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedlg = new SaveFileDialog();
            savedlg.Title = "Otwieranie";
            savedlg.Filter = "Image files (*.bmp;*.jpg; *.png)|*.bmp;*.jpg;*.png | All files (*.*) | *.*";
            savedlg.FileName = string.Format("NewImage{0}.bmp", _form1.newFileCounter);

            try
            {
                if (savedlg.ShowDialog() == DialogResult.OK)
                {
                    if (savedlg.FileName != "")
                    {
                        Bitmap bmp = this.pictureBox1.Image as Bitmap;
                        bmp.Save(savedlg.FileName,System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void add_Click(object sender, EventArgs e)
        {
            string FileName = string.Format("NewImage{0}.bmp", _form1.newFileCounter);
            Bitmap bmp = this.pictureBox1.Image as Bitmap;
            bmp.Save(FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            string path = string.Format(Application.StartupPath + "\\" + FileName);
            _form1.filesInLibrary.Add(path);
            _form1.addToLibrary(bmp);   
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }
    }
}
