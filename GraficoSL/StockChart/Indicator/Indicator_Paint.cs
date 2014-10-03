using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
  public partial class Indicator
  {
    private Path _pathUp;
    private Path _pathDown;
    private Path _pathNormal;

    internal void PaintNew()
    {
      if (!_visible || RecordCount == 0 || RecordCount < _chartPanel._chartX.indexInicial) return;

      Brush strokeUpBrush = new SolidColorBrush(UpColor == null ? _chartPanel._chartX.CandleCorAlta : UpColor.Value);
      Brush strokeDownBrush = new SolidColorBrush(DownColor == null ? _chartPanel._chartX.CandleCorBaixa : DownColor.Value);
      Brush strokeNormalBrush = new SolidColorBrush(_strokeColor);

      bool isOscillator = false;
      if (!ForceOscilatorPaint)
      {
        for (int i = _chartPanel._chartX.indexInicial; i < _chartPanel._chartX.indexFinal; i++)
        {
          if (!this[i].Value.HasValue || this[i].Value >= 0.0) continue;
          isOscillator = true;
          break;
        }
      }
      else
      {
        isOscillator = true;
      }

      if (_pathUp == null)
      {
        var c = _chartPanel._rootCanvas;

        _pathUp = new Path {Tag = this};
        c.Children.Add(_pathUp);
        Canvas.SetZIndex(_pathUp, ZIndexConstants.Indicators1);

        _pathDown = new Path {Tag = this};
        c.Children.Add(_pathDown);
        Canvas.SetZIndex(_pathDown, ZIndexConstants.Indicators1);

        _pathNormal = new Path {Tag = this};
        c.Children.Add(_pathNormal);
        Canvas.SetZIndex(_pathNormal, ZIndexConstants.Indicators1);
      }
      _pathUp.Stroke = strokeUpBrush;
      _pathDown.Stroke = strokeDownBrush;
      _pathNormal.Stroke = strokeNormalBrush;

      double x2 = _chartPanel._chartX.GetXPixel(0);
      double? y1;
      double? y2 = null;

      StringBuilder sbUp = new StringBuilder();
      StringBuilder sbDown = new StringBuilder();
      StringBuilder sbNormal = new StringBuilder();
      StringBuilder currentSb = sbNormal;
      int upCount = 0, downCount = 0, normalCount = 1;

      sbUp.BeginGeometryGroup();
      sbDown.BeginGeometryGroup();
      sbNormal.BeginGeometryGroup();

      int cnt = 0;
      for (int i = _chartPanel._chartX.indexInicial; i < _chartPanel._chartX.indexFinal; i++, cnt++)
      {
        double x1 = _chartPanel._chartX.GetXPixel(cnt);
        double? value = y1 = this[i].Value;
        if (!y1.HasValue) continue;
        y1 = GetY(y1.Value);

        if (i > 0 && i == _chartPanel._chartX.indexInicial)
          y2 = y1.Value;

        value = value.HasValue ? value : 0.0;

        #region brush logic

        if (_chartPanel._chartX.usarCoresLinhasSeries || _upColor.HasValue) // +/- change colors
        {
          if (!isOscillator)
          {
            if (i > 0)
            {
              if (this[i].Value > this[i - 1].Value)
              {
                currentSb = sbUp;
                upCount++;
              }
              else
              {
                if (this[i].Value < this[i - 1].Value)
                {
                  currentSb = sbDown;
                  downCount++;
                }
                else
                {
                  currentSb = sbNormal;
                  normalCount++;
                }
              }
            }
            else
            {
              currentSb = sbNormal;
              normalCount++;
            }
          }
          else
          {
            if (this[i].Value > 0)
            {
              currentSb = sbUp;
              upCount++;
            }
            else
            {
              if (this[i].Value < 0)
              {
                currentSb = sbDown;
                downCount++;
              }
              else
              {
                currentSb = sbNormal;
                normalCount++;
              }
            }
          }
        }

        #endregion

        bool linePainted = true;
        switch (_seriesType)
        {
            case EnumGeral.TipoSeriesEnum.Volume:
            if (this[i].Value.HasValue && y2.HasValue)
              currentSb.AddSegment(x1, (double)y1, x1, GetY(SeriesEntry._min));
            break;
            case EnumGeral.TipoSeriesEnum.Indicador:
            case EnumGeral.TipoSeriesEnum.Linha:
            if (_indicatorType == EnumGeral.IndicatorType.MACDHistograma || isOscillator)
            {
              if (value > 0)
              {
                y1 = GetY(value.Value);
                y2 = GetY(0);
              }
              else
              {
                y1 = GetY(0);
                y2 = GetY(value.Value);
              }

              if (this[i].Value.HasValue)
                currentSb.AddSegment(x1, (double)y1, x1, (double)y2);
            }
            else
            {
              if (i > _chartPanel._chartX.indexInicial)
              {
                linePainted = false;
                if ( /*this[i].Value.HasValue && */y2.HasValue && Math.Abs(x2 - x1) > 1)
                {
                  currentSb.AddSegment(x1, y1.Value, x2, y2.Value);
                  linePainted = true;
                }
              }
            }
            break;
          default:
            throw new IndicatorException("Indicator has an unsuported series type.", this);
        }

        if (linePainted || !y2.HasValue)
          y2 = y1;
        if (linePainted || (i == _chartPanel._chartX.indexInicial))
          x2 = x1;
      }

      sbUp.EndGeometryGroup();
      sbDown.EndGeometryGroup();
      sbNormal.EndGeometryGroup();

#if WPF
      if (upCount > 0)
        _pathUp.Data = (GeometryGroup)XamlReader.Parse(sbUp.ToString());
      if (downCount > 0)
        _pathDown.Data = (GeometryGroup)XamlReader.Parse(sbDown.ToString());
      if (normalCount > 0)
        _pathNormal.Data = (GeometryGroup)XamlReader.Parse(sbNormal.ToString());
#endif

      //      Debug.WriteLine("Indicaror " + FullName + " painted. Lines " + _lines.Count);
      if (_selected)
        ShowSelection();
    }


    internal override void Paint()
    {
      if (!_visible || RecordCount == 0 || RecordCount < _chartPanel._chartX.indexInicial) return;

      _lines.C = _chartPanel._rootCanvas;
      _lines.Start();

      Brush strokeUpBrush = new SolidColorBrush(UpColor == null ? _chartPanel._chartX.CandleCorAlta : UpColor.Value);
      Brush strokeDownBrush = new SolidColorBrush(DownColor == null ? _chartPanel._chartX.CandleCorBaixa : DownColor.Value);
      Brush strokeNormalBrush = new SolidColorBrush(_strokeColor);
      Brush currentBrush = new SolidColorBrush(_strokeColor);

      bool isOscillator = false;
      if (!ForceOscilatorPaint)
      {
        if (!ForceLinearChart)
        {
          for (int i = _chartPanel._chartX.indexInicial; i < _chartPanel._chartX.indexFinal; i++)
          {
            if (!this[i].Value.HasValue || this[i].Value >= 0.0) continue;

              switch(this.IndicatorType)
              {
                  case EnumGeral.IndicatorType.MediaMovelSimples:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                        isOscillator = false;
                      break;

                  case EnumGeral.IndicatorType.MediaMovelExponencial:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                          isOscillator = false;
                      break;

                  case EnumGeral.IndicatorType.MediaMovelTriangular:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                          isOscillator = false;
                      break;

                  case EnumGeral.IndicatorType.MediaMovelSerieTempo:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                          isOscillator = false;
                      break;

                  case EnumGeral.IndicatorType.MediaMovelVariavel:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                          isOscillator = false;
                      break;

                  case EnumGeral.IndicatorType.MediaMovelPonderada:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                          isOscillator = false;
                      break;

                  case EnumGeral.IndicatorType.VIDYA:
                      if (this.GetParameterValue(0).ToString().Contains("TRIX"))
                          isOscillator = false;
                      break;    

              }
            
            break;
          }
        }
      }
      else
      {
        isOscillator = true;
      }

      double x2 = _chartPanel._chartX.GetXPixel(0);
      double? y1;
      double? y2 = null;

      int cnt = 0;
      for (int i = _chartPanel._chartX.indexInicial; i < _chartPanel._chartX.indexFinal; i++, cnt++)
      {
        double x1 = _chartPanel._chartX.GetXPixel(cnt);
        double? value = y1 = this[i].Value;
        if (!y1.HasValue) continue;
        y1 = GetY(y1.Value);

        if (i > 0 && i == _chartPanel._chartX.indexInicial)
          y2 = y1.Value;

        value = value.HasValue ? value : 0.0;

        #region brush logic

        if (_chartPanel._chartX.usarCoresLinhasSeries || _upColor.HasValue) // +/- change colors
        {
          if (!isOscillator)
          {
            if (i > 0)
            {
              if (this[i].Value > this[i - 1].Value)
                currentBrush = strokeUpBrush;
              else
                currentBrush = this[i].Value < this[i - 1].Value ? strokeDownBrush : strokeNormalBrush;
            }
            else
              currentBrush = strokeNormalBrush;
          }
          else
          {
            if (this[i].Value > 0)
              currentBrush = strokeUpBrush;
            else
              currentBrush = this[i].Value < 0 ? strokeDownBrush : strokeNormalBrush;
          }
        }

        #endregion

        bool linePainted = true;
        switch (_seriesType)
        {
            case EnumGeral.TipoSeriesEnum.Volume:
            if (this[i].Value.HasValue && y2.HasValue)
              DrawLine(x1, (double)y1, x1, GetY(SeriesEntry._min), currentBrush);
            break;
          case EnumGeral.TipoSeriesEnum.Indicador:
          case EnumGeral.TipoSeriesEnum.Linha:
            if (_indicatorType == EnumGeral.IndicatorType.MACDHistograma || isOscillator)
            {
              if (value > 0)
              {
                y1 = GetY(value.Value);
                y2 = GetY(0);
              }
              else
              {
                y1 = GetY(0);
                y2 = GetY(value.Value);
              }

              if (this[i].Value.HasValue)
                DrawLine(x1, (double)y1, x1, (double)y2, currentBrush);
            }
            else
            {
              if (i > _chartPanel._chartX.indexInicial)
              {
                linePainted = false;
                if ( /*this[i].Value.HasValue && */y2.HasValue && Math.Abs(x2 - x1) > 1)
                {
                  DrawLine(x1, y1.Value, x2, y2.Value, currentBrush);
                  linePainted = true;
                }
              }
            }
            break;
          default:
            throw new IndicatorException("Indicator has an unsuported series type.", this);
        }

        if (linePainted || !y2.HasValue)
          y2 = y1;
        if (linePainted || (i == _chartPanel._chartX.indexInicial))
          x2 = x1;
      }

      _lines.Stop();
      _lines.Do(l =>
                  {
                    l.ZIndex = ZIndexConstants.Indicators1;
                    l._line.Tag = this;
#if WPF
                    l._line.ToolTip = _tooltip ?? (_tooltip = new ToolTip(this));
#endif
                  });

      //      Debug.WriteLine("Indicaror " + FullName + " painted. Lines " + _lines.Count);
      if (_selected)
        ShowSelection();
    }

    internal override void RemovePaint()
    {
      _lines.C = _chartPanel._rootCanvas;
      _lines.RemoveAll();
    }
  }
}
