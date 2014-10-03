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
    public class BarraDTO
    {
        public string Symbol { get; set; }
        public DateTime TradeDate { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosePrice { get; set; }
        public double Volume { get; set; }
        public double Variacao { get; set; }
        public bool BarraBanco { get; set; } 

        //TOODO:Alguma razao para isso abaixo?
        public override int GetHashCode()
        {
            unchecked
            {
                int result = TradeDate.GetHashCode();
                result = (result * 397) ^ OpenPrice.GetHashCode();
                result = (result * 397) ^ HighPrice.GetHashCode();
                result = (result * 397) ^ LowPrice.GetHashCode();
                result = (result * 397) ^ ClosePrice.GetHashCode();
                result = (result * 397) ^ Volume.GetHashCode();
                return result;
            }
        }

        public BarraDTO(string symbol, DateTime tradeDate, double openPrice, double highPrice, double lowPrice, double closePrice, double volume, bool barraBanco)
        {
            this.Symbol = symbol;
            this.TradeDate = tradeDate;
            this.OpenPrice = openPrice;
            this.HighPrice = highPrice;
            this.LowPrice = lowPrice;
            this.ClosePrice = closePrice;
            this.Volume = volume;
            this.BarraBanco = barraBanco;
        }
    }
}
