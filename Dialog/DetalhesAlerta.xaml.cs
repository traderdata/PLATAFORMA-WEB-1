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
using Traderdata.Client.TerminalWEB.DTO;
using Traderdata.Client.TerminalWEB.Util;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class DetalhesAlerta : ChildWindow
    {
       

   

        public DetalhesAlerta(List<AtivoLocalDTO> ativos)
        {
            InitializeComponent();
        }

     

        private void PopularCampos()
        {
            
        }

        private void btnSalvarAlerta_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void MontaAlerta()
        {
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnBuscaAtivo_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void txbAtivo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

