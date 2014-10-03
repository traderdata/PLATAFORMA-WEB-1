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

namespace Traderdata.Client.Componente.GraficoSL.Util
{
    public partial class ConfirmaExclusaoDialog : ChildWindow
    {

        public ConfirmaExclusaoDialog(string msg)
        {
            InitializeComponent();
            lblMsg.Text = msg;
        }

        public ConfirmaExclusaoDialog(string msg, string titulo)
        {
            InitializeComponent();
            lblMsg.Text = msg;
            lblTitulo.Text = titulo;
        }

   
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
          
            this.DialogResult = false;
        }

    }
}

