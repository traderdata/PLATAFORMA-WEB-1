using System.Windows;
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  ///<summary>
  ///</summary>
  public partial class ChartPanelMovePlaceholder : Control
  {
#if WPF
    static ChartPanelMovePlaceholder()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelMovePlaceholder), new FrameworkPropertyMetadata(typeof(ChartPanelMovePlaceholder)));
    }
#endif

#if SILVERLIGHT
    public ChartPanelMovePlaceholder()
    {
      DefaultStyleKey = typeof (ChartPanelMovePlaceholder);
    }
#endif

    internal bool Visible
    {
      get { return Visibility == Visibility.Visible; }
      set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
    }

    internal void ShowOnPanel(ChartPanel chartPanel)
    {
      Rect rcPanelPaintBounds = chartPanel.CanvasRect;
      Canvas.SetTop(this, chartPanel.Top);
      Canvas.SetLeft(this, rcPanelPaintBounds.Left);
      Width = rcPanelPaintBounds.Width;
    }
  }

}
