using System;
using System.Windows.Controls;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class Stock : Style
  {
    private readonly PaintObjectsManager<PaintObjects.Stock> _stocks = new PaintObjectsManager<PaintObjects.Stock>();

    public Stock(Series stock)
      : base(stock)
    {
    }

    private Color? _oldUpColor;
    private Color? _oldDownColor;
    private Color? _oldStrokeColor;

    private Brush _upBrush;
    private Brush _downBrush;
    private Brush _customBrush;
    private bool _subscribedToCustomBrush;

    public override bool Paint()
    {
        if (_series._seriesTypeOHLC != EnumGeral.TipoSerieOHLC.Ultimo) return false;

      StockChartX chartX = _series._chartPanel._chartX;
      Series open = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Abertura);
      Series high = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Maximo);
      if (high == null || high.Painted || high.RecordCount == 0) return false;
      Series low = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Minimo);
      if (low == null || low.Painted || low.RecordCount == 0) return false;
      Series close = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Ultimo);
      if (close == null || close.Painted || close.RecordCount == 0) return false;

       high.Painted = low.Painted = close.Painted = true;

       if (!_subscribedToCustomBrush)
       {
         _subscribedToCustomBrush = true;
         chartX.OnCandleCustomBrush += ChartX_OnCandleCustomBrush;
       }

      bool changeBrushes = false;
      Color upColor = _series._upColor.HasValue ? _series._upColor.Value : chartX.CandleCorAlta;
      Color downColor = _series._downColor.HasValue ? _series._downColor.Value : chartX.CandleCorBaixa;

      if (!_oldUpColor.HasValue || _oldUpColor.Value != upColor)
      {
        _upBrush = new SolidColorBrush(upColor);
        _oldUpColor = upColor;
        changeBrushes = true;
      }
      if (!_oldDownColor.HasValue || _oldDownColor.Value != downColor)
      {
        _downBrush = new SolidColorBrush(_series._downColor.HasValue ? _series._downColor.Value : chartX.CandleCorBaixa);
        _oldDownColor = downColor;
        changeBrushes = true;
      }
      if (!_oldStrokeColor.HasValue || _oldStrokeColor.Value != _series._strokeColor)
      {
        _customBrush = new SolidColorBrush(_series._strokeColor);
        _oldStrokeColor = _series._strokeColor;
        changeBrushes = true;
      }

      int rcnt = chartX.RecordCount;
      double x2 = chartX.GetXPixel(rcnt);
      double x1 = chartX.GetXPixel(rcnt - 1);
      double space = ((x2 - x1) / 2) - chartX.larguraBarra / 2;
      if (space > 20) space = 20;

      chartX.espacamentoBarra = space;

      int cnt = 0;

      ((Traderdata.Client.Componente.GraficoSL.StockChart.Stock)_series)._priceStyleStock = this;

      _stocks.C = _series._chartPanel._rootCanvas;
      _stocks.Start();

      for (int n = chartX.indexInicial; n < chartX.indexFinal; n++, cnt++)
      {
        if (!high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue) continue;

        PaintObjects.Stock stock = _stocks.GetPaintObject(_upBrush, _downBrush, _customBrush);
        stock.Init(_series);
        if (changeBrushes)
          stock.SetBrushes(_upBrush, _downBrush, _customBrush);
        stock.SetValues(open[n].Value, high[n].Value.Value, low[n].Value.Value, close[n].Value.Value, space, n - chartX.indexInicial);
      }

      _stocks.Stop();
      _stocks.Do(s => s.ZIndex = ZIndexConstants.PriceStyles1);

      return true;
    }

    private void ChartX_OnCandleCustomBrush(int index, Brush newbrush)
    {
      if (index >= 0 && index < _stocks.Count)
        _stocks[index].ForceCandlePaint();
    }

    public override void RemovePaint()
    {
      _stocks.RemoveAll();
    }

    internal void SetStrokeThickness(double thickness)
    {
      _stocks.Do(stock => stock.SetStrokeThickness(thickness));
    }
  }
}
