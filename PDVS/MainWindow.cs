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

		Gtk.ListStore ParametersListStore = new Gtk.ListStore (typeof (string));
		List<PGWLib.CustomObjects.PW_Parameter> listaParametros = new List<PGWLib.CustomObjects.PW_Parameter>();

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build();

			InicializaLogDll();
			InicializaParameters();

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

			LogaTransactionResult();

			int i = 0;
			foreach (string item in Enum.GetNames(typeof(PGWLib.Enums.E_PWOPER)))
			{
				//cmbOper.Items.Add(item);
				cmbOper.InsertText(i,item);
				i++;
			}
			addMandatoryParameters();




		}

		
		////////////////
		// incializa treeview log dll
		public void InicializaLogDll()
		{

			// cria a coluna associada a treeview
			Gtk.TreeViewColumn DllLogColumn = new Gtk.TreeViewColumn ();
		    DllLogColumn.Title = "Log";


			// cria a célula de texto que ira armazenar a informacao
			Gtk.CellRendererText DllLogNameCell = new Gtk.CellRendererText ();

			// Adiciona a celula a coluna
			DllLogColumn.PackStart(DllLogNameCell, true);


			// Indica a  celula o tipo de dado que será mostrado
			DllLogColumn.AddAttribute(DllLogNameCell, "text", 0);


			// adiciona a  coluna a  TreeView
			treeviewDllLog.AppendColumn(DllLogColumn);
			
			// associa o model a treeview
			treeviewDllLog.Model =  DllLogListStore;
				
		}


		////////////////
		// incializa treeview de parametros
		public void InicializaParameters()
		{

			// cria a coluna associada a treeview
			Gtk.TreeViewColumn ParametersColumn = new Gtk.TreeViewColumn ();
			ParametersColumn.Title = "Parameters";


			// cria a célula de texto que ira armazenar a informacao
			Gtk.CellRendererText ParametersNameCell = new Gtk.CellRendererText ();

			// Adiciona a celula a coluna
			ParametersColumn.PackStart(ParametersNameCell, true);


			// Indica a  celula o tipo de dado que será mostrado
			ParametersColumn.AddAttribute(ParametersNameCell, "text", 0);


			// adiciona a  coluna a  TreeView
			trvPar.AppendColumn(ParametersColumn);

			// associa o model a treeview
			trvPar.Model =  ParametersListStore;

		}

		// adiciona os parametros obrigatorios
		private void addMandatoryParameters()
		{

			addParameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTNAME.ToString(), (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTNAME, "PDVS");
			addParameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTVER.ToString(),  (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTVER, "1.0");
			addParameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTDEV.ToString(),  (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTDEV, "NTK Solutions Ltda");
			addParameter(PGWLib.Enums.E_PWINFO.PWINFO_AUTCAP.ToString(),  (int)PGWLib.Enums.E_PWINFO.PWINFO_AUTCAP, "28");  


		}


		// metodo para adicao de parametros
		private void addParameter(string sParName, int iParId,string sParVal)
		{
			PGWLib.CustomObjects.PW_Parameter par = new PGWLib.CustomObjects.PW_Parameter (sParName, (ushort)iParId, sParVal);
			listaParametros.Add(par);
			ParametersListStore.AppendValues(par.ToString());	

		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		// executa a transacao selecionada no combo box
		protected void OnButtonExecutarClicked (object sender, EventArgs e)
		{
			int ret = executeTransaction();
			confirmUndoTransaction(getTransactionResult(),ret);

		}

		// executa transação selecionada
		private int executeTransaction()
		{
			PGWLib.Enums.E_PWOPER operation = (PGWLib.Enums.E_PWOPER)(-1);
	        
		    ///////////////////////////////////////
			// descobre quem foi selecionado
			
			string s1 = "";
			string s2 = "";

			
			foreach (PGWLib.Enums.E_PWOPER item in Enum.GetValues(typeof(PGWLib.Enums.E_PWOPER)))
			{

				s1 = cmbOper.ActiveText;
				s2 = item.ToString();
				
				if(cmbOper.ActiveText == item.ToString())
				{
					operation = item;
					break;
				}


			}

			WriteLog ("\n----------------");
			WriteLog ("executeTransaction : " + operation.ToString() + "\n");


	        /////////////////////////////////


			PGWLib.PGWLib eft = new PGWLib.PGWLib();
			int ret = eft.startTransaction(operation, listaParametros);
			
			if (ret != 0)
			{
				string sError          = ((PGWLib.Enums.E_PWRET)ret).ToString();
				string sResultMessage  = eft.getResultMessage();

				// verifica se deu erro de transacao anterior pendente
				if (ret == (int)PGWLib.Enums.E_PWRET.PWRET_FROMHOSTPENDTRN)
				{
					// confirma a transacao que estava pendente
					WriteLog ("----------------");
					WriteLog ("Erro :" + sError + ": result message :" + sResultMessage + " ao executar a transação: " + operation.ToString());

					PGWLib.Enums.E_PWCNF transactionStatus = PGWLib.Enums.E_PWCNF.PWCNF_REV_AUTO_ABORT;
					ret = eft.confirmPendTransaction(transactionStatus, getTransactionResult());

					WriteLog ("----------------");

					string sTranpend = "Erro :" + sError + "-> Confirmando transacao pendente" ;

					WriteLog (sTranpend);

					ShowMessageBoxInfo(this, sTranpend);


					LogaTransactionResult();
					return ret;
				}
				else
				{
					WriteLog ("----------------");
					string sErrorMessage = "Erro :" + sError + " : result message : " + sResultMessage + " ao executar a transação: " + operation.ToString ();

					WriteLog(sErrorMessage);

					ShowMessageBoxError(this, sErrorMessage);
					LogaTransactionResult();
					return ret;
				}
			}

			WriteLog ("----------------");
			string sOKMessage = "Transacao : " + operation.ToString() + " OK"; 
			WriteLog (sOKMessage + "\n");

			ShowMessageBoxOK(this, sOKMessage);
			//ShowMessageBoxInfo(this,   "INFO  SE VIS PACEM PARA BELLUM");
			//ShowMessageBoxError(this,  "Error SE VIS PACEM PARA BELLUM");
			LogaTransactionResult();

			return ret;

		
		}

		// confirma transacaoes
		private int confirmUndoTransaction(List<PGWLib.CustomObjects.PW_Parameter> transactionResult,int RetTransaction)
		{
			int ret = 0;

			WriteLog ("----------------");
			WriteLog ("confirmUndoTransaction ");




			foreach (PGWLib.CustomObjects.PW_Parameter item in transactionResult)
			{
				if(item.parameterCode == (uint)PGWLib.Enums.E_PWINFO.PWINFO_CNFREQ & item.parameterValue == "1")
				{
					if (RetTransaction == (int)PGWLib.Enums.E_PWRET.PWRET_FROMHOSTPENDTRN)
					{
						PGWLib.Enums.E_PWCNF transactionStatus = PGWLib.Enums.E_PWCNF.PWCNF_REV_AUTO_ABORT;
						ret = eft.confirmPendTransaction(transactionStatus, transactionResult);
					}
					else
					{
						ConfirmationWindowDialog cw = new ConfirmationWindowDialog();
						PGWLib.Enums.E_PWCNF transactionStatus = PGWLib.Enums.E_PWCNF.PWCNF_REV_AUTO_ABORT;
						
						cw.Run();
						transactionStatus = cw.TransactionStatus;
						cw.Destroy();
											
						ret = eft.confirmUndoTransaction(transactionStatus, transactionResult);
					}
				}
			}

			WriteLog ("----------------");
			WriteLog ("Transacao OK");
			//LogaTransactionResult();
			return ret;

		}



		protected void OnCmbOperChanged (object sender, EventArgs e)
		{

		}

		/// Inicia a instalacao  do PDC
		protected void OnButtonInstalaClicked (object sender, EventArgs e)
		{
			executeInstalation();
		}
				
		// metodo que excuta a instalação do PDC
		private int executeInstalation()
		{

			PGWLib.Enums.E_PWOPER operation = PGWLib.Enums.E_PWOPER.PWOPER_INSTALL; // operacao de instalação

			WriteLog ("----------------");
			WriteLog ("Inicia Instalacao de PDC");

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
					LogaTransactionResult();
				}
			} 
			else 
			{
				WriteLog ("----------------");
				WriteLog ("Instalacao de PDC OK");
				LogaTransactionResult();

			}
			return ret;
		}


		// retorna lista com resultado da transação e loga as informacoes
		public List<PW_Parameter> getTransactionResult()
		{
			List<PW_Parameter> ret = new List<PW_Parameter>();

			ret = eft.getTransactionResult();

			LogDLL(ret);
			return ret;

		}

		// loga as informacaoes de retorno da ultima transacao
		public void  LogaTransactionResult()
		{
			List<PW_Parameter> ret = new List<PW_Parameter>();

			ret = eft.getTransactionResult();

			LogDLL(ret);
			

		}

		// escreve no log
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
							string line;
							while ((line = reader.ReadLine()) != null)
							{
				
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


		// metodo para limpar LOG
		protected void OnButtonLimpaLogClicked (object sender, EventArgs e)
		{
			DllLogListStore.Clear();
		}

		protected void OnTrvParSelectCursorRow (object o, SelectCursorRowArgs args)
		{
		//	throw new NotImplementedException ();
		}


		// metodo que adiciona parametros na listbox e na lista de parametros
		protected void OnBtnAdicionarClicked (object sender, EventArgs e)
		{

			AddParDialog cw = new AddParDialog();


			cw.Run();

			string sParName = "";
			int    iParId   = 0 ;
			string sParVal  = "";

			sParName = cw.sParName;
			iParId   = cw.iParId;
			sParVal  = cw.sParVal;

			cw.Destroy();

			foreach (PGWLib.CustomObjects.PW_Parameter item in listaParametros)
			{
				if (item.parameterName == sParName)
				{
					ShowMessageBoxError(this,"Você não pode inserir duas vezes o mesmo parâmetro");
					return;
				}
			}



			addParameter(sParName, iParId, sParVal);




		}

		// remove parametros do listbox e da lista de parametros a serem enviados
		protected void OnBtnRemoverClicked (object sender, EventArgs e)
		{
			TreeSelection my_selected_row = trvPar.Selection;
			TreeModel my_model;
			TreeIter my_iterator;
			string selected = "";
			//int pos = -1;
			if(my_selected_row.GetSelected(out my_model, out my_iterator))
			{

				selected = ParametersListStore.GetValue(my_iterator, 0).ToString ();
				TreePath [] pos = trvPar.Selection.GetSelectedRows();
				int index = pos[0].Indices[0];
				ParametersListStore.Remove(ref my_iterator);

				PGWLib.CustomObjects.PW_Parameter obj;
				obj = listaParametros[index];
				listaParametros.Remove(obj);
			}
		}

		// messagebox para informar erro
		void ShowMessageBoxError(Window parent,string message)
		{

			Gtk.MessageDialog md = new Gtk.MessageDialog (null, Gtk.DialogFlags.Modal, 
				                       Gtk.MessageType.Error, Gtk.ButtonsType.Close,
				                       message);
			
			md.Run ();
			md.Destroy ();
			md.Dispose ();
		}

		// messagebox para fornecer informacao
		void ShowMessageBoxInfo(Window parent,string message)
		{

			Gtk.MessageDialog md = new Gtk.MessageDialog (null, Gtk.DialogFlags.Modal, 
									   Gtk.MessageType.Warning, Gtk.ButtonsType.Close,
				                       message);

			md.Run ();
			md.Destroy ();
			md.Dispose ();
		}

		// messagebox para noticiar tansacao OK
		void ShowMessageBoxOK(Window parent,string message)
		{

			Gtk.MessageDialog md = new Gtk.MessageDialog (null, Gtk.DialogFlags.Modal, 
				                       Gtk.MessageType.Info, Gtk.ButtonsType.Ok,
				                       message);

			md.Run ();
			md.Destroy ();
			md.Dispose ();
		}
	
		// ativa a captura de dados pelo pinpad
		protected void OnBtnCapturaClicked (object sender, EventArgs e)
		{
			string param = "";
			int minLength = 0;
			int maxLength = 0;
			PGWLib.Enums.E_PWUserDataMessages message;


			WriteLog ("----------------");
			WriteLog ("Captura Dados do Pinpad \n");


			CaptureUserDataDialog selectUserDataWindow = new CaptureUserDataDialog();

			selectUserDataWindow.Run();

			minLength = selectUserDataWindow.Min;
			maxLength = selectUserDataWindow.Max;
		    message   = selectUserDataWindow.Captura;

			selectUserDataWindow.Destroy();


			if (minLength > 0)
			{

				PGWLib.PGWLib eft = new PGWLib.PGWLib();
				string userTypedValue = "";

				int ret = eft.getInputFromPP(ref userTypedValue, message, minLength, maxLength);
				if (ret != 0)
				{
					ShowMessageBoxInfo(this,string.Format("Erro ao executar a captura de dado no PINPad: {0}{1}{2} result  message :{3}", ret, Environment.NewLine, 
						((PGWLib.Enums.E_PWRET)ret).ToString(), eft.getResultMessage()));
					LogaTransactionResult();
					return;
				}

				ShowMessageBoxInfo(this,string.Format("Dado capturado no PINPad: {0}{1}{2}",message.ToString(), 
					                              Environment.NewLine, userTypedValue));
				
				LogaTransactionResult();
			}
		}

		// Faz a limpeza dos parametros mantendo os parametros obrigatorios
		protected void OnBtnLimpaParametrosClicked (object sender, EventArgs e)
		{
			ParametersListStore.Clear();
			listaParametros.Clear();
			addMandatoryParameters();
		}
	}


}





