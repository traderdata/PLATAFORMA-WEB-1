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
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.DTO
{
    public class ConfiguracaoPadraoDTO
    {
        public Tupla PeriodicidadeIntraday { get; set; }
        public Tupla PeriodicidadeDiaria { get; set; }
    }
}
