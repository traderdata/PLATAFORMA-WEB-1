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

namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class ConfiguraDarvaBox : ChildWindow
    {
        private double percentual;

        public double Percentual
        {
            get { return percentual; }
            set { percentual = value; }
        }

        public ConfiguraDarvaBox()
        {
            InitializeComponent();
        }

        public ConfiguraDarvaBox(double percentual)
        {
            InitializeComponent();
            this.percentual = percentual;

            numPercentual.Value = percentual;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            percentual = Convert.ToDouble(numPercentual.Value);

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

