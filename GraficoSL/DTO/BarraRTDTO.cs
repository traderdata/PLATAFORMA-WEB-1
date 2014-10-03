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

namespace Traderdata.Client.Componente.GraficoSL.DTO
{
    public class BarraRTDTO
    {
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFinal { get; set; }
        public bool Publicado { get; set; }
        public double Abertura { get; set; }
        public double Minimo { get; set; }
        public double Maximo { get; set; }
        public double Ultimo { get; set; }
        public double Quantidade { get; set; }
        public double Volume { get; set; }
        public double NumeroNegocio { get; set; }

    }
}
