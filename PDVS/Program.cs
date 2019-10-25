using System;
using Gtk;

namespace PDVS
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			try
			{
			  Application.Run ();
			}
			catch(Exception ex)
			{
				
			}
		}
	}
}
