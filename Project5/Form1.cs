using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Project5
{
    public partial class Form1 : Form
    {
        private Bitmap map; // bitmap containing picture to reduce
        private int MaxColors; // maximal number of colors - in the end of reduction could be less
        public Form1()
        {
            InitializeComponent();
            MaxColors = (int)Math.Pow(2,trackBar2.Value);
            map = null;
        }
        //--------------------------------------------------------
        // function to choose picture and show it in the Picture screen
        private void ChooseFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "JPG files (*.jpg) | *.jpg";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (Stream BitmapStream = System.IO.File.Open(dialog.FileName, System.IO.FileMode.Open))
                    {
                        Image img = Image.FromStream(BitmapStream);
                        map = new Bitmap(img);
                        Picture.Image = new Bitmap(map, new Size(Picture.Width, Picture.Height));
                    }
                }
            }
        }
        //--------------------------------------------------------
        // function when number of maximal amount of color has changed
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            MaxColors = (int)Math.Pow(2, trackBar2.Value);
            MaxOfColLabel.Text = "Maximum of colors: " + MaxColors;
        }
        //--------------------------------------------------------
        // function which start reduction of colors
        private void Load_Click(object sender, EventArgs e)
        {
            // if there is no picture added
            if (map == null)
            {
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show("First add picture to reduce.", "Missing file", buttons);
            }
            else
            {
                progressBar1.Value = 0;
                progressBar2.Value = 0;
                AfterReductionPicture.Image = null;
                AlongReductionPicture.Image = null;
                AlongReductionPicture.Refresh();
                AfterReductionPicture.Refresh();
                Reduce();
            }
        }
        private void Reduce()
        {
            Bitmap reduced;

            reduced = new Bitmap(map.Width, map.Height);
            CreateOctree CO = new CreateOctree(map, MaxColors, progressBar1);           
            CO.ReducePicture(ref reduced);
            AfterReductionPicture.Image = new Bitmap(reduced, new Size(AfterReductionPicture.Width, AfterReductionPicture.Height));
            
            AfterReductionPicture.Refresh();
            
            reduced = new Bitmap(map.Width, map.Height);
            CreateOctreeSecondMethod COSM = new CreateOctreeSecondMethod(map, MaxColors, progressBar2);
            COSM.ReducePicture(ref reduced);
            AlongReductionPicture.Image = new Bitmap(reduced, new Size(AlongReductionPicture.Width, AlongReductionPicture.Height));
            
            }
    }
}
