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
    public class ObjetoEstudoDTO
    {
        public bool PainelIndicadores { get; set; }
        public Brush Cor { get; set; }
        public int TipoObjeto { get; set; }
        public int Grossura { get; set; }
        public int IndexPainel { get; set; }
        public EnumGeral.TipoLinha TipoLinha { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int RecordInicial { get; set; }
        public int RecordFinal { get; set; }
        public double ValorInicial { get; set; }
        public double ValorFinal { get; set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public string Parametros { get; set; }

        public bool InfinitaADireita { get; set; }
        public bool Magnetica { get; set; }
        public bool Suporte { get; set; }
        public bool Resistencia { get; set; }
    }
}
