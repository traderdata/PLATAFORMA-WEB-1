using System.Windows;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties
{
  ///<summary>
  /// Interface for value presenters
  ///</summary>
  public interface IValuePresenter
  {
    ///<summary>
    /// Gets or sets the value
    ///</summary>
    object Value { get; set; }

    ///<summary>
    /// Gets the control that will show the value
    ///</summary>
    FrameworkElement Control { get; }
  }
}
