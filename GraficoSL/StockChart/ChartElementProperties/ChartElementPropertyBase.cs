using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties
{
  internal abstract partial class ChartElementPropertyBase
  {
    protected string _title;
    protected IValuePresenter _valuePresenter;

    protected ChartElementPropertyBase(string title)
    {
      _title = title;
    }
  }
}
