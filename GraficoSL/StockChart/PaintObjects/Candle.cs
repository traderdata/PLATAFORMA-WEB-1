using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects
{
  internal partial class Candle : IPaintObject
  {
    private double _open;
    private double _high;
    private double _low;
    private double _close;
    private Series _series;
    private double _space;
    private double _halfwick;
    private System.Windows.Shapes.Line _lineWick;
    private System.Windows.Shapes.Rectangle _rectCandle;
    private TranslateTransform _rcTransform;
    private TranslateTransform _lineTransform;
    private int _index;

    private Brush _up;
    private Brush _down;
    private Brush _candleUpOutline;
    private Brush _candleDownOutline;

    private bool _brushMustBeChanged = true;

    public void Init(Series series)
    {
      _series = series;
    }

//    private bool _eventsSet;
    public void SetValues(double open, double high, double low, double close,
      double nSpace, double halfwick, int index)
    {
      if (!_brushMustBeChanged)
        _brushMustBeChanged = (_open != open) ||
                              (_close != close) ||
                              (_index != index) || (_series._chartPanel._chartX.GetBarBrushChanged(index + _series._chartPanel._chartX.indexInicial));
      _open = open;
      _high = high;
      _low = low;
      _close = close;
      _space = nSpace;
      _halfwick = halfwick;
      _index = index;

//      if (!_eventsSet)
//      {
//        _high.PropertyChanged += Wick_OnPropertyChanged;
//        _low.PropertyChanged += Wick_OnPropertyChanged;
//
//        _open.PropertyChanged += Candle_OnPropertyChanged;
//        _close.PropertyChanged += Candle_OnPropertyChanged;
//
//        _eventsSet = true;
//      }

      RePaint();
    }

//    private void Candle_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
//    {
////      PaintCandle();
//    }
//
//    private void Wick_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
//    {
////      PaintWick();
//    }

    internal void PaintWick()
    {
      ChartPanel chartPanel = _series._chartPanel;
      StockChartX chartX = chartPanel._chartX;

      //if (_high.Value.Value == _low.Value.Value) return;

      double x = chartX.GetXPixel(_index);
      double y1 = _series.GetY(_high);
      double y2 = _series.GetY(_low);

      if (y2 == y1) 
        y1 = y2 - 2;
      _lineWick.StrokeThickness = _series._strokeThickness;
      if (_lineWick.Stroke == null || _series._strokeColor != ((SolidColorBrush)_lineWick.Stroke).Color)
        _lineWick.Stroke = new SolidColorBrush(_series._strokeColor);
      _lineWick.X1 = x;
      _lineWick.X2 = x;
      _lineWick.Y1 = y1;
      _lineWick.Y2 = y2;
      if (_lineWick.Tag == null)
        _lineWick.Tag = _series;
    }

    internal void PaintCandle()
    {
      ChartPanel chartPanel = _series._chartPanel;
      StockChartX chartX = chartPanel._chartX;

      double x = chartX.GetXPixel(_index);

      Rect rc;
      double y1;
      double y2;

      if (_rectCandle.Tag == null)
        _rectCandle.Tag = _series;
      if (_open > _close) //down
      {
        y1 = _series.GetY(_open);
        y2 = _series.GetY(_close);
        if (y1 + 3 > y2) y2 += 2;
        rc = new Rect(x - _space - _halfwick, y1, 2 * (_space + _halfwick), y2 - y1);
        if (_brushMustBeChanged)
        {
          _rectCandle.Fill = chartX.GetBarBrush(_index + chartX.indexInicial, _down);
          _brushMustBeChanged = false;
        }
        if (_candleDownOutline == null)
          _rectCandle.StrokeThickness = 0;
        else
        {
          _rectCandle.StrokeThickness = 1;
          _rectCandle.Stroke = _candleDownOutline;
        }

//        Canvas.SetLeft(_rectCandle, rc.Left);
//        Canvas.SetTop(_rectCandle, rc.Top);
      }
      else if (_open < _close) //up
      {
        y1 = _series.GetY(_close);
        y2 = _series.GetY(_open);
        if (y1 + 3 > y2) y2 += 2;
        rc = new Rect(x - _space - _halfwick, y1, 2 * (_space + _halfwick), y2 - y1);
        if (_brushMustBeChanged)
        {
          _rectCandle.Fill = chartX.GetBarBrush(_index + chartX.indexInicial, _up);
          _brushMustBeChanged = false;
        }
        if (_candleUpOutline == null)
          _rectCandle.StrokeThickness = 0;
        else
        {
          _rectCandle.StrokeThickness = 1;
          _rectCandle.Stroke = _candleUpOutline;
        }
//        Canvas.SetLeft(_rectCandle, rc.Left);
//        Canvas.SetTop(_rectCandle, rc.Top);
//        _rectCandle.Width = rc.Width;
//        _rectCandle.Height = rc.Height;
      }
      else //No change, flat bar
      {
        y1 = _series.GetY(_close);
        y2 = _series.GetY(_open);
        if (y2 == y1) y1 = y2 - 1;
        rc = new Rect(x - _space - _halfwick, y1, 2 * (_space + _halfwick), y2 - y1);
        if (_brushMustBeChanged)
        {
          _rectCandle.Fill = chartX.GetBarBrush(_index + chartX.indexInicial, _down);
          _brushMustBeChanged = false;
        }
        _rectCandle.StrokeThickness = 0;
//        Canvas.SetLeft(_rectCandle, rc.Left);
//        Canvas.SetTop(_rectCandle, rc.Top);
//        _rectCandle.Width = rc.Width;
//        _rectCandle.Height = rc.Height;
      }

      _rcTransform.X = rc.Left;
      _rcTransform.Y = rc.Top;
      if (_rectCandle.Width != rc.Width)
        _rectCandle.Width = rc.Width;
      if (_rectCandle.Height != rc.Height)
        _rectCandle.Height = rc.Height;
    }

    internal void RePaint()
    {
      PaintWick();
      PaintCandle();
    }

    public void SetArgs(params object[] args)
    {
      Debug.Assert(args.Length == 2);
      _up = (Brush)args[0];
      _down = (Brush)args[1];
      _brushMustBeChanged = true;
    }

    public void AddTo(Canvas canvas)
    {
      _lineWick = new System.Windows.Shapes.Line();
      canvas.Children.Add(_lineWick);
      _rectCandle = new System.Windows.Shapes.Rectangle();
      canvas.Children.Add(_rectCandle);

      _rcTransform = (TranslateTransform)(_rectCandle.RenderTransform = new TranslateTransform());
      Canvas.SetLeft(_rectCandle, 0);
      Canvas.SetTop(_rectCandle, 0);

      _lineTransform = (TranslateTransform)(_lineWick.RenderTransform = new TranslateTransform());
      _lineWick.X1 = _lineWick.X2 = 0;

      _rectCandle.Tag = _series;
      _lineWick.Tag = _series;
    }

    public void SetBrushes(Brush up, Brush down, Brush candleUpOutline, Brush candleDownOutline)
    {
      _up = up;
      _down = down;
      _candleUpOutline = candleUpOutline;
      _candleDownOutline = candleDownOutline;

      _brushMustBeChanged = true;
    }

    public void RemoveFrom(Canvas canvas)
    {
      Debug.Assert(_lineWick != null);
      Debug.Assert(_rectCandle != null);

//      _high.PropertyChanged -= Wick_OnPropertyChanged;
//      _low.PropertyChanged -= Wick_OnPropertyChanged;
//
//      _open.PropertyChanged -= Candle_OnPropertyChanged;
//      _close.PropertyChanged -= Candle_OnPropertyChanged;

      canvas.Children.Remove(_lineWick);
      canvas.Children.Remove(_rectCandle);
    }

    public int ZIndex
    {
      get { return Canvas.GetZIndex(_lineWick); }
      set
      {
        Canvas.SetZIndex(_lineWick, value);
        Canvas.SetZIndex(_rectCandle, value + 1);
      }
    }

    internal void ForceCandlePaint()
    {
      _brushMustBeChanged = true;
      PaintCandle();
    }
  }
}

