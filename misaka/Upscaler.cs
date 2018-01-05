using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;

namespace misaka
{
	class Upscaler
	{
		private int kernelSize = 10;
		private float weightFactor = 1.0f;

		public (Bitmap bicubic, Bitmap upscaled) Upscale(Image bitmap)
		{
			var orig = new Image<Rgb, byte>((Bitmap)bitmap);

			var upscaled = orig.Clone();
			//upscaled = upscaled.Not();
			upscaled = upscaled.Resize(2.0, Emgu.CV.CvEnum.Inter.Cubic);

			return (orig.Resize(2.0, Emgu.CV.CvEnum.Inter.Cubic).ToBitmap(), upscaled.ToBitmap());
		}

		public Image<Rgb, float> GetMagnetMagic(Image<Rgb, byte> input)
		{
			Image<Rgb, float> result = new Image<Rgb, float>(input.Size);

			byte[,,] data = input.Data;
			float[,,] resultData = result.Data;

			float globalMax = float.MinValue;
			float globalMin = float.MaxValue;

			for (int ax = 0; ax < input.Width; ax++)
			{
				for (int ay = 0; ay < input.Height; ay++)
				{
					int srcPixel = data[ay, ax, 0] + data[ay, ax, 1] + data[ay, ax, 2];

					if (srcPixel == 0)
						continue;

					float vx = 0;
					float vy = 0;

					int count = 0;

					for (int x = -kernelSize; x < kernelSize; x++)
					{
						for (int y = -kernelSize; y < kernelSize; y++)
						{
							if (x == 0 && y == 0)
								continue;

							int nx = ax + x;
							int ny = ay + y;

							if (nx < 0 || nx >= input.Width || ny < 0 || ny >= input.Height)
								continue;

							int targetPix = data[ny, nx, 0] + data[ny, nx, 1] + data[ny, nx, 2];
							float dx = x < 0 ? -kernelSize - x : kernelSize - x;
							float dy = y < 0 ? -kernelSize - y : kernelSize - y;

							//TODO: implement weight factor and falloff
							float magnitudeMultiplier = Math.Abs((targetPix - srcPixel) / 765f);

							dx *= magnitudeMultiplier;
							dy *= magnitudeMultiplier;

							vx += dx;
							vy += dy;

							count++;
						}
					}

					if (count == 0)
						continue;

					vx /= count;
					vy /= count;
					resultData[ay, ax, 0] = vx;
					resultData[ay, ax, 1] = vy;

					if (srcPixel < 384 && !((ax == 0 && ay == 0) || (ax == 0 && ay == 1)))
						resultData[ay, ax, 2] = 1;

					float magnitude = vx * vx + vy * vy;
					if (magnitude > globalMax)
						globalMax = magnitude;
					if (magnitude < globalMin)
						globalMin = magnitude;
				}
			}

			resultData[0, 0, 2] = (float)(Math.Sqrt(globalMax));
			resultData[0, 1, 2] = (float)(Math.Sqrt(globalMin));
			return result;
		}
	}
}
