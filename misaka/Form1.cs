using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace misaka
{
	public partial class Form1 : Form
	{
		Bitmap bicubic;
		Bitmap upscaled;
		Image original;

		public Form1()
		{
			InitializeComponent();

			var upscaler = new Upscaler();

			original = Image.FromFile(@"..\..\..\Resources\test1.png");
			(bicubic, upscaled) = upscaler.Upscale(original);
			pictureBox1.Image = bicubic;

			pictureBox1.MouseWheel += PictureBox1_MouseWheel;
		}

		private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
		{
			int clicks = e.Delta / 120;
			pictureBox1.AddZoomLevel(clicks);
			pictureBox1.Invalidate();
		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			pictureBox1.Image = upscaled;
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			if (!menuItem2.Checked)
				pictureBox1.Image = bicubic;
			else
				pictureBox1.Image = original;
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			menuItem2.Checked = !menuItem2.Checked;

			if (menuItem2.Checked)
			{
				pictureBox1.Image = original;
			}
			else
			{
				pictureBox1.Image = bicubic;
			}
		}
	}
}
