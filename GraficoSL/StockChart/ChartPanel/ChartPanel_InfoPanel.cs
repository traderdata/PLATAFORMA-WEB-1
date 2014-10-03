using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.Enum;
#if WPF
using System.Windows.Input;
#endif
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    internal class ChartPanelInfoPanelAble : IInfoPanelAble
    {
        internal ChartPanel ChartPanel { get; set; }
        public IEnumerable<InfoPanelItem> InfoPanelItems
        {
            get
            {
                yield return new ChartPanelInfoPanelItemX(ChartPanel);
                yield return new ChartPanelInfoPanelItemY(ChartPanel);

                foreach (Series series in ChartPanel.AllSeriesCollection)
                {
                    yield return new SeriesInfoPanelItem(series);
                }
            }
        }
    }

    internal class ChartPanelInfoPanelItemX : InfoPanelItem
    {
        public ChartPanel ChartPanelOwner { get; private set; }

        public ChartPanelInfoPanelItemX(ChartPanel chartPanel)
        {
            ChartPanelOwner = chartPanel;
        }

        public override string Caption
        {
            get { return "Barra (X)"; }
        }

        public override string Value(int record)
        {
            return (record + _infoPanel._chartX.indexInicial).ToString();
        }
        public override string Value()
        {
            return ((int)_infoPanel.GetReverseX() + _infoPanel._chartX.indexInicial).ToString();
        }
        public override string Sufixo()
        {
            return "";
        }

    }

    internal class ChartPanelInfoPanelItemY : InfoPanelItem
    {
        public ChartPanel ChartPanelOwner { get; private set; }

        public ChartPanelInfoPanelItemY(ChartPanel chartPanel)
        {
            ChartPanelOwner = chartPanel;
        }

        public override string Caption
        {
            get { return "Escala (Y)"; }
        }

        public override string Value(int record)
        {
            return string.Format(ChartPanelOwner.FormatYValueString, ChartPanel.MouseY);            
        }

        public override string Value()
        {
            return string.Format(ChartPanelOwner.FormatYValueString, ChartPanel.MouseY);            
        }

        public override string Sufixo()
        {
            return "";
        }
    }

    internal class SeriesInfoPanelItem : InfoPanelItem
    {
        public Series Series { get; private set; }
        public SeriesInfoPanelItem(Series series)
        {
            if (series._visibleInfoPanel)
                Series = series;
            else
                Series = null;
        }

        public override string Caption
        {            
            get {
                if (Series != null)
                    return Series.Title;
                else
                    return "";
            }
        }

        public override string Sufixo()
        {
            if (Series != null)
                return Series.GetSufixoInfoPanel();
            else
                return "";
        }

        public override string Value(int record)
        {
            if (Series != null)
            {
                int index = record + _infoPanel._chartX.indexInicial;
                if (index < 0 || index >= _infoPanel._chartX.indexFinal) return null;

                double? value = Series[index].Value;
                return !value.HasValue ? null : string.Format(Series._chartPanel.FormatYValueString, value.Value);
            }
            else
                return "";
        }

        public override string Value()
        {
            if (Series != null)
            {
                int index = (int)_infoPanel.GetReverseX() + _infoPanel._chartX.indexInicial;
                if (index < 0 || index >= _infoPanel._chartX.indexFinal) return null;

                double? value = Series[index].Value;
                return !value.HasValue ? null : string.Format(Series._chartPanel.FormatYValueString, value.Value);
            }
            else
                return "";

        }

        
    }
}

