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

namespace Traderdata.Client.Componente.GraficoSL.Enum
{
    public class Tupla
    {
        public int Value { get; set; }
        public string Name { get; set; }

        public Tupla(int value, string name)
        {
            this.Value = value;
            this.Name = name;
        }
    }
}
