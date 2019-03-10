using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace Steganografia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                ImageHolder image = new ImageHolder(path);
                pictureBox1.Image = image.getImage();
            }
            Console.WriteLine(result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
           pictureBox2.Image = DataHide.hideInformation(new Bitmap(pictureBox1.Image), textBox1.Text,textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox2.Image.Save("a.bmp");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = DataHide.showInformation(pictureBox1.Image, textBox3.Text, textBox2.Text);
        }
    }
}
