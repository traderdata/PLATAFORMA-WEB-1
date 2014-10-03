using System.Windows;
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  ///<summary>
  /// Represent the button from title bar
  ///</summary>
  public partial class ChartPanelTitleBarButton : Button
  {
#if WPF
    static ChartPanelTitleBarButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelTitleBarButton), new FrameworkPropertyMetadata(typeof(ChartPanelTitleBarButton)));
    }
#else
    public ChartPanelTitleBarButton()
    {
      DefaultStyleKey = typeof (ChartPanelTitleBarButton);
    }
#endif
  }

  ///<summary>
  /// Represent the button from title bar
  ///</summary>
  public class ChartPanelTitleBarButtonClose : Button
  {
#if WPF
    static ChartPanelTitleBarButtonClose()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelTitleBarButtonClose), new FrameworkPropertyMetadata(typeof(ChartPanelTitleBarButtonClose)));
    }
#else
    public ChartPanelTitleBarButtonClose()
    {
      DefaultStyleKey = typeof (ChartPanelTitleBarButtonClose);
    }
#endif
  }

}
