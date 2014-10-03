using Traderdata.Client.Componente.GraficoSL.Enum;
using System.ComponentModel;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public partial class StockChartX : INotifyPropertyChanged 
  {
    internal const string Property_StartIndex = "StartIndex";
    internal const string Property_EndIndex = "EndIndex";
    internal const string Property_NewRecord = "NewRecord";

    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    internal void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
