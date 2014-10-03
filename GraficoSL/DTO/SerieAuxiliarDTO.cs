using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Traderdata.Client.Componente.GraficoSL.DTO
{
    public class SerieAuxiliarDTO
    {
        public Color Cor { get; set; }
        public int Grossura { get; set; }
        public int IndexPainel { get; set; }
        public string Parametros { get; set; }
        public double AlturaPainel { get; set; }
        public bool PainelIndicadores { get; set; }
        public List<double> Dados { get; set; }
        public string Nome { get; set; }
    }
}
