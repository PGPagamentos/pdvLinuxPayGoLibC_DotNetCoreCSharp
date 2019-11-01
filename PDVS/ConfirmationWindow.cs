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
	public partial class ConfirmationWindow : Gtk.Window
	{
		public PGWLib.Enums.E_PWCNF TransactionStatus;

		public ConfirmationWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			int i = 0;
			foreach (string item in Enum.GetNames(typeof(PGWLib.Enums.E_PWCNF)))
			{
				cbmTranstatus.InsertText(i, item);
				i++;

			}
			//cmbTransactionStatuses.SelectedIndex = 0;

		}

	    protected void Show(ref PGWLib.Enums.E_PWCNF transactionStatus)
		{
			this.Show();
			//cmbTransactionStatuses.
			//cbmTransactionStatuses

			transactionStatus = (PGWLib.Enums.E_PWCNF)5;
			//transactionStatus = (PGWLib.Enums.E_PWCNF)Enum.Parse(typeof(PGWLib.Enums.E_PWCNF), cmbtransactionStatuses.SelectedItem.ToString());
		}

		protected void OnBtnApplyClicked (object sender, EventArgs e)
		{
			string s1 = "";
			string s2 = "";

			TransactionStatus = (PGWLib.Enums.E_PWCNF)(-1);
			foreach (PGWLib.Enums.E_PWCNF item in Enum.GetValues(typeof(PGWLib.Enums.E_PWCNF)))
			{
				//i = cbmTranstatus.Model.GetValue (it, 0).
				s1 = cbmTranstatus.ActiveText;
				s2 = item.ToString();
				if(cbmTranstatus.ActiveText == item.ToString())
				{
					TransactionStatus = item;
					break;
				}


			}

		}

		protected void OnCbmTransactionStatusesSelectionReceived (object o, Gtk.SelectionReceivedArgs args)
		{
			//throw new NotImplementedException ();
			//args.SelectionData.
		}

		protected void OnCbmTranstatusSelectionGet (object o, SelectionGetArgs args)
		{
			//throw new NotImplementedException ();
		}
	}
}

