using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_LineStudiesParams
  {
    internal static void Register_StaticText()
    {
      RegisterLineStudy(LineStudy.StudyTypeEnum.StaticText, typeof(StaticText), "Static Text");
    }
  }
}


namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
  ///<summary>
  /// Static Text
  ///</summary>
  public partial class StaticText : LineStudy, IMouseAble, IContextAbleLineStudy
  {
    private TextBlock _txt;
    private string _text;
    private string _fontName;
    private ContextLine _contextLine;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="key">Unique key for line study</param>
    /// <param name="stroke">Stroke brush</param>
    /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
    public StaticText(string key, Brush stroke, ChartPanel chartPanel)
      : base(key, stroke, chartPanel)
    {
      _studyType = StudyTypeEnum.StaticText;
    }

    internal override void SetArgs(params object[] args)
    {
      _extraArgs = args;

      Debug.Assert(args.Length > 0);
      _text = args[0].ToString();
    }

    internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
    {
      if (_txt == null && lineStatus != LineStatus.StartPaint)
        DrawLineStudy(rect, LineStatus.StartPaint);
      if (lineStatus == LineStatus.StartPaint)
      {
        _txt = new TextBlock
                 {
                   Tag = this,
                   Foreground = Stroke,
                   FontSize = StrokeThickness,
                   FontFamily = new FontFamily(_fontName = _chartX.FontFace),
                   Visibility = Visibility.Collapsed
                 };
        C.Children.Add(_txt);

        _txt.MouseLeftButtonDown += (sender, args) => MouseDown(sender, args);
        _txt.MouseEnter += (sender, args) => MouseEnter(sender, args);
        _txt.MouseLeave += (sender, args) => MouseLeave(sender, args);
        _txt.MouseMove += (sender, args) => MouseMove(sender, args);
        _txt.MouseLeftButtonUp += (sender, args) => MouseUp(sender, args);

        if (_contextLine == null)
          _contextLine = new ContextLine(this);

        _internalObjectCreated = true;

        return;
      }

      if (_txt == null) return;

      if (_txt.Visibility == Visibility.Collapsed)
        _txt.Visibility = Visibility.Visible;

      _txt.Text = _text;
      Canvas.SetLeft(_txt, rect.Right);
      Canvas.SetTop(_txt, rect.Bottom);

      Canvas.SetZIndex(_txt, ZIndexConstants.LineStudies1);
    }

    internal override List<SelectionDotInfo> GetSelectionPoints()
    {
      return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.TopLeft,
                     Position = new Point(_x2, _y2),
                     Clickable = false
                   },
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.TopRight,
                     Position = new Point(_x2 + _txt.ActualWidth, _y2),
                     Clickable = false
                   },
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.BottomLeft,
                     Position = new Point(_x2, _y2 + _txt.ActualHeight),
                     Clickable = false
                   },
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.BottomRight,
                     Position = new Point(_x2 + _txt.ActualWidth, _y2 + _txt.ActualHeight),
                     Clickable = false
                   }
               };
    }

    internal override void SetCursor()
    {
      if (_selectionVisible)
      {
        _txt.Cursor = Cursors.Hand;
        return;
      }
      if (_selectionVisible || _txt.Cursor == Cursors.Arrow) return;
      _txt.Cursor = Cursors.Arrow;
    }

    internal override void RemoveLineStudy()
    {
      C.Children.Remove(_txt);
    }

    internal override void SetStrokeThickness()
    {
      if (_txt != null)
        _txt.FontSize = StrokeThickness;
    }

    internal override void SetStroke()
    {
      if (_txt != null)
        _txt.Foreground = Stroke;
    }

    internal override void SetStrokeType()
    {
    }

    internal override void SetOpacity()
    {
      _txt.Opacity = Opacity;
    }

    ///<summary>
    /// Text that is shown 
    ///</summary>
    public string Text
    {
      get { return _text; }
      set
      {
        _text = value;
        if (_txt != null)
          _txt.Text = _text;
      }
    }
  
    /// <summary>
    /// Font name used to show the text
    /// </summary>
    public string FontName
    {
      get { return _fontName; }
      set
      {
        _fontName = value;
        if (_txt != null)
          _txt.FontFamily = new FontFamily(_fontName);
      }
    }

    #region Implementation of IMouseAble

    ///<summary>
    ///</summary>
    public event MouseButtonEventHandler MouseDown = delegate { };
    ///<summary>
    ///</summary>
    public event MouseEventHandler MouseEnter = delegate { };
    ///<summary>
    ///</summary>
    public event MouseEventHandler MouseLeave = delegate { };
    ///<summary>
    ///</summary>
    public event MouseEventHandler MouseMove = delegate { };
    ///<summary>
    ///</summary>
    public event MouseButtonEventHandler MouseUp = delegate { };

    #endregion

    #region Implementation of IContextAbleLineStudy

    /// <summary>
    /// Element to which context line is bound
    /// </summary>
    public UIElement Element
    {
      get { return _txt; }
    }

    /// <summary>
    /// Segment where context line shall be shown
    /// </summary>
    public Segment Segment
    {
      get { return new Segment(_x2, _y2, _x2 + _txt.ActualWidth, _y2); }
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

    /// <summary>
    /// Basic properties
    /// </summary>
    protected override IEnumerable<ChartElementProperties.IChartElementProperty> BaseProperties
    {
      get
      {
        GetChartElementColorProperty();
        yield return propertyStroke;

        GetChartElementStrokeThicknessProperty("Font Size");
        yield return propertyStrokeThickness;

        GetChartElementOpacityProperty();
        yield return propertyOpacity;
      }
    }

    #endregion

    public override string ObtemParametros(char strConcatenacao)
    {
        return Text;
    }

    public override void SetaParametros(string parametros, char strConcatenacao)
    {
        this.Text = parametros;
    }
  }
}

