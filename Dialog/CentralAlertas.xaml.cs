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

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class CentralAlertas : ChildWindow
    {

        private List<AtivoLocalDTO> ativos;

        public CentralAlertas(List<AtivoLocalDTO> ativos)
        {
            InitializeComponent();
        }

        private void btnRemoverAlerta_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnAdicionarAlerta_Click(object sender, RoutedEventArgs e)
        {
            DetalhesAlerta detalhe = new DetalhesAlerta(ativos);
            detalhe.Show();
        }

        private void CarregaGridAlertas()
        {
            
        }

        private void btnAlterarAlerta_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

