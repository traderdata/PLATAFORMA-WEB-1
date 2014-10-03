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
    internal static void Register_GannFan()
    {
      RegisterLineStudy(LineStudy.StudyTypeEnum.GannFan, typeof(GannFan), "Gann Fan");
    }
  }
}


namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
  /// <summary>
  /// Gann Fan line study
  /// </summary>
  public partial class GannFan : LineStudy, IContextAbleLineStudy
  {
    private readonly Line[] _lines = new Line[11];
    private ContextLine _contextLine;

    private readonly double[] _angles = new[]
                                          {
                                            1.0 / 16, 1.0 / 8, 1.0 / 4, 1.0 / 3, 1.0 / 2,
                                            1.0,
                                            2, 3, 4, 8, 16
                                          };

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="key">Unique key for line study</param>
    /// <param name="stroke">Stroke brush</param>
    /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
    public GannFan(string key, Brush stroke, ChartPanel chartPanel)
      : base(key, stroke, chartPanel)
    {
      _studyType = StudyTypeEnum.GannFan;
    }

    internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
    {
      if (lineStatus == LineStatus.StartPaint)
      {
        for (int i = 0; i < _lines.Length; i++)
        {
          _lines[i] = new Line {Tag = this};
          Canvas.SetZIndex(_lines[i], ZIndexConstants.LineStudies1);
          C.Children.Add(_lines[i]);
        }

        _contextLine = new ContextLine(this);

        _internalObjectCreated = true;

        return;
      }

      for (int i = 0; i < _lines.Length; i++ )
      {
        double cx = C.ActualWidth - rect.Right;
        double cy = rect.Bottom - (cx * _angles[i]);
        Utils.DrawLine(rect.Right, rect.Bottom, C.ActualWidth, cy, Stroke, StrokeType, StrokeThickness, _lines[i]);
      }
    }

    internal override void SetCursor()
    {
      if (_lines.Length == 0) return;
      Line l = _lines[0];
      if (_selectionVisible)
      {
        foreach (Line line in _lines)
        {
          line.Cursor = Cursors.Hand;
        }
        return;
      }
      if (_selectionVisible || l.Cursor == Cursors.Arrow) return;
      foreach (Line line in _lines)
      {
        line.Cursor = Cursors.Arrow;
      }
    }

    internal override List<SelectionDotInfo> GetSelectionPoints()
    {
      return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {Corner = Types.Corner.MoveAll, Position = new Point(_lines[0].X1, _lines[0].Y1)}
               };
    }

    internal override void Reset()
    {
      if (_x1 < 1 || _x2 < 1 || _lines.Length == 0) return;

      _x1Value = _x2Value = _chartX.GetReverseXInternal(_lines[0].X1) + _chartX.indexInicial;
      _y1Value = _y2Value = _chartPanel.GetReverseY(_lines[0].Y1);

      _x1 = _x2 = _chartX.GetXPixel(_x2Value - _chartX.indexInicial);
      _y1 = _y2 = _chartPanel.GetY(_y2Value);
    }

    internal override void SetStrokeThickness()
    {
      foreach (Line line in _lines)
      {
        line.StrokeThickness = StrokeThickness;
      }
    }

    internal override void SetStroke()
    {
      foreach (Line line in _lines)
      {
        line.Stroke = Stroke;
      }
   }

    internal override void SetStrokeType()
    {
      foreach (Line line in _lines)
      {
        Types.SetLinePattern(line, StrokeType);
      }
    }

    internal override void RemoveLineStudy()
    {
      foreach (Line line in _lines)
      {
        C.Children.Remove(line);
      }
    }

    internal override void SetOpacity()
    {
      foreach (var line in _lines)
      {
        line.Opacity = Opacity;
      }
    }

    #region Implementation of IContextAbleLineStudy

    /// <summary>
    /// Element to which context line is bound
    /// </summary>
    public UIElement Element
    {
      get { return _lines[5]; }
    }

    /// <summary>
    /// Segment where context line shall be shown
    /// </summary>
    public Segment Segment
    {
      get
      {
        return new Segment(new Point(_lines[0].X1, _lines[0].Y1), 150, -45).Inflate(-10);
      }
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
