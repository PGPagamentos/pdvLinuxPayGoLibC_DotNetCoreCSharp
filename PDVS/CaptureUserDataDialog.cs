using System;
using Gtk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;


namespace PDVS
{
	// janela para captura de dados do pinpad
	public partial class CaptureUserDataDialog : Gtk.Dialog
	{
		public int Min;
		public int Max;
		public PGWLib.Enums.E_PWUserDataMessages Captura;

		public CaptureUserDataDialog ()
		{
			this.Build ();


			int i = 0;



			foreach (string item in Enum.GetNames(typeof(PGWLib.Enums.E_PWUserDataMessages)))
			{

				cmbCaptura.InsertText(i, item);
				i++;

			}


		}

		// ativa a captura de dados selecionados no combo box
		protected void OnBtnCapturaClicked (object sender, EventArgs e)
		{

			//throw new NotImplementedException ();
			string s1 = "";
			string s2 = "";
			int codigo = 0;

			//TransactionStatus = (PGWLib.Enums.E_PWCNF)(-1);

			foreach (PGWLib.Enums.E_PWUserDataMessages item in Enum.GetValues(typeof(PGWLib.Enums.E_PWUserDataMessages)))
			{
				//i = cbmTranstatus.Model.GetValue (it, 0).
				s1 = cmbCaptura.ActiveText;
				s2 = item.ToString();
				codigo = (int)item;

				if(cmbCaptura.ActiveText == item.ToString())
				{

					Captura = item; 
					Min    = System.Convert.ToInt32(entryMin.Text.ToString());
					Max    = System.Convert.ToInt32(entryMax.Text.ToString());

					break;
				}
					
			}
			
		}
	}
}

