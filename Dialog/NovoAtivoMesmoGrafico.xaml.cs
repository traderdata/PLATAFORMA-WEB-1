using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ServiceModel;
using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows.Browser;
using System.Windows.Input;
using Traderdata.Client.TerminalWEB.DTO;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class NovoAtivoMesmoGrafico : ChildWindow
    {
        #region Campos e Construtores

        private string ativo = "";
        private bool mesmoGrafico = false;
        private Tupla periodicidade;
        private Tupla periodo;
        private List<AtivoLocalDTO> listaAtivo = new List<AtivoLocalDTO>();
        
        public NovoAtivoMesmoGrafico(List<AtivoLocalDTO> listaAtivo) 
        {
            InitializeComponent();

            this.listaAtivo = listaAtivo;                        
        }
        
        
        #endregion Campos e Construtores

        #region Propriedades
                
        /// <summary>
        /// Ativo escolhido.
        /// </summary>
        public string Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }

               
        #endregion Propriedades

        #region Eventos

        private void btn_Enter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Hand;
        }

        private void btn_Leave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Arrow;
        }


        /// <summary>
        /// Clique no botao pesquisa.
        /// Abre a tela de pesquisa de ativo.
        /// </summary>
        private void btnPesquisaAtivo_Click(object sender, RoutedEventArgs e)
        {
            PesquisaAtivo pesquisaAtivo = new PesquisaAtivo(txtAtivo.Text, listaAtivo);
            pesquisaAtivo.Closing += (sender1, e1) =>
            {
                if (pesquisaAtivo.DialogResult == true)
                {
                    txtAtivo.Text = pesquisaAtivo.Ativo;
                }
            };
            pesquisaAtivo.Show();
        }
        
        /// <summary>
        /// Tecla pressionada no txtAtivo.
        /// Se for pressionado enter, devo executar o procedimento de clique no botao OK.
        /// </summary>
        private void txtAtivo_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Carregar();
            }
        }

        /// <summary>
        /// Clique no botao OK
        /// Realiza verificacao de campos e fecha a janela.
        /// </summary>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Carregar();
        }

        /// <summary>
        /// Clique no botao Cancel
        /// Cancela a escolha de ativo, epriodo e periodicidade.
        /// </summary>
        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Evento disparado ao se pressionar qualquer tecla sobre o campo de ativo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAtivo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(sender is TextBox)) return;

            //do not handle ModifierKeys 
            if (Keyboard.Modifiers != ModifierKeys.None) return;
            if ((e.Key < Key.A) || (e.Key > Key.Z)) return;

            TextBox tb = (TextBox)sender;

            string s = new string(new char[] { (char)e.PlatformKeyCode });
            int selstart = tb.SelectionStart;
            tb.Text = tb.Text.Remove(selstart, tb.SelectionLength);
            tb.Text = tb.Text.Insert(selstart, s);

            tb.Select(selstart + 1, 0);
            e.Handled = true;
        }
       
        #endregion Eventos

        #region Métodos

        /// <summary>
        /// Realiza validação de preenchimento de campos.
        /// </summary>
        private void Validacao()
        {
            ativo = txtAtivo.Text;           
        }
                    
        /// <summary>
        /// Excecuta ação do botão OK, ou seja valida e fecha a tela se estiver tudo correto.
        /// </summary>
        private void Carregar()
        {
            bool carregou = false;
            if (txtAtivo.Text != "")
            {
                foreach (AtivoLocalDTO obj in listaAtivo)
                {
                    if (obj.Ativo == txtAtivo.Text.Trim())
                    {
                        Validacao();
                        this.DialogResult = true;
                        carregou = true;
                        break;
                    }
                }
                if (!carregou)
                    MessageBox.Show("Não é possível carregar este ativo.", "Aviso", MessageBoxButton.OK);
            }
            else
                MessageBox.Show("Digite um ativo para carregar o gráfico.", "Aviso", MessageBoxButton.OK);
            
        }


        #endregion Métodos
    }
}




