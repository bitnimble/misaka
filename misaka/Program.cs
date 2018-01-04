using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace misaka
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			string opencvPath = @"E:\Emgu\emgucv-windesktop 3.3.0.2824\libs\x86";
			string path = Environment.GetEnvironmentVariable("PATH");
			Environment.SetEnvironmentVariable("PATH", path + ";" + opencvPath);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
