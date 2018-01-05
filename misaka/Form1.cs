using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace misaka
{
	public partial class Form1 : Form
	{
		Bitmap bicubicBitmap;
		Bitmap upscaledBitmap;

		public Form1()
		{
			InitializeComponent();

			pictureBox1.MouseWheel += PictureBox1_MouseWheel;

			LoadImage(@"..\..\..\Resources\test1.png");
		}

		private void LoadImage(string filename)
		{

			var originalBitmap = Image.FromFile(filename);

			var original = new Image<Rgb, byte>((Bitmap)originalBitmap);
			var bicubic = original.Resize(2.0, Emgu.CV.CvEnum.Inter.Cubic);
			bicubicBitmap = bicubic.ToBitmap();

			var upscaler = new Upscaler();

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Image<Rgb, float> vectorSpace = upscaler.GetMagnetMagic(bicubic);
			var upscaled = upscaler.Upscale(bicubic, vectorSpace);

			long time = stopwatch.ElapsedMilliseconds;
			statusLabel1.Text = "Upscaling took " + time + "ms";

			upscaledBitmap = upscaled.ToBitmap();

			pictureBox1.Image = bicubicBitmap;
			pictureBox1.SetVectorMap(vectorSpace);
		}

		private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
		{
			int clicks = e.Delta / 120;
			pictureBox1.AddZoomLevel(clicks);
			pictureBox1.Invalidate();
		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			pictureBox1.Image = bicubicBitmap;
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			pictureBox1.Image = upscaledBitmap;
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			menuItem2.Checked = !menuItem2.Checked;

			if (menuItem2.Checked)
			{
				pictureBox1.RenderVectors = true;
			}
			else
			{
				pictureBox1.RenderVectors = false;
			}

			pictureBox1.Invalidate();
		}

		private void menuItem4_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				LoadImage(ofd.FileName);
			}
		}
	}
}
