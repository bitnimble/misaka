using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace misaka
{
	class NearestNeighbourPictureBox : PictureBox
	{
		public int x = 0;
		public int y = 0;
		private int zoomLevel = 10;

		private const int zoomSteps = 20; //In either direction
		private const float zoomMultiplier = 1.3f;
		private float[] zoomLevels = new float[zoomSteps + 1];

		private bool mouseDown = false;
		private Point lastPos = Point.Empty;

		public NearestNeighbourPictureBox()
		{
			zoomLevels[zoomSteps / 2] = 1.0f;
			for (int i = 0; i < zoomSteps / 2; i++)
			{
				zoomLevels[(zoomSteps / 2) - 1 - i] = zoomLevels[(zoomSteps / 2) - i] / zoomMultiplier;
				zoomLevels[(zoomSteps / 2) + 1 + i] = zoomLevels[(zoomSteps / 2) + i] * zoomMultiplier;
			}

			MouseDown += NearestNeighbourPictureBox_MouseDown;
			MouseUp += NearestNeighbourPictureBox_MouseUp;
			MouseMove += NearestNeighbourPictureBox_MouseMove;
		}

		private void NearestNeighbourPictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseDown)
			{
				x += e.X - lastPos.X;
				y += e.Y - lastPos.Y;

				lastPos = e.Location;
				this.Invalidate();
			}
		}

		private void NearestNeighbourPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			mouseDown = false;
			Cursor.Current = Cursors.Default;
		}

		private void NearestNeighbourPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			mouseDown = true;
			lastPos = e.Location;
			Cursor.Current = Cursors.SizeAll;
		}

		private Point GetCursorPixel()
		{
			float zoom = zoomLevels[zoomLevel];
			float centerTransX = (Width / 2) - (Image.Width * zoom / 2);
			float centerTransY = (Height / 2) - (Image.Height * zoom / 2);
			return new Point((int)((PointToClient(Cursor.Position).X - x - centerTransX) / zoom), (int)((PointToClient(Cursor.Position).Y - y - centerTransY) / zoom));
		}

		public void AddZoomLevel(int delta)
		{
			var oldHover = GetCursorPixel();

			zoomLevel += delta;

			if (zoomLevel > zoomSteps)
				zoomLevel = zoomSteps;
			if (zoomLevel < 0)
				zoomLevel = 0;

			var newHover = GetCursorPixel();

			float zoom = zoomLevels[zoomLevel];
			x += (int)((newHover.X - oldHover.X) * zoom);
			y += (int)((newHover.Y - oldHover.Y) * zoom);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

			float zoom = zoomLevels[zoomLevel];

			if (Image != null)
				pe.Graphics.TranslateTransform(((Width / 2) - (Image.Width * zoom / 2)), (Height / 2) - (Image.Height * zoom / 2));
			pe.Graphics.TranslateTransform(x, y);
			pe.Graphics.ScaleTransform(zoom, zoom);

			base.OnPaint(pe);
		}
	}
}
