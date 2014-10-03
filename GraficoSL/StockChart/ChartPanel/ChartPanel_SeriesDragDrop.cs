using System.Windows.Input;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.Enum;
using System;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public partial class ChartPanel
    {
        private MoveSeriesIndicator.MoveStatusEnum CanStartMoveSeries(Series series)
        {
            //a series can be moved if
            //1. single series on a panel - only to an existing panel
            //2. if a panel has more series - then to a new panel, or to an existing one
            if (series == null || !series.Selectable) return MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;

            if (series._seriesType == EnumGeral.TipoSeriesEnum.Candle)
            {
                if (_series.Count > 4) return MoveSeriesIndicator.MoveStatusEnum.MoverNovoPainel;
                //candle OHLC group, must have at least 5 series
                return _series.Count == 4 ? MoveSeriesIndicator.MoveStatusEnum.MoverPainelExistente : MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;
            }
            if (series._seriesType == EnumGeral.TipoSeriesEnum.Barra ||
              series._seriesType == EnumGeral.TipoSeriesEnum.BarraHLC)
            {
                if (_series.Count > 3) return MoveSeriesIndicator.MoveStatusEnum.MoverNovoPainel;
                return _series.Count == 3 ? MoveSeriesIndicator.MoveStatusEnum.MoverPainelExistente : MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover; //HLC group
            }
            //otherwise is single line series
            if (_series.Count > 1) return MoveSeriesIndicator.MoveStatusEnum.MoverNovoPainel;
            return _series.Count > 0 ? MoveSeriesIndicator.MoveStatusEnum.MoverPainelExistente : MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;
        }

        private void StartMoveSeries(Series series, int clickCount)
        {
            if (_seriesSelected != null && _seriesSelected != series)
                _seriesSelected.HideSelection();

            _seriesSelected = series;
            _seriesSelected.ShowSelection();

            if (_lineStudySelected != null)
                _lineStudySelected.Selected = false;

            Indicator indicator = series as Indicator;

            if (indicator != null)
                _chartX.FireIndicatorLeftClick(indicator);

            if (clickCount == 2)
            {
                if (indicator != null)
                {
                    _leftMouseDown = false;
                    if (!_chartX.FireIndicatorDoubleClick(indicator) && indicator.UserParams) //show series prop dialog
                        indicator.ShowParametersDialog();
                }
                _chartX.FireSeriesDoubleClick(series);
            }
            else
                _rootCanvas.CaptureMouse();
        }

        private MoveSeriesIndicator.MoveStatusEnum _moveStatusEnum;
        private ChartPanel _chartPanelToMoveTo;
        private void SeriesMoving(MouseEventArgs e)
        {
            MoveSeriesIndicator.MoveStatusEnum moveStatusEnum = CanStartMoveSeries(_seriesSelected);

            object o;
            EnumGeral.ObjetoSobCursor objectFromCursor = _chartX.GetObjectFromCursor(out o);

            _chartPanelToMoveTo = null;

            if (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover)
                _moveStatusEnum = MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;
            else switch (objectFromCursor)
                {
                    case EnumGeral.ObjetoSobCursor.PanelRightYAxis:
                    case EnumGeral.ObjetoSobCursor.PanelLeftYAxis:
                        if (_chartX.MaximizedPanel != null)
                            _moveStatusEnum = MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;
                        else
                            _moveStatusEnum = moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.MoverNovoPainel
                                                ? moveStatusEnum
                                                : MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;
                        break;
                    case EnumGeral.ObjetoSobCursor.PanelRightNonPaintableArea:
                    case EnumGeral.ObjetoSobCursor.PanelPaintableArea:
                    case EnumGeral.ObjetoSobCursor.PanelLeftNonPaintableArea:
                        _chartPanelToMoveTo = (ChartPanel)o;
                        _moveStatusEnum = _chartPanelToMoveTo._index != _index
                                            ? MoveSeriesIndicator.MoveStatusEnum.MoverPainelExistente
                                            : MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover;
                        break;
                }

            _panelsContainer.MostraMensagemMovimentacaoIndicador(_panelsContainer.ToPanelsHolder(e), _moveStatusEnum);
        }

        internal void MoveSeriesTo(Series seriesToMove, ChartPanel chartPanelToMoveTo, MoveSeriesIndicator.MoveStatusEnum moveStatusEnum)
        {
            try
            {
                _panelsContainer.EscondeMensagemMovimentacaoIndicador();
                _chartX.Status = StockChartX.StatusGrafico.Preparado;
                seriesToMove.HideSelection();
                _rootCanvas.ReleaseMouseCapture();

                if (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover) return;

                //Parte modificada pois dava erro as vezes
                //ChartPanel chartPanel = (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.MoverNovoPainel ? null : chartPanelToMoveTo) ??
                //                        _chartX.AddChartPanel();

                ChartPanel chartPanel = null;

                if (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.MoverNovoPainel)
                {
                    chartPanel = _chartX.AddChartPanel();
                    chartPanel.Background = _chartX.GetPanelBySeriesName(seriesToMove.Name).Background;

                    //TODO: Além de passar o fundo do chart anterior, devo passar as outras configuracoes
                }
                else
                    chartPanel = chartPanelToMoveTo;

                if (chartPanel != null)
                {
                    chartPanel._enforceSeriesSetMinMax = true;

                    seriesToMove.MoveToPanel(chartPanel);
                    if ((seriesToMove._seriesType != EnumGeral.TipoSeriesEnum.Candle &&
                         seriesToMove._seriesType != EnumGeral.TipoSeriesEnum.Barra) &&
                        seriesToMove._seriesType != EnumGeral.TipoSeriesEnum.BarraHLC)
                    {
                        _chartX.FireSeriesMoved(seriesToMove, _series.Count == 0 ? -1 : Index, chartPanel.Index);

                        _chartX.UpdateByTimer();
                        return;
                    }
                    //series is a part from (O)|HLC group, move the others too
                    Series seriesRelated;
                    if (seriesToMove.OHLCType != EnumGeral.TipoSerieOHLC.Abertura &&
                        (seriesRelated = GetSeriesOHLCV(seriesToMove, EnumGeral.TipoSerieOHLC.Abertura)) != null)
                    {
                        seriesRelated.MoveToPanel(chartPanel);
                    }
                    if (seriesToMove.OHLCType != EnumGeral.TipoSerieOHLC.Maximo &&
                        (seriesRelated = GetSeriesOHLCV(seriesToMove, EnumGeral.TipoSerieOHLC.Maximo)) != null)
                    {
                        seriesRelated.MoveToPanel(chartPanel);
                    }
                    if (seriesToMove.OHLCType != EnumGeral.TipoSerieOHLC.Minimo &&
                        (seriesRelated = GetSeriesOHLCV(seriesToMove, EnumGeral.TipoSerieOHLC.Minimo)) != null)
                    {
                        seriesRelated.MoveToPanel(chartPanel);
                    }
                    if (seriesToMove.OHLCType != EnumGeral.TipoSerieOHLC.Ultimo &&
                        (seriesRelated = GetSeriesOHLCV(seriesToMove, EnumGeral.TipoSerieOHLC.Ultimo)) != null)
                    {
                        seriesRelated.MoveToPanel(chartPanel);
                    }

                    _chartX.FireSeriesMoved(seriesToMove, _series.Count == 0 ? -1 : Index, chartPanel.Index);

                    _chartX.UpdateByTimer();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}


