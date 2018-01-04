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
		public (Bitmap bicubic, Bitmap upscaled) Upscale(Image bitmap)
		{
			var orig = new Image<Rgb, byte>((Bitmap)bitmap);

			var upscaled = orig.Clone();
			upscaled = upscaled.Not();
			upscaled = upscaled.Resize(2.0, Emgu.CV.CvEnum.Inter.Cubic);

			return (orig.Resize(2.0, Emgu.CV.CvEnum.Inter.Cubic).ToBitmap(), upscaled.ToBitmap());
		}
	}
}
