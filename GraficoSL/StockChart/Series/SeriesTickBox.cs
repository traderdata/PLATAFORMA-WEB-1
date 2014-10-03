using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  internal partial class SeriesTickBox : Canvas  
  {
    private readonly Series _series;
    private readonly TextBlock _text;

    public SeriesTickBox(Series series)
    {
      _series = series;
      _series.PropertyChanged += Series_OnPropertyChanged;
      StockChartX chartX = _series._chartPanel._chartX;
      chartX.PropertyChanged += ChartX_OnPropertyChanged;

      _text = new TextBlock {FontSize = chartX.FontSize, FontFamily = chartX.FontFamily};
      Children.Add(_text);
      SetLeft(_text, 2);
      SetTop(_text, 2);
      Width = Constants.YAxisWidth;
      Height = chartX.GetTextHeight("W") + 4;

      _text.IsHitTestVisible = false;
      IsHitTestVisible = false;
    }

    private void ChartX_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Show();
    }

    private void Series_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (Visibility != Visibility.Visible) return;
      switch (e.PropertyName)
      {
        case Series.PropertyStrokeBrush: 
          Background = _series.StrokeColorBrush;
          break;
        case Series.PropertyLastValue:
          Show();
          break;
      }
    }

    internal void Show()
    {
      if (Visibility != Visibility.Visible) return;

      DataEntry dataEntry = _series.DM.LastVisibleDataEntry(_series._seriesIndex);
      if (dataEntry == null || !dataEntry.Value.HasValue) return;

      double y = _series.GetY(dataEntry.Value.Value);
      if (y < 0)
        y = 0;
      SetTop(this, y);
      SetLeft(this, 0);
      SetZIndex(this, ZIndexConstants.SelectionPoint2);
      _text.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
      _text.Text = string.Format(_series._chartPanel.FormatYValueString, dataEntry.Value.Value);
    }
  }
}

