using Emgu.CV;
using Emgu.CV.Structure;
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
		private const float zoomMultiplier = 1.5f;
		private float[] zoomLevels = new float[zoomSteps + 1];

		private bool mouseDown = false;
		private Point lastPos = Point.Empty;

		private Image<Rgb, float> vectorMap;

		private Pen whitePen = new Pen(Color.White, 2);
		private Pen blackPen = new Pen(Color.Black, 2);

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

		public void SetVectorMap(Image<Rgb, float> vectorMap)
		{
			this.vectorMap = vectorMap;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			pe.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

			float zoom = zoomLevels[zoomLevel];

			if (Image != null)
				pe.Graphics.TranslateTransform(((Width / 2) - (Image.Width * zoom / 2)), (Height / 2) - (Image.Height * zoom / 2));
			pe.Graphics.TranslateTransform(x, y);
			pe.Graphics.ScaleTransform(zoom, zoom);

			base.OnPaint(pe);

			pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			pe.Graphics.ResetTransform();
			if (Image != null)
				pe.Graphics.TranslateTransform(((Width / 2) - (Image.Width * zoom / 2)), (Height / 2) - (Image.Height * zoom / 2));
			pe.Graphics.TranslateTransform(x, y);

			if (vectorMap != null)
			{
				float[,,] vectorData = vectorMap.Data;
				float max = vectorData[0, 0, 2];
				float min = vectorData[0, 1, 2];

				float cOffsetX = (0.95f * zoom / 2);
				float cOffsetY = (0.95f * zoom / 2);

				//Arrow is max 0.95 of the width/height of the pixel
				float maxLength = 0.95f * zoom;

				for (int x = 0; x < vectorMap.Width; x++)
				{
					for (int y = 0; y < vectorMap.Height; y++)
					{
						float vx = vectorData[y, x, 0];
						float vy = vectorData[y, x, 1];

						float magnitudeMult = maxLength / max;

						float nvx = vx * magnitudeMult;
						float nvy = vy * magnitudeMult;

						float startX = (x * zoom) + cOffsetX - nvx / 2;
						float startY = (y * zoom) + cOffsetY - nvy / 2;

						float destX = startX + nvx / 2;
						float destY = startY + nvy / 2;

						bool useWhite = vectorData[y, x, 2] == 0 ? false : true;
						Pen pen = useWhite ? whitePen : blackPen;
						Brush brush = useWhite ? Brushes.White : Brushes.Black;
						pe.Graphics.DrawLine(pen, startX, startY, destX, destY);
						pe.Graphics.DrawRectangle(Pens.Blue, startX - 1, startY + 1, 1, 1);
						pe.Graphics.DrawRectangle(Pens.Red, x * zoom, y * zoom, zoom, zoom);
					}
				}
			}
		}
	}
}
