using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class HeikinAshi : Style
  {
    internal Brush _downBrush;
    internal Brush _upBrush;
    internal Brush _candleDownOutlineBrush;
    internal Brush _candleUpOutlineBrush;

    private readonly PaintObjectsManager<CandleHeikinAshi> _candles = new PaintObjectsManager<CandleHeikinAshi>();

    public HeikinAshi(Traderdata.Client.Componente.GraficoSL.StockChart.Stock stock)
      : base(stock)
    {
    }

    public override bool Paint()
    {
      //Find Series
        Series open = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Abertura);
      if (open == null || open.RecordCount == 0 || open.Painted) return false;
      Series high = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Maximo);
      if (high == null || high.RecordCount == 0 || high.Painted) return false;
      Series low = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Minimo);
      if (low == null || low.RecordCount == 0 || low.Painted) return false;
      Series close = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Ultimo);
      if (close == null || close.RecordCount == 0 || close.Painted) return false;

      open.Painted = high.Painted = low.Painted = close.Painted = true;

      StockChartX chartX = _series._chartPanel._chartX;
      chartX.xMap = new double[chartX.xCount = 0];

      const int iStep = 1;
      int rcnt = chartX.RecordCount;
      double x2 = chartX.GetXPixel(rcnt);
      double x1 = chartX.GetXPixel(rcnt - 1);
      double space = ((x2 - x1) / 2) - chartX.larguraBarra / 2;
      if (space > 20) space = 20;
      if (space > _series._chartPanel._chartX.espacamentoBarra)
        _series._chartPanel._chartX.espacamentoBarra = space;

      space = Math.Ceiling(space * 0.75);

      Color upColor = _series._upColor.HasValue ? _series._upColor.Value : chartX.CandleCorAlta;
      Color downColor = _series._downColor.HasValue ? _series._downColor.Value : chartX.CandleCorBaixa;
      _upBrush = !chartX.Estilo3D
                   ? (Brush)new SolidColorBrush(upColor)
                   : new LinearGradientBrush
                       {
                         StartPoint = new Point(0, 0.5),
                         EndPoint = new Point(1, 0.5),
                         GradientStops =
                           {
                             new GradientStop { Color = upColor, Offset = 0 },
                             new GradientStop { Color = Constants.FadeColor, Offset = 1.25 }
                           }
                       };
      _downBrush = !chartX.Estilo3D
                     ? (Brush)new SolidColorBrush(downColor)
                     : new LinearGradientBrush
                         {
                           StartPoint = new Point(0, 0.5),
                           EndPoint = new Point(1, 0.5),
                           GradientStops =
                             {
                               new GradientStop { Color = downColor, Offset = 0 },
                               new GradientStop { Color = Constants.FadeColor, Offset = 1.25 }
                             }
                         };
      if (chartX.corContornoCandleBaixa.HasValue)
        _candleDownOutlineBrush = new SolidColorBrush(chartX.corContornoCandleBaixa.Value);
      if (chartX.corContornoCandleAlta.HasValue)
        _candleUpOutlineBrush = new SolidColorBrush(chartX.corContornoCandleAlta.Value);

      double wick = chartX.larguraBarra;
      double halfwick = wick / 2;
      if (halfwick < 1)
        halfwick = 1;

      int n;

      //int t = Environment.TickCount;
      _candles.C = _series._chartPanel._rootCanvas;
      _candles.Start();

      Debug.WriteLine("StartIndex " + chartX.indexInicial + " EndIndex " + chartX.indexFinal);

      for (n = chartX.indexInicial; n < chartX.indexFinal; n += iStep)
      {
        if (n == 0 && (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue))
          continue;
        if (n > 0 && (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue ||
          !open[n - 1].Value.HasValue || !close[n - 1].Value.HasValue))
          continue;

        CandleHeikinAshi candle = _candles.GetPaintObject(_upBrush, _downBrush);
        candle.Init(_series);
        candle.SetBrushes(_upBrush, _downBrush, _candleUpOutlineBrush, _candleDownOutlineBrush);
        candle.SetValues(open[n], high[n], low[n], close[n], space, halfwick, n - chartX.indexInicial);
      }
      _candles.Stop();

      _candles.Do(c => c.ZIndex = ZIndexConstants.PriceStyles1);

      Debug.WriteLine("Candles count " + _candles.Count);

      return true;
    }

    public override void RemovePaint()
    {
      _candles.RemoveAll();
    }
  }
}
