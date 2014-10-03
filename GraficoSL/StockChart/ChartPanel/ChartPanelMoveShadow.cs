using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  /// <summary>
  /// Used when moving panel, it shows if the panel can be moved to a new place or not
  /// </summary>
  public partial class ChartPanelMoveShadow : Control
  {
#if WPF
    static ChartPanelMoveShadow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelMoveShadow), new FrameworkPropertyMetadata(typeof(ChartPanelMoveShadow)));

      IsOkToMoveProperty = DependencyProperty.Register("IsOkToMove", typeof (bool), typeof (ChartPanelMoveShadow),
                                                       new PropertyMetadata(false));
    }
#endif

#if SILVERLIGHT
    public ChartPanelMoveShadow()
    {
      DefaultStyleKey = typeof (ChartPanelMoveShadow);
      BackgroundEx = Brushes.DarkRed;
    }
#endif


#if SILVERLIGHT
    public static readonly DependencyProperty BackgroundExProperty =
      DependencyProperty.Register("BackgroundEx", typeof (Brush), typeof (ChartPanelMoveShadow),
                                  new PropertyMetadata((d, e) => ((ChartPanelMoveShadow)d).OnBackgroundExChanged()));

    public Brush BackgroundEx
    {
      get { return (Brush)GetValue(BackgroundExProperty); }
      set { SetValue(BackgroundExProperty, value); }
    }

    protected virtual void OnBackgroundExChanged()
    {
    }

    public static readonly DependencyProperty IsOkToMoveProperty =
      DependencyProperty.Register("IsOkToMove", typeof (bool), typeof (ChartPanelMoveShadow),
                                  new PropertyMetadata((d, e) => ((ChartPanelMoveShadow)d).OnIsOkToMoveChanged(d, e)));

    public bool IsOkToMove
    {
      get { return (bool)GetValue(IsOkToMoveProperty); }
      set { SetValue(IsOkToMoveProperty, value); }
    }

    protected virtual void OnIsOkToMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      bool newValue = (bool)e.NewValue;
      BackgroundEx = newValue ? Brushes.LightBlue : Brushes.DarkRed;
    }
#endif
#if WPF
    internal static readonly DependencyProperty IsOkToMoveProperty;
    internal bool IsOkToMove
    {
      get { return (bool)GetValue(IsOkToMoveProperty); }
      set { SetValue(IsOkToMoveProperty, value); }
    }
#endif

    internal void InitFromPanel(ChartPanel chartPanel)
    {
      Rect rcPanelBounds = chartPanel.CanvasRect;
      Canvas.SetTop(this, chartPanel.Top);
      Canvas.SetLeft(this, rcPanelBounds.Left);
      Width = rcPanelBounds.Width;
      Height = rcPanelBounds.Height + chartPanel.TitleBarHeight;
      IsOkToMove = false;
    }

    internal bool Visible
    {
      get { return Visibility == Visibility.Visible; }
      set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
    }

    internal double Top
    {
      get { return Canvas.GetTop(this); }
      set { Canvas.SetTop(this, value); }
    }
  }
}

