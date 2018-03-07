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
    public partial class Form1 : Form
    {
        public bool leftflag = false;
        public bool rightflag = false;
        public double alpha = 0.5;
        public BackgroundWorker bw = new BackgroundWorker();
        public BackgroundWorker bw2 = new BackgroundWorker();
        public Bitmap bwBmp1;
        public Bitmap bw2Bmp1;
        public Bitmap bwBmp2;
        public Bitmap bw2Bmp2;
        public Bitmap newBmp = Properties.Resources.NoImage;
        public Bitmap bwBmp;
        public Bitmap bw2Bmp;
        public int newFileCounter = -1;
        public List<string> filesInLibrary = new List<string>();
        public bool picSelected = false;
        public Panel panelSelected = null; 


        public Form1()
        {
            InitializeComponent();
            initializeWorkers();
            this.label2.Visible = false;
            this.progressBar1.Visible = false;
            this.progressBar2.Visible = false;
            this.trackBar1.Value = 5;
            this.button1.Enabled = false;
            KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyPressed);
            this.flowLayoutPanel1.DragEnter += new DragEventHandler(DragEnterEvent);
            this.flowLayoutPanel1.DragDrop += new DragEventHandler(DragAndDrop);
        }

        public void DragEnterEvent(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        public void DragAndDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    bool exist = false;
                    foreach (string dest in this.filesInLibrary)
                    {
                        if (dest == file)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if(!exist)
                    {
                        this.filesInLibrary.Add(file);
                        Bitmap bmp = Image.FromFile(file) as Bitmap;
                        addToLibrary(bmp,file);
                    }
                }              
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void blend1()
        {
            Bitmap bmp1 = new Bitmap(this.pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(this.pictureBox2.Image);
            int width = System.Math.Min(bmp1.Width, bmp2.Width);
            int height = System.Math.Min(bmp1.Height, bmp2.Height);
            Bitmap retBmp = new Bitmap(width, height);
            double alfa = this.alpha;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color colour1 = bmp1.GetPixel(i, j);
                    Color colour2 = bmp2.GetPixel(i, j);
                    int newR = (int)(alfa * colour1.R + (1 - alfa) * colour2.R);
                    int newG = (int)(alfa * colour1.G + (1 - alfa) * colour2.G);
                    int newB = (int)(alfa * colour1.B + (1 - alfa) * colour2.B);
                    Color newColor = Color.FromArgb(newR, newG, newB);

                    ((Bitmap)retBmp).SetPixel(i, j, newColor);
                }
                bw.ReportProgress(100 * i / width);
            }
            this.bwBmp = retBmp;

        }

        public void blend2()
        {

            Bitmap bmp1 = new Bitmap(this.pictureBox1.Image);  
            Bitmap bmp2 = new Bitmap(this.pictureBox2.Image);



            int width = System.Math.Min(bmp1.Width, bmp2.Width);
            int height = System.Math.Min(bmp1.Height,bmp2.Height);
            Bitmap retBmp = new Bitmap(width, height);
            double alfa = this.alpha;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color colour1 = bmp1.GetPixel(i, j);
                    Color colour2 = bmp2.GetPixel(i, j);
                    int newR = (int)(alfa * colour1.R + (1 - alfa) * colour2.R);
                    int newG = (int)(alfa * colour1.G + (1 - alfa) * colour2.G);
                    int newB = (int)(alfa * colour1.B + (1 - alfa) * colour2.B);
                    Color newColor = Color.FromArgb(newR, newG, newB);

                    ((Bitmap)retBmp).SetPixel(i, j, newColor);
                }
                bw2.ReportProgress(100 * i / width);
            }
            this.bw2Bmp = retBmp;

        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
           if(e.KeyCode == Keys.F12)
            {

                if(!leftflag)
                {
                    var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                    this.pictureBox1.Image = bmpScreenshot;
                    this.leftflag = true;
                }
                else 
                {
                    var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                    this.pictureBox2.Image = bmpScreenshot;
                    this.rightflag = true;
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                if(picSelected)
                {
                    for(int i = 0; i < this.filesInLibrary.Count; i++)
                    {
                        if(this.panelSelected.Name == this.filesInLibrary[i])
                        {
                            this.filesInLibrary.RemoveAt(i);
                            break;
                        }
                    }

                    this.flowLayoutPanel1.Controls.Remove(this.panelSelected);
                    this.panelSelected = null;
                    this.picSelected = false;
                }
            }

                CheckPressButton();
        }

        public void addToLibrary(Bitmap bmp , string filename = "NonPath")
        {
            Panel panel = new Panel();
            panel.BackColor = Color.White;
            panel.Width = 150;
            panel.Height = 150;
            panel.Padding = new Padding { All = 5 };
            panel.Name = filename;

            PictureBox pic = new PictureBox();
            pic.Width = 150;
            pic.Height = 150;
            pic.Image = bmp;
            pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pic.Dock = DockStyle.Fill;
            pic.Click += new EventHandler(pic_Click);

            panel.Controls.Add(pic);
            this.flowLayoutPanel1.Controls.Add(panel);
        }

        private void pic_Click(object sender, EventArgs e)
        {
            PictureBox pc = sender as PictureBox;
            Panel pan = pc.Parent as Panel;

            if(!this.picSelected)
            {
                pan.BackColor = Color.Orange;
                this.panelSelected = pan;
                picSelected = true;
            }
            else
            {
                if(pan == this.panelSelected)
                {
                    this.panelSelected.BackColor = Color.White;
                    this.picSelected = false;
                    return;
                }
                this.panelSelected.BackColor = Color.White;
                pan.BackColor = Color.Orange;
                this.panelSelected = pan;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(this.picSelected)
            {
                foreach (PictureBox pic in this.panelSelected.Controls)
                    this.pictureBox1.Image = pic.Image as Bitmap;
                this.leftflag = true;
                if (this.rightflag)
                    this.button1.Enabled = true;
                return;
            }

            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Otwieranie";
                    dlg.Filter = "Image files (*.bmp;*.jpg; *.png)|*.bmp;*.jpg;*.png | All files (*.*) | *.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string ext = System.IO.Path.GetExtension(dlg.FileName);
                        if (ext != ".bmp" && ext != ".jpg" && ext != ".png")
                        {
                            MessageBox.Show("Wrong file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.pictureBox1.Image = Properties.Resources.NoImage;
                            this.leftflag = false;
                            return;
                        }

                        this.pictureBox1.Image = new Bitmap(dlg.FileName);
                        this.leftflag = true;
                    }
                    else
                    {
                        this.pictureBox1.Image = Properties.Resources.NoImage;
                        this.leftflag = false;
                        this.button1.Enabled = false;
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Wrong file", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CheckPressButton();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (this.picSelected)
            {
                foreach (PictureBox pic in this.panelSelected.Controls)
                    this.pictureBox2.Image = pic.Image as Bitmap;
                this.rightflag = true;
                if (this.leftflag)
                    this.button1.Enabled = true;
                return;
            }

            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Otwieranie";
                    dlg.Filter = "Image files (*.bmp;*.jpg; *.png)|*.bmp;*.jpg;*.png | All files (*.*) | *.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string ext = System.IO.Path.GetExtension(dlg.FileName);
                        if (ext != ".bmp" && ext != ".jpg" && ext != ".png")
                        {
                            MessageBox.Show("Wrong file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.pictureBox2.Image = Properties.Resources.NoImage;
                            this.rightflag = false;
                            return;
                        }

                        this.pictureBox2.Image = new Bitmap(dlg.FileName);
                        this.rightflag = true;
                    }
                    else
                    {
                        this.pictureBox2.Image = Properties.Resources.NoImage;
                        this.rightflag = false;
                        this.button1.Enabled = false;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CheckPressButton();
        }


        private void CheckPressButton()
        {
            if (this.rightflag && this.leftflag)
                this.button1.Enabled = true;
        }
        private void initializeWorkers()
        {
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = false;
            bw2.WorkerReportsProgress = true;
            bw2.WorkerSupportsCancellation = false;

            bw.DoWork += new DoWorkEventHandler(EventDoWork1);
            bw2.DoWork += new DoWorkEventHandler(EventDoWork2);
            bw.ProgressChanged += new ProgressChangedEventHandler(EventProgress1);
            bw2.ProgressChanged += new ProgressChangedEventHandler(EventProgress2);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EventEndWork1);
            bw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EventEndWork2);
        }

        private void EventEndWork1(object sender , RunWorkerCompletedEventArgs e)
        {
            this.button1.Enabled = true;
            this.newBmp = this.bwBmp;
            this.label2.Visible = false;
            this.progressBar1.Visible = false;
            this.progressBar1.Value = 0;
            this.newFileCounter++;
            Form2 form = new Form2(this);
            form.ShowPicture(this.newBmp);
            form.ShowDialog();
        }

        private void EventEndWork2(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button1.Enabled = true;
            this.newBmp = this.bw2Bmp;
            this.label2.Visible = false;
            this.progressBar2.Visible = false;
            this.progressBar2.Value = 0;
            this.newFileCounter++;
            Form2 form = new Form2(this);
            form.ShowPicture(this.newBmp);
            form.ShowDialog();
        }

        private void EventProgress1(object sender , ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void EventProgress2(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar2.Value = e.ProgressPercentage;
        }

        private void EventDoWork1(object sender, DoWorkEventArgs e)
        {
            blend1();
        }

        private void EventDoWork2(object sender, DoWorkEventArgs e)
        {
            blend2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!bw.IsBusy)
            {
                if (bw2.IsBusy)
                    this.button1.Enabled = false;
                this.label2.Visible = true;
                this.progressBar1.Visible = true;
                bw.RunWorkerAsync();
            }
            else if (!bw2.IsBusy)
            {
                if (bw.IsBusy)
                    this.button1.Enabled = false;
                this.label2.Visible = true;
                this.progressBar2.Visible = true;
                bw2.RunWorkerAsync();
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.alpha =((double) ((TrackBar)sender).Value / 10);
        }
    }
}
