﻿//Change log

//Data              Responsavel         Versão      Mudança
//18/10/18          Thiago Santos       3.0         1) Ajustes gerais
//
//16/10/18          Thiago Santos       2.0         1) Nova funcionalidade, de captura de dado via PINPad
//
//10/06/18          Thiago Santos       1.0         1) Versão inicial da aplicação

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;

namespace PDVS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PGWLib.PGWLib eft;

        public MainWindow()
        {
            InitializeComponent();

            eft = new PGWLib.PGWLib();

            foreach (string item in Enum.GetNames(typeof(E_PWOPER)))
            {
                cmbOper.Items.Add(item);
            }
            addMandatoryParameters();
        }

        #region Interface Elements

        private void btnAddParam_Click(object sender, RoutedEventArgs e)
        {
            openParamWindow();
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lstParameters.Items.Clear();
            addMandatoryParameters();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {

            if (!validateFields())
            {
                MessageBox.Show("Preencha os campos corretamente.");
                return;
            }

            int ret = executeTransaction();

            confirmUndoTransaction(getTransactionResult(),ret);

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lstParameters.SelectedIndex == -1) return;
            lstParameters.Items.RemoveAt(lstParameters.SelectedIndex);
        }

        private void lstParameters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstParameters.SelectedIndex == -1) return;
            PW_Parameter param = (PW_Parameter)lstParameters.SelectedItem;
            openParamWindow(param.parameterName, param.parameterValue, lstParameters.SelectedIndex);
        }

        #endregion

        private void addMandatoryParameters()
        {
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTNAME.ToString(), (int)E_PWINFO.PWINFO_AUTNAME, "PDVS"));
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTVER.ToString(), (int)E_PWINFO.PWINFO_AUTVER, "1.0"));
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTDEV.ToString(), (int)E_PWINFO.PWINFO_AUTDEV, "NTK Solutions Ltda"));
            lstParameters.Items.Add(new PW_Parameter(E_PWINFO.PWINFO_AUTCAP.ToString(), (int)E_PWINFO.PWINFO_AUTCAP, "28"));  
        }

        private void openParamWindow(string param = "", string value = "", int itemPosition = -1)
        {
            AddParamWindow addParamWindow = new AddParamWindow(param, value);
            addParamWindow.ShowDialog(ref param, ref value);

            if (!string.IsNullOrEmpty(param) & !string.IsNullOrEmpty(value) & !string.IsNullOrWhiteSpace(value))
            {
                PW_Parameter parameterObject = new PW_Parameter();

                E_PWINFO receivedParam = (E_PWINFO)Enum.Parse(typeof(E_PWINFO), param);

                parameterObject.parameterName = param;
                parameterObject.parameterCode = (ushort)receivedParam;
                parameterObject.parameterValue = value;

                foreach (PW_Parameter item in lstParameters.Items)
                {
                    if (item.parameterName == parameterObject.parameterName)
                    {
                        MessageBox.Show("Você não pode inserir duas vezes o mesmo parâmetro");
                        return;
                    }
                }

                if (itemPosition != -1) lstParameters.Items.RemoveAt(itemPosition);
                lstParameters.Items.Add(parameterObject);
            }
        }

        private bool validateFields()
        {
            if (cmbOper.SelectedIndex == -1) return false;
            if (lstParameters.Items.Count == 0) return false;
            return true;
        }

        private int executeTransaction()
        {
            E_PWOPER operation = (E_PWOPER)Enum.Parse(typeof(E_PWOPER), cmbOper.SelectedValue.ToString());

            PGWLib.PGWLib eft = new PGWLib.PGWLib();
            int ret = eft.startTransaction(operation, lstParameters.Items.Cast<PW_Parameter>().ToList());
            if (ret != 0)
            {
                string sError = ((E_PWRET)ret).ToString();
                int iError = ret;

                // verifica se deu erro de transacao anterior pendente
                if (ret == (int)E_PWRET.PWRET_FROMHOSTPENDTRN)
                {
                    // confirma a transacao que estava pendente
                    
                    E_PWCNF transactionStatus = E_PWCNF.PWCNF_REV_AUTO_ABORT;
                    ret = eft.confirmPendTransaction(transactionStatus, getTransactionResult());
                    MessageBox.Show(string.Format("Erro ao executar a transação: {0}{1}{2}{3}{4}", iError, Environment.NewLine, sError, Environment.NewLine,
                                                   ">> TRANSACAO PENDENTE RESOLVIDA <<"));
                }
                else
                {
                    MessageBox.Show(string.Format("Erro ao executar a transação: {0}{1}{2}", iError, Environment.NewLine, sError));                
                }
            }
            return ret;
        }

        private List<PW_Parameter> getTransactionResult()
        {
            List<PW_Parameter> ret = new List<PW_Parameter>();
            
            ret = eft.getTransactionResult();

            //ResultWindow rw = new ResultWindow(ret);
            //rw.Show();
            LogDLL(ret);
            return ret;

        }

        private int confirmUndoTransaction(List<PW_Parameter> transactionResult,int RetTransaction)
        {
            int ret = 0;

            foreach (PW_Parameter item in transactionResult)
            {
                if(item.parameterCode == (uint)E_PWINFO.PWINFO_CNFREQ & item.parameterValue == "1")
                {
                    if (RetTransaction == (int)E_PWRET.PWRET_FROMHOSTPENDTRN)
                    {
                        E_PWCNF transactionStatus = E_PWCNF.PWCNF_REV_AUTO_ABORT;
                        ret = eft.confirmPendTransaction(transactionStatus, transactionResult);
                    }
                    else
                    {
                        ConfirmationWindow cw = new ConfirmationWindow();
                        E_PWCNF transactionStatus = E_PWCNF.PWCNF_REV_AUTO_ABORT;
                        cw.ShowDialog(ref transactionStatus);
                        ret = eft.confirmUndoTransaction(transactionStatus, transactionResult);
                    }
                }
            }

            return ret;

        }

        private void btnCaptureWithPinpad_Click(object sender, RoutedEventArgs e)
        {
            string param = "";
            int minLength = 0;
            int maxLength = 0;

            SelectUserDataWindow selectUserDataWindow = new SelectUserDataWindow();
            selectUserDataWindow.ShowDialog(ref param, ref minLength, ref maxLength);

            if (minLength > 0)
            {
                E_PWUserDataMessages message = (E_PWUserDataMessages)Enum.Parse(typeof(E_PWUserDataMessages), param);

                PGWLib.PGWLib eft = new PGWLib.PGWLib();
                string userTypedValue = "";

                int ret = eft.getInputFromPP(ref userTypedValue, message, minLength, maxLength);
                if (ret != 0)
                {
                    MessageBox.Show(string.Format("Erro ao executar a captura de dado no PINPad: {0}{1}{2}", ret, Environment.NewLine, ((E_PWRET)ret).ToString()));
                    return;
                }

                MessageBox.Show(string.Format("Dado capturado no PINPad: {0}{1}", Environment.NewLine, userTypedValue));

            }
        }

        /////////////////////////////////////////////////////
        // Método utlizado para logar o retorno da execução 
        // das funções da DLL Pay Go
        public void LogDLL(List<PW_Parameter> result)
        {
            InitializeComponent();

            foreach (PW_Parameter item in result)
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
                            listBox.Items.Add(line);
                        }
                    }

                    //////////////////////////////
                }
                else
                {
                    listBox.Items.Add(item.ToString());
                }
            }
        }
        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}

