using Traderdata.Client.Componente.GraficoSL.Enum;
using System;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class Style
  {
    internal Series _series;

    public Style(Series stock)
    {
      _series = stock;
    }

    internal void SetStockSeries(Traderdata.Client.Componente.GraficoSL.StockChart.Stock stock)
    {
      _series = stock;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>true - if series was painted, false - otherwise</returns>
    public virtual bool Paint() {
      throw new NotImplementedException(); }
    public virtual void RemovePaint() {
      throw new NotImplementedException(); }
  }
}
