using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles
{
    internal partial class DarvasBoxes : Style
    {
        private readonly PaintObjectsManager<Rectangle> _rects = new PaintObjectsManager<Rectangle>();
        private readonly PaintObjectsManager<Rectangle3D> _rects3D = new PaintObjectsManager<Rectangle3D>();

        public DarvasBoxes(Traderdata.Client.Componente.GraficoSL.StockChart.Stock stock)
            : base(stock)
        {
        }

        public void SetSeriesStock(Series stock)
        {
            _series = (Traderdata.Client.Componente.GraficoSL.StockChart.Stock)stock;
        }

        /// <summary>
        /// Método responsável por desenhar o darva box.
        /// </summary>
        /// <returns></returns>
        public override bool Paint()
        {
            //Limpando os valores de estilo
            StockChartX chartX = _series._chartPanel._chartX;
            chartX.estiloPrecoValor1.Clear();
            chartX.estiloPrecoValor2.Clear();
            chartX.estiloPrecoValor3.Clear();

            //Se o tipo da serie for diferente de Ulitmo, deve encerrar o metodo
            if (_series.OHLCType != EnumGeral.TipoSerieOHLC.Ultimo) 
                return false;

            //Obtendo serie de fechamento
            Series close = _series;

            //Obtendo serie de Minimo
            Series low = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Minimo);

            //Se a serie for invalida, devo encerrar o metodo
            if (low == null) 
                return false;

            //Obtendo serie de Maximo
            Series high = chartX.GetSeriesOHLCV(_series, EnumGeral.TipoSerieOHLC.Maximo);

            //Se a serie for invalida, devo encerrar o metodo
            if (high == null) 
                return false;

            //Garantindo que o darva boxes nunca seja 1 quando o grafico eh semilog
            if ((chartX.EscalaTipo == EnumGeral.TipoEscala.Semilog) && (chartX.darvasPercent == 1))
                chartX.darvasPercent = 0.99;

            /*
            Initial box top is the high of day 1.

            The first step is to find a new high that is higher than the high of day 1.
            The high can be found anytime - even after 5 days.
            But once the bottom has been found, the box is complete.

            To find the bottom, the low must be after day 2 of the day the last box
            top was found and must be lower than the low of original day 1 low.

            The bottom is always found last and a new high may not be found once 
            the bottom is locked in - the Darvas box is complete then.

            A new box is started when the price breaks out of top or bottom, 

            The bottom stop loss box is drawn as a percentage of the last price.
            */

            Brush brDarvas = new SolidColorBrush(Color.FromArgb(0xFF, 0, 0, 255));
            Brush brDarvaseIncomplete = new SolidColorBrush(Color.FromArgb(0xFF, 100, 115, 255));
            Brush topLeft = new SolidColorBrush(Color.FromArgb(0xFF, 240, 240, 240));
            Brush bottomRight = new SolidColorBrush(Color.FromArgb(0xFF, 150, 150, 150));
            Brush brGradStopLoss = Utils.CreateFadeVertBrush(Color.FromArgb(0xFF, 255, 255, 255), Colors.Red);

            double boxTop = 0;
            double boxBottom = 0;
            int bottomFound = 0;
            int cnt = 0;
            int start = 0;
            int state = 0;

            _rects.C = _rects3D.C = _series._chartPanel._rootCanvas;
            _rects3D.Start();
            _rects.Start();


            for (int n = chartX.indexInicial; n < chartX.indexFinal; n++, cnt++)
            {
                if (!close[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue) 
                    continue;

                double x1;
                double x2;
                double y1;
                double y2;

                if (n == chartX.indexFinal - 1)
                {
                    x1 = chartX.GetXPixel(start);
                    x2 = chartX.GetXPixel(cnt);
                    y1 = _series._chartPanel.GetY(boxTop);
                    y2 = _series._chartPanel.GetY(boxBottom);

                    if (state == 5) // draw the last COMPLETED box
                    {
                        Rectangle rect = Utils.DrawRectangle(x1, y1, x2 + 2, y2, brDarvas, _rects);
                        rect._rectangle.Opacity = 0.5;

                        Utils.Draw3DRectangle(x1, y1, x2 + 2, y2, topLeft, bottomRight, _rects3D);
                    }
                    else if (bottomFound > 0)
                    {
                        Rectangle rect = Utils.DrawRectangle(x1, y1, x2 + 2, y2, brDarvaseIncomplete, _rects);
                        rect._rectangle.Opacity = 0.5;

                        Utils.Draw3DRectangle(x1, y1, x2 + 2, y2, topLeft, bottomRight, _rects3D);
                    }

                    // Gradient stop loss box
                    double valorAux = boxBottom - (boxBottom * chartX.darvasPercent);

                    double y3 = _series._chartPanel.GetY(valorAux);
                    if (y3 < y2)
                        y3 = y2;

                    Utils.DrawRectangle(x1, y2, x2 + 2, y3, brGradStopLoss, _rects);
                    
                    break;
                }

                if (state == 0)
                { // Start of a new box
                    // Save new box top and bottom
                    start = cnt;
                    bottomFound = 0;
                    boxTop = high[n].Value.Value;
                    boxBottom = -1;
                    state = 1;
                }

                switch (state)
                {
                    case 1:
                        if (high[n].Value.Value > boxTop)
                        {
                            boxTop = high[n].Value.Value;
                            state = 1;
                        }
                        else
                        {
                            state = 2;
                        }
                        break;
                    case 2:
                        if (high[n].Value.Value > boxTop)
                        {
                            boxTop = high[n].Value.Value;
                            state = 1;
                        }
                        else
                        {
                            bottomFound = n;
                            boxBottom = low[n].Value.Value;
                            state = 3;
                        }
                        break;
                    case 3:
                        if (high[n].Value.Value > boxTop)
                        {
                            boxTop = high[n].Value.Value;
                            state = 1;
                        }
                        else
                        {
                            if (low[n].Value.Value < boxBottom)
                            {
                                boxBottom = low[n].Value.Value;
                                bottomFound = n;
                                state = 3;
                            }
                            else
                            {
                                state = 4;
                            }
                        }
                        break;
                    case 4:
                        if (high[n].Value.Value > boxTop)
                        {
                            boxTop = high[n].Value.Value;
                            state = 1;
                        }
                        else
                        {
                            if (low[n].Value.Value < boxBottom)
                            {
                                boxBottom = low[n].Value.Value;
                                bottomFound = n;
                                state = 3;
                            }
                            else
                            {
                                state = 5; // Darvas box is complete
                            }
                        }
                        break;
                }

                if (state != 5) 
                    continue;


                if (low[n].Value.Value >= boxBottom && high[n].Value.Value <= boxTop) 
                    continue;

                x1 = chartX.GetXPixel(start);
                x2 = chartX.GetXPixel(cnt);
                y1 = _series._chartPanel.GetY(boxTop);
                y2 = _series._chartPanel.GetY(boxBottom);

                Rectangle rectangle = Utils.DrawRectangle(x1, y1, x2, y2, brDarvas, _rects);
                rectangle._rectangle.Opacity = 0.5;

                Utils.Draw3DRectangle(x1, y1, x2, y2, topLeft, bottomRight, _rects3D);

                double seriesValueAux = boxBottom - (boxBottom * chartX.darvasPercent);
                Utils.DrawRectangle(x1, y2, x2, _series._chartPanel.GetY(seriesValueAux), brGradStopLoss, _rects);

                state = 0;
                cnt--;
                n--;
            }
            _rects3D.Stop();
            _rects.Stop();

            _rects.Do(r => r.ZIndex = ZIndexConstants.DarvasBoxes1);
            _rects3D.Do(r => r.ZIndex = ZIndexConstants.DarvasBoxes2);

            return true;
        }

        public override void RemovePaint()
        {
            _rects3D.RemoveAll();
            _rects.RemoveAll();
        }
    }
}
