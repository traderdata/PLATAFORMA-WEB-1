using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Models
{
  internal partial class SeriesModelBase
  {
    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
    public bool IsOscillator { get; private set; }
    public EnumGeral.TipoSeriesEnum SeriesType { get; private set; }
    public Series Series { get; private set; }

    public SeriesModelBase(int startIndex, int endIndex, bool isOscillator, EnumGeral.TipoSeriesEnum seriesType, Series series)
    {
      StartIndex = startIndex;
      EndIndex = endIndex;
      IsOscillator = isOscillator;
      SeriesType = seriesType;
      Series = series;
    }

    
  }
}
