using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles.Models;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class Candles : Style
  {
    internal Brush _downBrush;
    internal Brush _upBrush;
    internal Brush _candleDownOutlineBrush;
    internal Brush _candleUpOutlineBrush;

    private bool _subscribedToCustomBrush;

    private readonly PaintObjectsManager<Candle> _candles = new PaintObjectsManager<Candle>();

    public Candles(Series stock)
      : base(stock)
    {
    }

    private bool? _old3DStyle;
    private Color? _oldUpColor;
    private Color? _oldDownColor;
    private Color? _oldCandleDownOutline;
    private Color? _oldCandleUpOutline;

    private StockChartX ChartX;
    private double _halfwick;

    private Color _upColor;
    private Color _downColor;

    private void CalculateCandleSpacing()
    {
      ChartX = _series._chartPanel._chartX;
      ChartX.xMap = new double[ChartX.xCount = 0];

      int rcnt = ChartX.VisibleRecordCount;

      double x2 = ChartX.GetXPixel(rcnt);
      double x1 = ChartX.GetXPixel(rcnt - 1);
      double space = ((x2 - x1) / 2) - ChartX.larguraBarra / 2;

      if (space > 0.0)
      {
        if (space > 20) space = 20;
        space = space * 0.75; //25%
      }
      else
      {
        space = 1;
      }

      ChartX.espacamentoBarra = space;

      double wick = ChartX.larguraBarra;
      _halfwick = wick / 2;
    }

    public override bool Paint()
    {
        if (_series.OHLCType == EnumGeral.TipoSerieOHLC.Volume) return false;
      //Find Series
        Series open = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Abertura);
      if (open == null || open.RecordCount == 0 || open.Painted) return false;
      Series high = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Maximo);
      if (high == null || high.RecordCount == 0 || high.Painted) return false;
      Series low = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Minimo);
      if (low == null || low.RecordCount == 0 || low.Painted) return false;
      Series close = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Ultimo);
      if (close == null || close.RecordCount == 0 || close.Painted) return false;

      _series = close;

      open.Painted = high.Painted = low.Painted = close.Painted = true;

      CalculateCandleSpacing();
      if (ChartX.espacamentoBarra < 0)
        return false;

      _upColor = _series._upColor.HasValue ? _series._upColor.Value : ChartX.CandleCorAlta;
      _downColor = _series._downColor.HasValue ? _series._downColor.Value : ChartX.CandleCorBaixa;

      if (ChartX.OptimizePainting)
      {
        return PaintOptimized(new[] {open, high, low, close}, _series);
      }

      if (!_subscribedToCustomBrush)
      {
        _subscribedToCustomBrush = true;
        ChartX.OnCandleCustomBrush += ChartX_OnCandleCustomBrush;
      }


      bool setBrushes = false;
      if (!_old3DStyle.HasValue || _old3DStyle.Value != ChartX.Estilo3D ||
        !_oldUpColor.HasValue || _oldUpColor.Value != _upColor ||
        !_oldDownColor.HasValue || _oldDownColor.Value != _downColor ||
        (ChartX.corContornoCandleBaixa.HasValue && ChartX.corContornoCandleBaixa.Value != _oldCandleDownOutline) ||
        ChartX.corContornoCandleAlta.HasValue && ChartX.corContornoCandleAlta.Value != _oldCandleUpOutline)
      {

        setBrushes = true;
        _old3DStyle = ChartX.Estilo3D;
        _oldUpColor = _upColor;
        _oldDownColor = _downColor;

        _upBrush = !ChartX.Estilo3D
                     ? (Brush)new SolidColorBrush(_upColor)
                     : new LinearGradientBrush
                         {
                           StartPoint = new Point(0, 0.5),
                           EndPoint = new Point(1, 0.5),
                           GradientStops =
                             {
                               new GradientStop
                                 {
                                   Color = _upColor,
                                   Offset = 0
                                 },
                               new GradientStop
                                 {
                                   Color = Constants.FadeColor,
                                   Offset = 1.25
                                 }
                             }
                         };
#if WPF
        _upBrush.Freeze();
#endif

        _downBrush = !ChartX.Estilo3D
                       ? (Brush)new SolidColorBrush(_downColor)
                       : new LinearGradientBrush
                           {
                             StartPoint = new Point(0, 0.5),
                             EndPoint = new Point(1, 0.5),
                             GradientStops =
                               {
                                 new GradientStop
                                   {
                                     Color = _downColor,
                                     Offset = 0
                                   },
                                 new GradientStop
                                   {
                                     Color = Constants.FadeColor,
                                     Offset = 1.25
                                   }
                               }
                           };
#if WPF
        _downBrush.Freeze();
#endif

        if (ChartX.corContornoCandleBaixa.HasValue)
        {
          _oldCandleDownOutline = ChartX.corContornoCandleBaixa.Value;
          _candleDownOutlineBrush = new SolidColorBrush(ChartX.corContornoCandleBaixa.Value);
#if WPF
          _candleDownOutlineBrush.Freeze();
#endif
        }
        if (ChartX.corContornoCandleAlta.HasValue)
        {
          _oldCandleUpOutline = ChartX.corContornoCandleAlta.Value;
          _candleUpOutlineBrush = new SolidColorBrush(ChartX.corContornoCandleAlta.Value);
#if WPF
          _candleUpOutlineBrush.Freeze();
#endif
        }
      }

      int n;
      
      _candles.C = _series._chartPanel._rootCanvas;
      _candles.Start();

      for (n = ChartX.indexInicial; n < ChartX.indexFinal; n++)
      {
        if (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue)
          continue;

        Candle candle = _candles.GetPaintObject(_upBrush, _downBrush);
        candle.Init(_series);
        if (setBrushes)
          candle.SetBrushes(_upBrush, _downBrush, _candleUpOutlineBrush, _candleDownOutlineBrush);
        candle.SetValues(open[n].Value.Value, high[n].Value.Value, low[n].Value.Value, close[n].Value.Value, ChartX.espacamentoBarra,
                         _halfwick, n - ChartX.indexInicial);
      }
      _candles.Stop();

      _candles.Do(c => c.ZIndex = ZIndexConstants.PriceStyles1);

      return true;
    }

    private void ChartX_OnCandleCustomBrush(int index, Brush brush)
    {
      if (index >= 0 && index < _candles.Count)
        _candles[index].ForceCandlePaint();
    }

    public override void RemovePaint()
    {
      if (!ChartX.OptimizePainting)
      {
        _candles.RemoveAll();
      }
      else
      {
        Canvas c = _series._chartPanel._rootCanvas;
        c.Children.Remove(_pathCandlesDown);
        c.Children.Remove(_pathCandlesUp);
        c.Children.Remove(_pathWicks);
      }
    }

    private Path _pathCandlesDown;
    private Path _pathCandlesUp;
    private Path _pathWicks;

    public bool PaintOptimized(Series[] series, Series seriesClose)
    {
      PriceStyleStandardModel model 
        = new PriceStyleStandardModel(ChartX.indexInicial, ChartX.indexFinal, series);

      if (_pathCandlesDown == null)
      {
        _pathWicks = new Path();
        _series._chartPanel._rootCanvas.Children.Add(_pathWicks);
        Canvas.SetZIndex(_pathWicks, ZIndexConstants.PriceStyles1);

        _pathCandlesDown = new Path();
        _series._chartPanel._rootCanvas.Children.Add(_pathCandlesDown);
        Canvas.SetZIndex(_pathCandlesDown, ZIndexConstants.PriceStyles1 + 1);

        _pathCandlesUp = new Path();
        _series._chartPanel._rootCanvas.Children.Add(_pathCandlesUp);
        Canvas.SetZIndex(_pathCandlesUp, ZIndexConstants.PriceStyles1 + 1);
      }
      _pathWicks.Stroke = new SolidColorBrush(seriesClose._strokeColor);
      _pathCandlesDown.Fill = new SolidColorBrush(_downColor);
      _pathCandlesUp.Fill = new SolidColorBrush(_upColor);

      model.Space = ChartX.espacamentoBarra;
      model.HalfWick = _halfwick;

      StringBuilder sbDown = new StringBuilder();
      StringBuilder sbUp = new StringBuilder();
      StringBuilder sbWicks = new StringBuilder();

      sbDown.BeginGeometryGroup();
      sbUp.BeginGeometryGroup();
      sbWicks.BeginGeometryGroup();

      int wicksCount = 0;
      int downCount = 0;
      int upCount = 0;
      foreach (PriceStyleStandardModel.Values value in model.WickValues)
      {
        PriceStyleStandardModel.WickValueType wick = value.WickValue;
        if (wick != null)
        {
          sbWicks.AddSegment(new Point(wick.X, wick.Y1), new Point(wick.X, wick.Y2));
          wicksCount++;
        }

        if (value.CandleValue.Type == PriceStyleStandardModel.CandleType.Down)
        {
          sbDown.AddRectangle(new Rect(new Point(value.CandleValue.X1, value.CandleValue.Y1),
                                       new Point(value.CandleValue.X2, value.CandleValue.Y2)));
          downCount++;
        }
        else
        {
          sbUp.AddRectangle(new Rect(new Point(value.CandleValue.X1, value.CandleValue.Y1),
                                     new Point(value.CandleValue.X2, value.CandleValue.Y2)));
          upCount++;
        }
      }

      sbDown.EndGeometryGroup();
      sbUp.EndGeometryGroup();
      sbWicks.EndGeometryGroup();

#if WPF
      if (downCount > 0)
        _pathCandlesDown.Data = (GeometryGroup)XamlReader.Parse(sbDown.ToString());
      if (upCount > 0)
        _pathCandlesUp.Data = (GeometryGroup)XamlReader.Parse(sbUp.ToString());
      if (wicksCount > 0)
        _pathWicks.Data = (GeometryGroup)XamlReader.Parse(sbWicks.ToString());
#endif
#if SILVERLIGHT
      if (downCount > 0)
        _pathCandlesDown.Data = (GeometryGroup)XamlReader.Load(sbDown.ToString());
      if (upCount > 0)
        _pathCandlesUp.Data = (GeometryGroup)XamlReader.Load(sbUp.ToString());
      if (wicksCount > 0)
        _pathWicks.Data = (GeometryGroup)XamlReader.Load(sbWicks.ToString());
#endif

      return true;
    }
  }
}
