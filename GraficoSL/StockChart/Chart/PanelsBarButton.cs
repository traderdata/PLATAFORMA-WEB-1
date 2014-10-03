#if WPF
using System.Windows;
#endif
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  ///<summary>
  ///</summary>
  public partial class PanelsBarButton : Button 
  {
#if WPF
    static PanelsBarButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelsBarButton), new FrameworkPropertyMetadata(typeof(PanelsBarButton)));
    }
#else
    public PanelsBarButton()
    {
      DefaultStyleKey = typeof (PanelsBarButton);
    }
#endif
  }
}



