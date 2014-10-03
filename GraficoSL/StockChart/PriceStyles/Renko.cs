using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
  internal partial class Renko : Style
  {
    private readonly PaintObjectsManager<Rectangle> _rects = new PaintObjectsManager<Rectangle>();

    public Renko(Traderdata.Client.Componente.GraficoSL.StockChart.Stock stock) : base(stock)
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
        if (_series.OHLCType != EnumGeral.TipoSerieOHLC.Ultimo) return false;
      Series close = _series;

      StockChartX chartX = _series._chartPanel._chartX;
      double width = chartX.PaintableWidth;

      double boxSize = chartX.parametrosEstiloPreco[0];
      if (boxSize > 50 || boxSize < 0.0001)
        boxSize = 1;

      chartX.parametrosEstiloPreco[0] = boxSize;

      double nClose, nHH = 0, nLL = 0;
      const int white = 1;
      const int black = 2;
      int brick = 0; // black or white
      int totalBricks = 0;


      chartX.xMap = new double[chartX.indexFinal - chartX.indexInicial + 1];
      int cnt = 0;

      chartX.estiloPrecoValor1.Clear();
      chartX.estiloPrecoValor2.Clear();
      chartX.estiloPrecoValor3.Clear();

      int n;
      // Calculate from beginning, but only show between startIndex and endIndex
      for (n = 0; n < chartX.indexFinal; n++)
      {
        if (!close[n].Value.HasValue) continue;
        nClose = close[n].Value.Value;


        if (brick == 0)
        { // Prime first brick        
          double nClose2 = close[n + 1].Value.Value;
          if (nClose2 > nClose)
          {
            brick = white; nHH = nClose + boxSize; nLL = nClose;
          }
          else
          {
            brick = black; nHH = nClose2; nLL = nClose2 - boxSize;
          }
          if (n >= chartX.indexInicial && n <= chartX.indexFinal)
          {
            totalBricks = 1;
          }
        }


        if (nClose < nLL - boxSize)
        {
          brick = black;
          nHH = nLL;
          nLL = nHH - boxSize;
          if (n >= chartX.indexInicial && n <= chartX.indexFinal)
            totalBricks++;
        }
        else if (nClose > nHH + boxSize)
        {
          brick = white;
          nLL = nHH;
          nHH = nLL + boxSize;
          if (n >= chartX.indexInicial && n <= chartX.indexFinal)
            totalBricks++;
        }
      }

      chartX.xCount = totalBricks;

      // Paint columns
      brick = 0;
      double x = chartX.EspacoEsquerdaGrafico;
      if (totalBricks == 0) return false;
      double space = width / totalBricks;

      Color upColor = _series._upColor.HasValue ? _series._upColor.Value : chartX.CandleCorAlta;
      Color downColor = _series._downColor.HasValue ? _series._downColor.Value : chartX.CandleCorBaixa;
      Brush upBrush = new SolidColorBrush(upColor);
      Brush downBrush = new SolidColorBrush(downColor);
      Brush upGradBrush = Utils.CreateFadeVertBrush(upColor, Colors.Black);
      Brush downGradBrush = Utils.CreateFadeVertBrush(downColor, Colors.Black);

      _rects.C = _series._chartPanel._rootCanvas;
      _rects.Start();

      totalBricks = 0;

      for (n = 0; n < chartX.indexFinal; n++)
      {
        if (close[n].Value.HasValue)
        {
          // Calculate Renko
          nClose = (double)close[n].Value;


          if (brick == 0)
          { // Prime first brick        
            double nClose2 = (double)close[n + 1].Value;
            if (nClose2 > nClose)
            {
              brick = white; nHH = nClose + boxSize; nLL = nClose;
            }
            else
            {
              brick = black; nHH = nClose2; nLL = nClose2 - boxSize;
            }
            if (n >= chartX.indexInicial && n <= chartX.indexFinal)
            {
              totalBricks = 1;
            }
            x = chartX.EspacoEsquerdaGrafico;
          }

          if (nClose < nLL - boxSize)
          {

            // Paint last white brick
            if (n >= chartX.indexInicial && n <= chartX.indexFinal)
            {
              PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, brick);
              totalBricks++;
              x += space;
            }

            brick = black;
            nHH = nLL;
            nLL = nHH - boxSize;

          }
          else if (nClose > nHH + boxSize)
          {
            // Paint last black brick       
            if (n >= chartX.indexInicial && n <= chartX.indexFinal)
            {
              PaintBox(upBrush, downBrush, upGradBrush, downGradBrush, x, space, nHH, nLL, brick);
              totalBricks++;
              x += space;
            }
            brick = white;
            nLL = nHH;
            nHH = nLL + boxSize;
          }

          // Record the x value
          if (n >= chartX.indexInicial && n <= chartX.indexFinal)
          {
            chartX.xMap[cnt] = x + (space / 2);
            cnt++;
          }
        }

        chartX.estiloPrecoValor1.Add(new PriceStyleValue(close[n].TimeStamp, nHH));
        chartX.estiloPrecoValor2.Add(new PriceStyleValue(close[n].TimeStamp, nLL));
        chartX.estiloPrecoValor3.Add(new PriceStyleValue(close[n].TimeStamp, brick == white ? 1 : -1));
      }

      _rects.Stop();

      _rects.Do(r => r.ZIndex = r.ZIndex = ZIndexConstants.PriceStyles1);

      return true;
    }

    private void PaintBox(Brush upBrush, Brush downBrush, Brush upGradBrush, Brush downGradBrush, double x, double space, double top, double bottom, int direction)
    {
      Types.RectEx box = new Types.RectEx
                           {
                             Top = _series._chartPanel.GetY(top),
                             Bottom = _series._chartPanel.GetY(bottom),
                             Left = x,
                             Right = (x + space)
                           };
      StockChartX chartX = _series._chartPanel._chartX;
      if (chartX.Estilo3D)
      {
        switch (direction)
        {
          case 1:
            Utils.DrawRectangle(box, upGradBrush, _rects);
            break;
          case 2:
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
          case 2:
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
