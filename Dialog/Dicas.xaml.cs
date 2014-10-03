using System;
using System.Windows;
using System.Windows.Controls;
using System.ServiceModel;


namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class Dicas : ChildWindow
    {
        #region Campos e Construtores

        public Dicas()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exc)
            {
                MostraMessageErro(exc, "Ocorreu um erro ao carregar.", "Construtor");
            }
        }

        
        #endregion Campos e Construtores

        #region Eventos

        private void btnTentarNovamente_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion Eventos
        
        #region MostraMessageErro()
        /// <summary>
        /// Mostra a mensagem desejada e dá opcção de ver detalhes do erro.
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="msg"></param>
        /// <param name="metodo"></param>
        /// <param name="titulo"></param>
        private void MostraMessageErro(Exception exc, string msg, string metodo = "", string titulo = "Atenção")
        {
            if (msg.EndsWith("."))
                msg = msg.Substring(0, msg.Length - 1);

            if (MessageBox.Show(msg + ". Deseja visualizar a mensagem de erro?", titulo, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                MessageBox.Show("Método: " + metodo + "    Erro: " + exc.ToString());
        }
        #endregion MostraMessageErro()

    }
}
