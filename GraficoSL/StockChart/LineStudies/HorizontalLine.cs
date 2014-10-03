using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Line=System.Windows.Shapes.Line;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_LineStudiesParams
  {
    internal static void Register_HorizontalLine()
    {
      RegisterLineStudy(LineStudy.StudyTypeEnum.HorizontalLine, typeof(HorizontalLine), "Horizontal Line");
    }
  }
}


namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
  /// <summary>
  /// Horizontal line
  /// </summary>
  public partial class HorizontalLine : LineStudy, IContextAbleLineStudy
  {
    private Line _line;
    private TextBlock _txt;
    private ContextLine _contextLine;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="key">Unique key for line study</param>
    /// <param name="stroke">Stroke brush</param>
    /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
    public HorizontalLine(string key, Brush stroke, ChartPanel chartPanel)
      : base(key, stroke, chartPanel)
    {
      _studyType = StudyTypeEnum.HorizontalLine;
    }

    internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
    {
      if (_txt == null && lineStatus != LineStatus.StartPaint)
        DrawLineStudy(rect, LineStatus.StartPaint);
      if (lineStatus == LineStatus.StartPaint)
      {
        _line = new Line {Stroke = Stroke, StrokeThickness = 1, Tag = this};
        Types.SetLinePattern(_line, StrokeType);
        C.Children.Add(_line);

        Canvas.SetZIndex(_line, ZIndexConstants.LineStudies1);

        _txt = new TextBlock
                 {
                   FontFamily = new FontFamily(_chartX.FontFace),
                   FontSize = _chartX.FontSize,
                   Foreground = _chartX.FontForeground,
                   Tag = this
                 };
        C.Children.Add(_txt);
        Canvas.SetZIndex(_txt, ZIndexConstants.LineStudies1);

        _contextLine = new ContextLine(this);

        _internalObjectCreated = true;

        return;
      }

      bool showText = true;
      if (_params.Length > 0 && _params[0].HasValue)
        showText = Convert.ToBoolean(_params[0]);
      _txt.Visibility = showText ? Visibility.Visible : Visibility.Collapsed;

      if (showText)
      {
        string scalePrecision = ".00";
        if (_chartX.EscalaPrecisao > 0)
          scalePrecision = ".".PadRight(_chartPanel._chartX.EscalaPrecisao + 1, '0');

        _txt.Text = _chartPanel.GetReverseY(rect.Bottom).ToString(scalePrecision);
        Canvas.SetLeft(_txt, C.ActualWidth - _chartX.GetTextWidth(_txt.Text) - 10);
        double textHeight = _chartX.GetTextHeight(_txt.Text);
        if (rect.Bottom - textHeight > 2)
          Canvas.SetTop(_txt, rect.Bottom  - textHeight - 2);
        else
          Canvas.SetTop(_txt, rect.Bottom + 2);
      }

      _line.X1 = 0;
      _line.X2 = C.ActualWidth;
      _line.Y1 = _line.Y2 = rect.Bottom;
//      Canvas.SetLeft(_line, 0);
//      _line.Width = C.ActualWidth;
//      Canvas.SetTop(_rectangle, rect.Bottom);
    }

    internal override List<SelectionDotInfo> GetSelectionPoints()
    {
      return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {Corner = Types.Corner.BottomCenter, Position = new Point(C.ActualWidth / 2, _y2)}
               };
    }

    internal override void SetCursor()
    {
      if (_selectionVisible)
      {
        _line.Cursor = Cursors.Hand;
        return;
      }
      if (_selectionVisible || _line.Cursor == Cursors.Arrow) return;
      _line.Cursor = Cursors.Arrow;
    }

    internal override void SetStrokeThickness()
    {
      if (_line != null)
        _line.StrokeThickness = StrokeThickness;
    }

    internal override void SetStroke()
    {
      if (_line != null)
        _line.Stroke = Stroke;
    }

    internal override void SetStrokeType()
    {
      if (_line != null)
        Types.SetLinePattern(_line, StrokeType);
    }

    internal override void RemoveLineStudy()
    {
      C.Children.Remove(_line);
      C.Children.Remove(_txt);
    }

    internal override void SetOpacity()
    {
      _line.Opacity = _txt.Opacity = Opacity;
    }

    #region Implementation of IContextAbleLineStudy

    /// <summary>
    /// Element to which context line is bound
    /// </summary>
    public UIElement Element
    {
      get { return _line; }
    }

    /// <summary>
    /// Segment where context line shall be shown
    /// </summary>
    public Segment Segment
    {
      get { return new Segment(_line.X1, _line.Y1, _line.X2, _line.Y2).Inflate(-20); } 
    }

    /// <summary>
    /// Parent where <see cref="IContextAbleLineStudy.Element"/> belongs
    /// </summary>
    public Canvas Parent
    {
      get { return C; }
    }

    /// <summary>
    /// Gets if <see cref="IContextAbleLineStudy.Element"/> is selected
    /// </summary>
    public bool IsSelected
    {
      get { return _selected; }
    }

    /// <summary>
    /// Z Index of <see cref="IContextAbleLineStudy.Element"/>
    /// </summary>
    public int ZIndex
    {
      get { return ZIndexConstants.LineStudies1; }
    }

    /// <summary>
    /// Gets the chart object associated with <see cref="IContextAbleLineStudy.Element"/> object
    /// </summary>
    public StockChartX Chart
    {
      get { return _chartX; }
    }

    /// <summary>
    /// Gets the reference to <see cref="LineStudies.LineStudy"/> 
    /// </summary>
    public LineStudy LineStudy
    {
      get { return this; }
    }

    #endregion
  }
}
