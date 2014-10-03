using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class ThreeLineBreak : Style
  {
    private double[] _highs;
    private double[] _lows;
    private readonly PaintObjectsManager<Rectangle> _rects = new PaintObjectsManager<Rectangle>();

    public ThreeLineBreak(Traderdata.Client.Componente.GraficoSL.StockChart.Stock stock) : base(stock)
    {
    }

    public override bool Paint()
    {
      /*
      pCtrl->priceStyleParams[0] = lines
      7 columns
      width = 300
      300 / 7 = 42.85 pixels per column
      */

        if (_series.OHLCType == EnumGeral.TipoSerieOHLC.Abertura)
        return false;

      StockChartX chartX = _series._chartPanel._chartX;
      Series high = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Maximo);
      if (high == null || high.Painted || high.RecordCount == 0) return false;
      Series low = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Minimo);
      if (low == null || low.Painted || low.RecordCount == 0) return false;
      Series close = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Ultimo);
      if (close == null || close.Painted || close.RecordCount == 0) return false;

      high.Painted = low.Painted = close.Painted = true;

      double width = chartX.PaintableWidth;

      double lines = chartX.parametrosEstiloPreco[0];
      if (lines > 50 || lines < 1)
      {
        lines = 3;
        chartX.parametrosEstiloPreco[0] = lines;
      }

      _highs = new double[(int)lines];
      _lows = new double[(int)lines];

      double nClose, nHH = 0, nLL = 0;
      const int white = 1;
      const int black = 2;
      double nStart = 0;
      int block = 0; // black or white
      int totalBlocks = 0;

      chartX.xMap = new double[chartX.indexFinal - chartX.indexInicial + 1];
      int cnt = 0;

      chartX.estiloPrecoValor1.Clear();
      chartX.estiloPrecoValor2.Clear();
      chartX.estiloPrecoValor3.Clear();

      // Count columns that will fit on screen
      int n;
      for (n = 0; n < chartX.indexFinal; n++)
      {
        if (!high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue) continue;
        // Calculate N Line Break
        nClose = (double)close[n].Value;


        switch (block)
        {
          case white:
            if (IsNewBlock(-1, nClose))
            {
              nHH = nStart; // New black block
              nLL = nClose;
              nStart = nClose;
              block = black;
              AddBlock(nHH, nLL);
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                totalBlocks++;
              }
            }
            if (IsNewBlock(1, nClose))
            {
              nHH = nClose; // New white block
              nLL = nStart;
              nStart = nClose;
              block = white;
              AddBlock(nHH, nLL);
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                totalBlocks++;
              }
            }
            break;
          case black:
            if (IsNewBlock(1, nClose))
            {
              nHH = nClose; // New white block
              nLL = nStart;
              nStart = nClose;
              block = white;
              AddBlock(nHH, nLL);
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                totalBlocks++;
              }
            }
            if (IsNewBlock(-1, nClose))
            {
              nHH = nStart; // New black block
              nLL = nClose;
              nStart = nClose;
              block = black;
              AddBlock(nHH, nLL);
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                totalBlocks++;
              }
            }
            break;
        }

        if (block != 0) continue; // Prime first block        
        double nClose2 = (double)close[n + 1].Value;
        if (nClose2 > nClose)
        {
          block = white; nHH = nClose2; nLL = nClose; nStart = nClose;
        }
        else
        {
          block = black; nHH = nClose; nLL = nClose2; nStart = nClose2;
        }
        AddBlock(nHH, nLL);
        if (n >= chartX.indexInicial && n <= chartX.indexFinal)
        {
          totalBlocks++;
        }
      }

      chartX.xCount = totalBlocks;
      _highs = new double[(int)lines];
      _lows = new double[(int)lines];
      Color upColor = _series._upColor.HasValue ? _series._upColor.Value : chartX.CandleCorAlta;
      Color downColor = _series._downColor.HasValue ? _series._downColor.Value : chartX.CandleCorBaixa;
      Brush upBrush = new SolidColorBrush(upColor);
      Brush downBrush = new SolidColorBrush(downColor);
      Brush upGradBrush = Utils.CreateFadeVertBrush(upColor, Colors.Black);
      Brush downGradBrush = Utils.CreateFadeVertBrush(downColor, Colors.Black);

      _rects.C = _series._chartPanel._rootCanvas;
      _rects.Start();


      // Paint columns
      block = 0;
      double x = chartX.EspacoEsquerdaGrafico;
      if (totalBlocks == 0) return false;
      double space = width/ totalBlocks;
      totalBlocks = 0;

      // Calculate from beginning, but only show between startIndex and endIndex
      for (n = 0; n < chartX.indexFinal; n++)
      {
        if (!high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue) continue;
        // Calculate N Line Break
        nClose = (double)close[n].Value;

        switch (block)
        {
          case white:
            if (IsNewBlock(-1, nClose))
            {

              // Paint last white block
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, 1, n, close);
                x += space;
              }

              nHH = nStart; // New black block
              nLL = nClose;
              nStart = nClose;
              block = black;
              AddBlock(nHH, nLL);
              totalBlocks++;
            }
            if (IsNewBlock(1, nClose))
            {

              // Paint last black block
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, 1, n, close);
                x += space;
              }

              nHH = nClose; // New white block
              nLL = nStart;
              nStart = nClose;
              block = white;
              AddBlock(nHH, nLL);
              totalBlocks++;
            }
            break;
          case black:
            if (IsNewBlock(1, nClose))
            {

              // Paint last white block
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, -1, n, close);
                x += space;
              }

              nHH = nClose; // New white block
              nLL = nStart;
              nStart = nClose;
              block = white;
              AddBlock(nHH, nLL);
              totalBlocks++;
            }
            if (IsNewBlock(-1, nClose))
            {

              // Paint last black block
              if (n >= chartX.indexInicial && n <= chartX.indexFinal)
              {
                PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, -1, n, close);
                x += space;
              }

              nHH = nStart; // New black block
              nLL = nClose;
              nStart = nClose;
              block = black;
              AddBlock(nHH, nLL);
              totalBlocks++;
            }
            break;
        }

        if (block == 0)
        { // Prime first block        
          double nClose2 = (double)close[n + 1].Value;
          if (nClose2 > nClose)
          {
            block = white; nHH = nClose2; nLL = nClose; nStart = nClose;
          }
          else
          {
            block = black; nHH = nClose; nLL = nClose2; nStart = nClose2;
          }
          AddBlock(nHH, nLL);
          if (n >= chartX.indexInicial && n <= chartX.indexFinal)
          {
            totalBlocks = 1;
          }
          x = chartX.EspacoEsquerdaGrafico;
        }

        // Record the x value
        if (n >= chartX.indexInicial && n <= chartX.indexFinal)
        {
          chartX.xMap[cnt] = x + (space / 2);
          cnt++;
        }
      }

      // Finish last block
      if (block == black)
        PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, -1, n, close);
      else
        PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, 1, n, close);

      _rects.Stop();

      _rects.Do(r => r.ZIndex = ZIndexConstants.PriceStyles1);

      return true;
    }

    private bool IsNewBlock(int direction, double close)
    {
      StockChartX chartX = _series._chartPanel._chartX;
      int lines = (int)chartX.parametrosEstiloPreco[0];
      int exceed = 0;

      for (int n = 0; n < lines; ++n)
      {
        switch (direction)
        {
          case 1:
            if (close > _highs[n] || _highs[n] == 0) exceed++;
            break;
          case -1:
            if (close < _lows[n] || _lows[n] == 0) exceed++;
            break;
        }
      }
      return exceed == lines;
    }

    private void AddBlock(double high, double low)
    {
      StockChartX chartX = _series._chartPanel._chartX;
      int lines = (int)chartX.parametrosEstiloPreco[0] - 1;

      for (int n = 0; n < lines; ++n)
      {
        _highs[n] = _highs[n + 1];
        _lows[n] = _lows[n + 1];
      }

      _highs[lines] = high;
      _lows[lines] = low;
    }

    private int _prevIndex;
    private void PaintBox(Brush upBrush, Brush downBrush, Brush upGradBrush, Brush downGradBrush,
      double x, double space, double top, double bottom, int direction, int index,
      Series close)
    {
      StockChartX chartX = _series._chartPanel._chartX;
      for (int i = _prevIndex + 1; i < index; i++)
      {
        chartX.estiloPrecoValor1.Add(new PriceStyleValue(close[i].TimeStamp, top));
        chartX.estiloPrecoValor2.Add(new PriceStyleValue(close[i].TimeStamp, bottom));
        chartX.estiloPrecoValor3.Add(new PriceStyleValue(close[i].TimeStamp, direction));
      }
      _prevIndex = index;

      Types.RectEx box = new Types.RectEx
      {
        Top = _series._chartPanel.GetY(top),
        Bottom = _series._chartPanel.GetY(bottom),
        Left = x,
        Right = (x + space)
      };
      if (box.Height < 1)
        box.Bottom++;
      if (chartX.Estilo3D)
      {
        switch (direction)
        {
          case 1:
            Utils.DrawRectangle(box, upGradBrush, _rects);
            break;
          default:
            Utils.DrawRectangle(box, downGradBrush, _rects);
            break;
        }
      }
      else
      {
        switch (direction)
        {
          case 1:
            Utils.DrawRectangle(box, upBrush, _rects);
            break;
          default:
            Utils.DrawRectangle(box, downBrush, _rects);
            break;
        }
      }
    }

    public override void RemovePaint()
    {
      _rects.RemoveAll();
    }
  }
}
