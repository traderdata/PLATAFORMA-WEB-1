using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class Kagi : Style
  {
    private const int thin = 1, thick = 3;
    readonly PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();

    public Kagi(Traderdata.Client.Componente.GraficoSL.StockChart.Stock stock) : base(stock)
    {
    }

    public override bool Paint()
    {
        if (_series.OHLCType == EnumGeral.TipoSerieOHLC.Abertura || _series.OHLCType == EnumGeral.TipoSerieOHLC.Volume)
        return false;
        Series sHigh = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Maximo);
      if (sHigh == null || sHigh.RecordCount == 0 || sHigh.Painted) return false;
      Series sLow = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Minimo);
      if (sLow == null || sLow.RecordCount == 0 || sLow.Painted) return false;
      Series sClose = _series._chartPanel.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Ultimo);
      if (sClose == null || sClose.RecordCount == 0 || sClose.Painted) return false;

      sHigh.Painted = sLow.Painted = sClose.Painted = true;

      StockChartX chartX = _series._chartPanel._chartX;

      double reversalSize = chartX.parametrosEstiloPreco[0];
      if (reversalSize > 50 || reversalSize < 0.0001)
        reversalSize = 1;

      double ptsOrPct = chartX.parametrosEstiloPreco[1];
      if (ptsOrPct != 2 && ptsOrPct != 1)
        ptsOrPct = 1; // Points = 1, Percent = 2

      chartX.parametrosEstiloPreco[1] = ptsOrPct;
      chartX.parametrosEstiloPreco[0] = reversalSize;

      double reverse; 

      chartX.estiloPrecoValor1.Clear();
      chartX.estiloPrecoValor2.Clear();
      chartX.estiloPrecoValor3.Clear();

      double nClose = 0, nClose2;
      double start = 0;
      const int percent = 2;
      const int up = 1;
      const int down = 2;
      int direction = 0; // up or down  
      int weight = 0; // thick or thin
      int totalBars = 0;
      double max = 0, min = 0;
      double oldMax = 0, oldMin = 0;

      if (sClose.RecordCount < 3) return false;

      chartX.xMap = new double[chartX.indexFinal - chartX.indexInicial + 1];
      int cnt = 0;

      // Count columns that will fit on screen
      int n;
      for (n = 0; n < chartX.indexFinal + 1; n++)
      {
        if (n >= sClose.RecordCount) continue;
        if (!sClose[n].Value.HasValue) continue;
        nClose = sClose[n].Value.Value;

        if (ptsOrPct == percent)
          reverse = nClose * reversalSize; // Percent
        else
          reverse = reversalSize; // Points

        if (direction == 0)
        { // First bar
          nClose2 = sClose[n + 1].Value.Value;
          if (nClose2 > nClose)
          {
            direction = up;
            weight = thick;
            start = nClose;
            max = nClose;
          }
          else
          {
            direction = down;
            weight = thin;
            start = nClose2;
            min = nClose2;
          }
        }

        switch (direction)
        {
          case up:
            if (nClose > max)
            {
              max = nClose;
              if (max > start) weight = thick;
            }
            else if (nClose < max - reverse)
            {
              direction = down;
              start = max;
              min = nClose;
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
                totalBars++;
            }
            break;
          case down:
            if (nClose < min)
            {
              min = nClose;
              if (min < start) weight = thin;
            }
            else if (nClose > min + reverse)
            {
              direction = up;
              start = min;
              max = nClose;
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
                totalBars++;
            }
            break;
        }
      }

      chartX.xCount = totalBars;

      if (totalBars == 0) return false;
      double space = chartX.PaintableWidth / totalBars;
      totalBars = 0;
      direction = 0;
      int pWeight = 0;

      _lines.C = _series._chartPanel._rootCanvas;
      _lines.Start();
      // Calculate from beginning, but only show between startIndex and endIndex
      double x = _series._chartPanel._chartX.EspacoEsquerdaGrafico;
      for (n = 0; n < chartX.indexFinal; n++)
      {
        if (sClose[n].Value.HasValue)
        {
          nClose = sClose[n].Value.Value;


          if (ptsOrPct == percent)
            reverse = nClose * reversalSize; // Percent
          else
            reverse = reversalSize; // Points

          if (direction == 0)
          { // First bar
            nClose2 = sClose[n + 1].Value.Value;
            if (nClose2 > nClose)
            {
              direction = up;
              weight = thick;
              max = nClose;
              min = nClose;
              oldMin = nClose2;
              oldMax = nClose;
              pWeight = thick;
            }
            else
            {
              direction = down;
              weight = thin;
              min = nClose2;
              max = nClose2;
              oldMin = nClose;
              oldMax = nClose2;
              pWeight = thin;
            }
          }

          switch (direction)
          {
            case up:
              if (nClose > max)
              {
                max = nClose;
                if (max > oldMax)
                {
                  weight = thick;
                }
              }
              else if (nClose < max - reverse)
              {
                // Paint previous up bar
                if (weight == pWeight) oldMax = 0;
                if (n >= chartX.indexInicial && n <= chartX.indexFinal)
                {
                  PaintBar(x, space, max, min, up, weight, oldMax);
                  x += space;
                }

                pWeight = weight;
                direction = down;
                oldMin = min;
                min = nClose;
                totalBars++;
              }
              break;
            case down:
              if (nClose < min)
              {
                min = nClose;
                if (min < oldMin)
                {
                  weight = thin;
                }
              }
              else if (nClose > min + reverse)
              {

                // Paint previous down bar
                if (weight == pWeight) oldMin = 0;
                if (n >= chartX.indexInicial && n <= chartX.indexFinal)
                {
                  PaintBar(x, space, max, min, down, weight, oldMin);
                  x += space;
                }

                pWeight = weight;
                direction = up;
                oldMax = max;
                max = nClose;
                totalBars++;
              }
              break;
          }

          // Record the x value
          if (n >= chartX.indexInicial && n <= chartX.indexFinal)
          {
            chartX.xMap[cnt] = x + (space / 2);
            cnt++;
          }
        }

        chartX.estiloPrecoValor1.Add(new PriceStyleValue(sClose[n].TimeStamp, max));
        chartX.estiloPrecoValor2.Add(new PriceStyleValue(sClose[n].TimeStamp, min));
        chartX.estiloPrecoValor3.Add(new PriceStyleValue(sClose[n].TimeStamp, (direction == up) ? 1 : -1));
      }

      switch (direction)
      {
        case up:
          if (nClose > max)
          {
            max = nClose;
            if (max >= oldMax)
            {
              weight = thick;
            }
          }
          if (weight == pWeight) oldMax = 0;
          break;
        case down:
          if (nClose <= min)
          {
            min = nClose;
            if (min < oldMin)
            {
              weight = thin;
            }
          }
          if (weight == pWeight) oldMin = 0;
          break;
      }

      if (direction == down)
      {
        PaintBar(x, 0, max, min, down, weight, oldMin);
      }
      else
      {
        PaintBar(x, 0, max, min, up, weight, oldMax);
      }

      chartX.estiloPrecoValor1.Add(new PriceStyleValue(sClose[n - 1].TimeStamp, max));
      chartX.estiloPrecoValor2.Add(new PriceStyleValue(sClose[n - 1].TimeStamp, min));
      chartX.estiloPrecoValor3.Add(new PriceStyleValue(sClose[n - 1].TimeStamp, (direction == up) ? 1 : -1));

      _lines.Stop();

      _lines.Do(l => l.ZIndex = ZIndexConstants.PriceStyles1);

      return true;
    }

    private void PaintBar(double x, double space, double top, double bottom, int direction, int weight, double changePrice)
    {
      Brush brushUp =
        new SolidColorBrush(_series._upColor.HasValue ? _series._upColor.Value : _series._chartPanel._chartX.CandleCorAlta);
      Brush brushDown =
        new SolidColorBrush(_series._downColor.HasValue ? _series._downColor.Value : _series._chartPanel._chartX.CandleCorBaixa);

      top = _series._chartPanel.GetY(top);
      bottom = _series._chartPanel.GetY(bottom);

      double y = top;
      if (y > bottom)
      {
        top = bottom;
        bottom = y;
      }

      if (changePrice > 0) 
        changePrice = _series._chartPanel.GetY(changePrice);

      if (direction == 1)
      {
        if (changePrice > 0)
        {
          Utils.DrawLine(x, top, x, changePrice, brushUp, _series._strokePattern, thick, _lines);
          Utils.DrawLine(x, changePrice, x, bottom, brushUp, _series._strokePattern, thin, _lines);
          Utils.DrawLine(x, top, x + space, top, brushUp, _series._strokePattern, thin, _lines);
        }
        else
        {
          Utils.DrawLine(x, top, x, bottom, brushUp, _series._strokePattern, weight, _lines);
          Utils.DrawLine(x, top, x + space, top, brushUp, _series._strokePattern, weight, _lines);            
        }
      }
      else
      {
        if (changePrice > 0)
        {
          Utils.DrawLine(x, top, x, changePrice, brushDown, _series._strokePattern, thick, _lines);
          Utils.DrawLine(x, changePrice, x, bottom, brushDown, _series._strokePattern, thin, _lines);
          Utils.DrawLine(x, bottom, x + space, bottom, brushDown, _series._strokePattern, thin, _lines);            
        }
        else
        {
          Utils.DrawLine(x, top, x, bottom, brushDown, _series._strokePattern, weight, _lines);
          Utils.DrawLine(x, bottom, x + space, bottom, brushDown, _series._strokePattern, weight, _lines);  
        }
      }

      _lines.Stop();
    }

    public override void RemovePaint()
    {
      _lines.RemoveAll();
    }
  }
}
