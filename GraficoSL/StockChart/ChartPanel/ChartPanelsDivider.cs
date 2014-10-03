using System.Windows;
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  /// <summary>
  /// Used to show the divider when resizing panels
  /// </summary>
  public partial class ChartPanelsDivider : Control
  {
#if WPF
    static ChartPanelsDivider()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelsDivider), new FrameworkPropertyMetadata(typeof(ChartPanelsDivider)));
    }
#endif
#if SILVERLIGHT
    public ChartPanelsDivider()
    {
      DefaultStyleKey = typeof (ChartPanelsDivider);
    }
#endif

    internal bool Visible
    {
      get { return Visibility == Visibility.Visible; }
      set
      {
         Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    private double _y;
    internal double Y
    {
      get { return _y; }
      set
      {
        if (_bIsOk)
        {
          Canvas.SetTop(this, _y = value - ActualHeight / 2);
#if SILVERLIGHT
          ActualWidthEx = ((Canvas)Parent).ActualWidth;
#endif
        }
      }
    }

    private bool _bIsOk = true;
    internal bool IsOK
    {
      get { return _bIsOk; }
      set
      {
        _bIsOk = value;
      }
    }
    #if SILVERLIGHT
    public static readonly DependencyProperty ActualWidthExProperty =
      DependencyProperty.Register("ActualWidthEx", typeof (double), typeof (ChartPanelsDivider),
                                  new PropertyMetadata((d, e) => ((ChartPanelsDivider)d).OnActualWidthExChanged(d, e)));

    public double ActualWidthEx
    {
      get { return (double)GetValue(ActualWidthExProperty); }
      set { SetValue(ActualWidthExProperty, value); }
    }

    protected virtual void OnActualWidthExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }
    #endif
  }
}



