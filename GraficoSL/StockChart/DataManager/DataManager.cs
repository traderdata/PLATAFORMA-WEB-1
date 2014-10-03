using System;
using System.Collections.Generic;
using System.Diagnostics;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;
using Traderdata.Client.Componente.GraficoSL.StockChart.Exceptions;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.Enum;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif
#if WPF
using System.Text;
using System.Xml;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart.DataManager
{
    /// <summary>
    /// Gets the operation for volume tick appending
    /// </summary>
    public enum AppendTickVolumeBehavior
    {
        /// <summary>
        /// With every tick volume value is incremented by the new passed value
        /// </summary>
        Increment,
        /// <summary>
        /// At every tick the current volume value is replaced by the new one
        /// </summary>
        Change
    }

    internal partial class DataManager
    {
        private readonly StockChartX _chart;

        public static double UltimaVariacao = 0;

        private class TickEntry
        {
            public DateTime _timeStamp;
            public double _lastPrice;
            public double _lastVolume;
        }
        /// <summary>
        /// Contains tick values for all symbols on chart
        /// </summary>
        private readonly Dictionary<string, List<TickEntry>> _tickValues = new Dictionary<string, List<TickEntry>>(10);

        private int _tickPeriodicity = 5; //periodicity when compressing bars - seconds
        private EnumGeral.CompressaoTickEnum _tickCompressionType = EnumGeral.CompressaoTickEnum.Tempo;
        /// <summary>
        /// This list will have all the timestamps involved in the chart, series will have only a list of doubles to keep their values
        /// </summary>
        private readonly SortedList<DateTime, int> _timestamps = new SortedList<DateTime, int>();
        private readonly Dictionary<DateTime, int> _timestampIndexes = new Dictionary<DateTime, int>();

       

        public class SeriesEntry
        {
            internal double _min, _max;//linear
            internal double _visibleMin, _visibleMax;
            internal bool _visibleDataChanged;
            internal bool _dataChanged;

            public string Name;
            public Series Series;
            public readonly DataEntryCollection Data = new DataEntryCollection();
        }

        /// <summary>
        /// Holds series indexes by their OHLC type and name
        /// </summary>
        private readonly Dictionary<EnumGeral.TipoSerieOHLC, Dictionary<string, int>> _seriesToIndex =
          new Dictionary<EnumGeral.TipoSerieOHLC, Dictionary<string, int>>(100);

        private readonly List<SeriesEntry> _seriesList = new List<SeriesEntry>();

        public DataManager(StockChartX chart)
        {
            _chart = chart;
        }

        public int RecordCount
        {
            get { return _timestamps.Count; }
        }

        public int TickPeriodicity
        {
            get { return _tickPeriodicity; }
            set
            {
                if (value < 5)
                    throw new ArgumentOutOfRangeException();

                if (_tickPeriodicity == value) return;
                _tickPeriodicity = value;
                if (_chart.ChartType == EnumGeral.TipoGraficoEnum.Tick)
                    ReCompressTicks();
            }
        }

        public EnumGeral.CompressaoTickEnum TickCompressionType
        {
            get { return _tickCompressionType; }
            set
            {
                if (_tickCompressionType == value) return;
                _tickCompressionType = value;
                if (_chart.ChartType == EnumGeral.TipoGraficoEnum.Tick)
                    ReCompressTicks();
            }
        }

        public bool SeriesExists(string seriesName, EnumGeral.TipoSerieOHLC ohlcType)
        {
            Dictionary<string, int> nameIndex;
            return (_seriesToIndex.TryGetValue(ohlcType, out nameIndex) && nameIndex.ContainsKey(seriesName));
        }

        public int SeriesIndex(string seriesName, EnumGeral.TipoSerieOHLC ohlcType)
        {
            if (!SeriesExists(seriesName, ohlcType)) return -1;
            return _seriesToIndex[ohlcType][seriesName];
        }

        public void BindSeries(Series series)
        {
            lock (_seriesList)
            {
                series._seriesIndex = _seriesToIndex[series.OHLCType][series.Name];
                _seriesList[series._seriesIndex].Series = series;
            }
        }

        private void RegisterSeriesIndex(string seriesName, EnumGeral.TipoSerieOHLC ohlcType, int index)
        {
            Dictionary<string, int> name2Index;
            if (!_seriesToIndex.TryGetValue(ohlcType, out name2Index))
                _seriesToIndex[ohlcType] = new Dictionary<string, int>();

            _seriesToIndex[ohlcType][seriesName] = index;
        }

        internal void UnRegisterSeries(string seriesName, EnumGeral.TipoSerieOHLC ohlcType)
        {
            Dictionary<string, int> nameIndex;
            if (!_seriesToIndex.TryGetValue(ohlcType, out nameIndex)) return;
            if (!nameIndex.ContainsKey(seriesName)) return;

            int seriesIndex = nameIndex[seriesName];
            nameIndex.Remove(seriesName);
            _seriesList.RemoveAt(seriesIndex);

            //re-index other series
            for (int i = 0; i < _seriesList.Count; i++)
            {
                SeriesEntry seriesEntry = _seriesList[i];

                _seriesToIndex[seriesEntry.Series.OHLCType][seriesEntry.Series.Name] =
                  seriesEntry.Series._seriesIndex = i;
            }

            //removed series might be an input parameter for on of the indicators from the chart
            //go over all indicator and check
            foreach (ChartPanel chartPanel in _chart.paineisContainers.Panels)
            {
                foreach (Indicator indicator in chartPanel.IndicatorsCollection)
                {
                    if (indicator.FullName == seriesName) continue;
                    indicator._calculated = false;
                    indicator.Calculate();
                }
            }

            _chart.ReCalc = true;
        }

        public void AddOHLCSeries(string seriesName)
        {
            SeriesEntry seriesEntry;

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Abertura, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Maximo, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Minimo, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Ultimo, _seriesList.Count - 1);
        }

        public void AddHLCSeries(string seriesName)
        {
            SeriesEntry seriesEntry;
            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Maximo, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Minimo, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, EnumGeral.TipoSerieOHLC.Ultimo, _seriesList.Count - 1);
        }

        public int AddSeries(string seriesName, EnumGeral.TipoSerieOHLC ohlcType)
        {
            SeriesEntry seriesEntry;
            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, ohlcType, _seriesList.Count - 1);

            return _seriesList.Count - 1;
        }

        public void AppendOHLCValues(string seriesName, DateTime timeStamp, double? open, double? high, double? low, double? close)
        {
            if (!SeriesExists(seriesName, EnumGeral.TipoSerieOHLC.Abertura))
                throw new SeriesDoesNotExistsException(seriesName);
            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Abertura][seriesName], timeStamp, open);
            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Maximo][seriesName], timeStamp, high);
            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Minimo][seriesName], timeStamp, low);
            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Ultimo][seriesName], timeStamp, close);
        }

        public void AppendHLCValues(string seriesName, DateTime timeStamp, double? high, double? low, double? close)
        {
            if (!SeriesExists(seriesName, EnumGeral.TipoSerieOHLC.Abertura))
                throw new SeriesDoesNotExistsException(seriesName);

            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Maximo][seriesName], timeStamp, high);
            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Minimo][seriesName], timeStamp, low);
            AppendValue(_seriesToIndex[EnumGeral.TipoSerieOHLC.Ultimo][seriesName], timeStamp, close);
        }

        /// <summary>
        /// Appends a value to a series
        /// </summary>
        /// <param name="seriesName"></param>
        /// <param name="ohlcType"></param>
        /// <param name="timeStamp"></param>
        /// <param name="value"></param>
        public void AppendValue(string seriesName, EnumGeral.TipoSerieOHLC ohlcType, DateTime timeStamp, double? value)
        {
            if (!SeriesExists(seriesName, ohlcType))
                throw new SeriesDoesNotExistsException(seriesName);

            AppendValue(_seriesToIndex[ohlcType][seriesName], timeStamp, value);
        }

        public double? GetValue(int seriesIndex, int recordIndex)
        {
            return _seriesList[seriesIndex].Data[recordIndex].Value;
        }

        public DataEntryCollection GetDataCollection(int seriesIndex)
        {
            return _seriesList[seriesIndex].Data;
        }

        internal SeriesEntry this[int seriesIndex]
        {
            get
            {
                return _seriesList[seriesIndex];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns>index of the timestamp, -1 is such a timestamp doesn't exists</returns>
        public int GetTimeStampIndex(DateTime timestamp)
        {
            if (_timestamps.ContainsKey(timestamp))
                return _timestamps.IndexOfKey(timestamp);
            return -1;
        }

        /// <summary>
        /// Returns an aproximated record index for a given timeStamp.
        /// Usefull when an exact timestamp is unknown
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="roundUp">
        /// if true - returns the next recordCount that has a value greater then given timestamp
        /// if false - returns the previos recordCount that has a value less then given timestamp
        /// </param>
        /// <returns>Timestamp index, -1 if no such aproximate value</returns>
        public int GetTimeStampIndex(DateTime timestamp, bool roundUp)
        {
#if WPF
      long ticks = timestamp.ToBinary();
#endif
#if SILVERLIGHT
            double ticks = timestamp.ToJDate();
#endif
            for (int i = 0; i < _timestamps.Keys.Count; i++)
            {
#if WPF
        long t = _timestamps.Keys[i].ToBinary();
#endif
#if SILVERLIGHT
                double t = _timestamps.Keys[i].ToJDate();
#endif
                if (t == ticks)
                    return i;
                if (t < ticks)
                    continue;
                if (roundUp)
                    return i;
                return i - 1;
            }
            return -1;
        }
        /// <summary>
        /// Returns TimeStamp by index. 
        /// If index is out of range it returns DateTime.MinValue
        /// Doesn't take care of chartX._startIndex
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DateTime GetTimeStampByIndex(int index)
        {
            return (index < _timestamps.Count && index >= 0) ? _timestamps.Keys[index] : DateTime.MinValue;
        }

        internal DateTime TS(int index)
        {
            return _timestamps.Keys[index];
        }

        internal void GetStartEndTimeStamp(out DateTime timeStampStart, out DateTime timeStampEnd)
        {
            timeStampStart = timeStampEnd = DateTime.MinValue;
            if (_timestamps.Count == 0) return;
            timeStampStart = _timestamps.Keys[_chart.indexInicial];
            timeStampEnd = _timestamps.Keys[_chart.indexFinal > 0 ? _chart.indexFinal - 1 : _chart.indexFinal];
        }

        public void ClearAll()
        {
            _timestamps.Clear();
            _timestampIndexes.Clear();
            _seriesList.Clear();
            _tickValues.Clear();
            _seriesToIndex.Clear();
        }

        public void ClearData()
        {
            _timestamps.Clear();
            _timestampIndexes.Clear();
            _tickValues.Clear();
            //clear series data
            foreach (SeriesEntry entry in _seriesList)
            {
                entry.Data.Clear();
            }
        }

        ///<summary>
        /// Edit a value for a given series at a specified position
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditValueCustom(Series series, DateTime timeStamp, double? newValue)
        {
            int valueIndex = this.GetTimeStampIndex(timeStamp);
            if (valueIndex == -1) return;
            if (series != null)
            {
                series[valueIndex].Value = newValue;
                series.SeriesEntry.Series.OnPropertyChanged(Series.PropertyLastValue);
                //_seriesList[seriesIndex].Series.OnPropertyChanged
            }
        }

        ///<summary>
        /// Edita a variacao
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditVariacao(Series series, double variacao)
        {
            if (series != null)
            {
                int valueIndex = series.RecordCount-1;
                series[valueIndex].Value = variacao;
                series.SeriesEntry.Series.OnPropertyChanged(Series.PropertyLastValue);                
            }
        }
        public void AppendValue(int seriesIndex, DateTime timeStamp, double? value)
        {
            //      if (_chart.ChartType == Chart.StockChartX.ChartTypeEnum.Tick)
            //      {
            //        MessageBox.Show("At the moment chart accepts only tick values.", "Error", MessageBoxButton.OK,
            //                        MessageBoxImage.Error);
            //        return;
            //      }

            int newIndex;
            bool updateLineStudies = false;

            if (!_timestampIndexes.ContainsKey(timeStamp))
            {
                _timestampIndexes[timeStamp] = 1; //save the timestamp


                if (_chart.indexFinal == _timestamps.Count) //chart is not shrinked
                    _chart.indexFinal = _timestamps.Count + 1;
                else
                    _chart.indexFinal++;

                if (_chart.ManterNivelZoom && _chart.indexFinal != _timestamps.Count)
                    _chart.indexInicial++;

                _timestamps.Add(timeStamp, 1);

                newIndex = _timestamps.IndexOfKey(timeStamp);
                for (int i = 0; i < _seriesList.Count; i++)
                {
                    _seriesList[i].Data.Insert(newIndex,
                                                new DataEntry(null)
                                                  {
                                                      _collectionOwner = _seriesList[i].Data,
                                                      _dataManager = this
                                                  });
                    _seriesList[i]._dataChanged = true;
                    _seriesList[i]._visibleDataChanged = true;
                    //update index value
                    for (int j = newIndex; j < _seriesList[i].Data.Count; j++)
                        _seriesList[i].Data[j].Index = j;
                }

                updateLineStudies = true;
                _chart.ReCalc = true;
                _chart.OnPropertyChanged(StockChartX.Property_NewRecord);
            }
            else
            {
                newIndex = _timestamps.IndexOfKey(timeStamp);
            }

            _seriesList[seriesIndex].Data[newIndex].Value = value;
            _seriesList[seriesIndex]._dataChanged = true;

            if (newIndex == RecordCount - 1)
                _seriesList[seriesIndex].Series.OnPropertyChanged(Series.PropertyLastValue);

            if (updateLineStudies)
            {
                foreach (var chartPanel in _chart.paineisContainers.Panels)
                {
                    foreach (var lineStudy in chartPanel._lineStudies)
                    {
                        lineStudy.UpdatePosition(newIndex);
                    }
                }
            }
        }

        public void AppendTickValue(string symbolName, DateTime timeStamp, double value, double volumeValue)
        {
            //      if (_chart.ChartType == StockChartX.ChartTypeEnum.OHLC)
            //      {
            //        MessageBox.Show("At the moment chart accepts only OHLC values.", "Error", MessageBoxButton.OK,
            //                        MessageBoxImage.Error);
            //        return;
            //      }
            if (RecordCount > 0 && _timestamps.Keys[RecordCount - 1] > timeStamp) return; //the new timestamp must be greater then last timestamp 

            SeriesEntry open = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Abertura)
                                 ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Abertura][symbolName]]
                                 : null;
            SeriesEntry high = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Maximo)
                                 ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Maximo][symbolName]]
                                 : null;
            SeriesEntry low = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Minimo)
                                 ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Minimo][symbolName]]
                                 : null;
            SeriesEntry close = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Ultimo)
                                 ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Ultimo][symbolName]]
                                 : null;
            SeriesEntry volume = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Volume)
                                 ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Volume][symbolName]]
                                 : null;

            List<TickEntry> tickList;
            if (!_tickValues.TryGetValue(symbolName, out tickList))
                _tickValues[symbolName] = (tickList = new List<TickEntry>());
            //store in local memory all tick entries, they might be used later to compress with a different tick periodicity
            tickList.Add(new TickEntry { _timeStamp = timeStamp, _lastPrice = value, _lastVolume = volumeValue });

            if (RecordCount == 0) //no records yet, create the first entry
            {
                if (open != null) AppendValue(open.Series._seriesIndex, timeStamp, value);
                if (high != null) AppendValue(high.Series._seriesIndex, timeStamp, value);
                if (low != null) AppendValue(low.Series._seriesIndex, timeStamp, value);
                if (close != null) AppendValue(close.Series._seriesIndex, timeStamp, value);
                if (volume != null) AppendValue(volume.Series._seriesIndex, timeStamp, volumeValue);
                return;
            }

            bool newCandle;

            switch (_tickCompressionType)
            {
                case EnumGeral.CompressaoTickEnum.Tempo:
                    newCandle = (timeStamp - _timestamps.Keys[RecordCount - 1]).TotalSeconds > _tickPeriodicity;
                    break;
                case EnumGeral.CompressaoTickEnum.Ticks:
                    newCandle = _tickValues[symbolName].Count % _tickPeriodicity == 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("");
            }

            if (newCandle) //just create a new candle and initialize all needed values
            {
                if (open != null) AppendValue(open.Series._seriesIndex, timeStamp, value);
                if (high != null) AppendValue(high.Series._seriesIndex, timeStamp, value);
                if (low != null) AppendValue(low.Series._seriesIndex, timeStamp, value);
                if (close != null) AppendValue(close.Series._seriesIndex, timeStamp, value);
                if (volume != null) AppendValue(volume.Series._seriesIndex, timeStamp, volumeValue);
                Debug.WriteLine("A new candle was created");
                return;
            }

            if (high != null && value > high.Data[RecordCount - 1].Value) //find highest value
                high.Data[RecordCount - 1].Value = value;
            if (low != null && value < low.Data[RecordCount - 1].Value) //find the lowest value
                low.Data[RecordCount - 1].Value = value;
            if (close != null)
                close.Data[RecordCount - 1].Value = value; //update close value
            if (volume != null)
            {
                switch (_chart.AppendTickVolumeBehavior)
                {
                    case AppendTickVolumeBehavior.Increment:
                        volume.Data[RecordCount - 1].Value += volumeValue; //increment volume
                        break;
                    case AppendTickVolumeBehavior.Change:
                        volume.Data[RecordCount - 1].Value = volumeValue; //change volume
                        break;
                }
            }
        }

        private void ReCompressTicks()
        {
            foreach (KeyValuePair<string, List<TickEntry>> pair in _tickValues)
            {
                string symbolName = pair.Key;

                SeriesEntry open = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Abertura)
                                   ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Abertura][symbolName]]
                                   : null;
                SeriesEntry high = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Maximo)
                                     ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Maximo][symbolName]]
                                     : null;
                SeriesEntry low = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Minimo)
                                     ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Minimo][symbolName]]
                                     : null;
                SeriesEntry close = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Ultimo)
                                     ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Ultimo][symbolName]]
                                     : null;
                SeriesEntry volume = SeriesExists(symbolName, EnumGeral.TipoSerieOHLC.Volume)
                                     ? this[_seriesToIndex[EnumGeral.TipoSerieOHLC.Volume][symbolName]]
                                     : null;

                if (open == null && high == null && low == null && close == null) continue;
                if (open != null) ClearValues(open.Series._seriesIndex);
                if (high != null) ClearValues(high.Series._seriesIndex);
                if (low != null) ClearValues(low.Series._seriesIndex);
                if (close != null) ClearValues(close.Series._seriesIndex);
                if (volume != null) ClearValues(volume.Series._seriesIndex);

                int tickIndex = 0;
                foreach (TickEntry tickEntry in pair.Value)
                {
                    bool newCandle;
                    switch (_tickCompressionType)
                    {
                        case EnumGeral.CompressaoTickEnum.Ticks:
                            newCandle = tickIndex % _tickPeriodicity == 0;
                            break;
                        case EnumGeral.CompressaoTickEnum.Tempo:
                            newCandle = tickIndex == 0 ||
                                        (tickEntry._timeStamp - _timestamps.Keys[RecordCount - 1]).TotalSeconds > _tickPeriodicity;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("");
                    }
                    tickIndex++;
                    if (newCandle) //create a new candle
                    {
                        if (open != null) AppendValue(open.Series._seriesIndex, tickEntry._timeStamp, tickEntry._lastPrice);
                        if (high != null) AppendValue(high.Series._seriesIndex, tickEntry._timeStamp, tickEntry._lastPrice);
                        if (low != null) AppendValue(low.Series._seriesIndex, tickEntry._timeStamp, tickEntry._lastPrice);
                        if (close != null) AppendValue(close.Series._seriesIndex, tickEntry._timeStamp, tickEntry._lastPrice);
                        if (volume != null) AppendValue(volume.Series._seriesIndex, tickEntry._timeStamp, tickEntry._lastVolume);
                        continue;
                    }

                    if (high != null && tickEntry._lastPrice > high.Data[RecordCount - 1].Value) //find highest value
                        high.Data[RecordCount - 1].Value = tickEntry._lastPrice;
                    if (low != null && tickEntry._lastPrice < low.Data[RecordCount - 1].Value) //find the lowest value
                        low.Data[RecordCount - 1].Value = tickEntry._lastPrice;
                    if (close != null)
                        close.Data[RecordCount - 1].Value = tickEntry._lastPrice; //update close value
                    if (volume != null)
                        volume.Data[RecordCount - 1].Value += tickEntry._lastVolume; //increment volume
                }
            }
            _chart.ReCalc = true;
        }

        public void GetMinMax(int seriesIndex, out double min, out double max)
        {
            SeriesEntry seriesEntry = this[seriesIndex];
            if (!seriesEntry._dataChanged)
            {
                min = seriesEntry._min;
                max = seriesEntry._max;
                return;
            }
            min = double.MaxValue;
            max = double.MinValue;

            foreach (DataEntry dataEntry in this[seriesIndex].Data)
            {
                if (!dataEntry.Value.HasValue) continue;
                if (dataEntry.Value.Value > max)
                    max = dataEntry.Value.Value;
                else if (dataEntry.Value.Value < min)
                    min = dataEntry.Value.Value;
            }
            seriesEntry._min = min;
            seriesEntry._max = max;
            seriesEntry._dataChanged = false;
        }

        public double Max(int seriesIndex)
        {
            double res = double.MinValue;
            foreach (DataEntry dataEntry in this[seriesIndex].Data)
            {
                if (dataEntry.Value.HasValue && dataEntry.Value.Value > res)
                    res = dataEntry.Value.Value;
            }
            return res;
        }

        public double Min(int seriesIndex)
        {
            double res = double.MaxValue;
            foreach (DataEntry dataEntry in this[seriesIndex].Data)
            {
                if (dataEntry.Value.HasValue && dataEntry.Value.Value < res)
                    res = dataEntry.Value.Value;
            }
            return res;
        }

        public double MaxFromInterval(int seriesIndex, ref int startIndex, ref int endIndex)
        {
            DataEntryCollection data = this[seriesIndex].Data;

            endIndex = Math.Min(endIndex, RecordCount);
            startIndex = Math.Min(Math.Max(startIndex, 0), endIndex);
            double res = double.MinValue;
            for (int i = startIndex; i < endIndex; i++)
            {
                double? value = data[i].Value;
                if (value.HasValue && value.Value > res)
                    res = value.Value;
            }
            return res;
        }

        public double MinFromInterval(int seriesIndex, ref int startIndex, ref int endIndex)
        {
            DataEntryCollection data = this[seriesIndex].Data;

            endIndex = Math.Min(endIndex, RecordCount);
            startIndex = Math.Min(Math.Max(startIndex, 0), endIndex);
            double res = double.MaxValue;
            for (int i = startIndex; i < endIndex; i++)
            {
                double? value = data[i].Value;
                if (value.HasValue && value.Value < res)
                    res = value.Value;
            }
            return res;
        }

        public void VisibleMinMax(int seriesIndex, out double min, out double max)
        {
            SeriesEntry seriesEntry = this[seriesIndex];
            if (!seriesEntry._visibleDataChanged)
            {
                min = seriesEntry._visibleMin;
                max = seriesEntry._visibleMax;
                return;
            }
            int startIndex = _chart.indexInicial;
            int endIndex = _chart.indexFinal;

            if (seriesEntry.Series._seriesTypeOHLC != EnumGeral.TipoSerieOHLC.Volume)
                seriesEntry._visibleMin = min = MinFromInterval(seriesIndex, ref startIndex, ref endIndex);
            else
                seriesEntry._visibleMin = min = 0;
            seriesEntry._visibleMax = max = MaxFromInterval(seriesIndex, ref startIndex, ref endIndex);
            //seriesEntry._visibleDataChanged = false; //TODO
        }

        public void ClearValues(int seriesIndex)
        {
            SeriesEntry seriesEntry = this[seriesIndex];
            for (int i = 0; i < seriesEntry.Data.Count; i++)
            {
                seriesEntry.Data[i].Value = null;
            }
        }

        public DataEntry LastVisibleDataEntry(int seriesIndex)
        {
            SeriesEntry seriesEntry = this[seriesIndex];
            if (_chart.indexFinal == 0 || RecordCount == 0)
                return new DataEntry(null);
            return seriesEntry.Data[_chart.indexFinal - 1];
        }

        private void InitSeries(SeriesEntry seriesEntry)
        {
            //if a series was added after some other series we must append null values
            for (int i = seriesEntry.Data.Count; i < _timestamps.Count; i++)
            {
                seriesEntry.Data.Add(new DataEntry(null) { _collectionOwner = seriesEntry.Data, _dataManager = this, Index = i });
            }
            seriesEntry._visibleDataChanged = true;
        }

#if WPF
    internal void SaveToXml(XmlNode xRoot, XmlDocument xmlDocument)
    {
      XmlNode xmlData = xmlDocument.CreateElement("SeriesData");
      xRoot.AppendChild(xmlData);

      XmlNode xmlTimeStamps = xmlDocument.CreateElement("TimeStamps");
      xmlData.AppendChild(xmlTimeStamps);
      StringBuilder sb = new StringBuilder(1000);
      foreach (KeyValuePair<DateTime, int> pair in _timestamps)
      {
        sb.Append(pair.Key.ToBinary()).Append(" ");
      }
      xmlTimeStamps.InnerText = sb.ToString();

      XmlNode xmlTickData = xmlDocument.CreateElement("TickData");
      xmlData.AppendChild(xmlTickData);
      sb.Length = 0;
      foreach (KeyValuePair<string, List<TickEntry>> pair in _tickValues)
      {
        XmlNode xmlTickSymbol = xmlDocument.CreateElement("Symbol");
        xmlTickData.AppendChild(xmlTickSymbol);
        XmlAttribute attribute = xmlDocument.CreateAttribute("Name");
        attribute.Value = pair.Key;
        xmlTickSymbol.Attributes.Append(attribute);

        sb.Length = 0;
        foreach (TickEntry entry in pair.Value)
        {
          sb.Append(entry._timeStamp.ToBinary()).Append(" ");
          sb.Append(entry._lastPrice).Append(" ");
          sb.Append(entry._lastVolume).Append(" ");
        }
        xmlTickSymbol.InnerText = sb.ToString();
      }

      XmlNode xmlOhlcData = xmlDocument.CreateElement("OhlcData");
      xmlData.AppendChild(xmlOhlcData);
      foreach (SeriesEntry entry in _seriesList)
      {
        if (entry.Series.OHLCType == SeriesTypeOHLC.Unknown) continue;

        XmlNode xmlSeries = xmlDocument.CreateElement("Series");
        xmlOhlcData.AppendChild(xmlSeries);
        XmlAttribute attribute = xmlDocument.CreateAttribute("Name");
        attribute.Value = entry.Name;
        xmlSeries.Attributes.Append(attribute);
        attribute = xmlDocument.CreateAttribute("Index");
        attribute.Value = entry.Series._seriesIndex.ToString();
        xmlSeries.Attributes.Append(attribute);
        attribute = xmlDocument.CreateAttribute("OhlcType");
        attribute.Value = entry.Series.OHLCType.ToString();
        xmlSeries.Attributes.Append(attribute);

        sb.Length = 0;
        foreach (DataEntry dataEntry in entry.Data)
        {
          sb.Append(dataEntry.Value).Append(" ");
        }
        xmlSeries.InnerText = sb.ToString();
      }
    }

    internal void LoadFromXml(XmlNode xRoot)
    {
      XmlNode xmlData = xRoot.SelectSingleNode("SeriesData");
      if (xmlData == null) return;

      XmlNode xmlTimeStamps = xmlData.SelectSingleNode("TimeStamps");
      if (xmlTimeStamps == null) return;

      ClearData();

      string[] timeStamps = xmlTimeStamps.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
      foreach (string timeStamp in timeStamps)
      {
        DateTime time = DateTime.FromBinary(long.Parse(timeStamp));
        _timestamps.Add(time, 1);
        _timestampIndexes[time] = 1;
      }
      _chart._endIndex = _timestamps.Count;

      XmlNode xmlTickData = xmlData.SelectSingleNode("TickData");
      if (xmlTickData != null)
      {
        foreach (XmlNode childNode in xmlTickData.ChildNodes)
        {
          if (childNode.Name != "Symbol") continue;
          List<TickEntry> tickData;
          string symbol = childNode.Attributes["Name"].Value;
          if (!_tickValues.TryGetValue(symbol, out tickData))
            _tickValues[symbol] = (tickData = new List<TickEntry>(1000));
          string[] values = childNode.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
          for (int i = 0; i < values.Length; i += 3)
          {
            tickData.Add(new TickEntry
                           {
                             _timeStamp = DateTime.FromBinary(long.Parse(values[i])),
                             _lastPrice = double.Parse(values[i + 1]),
                             _lastVolume = double.Parse(values[i + 2])
                           });
          }
        }  
      }
      foreach (var entry in _seriesList)
      {
        InitSeries(entry);
      }

      if (_chart.ChartType == ChartTypeEnum.Tick)
      {
        //initialize th series
        ReCompressTicks();
        return;
      }

      XmlNode xmlOhlcData = xmlData.SelectSingleNode("OhlcData");
      if (xmlOhlcData == null) return;
      foreach (XmlNode childNode in xmlOhlcData.ChildNodes)
      {
        if (childNode.Name != "Series") return;

        int seriesIndex = int.Parse(childNode.Attributes["Index"].Value);
        string symbol = childNode.Attributes["Name"].Value;
        SeriesTypeOHLC ohlcType =
          (SeriesTypeOHLC)Enum.Parse(typeof (SeriesTypeOHLC),
                                                   childNode.Attributes["OhlcType"].Value);
        int existingSeriesIndex = SeriesIndex(symbol, ohlcType);
        if (seriesIndex != existingSeriesIndex) //in case chart has already some series
          seriesIndex = existingSeriesIndex;
        if (seriesIndex >= _seriesList.Count || seriesIndex == -1)
        {
          Debug.WriteLine("Series index out of range.");
          continue;
        }

        string[] values = childNode.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        int valueIndex = 0;
        foreach (string value in values)
        {
          _seriesList[seriesIndex].Data[valueIndex] =
            new DataEntry(double.Parse(value))
              {
                _collectionOwner = _seriesList[seriesIndex].Data,
                _dataManager = this,
                Index = valueIndex
              };
          valueIndex++;
        }        
      }
    }
#endif
    }
}

