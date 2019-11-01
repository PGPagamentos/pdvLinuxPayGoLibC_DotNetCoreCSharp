using System;
using Gtk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static PDVS.ConfirmationWindow;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;

namespace PDVS
{
	public partial class ConfirmationWindowDialog : Gtk.Dialog
	{
		public PGWLib.Enums.E_PWCNF TransactionStatus;
		public ConfirmationWindowDialog ()
		{
			this.Build();
			int i = 0;
			foreach (string item in Enum.GetNames(typeof(PGWLib.Enums.E_PWCNF)))
			{
				cmbStTrans.InsertText(i, item);
				i++;

			}
		}

		protected void OnBtnAplicarClicked (object sender, EventArgs e)
		{
			//throw new NotImplementedException ();
			string s1 = "";
			string s2 = "";

			TransactionStatus = (PGWLib.Enums.E_PWCNF)(-1);
			foreach (PGWLib.Enums.E_PWCNF item in Enum.GetValues(typeof(PGWLib.Enums.E_PWCNF)))
			{
				//i = cbmTranstatus.Model.GetValue (it, 0).
				s1 = cmbStTrans.ActiveText;
				s2 = item.ToString();
				if(cmbStTrans.ActiveText == item.ToString())
				{
					TransactionStatus = item;
					break;
				}


			}



		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{

		}
	}
}

