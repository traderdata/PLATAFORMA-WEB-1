using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public partial class StockChartX
  {
    private bool _calendarMouseLeftButtonDown;
#if WPF
    private bool _calendarMouseRightButtonDown;
#endif
    private double _calendarResizeStartX;
    private double _calendarMoveStartX;

    private const string TimerResize = "TimerResize";
    private const string TimerMove = "TimerMove";
    private const string TimerUpdate = "TimerUpdate";
    private const string TimerCrossHairs = "TimerCrossHairs";
    private const string TimerInfoPanel = "TimerInfoPanel";

    private readonly ChartTimers _timers = new ChartTimers();
 
#if WPF
    private void Calendar_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      _calendar.ReleaseMouseCapture();
      _calendarMouseRightButtonDown = false;
      _timers.StopTimerWork(TimerMove);
      _calendar.Cursor = Cursors.Arrow;
    }

    private void Calendar_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      _calendarMouseRightButtonDown = true;
      _calendar.CaptureMouse();
      _calendarMoveStartX = e.GetPosition(_calendar).X;
      _timers.StartTimerWork(TimerMove);
      _calendar.Cursor = Cursors.ScrollWE;
    }
#endif
    internal bool _ctrlDown;

    private bool _calendarResizing;
    private bool _calendarMoving;
    private void Calendar_OnKeyUp(object sender, KeyEventArgs e)
    {
      _ctrlDown = false;      
    }

    private void Calendar_OnKeyDown(object sender, KeyEventArgs e)
    {
#if SILVERLIGHT
      if (e.Key == Key.Ctrl)
        _ctrlDown = true;
#endif
#if WPF
      if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        _ctrlDown = true;
#endif
    }
    private void Calendar_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      _calendarMouseLeftButtonDown = false;
      calendario.ReleaseMouseCapture();

      if (_calendarResizing) _timers.StopTimerWork(TimerResize);
      if (_calendarMoving) _timers.StopTimerWork(TimerMove);

      calendario.Cursor = Cursors.Arrow;
    }

    private void Calendar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _calendarMouseLeftButtonDown = true;
      calendario.CaptureMouse();
      _calendarResizeStartX = e.GetPosition(calendario).X;

      _calendarResizing = _calendarMoving = false;
      if (!_ctrlDown)
      {
        _timers.StartTimerWork(TimerResize);
        _calendarResizing = true;
      }
      else
      {
        _timers.StartTimerWork(TimerMove);
        _calendarMoving = true;
      }
      calendario.Cursor = Cursors.SizeWE;
    }

    private const int _pixelsGapToResize = 5;
    private void ResizeChart()
    {
      bool update = false;
      if (!_calendarMouseLeftButtonDown || RecordCount < 5) return;
      int startIndex = indexInicial;
      int endIndex = indexFinal;

      Point p = Mouse.GetPosition(calendario);
      if (p.X >= _calendarResizeStartX + _pixelsGapToResize) //right - increase size
      {
        if (indexFinal - indexInicial <= 5) return; //5 bars at least
        indexInicial++;
        indexFinal--;
        update = true;
      }
      else if (p.X <= _calendarResizeStartX - _pixelsGapToResize)
      {
        int oldStartIndex = indexInicial;
        int oldEndIndex = indexFinal;
        if (indexInicial > 0) indexInicial--;
        if (indexFinal < RecordCount) indexFinal++;
        update = (oldStartIndex != indexInicial || oldEndIndex != indexFinal);
      }
      if (update)
      {
        calendario.Paint();
        paineisContainers.Panels.ForEach(panel =>
                                          {
                                            panel._enforceSeriesSetMinMax = true;
                                            panel.Paint();
                                            panel.PaintXGrid();
                                          });
        if (startIndex != indexInicial)
          OnPropertyChanged(Property_StartIndex);
        if (endIndex != indexFinal)
          OnPropertyChanged(Property_EndIndex);

        FireZoom();
      }
      _calendarResizeStartX = p.X;
    }

    private void MoveChart()
    {
#if WPF
      if (!_calendarMouseRightButtonDown) return;
#endif
      bool update = false;

      Point p = Mouse.GetPosition(calendario);
      if (p.X >= _calendarMoveStartX + _pixelsGapToResize) //going right direction
      {
        if (indexFinal == RecordCount) return;
        indexInicial++;
        indexFinal++;
        update = true;
      }
      else if (p.X <= _calendarMoveStartX - _pixelsGapToResize) //going left direction
      {
        if (indexInicial == 0) return;
        indexInicial--;
        indexFinal--;
        update = true;
      }
      if (update)
      {
        calendario.Paint();
        paineisContainers.Panels.ForEach(panel =>
                                          {
                                            panel._enforceSeriesSetMinMax = true;
                                            panel.Paint();
                                            panel.PaintXGrid();
                                          });
        OnPropertyChanged(Property_StartIndex);
        OnPropertyChanged(Property_EndIndex);

        FireChartScroll();
      }
      _calendarMoveStartX = p.X;
    }

    private void MoveCrossHairs()
    {
      paineisContainers.ShowCrossHairs();
    }

    public void MoveCrossHairs(double record, double y)
    {
        paineisContainers.ShowCrossHairs(record,y);
    }

    internal void ShowInfoPanelInternal()
    {
      if (paineisContainers == null) return;
      paineisContainers.MostraInfoPanelInternal();
    }

    internal void ShowInfoPanelInternal(int record)
    {
        if (paineisContainers == null) return;
        paineisContainers.MostraInfoPanelInternal(record);
    }

    internal void StartShowingInfoPanel()
    {
      _timers.StartTimerWork(TimerInfoPanel);
      paineisContainers.MostraInfoPanelInternal();
    }

    internal void StopShowingInfoPanel()
    {
      _timers.StopTimerWork(TimerInfoPanel);
      paineisContainers.EscondeInfoPanel();
    }
  }
}