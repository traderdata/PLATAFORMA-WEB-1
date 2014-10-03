using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Collections.Generic;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties
{
  ///<summary>
  /// Defines the interface for propertyable elements from chart
  ///</summary>
  public interface IChartElementPropertyAble
  {
    ///<summary>
    /// Gets the element title
    ///</summary>
    string Title { get; }

    ///<summary>
    /// Gets a <see cref="IEnumerable{T}"/> of properties for this element
    ///</summary>
    IEnumerable<IChartElementProperty> Properties { get; }
  }
}
