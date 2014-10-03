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
    public class IndicadorDTO
    {
        public Color CorAlta { get; set; }
        public Color CorBaixa { get; set; }
        public int Grossura { get; set; }
        public int IndexPainel { get; set; }
        public string Parametros { get; set; }
        public int TipoIndicador { get; set; }
        public EnumGeral.TipoLinha TipoLinha { get; set; }
        public Color CorSerieFilha1 { get; set; }
        public Color CorSerieFilha2 { get; set; }
        public int GrossuraSerieFilha1 { get; set; }
        public int GrossuraSerieFilha2 { get; set; }
        public EnumGeral.TipoLinha TipoLinhaSerieFilha1 { get; set; }
        public EnumGeral.TipoLinha TipoLinhaSerieFilha2 { get; set; }
        public double AlturaPainel { get; set; }
        public int StatusPainel { get; set; }
        public bool PainelIndicadoresLateral { get; set; }
        public bool PainelPreco { get; set; }
        public bool PainelVolume { get; set; }
        public bool PainelIndicadoresAbaixo { get; set; }
    }
}
