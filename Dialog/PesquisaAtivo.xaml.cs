using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using Traderdata.Client.TerminalWEB.DTO;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class PesquisaAtivo : ChildWindow
    {

        #region Campos e Construtores

        private string ativo = "";
        private List<AtivoLocalDTO> listaAtivos = new List<AtivoLocalDTO>();
        List<AtivoLocalDTO> listaAux;

        public string Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }


        public PesquisaAtivo(string param, List<AtivoLocalDTO> listaAtivosParam)
        {
            InitializeComponent();
            this.listaAtivos = listaAtivosParam;
            txtAtivo.Text = param;
            BuscarAtivos();
        }

        #endregion Campos e Construtores

        #region Eventos

        private void btn_Enter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Hand;
        }

        private void btn_Leave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Arrow;
        }
              
        /***********************************************************************************************
        * Evento: Botão OK.
        * Descrição: Grava os listaAtivo selecionados na pesquisa.
        ***********************************************************************************************/
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (gridPesquisaAtivo.SelectedItems != null)
            {
                ativo = ((AtivoLocalDTO)gridPesquisaAtivo.SelectedItem).Ativo;
                this.DialogResult = true;
            }
            else
                MessageBox.Show("Selecione um ativo.", "Aviso", MessageBoxButton.OK);
        }

        /***********************************************************************************************
        * Evento: Botão Cancelar
        * Descrição: Cancela a chamada da pesquisa.
        ***********************************************************************************************/
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Evento disparado ao editar o texto da busca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAtivo_TextChanged(object sender, TextChangedEventArgs e)
        {
            BuscarAtivos();
        }

        /// <summary>
        /// Evento disparado ao se pressionar alguma tecla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void txtAtivo_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (!(sender is TextBox)) return;

        //    //do not handle ModifierKeys 
        //    if (Keyboard.Modifiers != ModifierKeys.None) return;
        //    if ((e.Key < Key.A) || (e.Key > Key.Z)) return;

        //    TextBox tb = (TextBox)sender;

        //    string s = new string(new char[] { (char)e.PlatformKeyCode });
        //    int selstart = tb.SelectionStart;
        //    tb.Text = tb.Text.Remove(selstart, tb.SelectionLength);
        //    tb.Text = tb.Text.Insert(selstart, s);

        //    tb.Select(selstart + 1, 0);
        //    e.Handled = true;

        //    //Carregar a busca
        //    BuscarAtivos();
        //}

        #endregion Eventos

        #region Métodos

     
        /// <summary>
        /// Metodo que atualiza a lista
        /// </summary>
        private void BuscarAtivos()
        {
            listaAux = new List<AtivoLocalDTO>();
            foreach (AtivoLocalDTO obj in listaAtivos)
            {
                if ((obj.Ativo.Contains(txtAtivo.Text.ToUpper()) ||
                    (obj.Empresa.Contains(txtAtivo.Text.ToUpper()))))
                    listaAux.Add(obj);
            }

            //Limpando o grid
            gridPesquisaAtivo.ItemsSource = null;

            //Resetando o gruid            
            gridPesquisaAtivo.ItemsSource = listaAux;
        }

        #endregion Métodos    
    }
}
