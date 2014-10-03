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

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class CadastroConcluido : ChildWindow
    {
        public CadastroConcluido()
        {
            InitializeComponent();
        }

        private void btnRetornaLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btn_Enter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Hand;
        }

        private void btn_Leave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Fecha a janela de cadastro pelo botão de fechar da janela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

