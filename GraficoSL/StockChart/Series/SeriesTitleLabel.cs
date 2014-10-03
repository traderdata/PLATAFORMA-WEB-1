using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    /// <summary>
    /// internal usage
    /// </summary>
    public partial class SeriesTitleLabel : INotifyPropertyChanged
    {
        /// <summary>
        /// internal usage
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Series _series;

        /// <summary>
        /// internal usage
        /// </summary>
        public SeriesTitleLabel(Series series)
        {
            _series = series;
            _series.PropertyChanged += Series_OnPropertyChanged;
            _series._chartPanel._chartX.PropertyChanged += ChartX_OnPropertyChanged;
        }

        internal void UnSubscribe()
        {
            _series.PropertyChanged -= Series_OnPropertyChanged;
            _series._chartPanel._chartX.PropertyChanged -= ChartX_OnPropertyChanged;
        }

        internal Series Series
        {
            get { return _series; }
        }

        private void ChartX_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StockChartX.Property_EndIndex)
            {
                OnPropertyChanged("Title");
            }
        }

        private void Series_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Series.PropertyStrokeBrush:
                case Series.PropertyTitleBrush:
                    OnPropertyChanged("SeriesStroke");
                    break;
                case Series.PropertyLastValue:
                    OnPropertyChanged("Title");
                    break;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// internal usage
        /// </summary>
        public string Title
        {
            get
            {
                Debug.WriteLine("TitleSeries Series " + _series + " YValueString " + _series._seriesIndex);
                
                //string result = return _series.Title + " = " +
                //               string.Format(_series._chartPanel.FormatYValueString,
                //                         _series.DM.LastVisibleDataEntry(_series._seriesIndex).Value);


                if (_series._visibleTitlebar)
                    if (_series._titleBarCaption != "!")
                        return _series._titleBarCaption + " = " +
                               string.Format(_series._chartPanel.FormatYValueString,
                                         _series.DM.LastVisibleDataEntry(_series._seriesIndex).Value + _series._titleBarCaptionComplemento);
                    else
                        return _series.Title + " = " +
                               string.Format(_series._chartPanel.FormatYValueString,
                                         _series.DM.LastVisibleDataEntry(_series._seriesIndex).Value);
                else
                    return "";
                
                                    
            }
        }

        public double Variacao()
        {
            return StockChartX.Variacao;
        }

        /// <summary>
        /// internal usage
        /// </summary>
        public Brush SeriesStroke
        {
            get { return _series.TitleBrush ?? _series.StrokeColorBrush; }
        }

        ///<summary>
        /// Gets whether to show the frame arround title
        ///</summary>
        public Visibility ShowFrame
        {
            get { return _series is IChartElementPropertyAble ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}
