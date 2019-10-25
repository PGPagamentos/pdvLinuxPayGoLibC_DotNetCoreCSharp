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


public partial class MainWindow: Gtk.Window
{

	PGWLib.PGWLib eft;
	Gtk.ListStore DllLogListStore = new Gtk.ListStore (typeof (string));
	List<PGWLib.CustomObjects.PW_Parameter> listaParametros = new List<PGWLib.CustomObjects.PW_Parameter>();

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		InicializaLogDll();

		eft = new PGWLib.PGWLib();
		int ret;
		ret  = eft.Init();

		if (ret == 0) 
		{
			WriteLog ("----------------");
			WriteLog ("PW_INIT OK");
		}
		else 
		{
			WriteLog ("----------------");
			WriteLog ("PW_INIT ERROR");
		}

		//LogaTransactionResult();

		int i = 0;
		foreach (string item in Enum.GetNames(typeof(PGWLib.Enums.E_PWOPER)))
		{
			//cmbOper.Items.Add(item);
			cmbOper.InsertText(i,item);
			i++;
		}
		addMandatoryParameters();



	}

	// incializa treeview log dll
	public void InicializaLogDll()
	{

		//Gtk.ListStore parameterListStore = new Gtk.ListStore (typeof (string));

		//pathParameter = new TreePath();
		// Create a column for the song title
		Gtk.TreeViewColumn parameterColumn = new Gtk.TreeViewColumn ();
		parameterColumn.Title = "Parameters";


		// Create the text cell that will display the artist name
		Gtk.CellRendererText parameterNameCell = new Gtk.CellRendererText ();

		// Add the cell to the column
		parameterColumn.PackStart (parameterNameCell, true);


		// Tell the Cell Renderers which items in the model to display
		parameterColumn.AddAttribute (parameterNameCell, "text", 0);


		// Add the columns to the TreeView
		treeviewDllLog.AppendColumn (parameterColumn);



		// Create a model that will hold two strings - Artist Name and Song Title
		//Gtk.ListStore parameterListStore = new Gtk.ListStore (typeof (string));

		// Assign the model to the TreeView
		treeviewDllLog.Model =  DllLogListStore;
			
	}


	private void addMandatoryParameters()
	{

		listaParametros.Add(new PGWLib.CustomObjects.PW_Parameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTNAME.ToString(), (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTNAME, "PDVS"));
		listaParametros.Add(new PGWLib.CustomObjects.PW_Parameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTVER.ToString(),  (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTVER, "1.0"));
		listaParametros.Add(new PGWLib.CustomObjects.PW_Parameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTDEV.ToString(),  (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTDEV, "NTK Solutions Ltda"));
		listaParametros.Add(new PGWLib.CustomObjects.PW_Parameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTCAP.ToString(),  (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTCAP, "28"));  


	}
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButtonExecutarClicked (object sender, EventArgs e)
	{
		throw new NotImplementedException ();
	}

	protected void OnCmbOperChanged (object sender, EventArgs e)
	{
		throw new NotImplementedException ();
	}

	/// instala o PDC
	protected void OnButtonInstalaClicked (object sender, EventArgs e)
	{
		executeInstalation();
				
	}

	// metodo que excuta a instalação do PDC
	private int executeInstalation()
	{


		PGWLib.Enums.E_PWOPER operation = PGWLib.Enums.E_PWOPER.PWOPER_INSTALL; // operacao de instalação

		//PGWLib.PGWLib eft = new PGWLib.PGWLib();
		//eft.Init();

		//getTransactionResult();
		int ret = eft.startTransaction(operation, listaParametros);

			if (ret != 0) 
			{
				string sError = ((PGWLib.Enums.E_PWRET)ret).ToString ();
				int iError = ret;

				WriteLog ("----------------");
				WriteLog ("Instalacao de PDC Erro :" + sError);

				// verifica se deu erro de transacao anterior pendente
				if (ret == (int)PGWLib.Enums.E_PWRET.PWRET_FROMHOSTPENDTRN) 
				{
					// confirma a transacao que estava pendente

					PGWLib.Enums.E_PWCNF transactionStatus = PGWLib.Enums.E_PWCNF.PWCNF_REV_AUTO_ABORT;
					ret = eft.confirmPendTransaction (transactionStatus, getTransactionResult ());
					WriteLog("----------------");
					WriteLog(string.Format("Erro ao executar a transação: {0}{1}{2}{3}{4}", iError, Environment.NewLine, sError, Environment.NewLine));
				} 
				else 
				{
					WriteLog(string.Format("Erro ao executar a transação: {0}{1}{2}", iError, Environment.NewLine, sError));                
				}
			} 
			else 
			{
				WriteLog ("----------------");
				WriteLog ("Instalacao de PDC OK");

			}
		return ret;
	}

	
	public List<PW_Parameter> getTransactionResult()
	{
		List<PW_Parameter> ret = new List<PW_Parameter>();

		ret = eft.getTransactionResult();

		//ResultWindow rw = new ResultWindow(ret);
		//rw.Show();
		//LogDLL(ret);
		return ret;

	}

	public void  LogaTransactionResult()
	{
		List<PW_Parameter> ret = new List<PW_Parameter>();

		ret = eft.getTransactionResult();

		//ResultWindow rw = new ResultWindow(ret);
		//rw.Show();
		LogDLL(ret);
		

	}


	public void  WriteLog(string sMessage)
	{
		DllLogListStore.AppendValues(sMessage);	
	}

	/////////////////////////////////////////////////////
	// Método utlizado para logar o retorno da execução 
	// das funções da DLL Pay Go
	public void LogDLL(List<PW_Parameter> result)
	{
		//InitializeComponent();

		foreach (PW_Parameter item in result)
		{

			try
			{
				// se é recibo quebra linha por linha e insere no listbox
				// para resolver bug de scroll dos recibos.
				if ((item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTCHOLDER) ||
					(item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTCHSHORT) ||
					(item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTFULL) ||
					(item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTMERCH) ||
					(item.parameterCode == (ushort)E_PWINFO.PWINFO_RCPTPRN)
				)
				{


					string _input = item.ToString(); 
					////////////////////////////
					using (StringReader reader = new StringReader(_input))
					{
						// Loop over the lines in the string.
						//int count = 0;
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							//count++;
							//Console.WriteLine("Line {0}: {1}", count, line);
							//listBox.Items.Add(line);
							DllLogListStore.AppendValues(line);
						}
					}

					//////////////////////////////
				}
				else
				{
					DllLogListStore.AppendValues(item.ToString());
				}
			}
			catch(Exception ex)
			{
				DllLogListStore.AppendValues(ex.Message.ToString());
			}
			
		}
	}


		protected void OnButtonLimpaLogClicked (object sender, EventArgs e)
		{
			DllLogListStore.Clear();
		}
}

}





