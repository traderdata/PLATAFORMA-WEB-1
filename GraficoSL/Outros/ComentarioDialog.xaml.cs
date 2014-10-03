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

namespace Traderdata.Client.Componente.GraficoSL.Outros
{
    public partial class ComentarioDialog : ChildWindow
    {
        public string Comentario { get; set; }
        
        public ComentarioDialog()
        {
            InitializeComponent();            
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Comentario = txtNovoComentario.Text;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

