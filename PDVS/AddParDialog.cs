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
	public partial class AddParDialog : Gtk.Dialog
	{
		public string sParName; 
		public int    iParId;
		public string sParVal;

		public AddParDialog()
		{
			this.Build ();

			int i = 0;
			foreach (string item in Enum.GetNames(typeof(PGWLib.Enums.E_PWINFO)))
			{
				
				cmbParametros.InsertText(i, item);
				i++;

			}

		}


		protected void OnBtnAdicionaClicked (object sender, EventArgs e)
		{
			//throw new NotImplementedException ();
			string s1 = "";
			string s2 = "";
			int codigo = 0;

			//TransactionStatus = (PGWLib.Enums.E_PWCNF)(-1);
			foreach (PGWLib.Enums.E_PWINFO item in Enum.GetValues(typeof(PGWLib.Enums.E_PWINFO)))
			{
				//i = cbmTranstatus.Model.GetValue (it, 0).
				s1 = cmbParametros.ActiveText;
				s2 = item.ToString();
				codigo = (int)item;

				if(cmbParametros.ActiveText == item.ToString())
				{
					
					sParName = item.ToString(); 
					iParId   = (int)item;
					sParVal  = entryValor.Text.ToString();

					break;
				}


			}

		}
	}
}

