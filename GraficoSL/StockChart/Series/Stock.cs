using System;
using System.Diagnostics;
using Traderdata.Client.Componente.GraficoSL.StockChart.PriceStyles;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  internal partial class Stock : Series
  {
    internal PriceStyles.Stock _priceStyleStock;

    internal Stock(string name, EnumGeral.TipoSeriesEnum seriesType, EnumGeral.TipoSerieOHLC seriesTypeOHLC, ChartPanel chartPanel)
      : base (name, seriesType, seriesTypeOHLC, chartPanel)
    {
      Init();

      _chartPanel._chartX.ChartReseted += ChartX_OnChartReseted;

      _priceStyleStock = null;
    }

    private void ChartX_OnChartReseted(object sender, EventArgs e)
    {
      _priceStyle = null;
      _darvasBoxes = null;
      _priceStyleType = EnumGeral.EstiloPrecoEnum.Padrao;
      _seriesTypeType = EnumGeral.TipoSeriesEnum.Linha;
    }

    private Style _priceStyle;
    private EnumGeral.EstiloPrecoEnum _priceStyleType;
    private EnumGeral.TipoSeriesEnum _seriesTypeType;
    private DarvasBoxes _darvasBoxes;

    internal override void Paint()
    {
      //if (_painted) return;

      Style ps = null;
      if (_priceStyleType != _chartPanel._chartX.estiloPreco || _seriesTypeType != _seriesType)
      {
          if (_chartPanel._chartX.estiloPreco != EnumGeral.EstiloPrecoEnum.Padrao)
        {
          switch (_chartPanel._chartX.estiloPreco)
          {
              case EnumGeral.EstiloPrecoEnum.Kagi:
              ps = new Kagi(this);
              break;
              case EnumGeral.EstiloPrecoEnum.CandleVolume:
              case EnumGeral.EstiloPrecoEnum.EquiVolume:
              case EnumGeral.EstiloPrecoEnum.EquiVolumeShadow:
              ps = new EquiVolume(this);
              break;
              case EnumGeral.EstiloPrecoEnum.PontoEFigura:
              ps = new PointAndFigure(this);
              break;
              case EnumGeral.EstiloPrecoEnum.Renko:
              ps = new Renko(this);
              break;
              case EnumGeral.EstiloPrecoEnum.ThreeLineBreak:
              ps = new ThreeLineBreak(this);
              break;
              case EnumGeral.EstiloPrecoEnum.HeikinAshi:
              ps = new HeikinAshi(this);
              break;
          }
        }
        else
        {
          switch (_seriesType)
          {
              case EnumGeral.TipoSeriesEnum.Candle:
              ps = new Candles(this);
              break;
              case EnumGeral.TipoSeriesEnum.BarraHLC:
              case EnumGeral.TipoSeriesEnum.Barra:
              ps = new PriceStyles.Stock(this);
              break;
              case EnumGeral.TipoSeriesEnum.Linha:
              ps = new Linear(this);
              break;
          }
        }
        if (_priceStyle != null)
          _priceStyle.RemovePaint();
      }

      if (_darvasBoxes != null)
        _darvasBoxes.RemovePaint();
      if (_chartPanel._chartX.estiloPreco == EnumGeral.EstiloPrecoEnum.Padrao || _chartPanel._chartX.estiloPreco == EnumGeral.EstiloPrecoEnum.HeikinAshi)
      {
        if (_darvasBoxes == null)
          _darvasBoxes = new DarvasBoxes(this);
        _darvasBoxes.SetSeriesStock(this);
        if (_chartPanel._chartX.darwasBoxes)
          _darvasBoxes.Paint();
      }

      if (_priceStyle != null || ps != null)
      {
        (ps ?? _priceStyle).SetStockSeries(this);
        if (!(ps ?? _priceStyle).Paint()) return;
      }
      if (Selected)
        ShowSelection();
      if (ps == null) return;
      _priceStyle = ps;
      _priceStyleType = _chartPanel._chartX.estiloPreco;
      _seriesTypeType = _seriesType;
    }

    internal override void MoveToPanel(ChartPanel chartPanel)
    {
      if (_priceStyle == null) return;

      Debug.Assert(_priceStyleType == EnumGeral.EstiloPrecoEnum.Padrao || _priceStyleType == EnumGeral.EstiloPrecoEnum.HeikinAshi);

      _priceStyle.RemovePaint();
      _chartPanel.DeleteSeries(this);
      _chartPanel = chartPanel;
      _chartPanel.AddSeries(this);

      base.MoveToPanel(chartPanel);
    }

    internal override void SetStrokeColor()
    {
    }

    internal override void SetStrokeThickness()
    {
      if (_priceStyleStock != null)
        _priceStyleStock.SetStrokeThickness(_strokeThickness);
    }

    internal override void RemovePaint()
    {
      if (_priceStyle != null)
        _priceStyle.RemovePaint();
    }
  }
}
