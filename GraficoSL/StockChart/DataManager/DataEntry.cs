using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Data
{
  /// <summary>
  /// Provides information about values stored in a series
  /// </summary>
  public partial class DataEntry : INotifyPropertyChanged
  {
    internal DataEntryCollection _collectionOwner;
    internal DataManager.DataManager _dataManager;

    ///<summary>
    /// Initializes a new instance of the <seealso cref="DataEntry"/> class.
    ///</summary>
    ///<param name="value">Value</param>
    public DataEntry(double? value)
    {
      _value = value;
    }

    ///<summary>
    /// Gets the index of data entry
    ///</summary>
    public int Index { get; set; }

    ///<summary>
    /// Gets the reference to the owener-series
    ///</summary>
    public Series SeriesOwner { get; internal set; }

    /// <summary>
    /// Occurs when an internal property changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private double? _value;
    ///<summary>
    /// Gets or sets the value
    ///</summary>
    public double? Value
    {
      get { return _value; }
      set
      {
        if (_value == value) return;
        _value = value;
        OnPropertyChanged("value");
      }
    }

    ///<summary>
    /// Gets the timestamp 
    ///</summary>
    public DateTime TimeStamp
    {
      get { return _dataManager.GetTimeStampByIndex(Index); }
    }

    private bool _subscribed;
    internal void SubscribeEvent(PropertyChangedEventHandler handler)
    {
      if (_subscribed) return;
      _subscribed = true;
      PropertyChanged += handler;
    }
  }

  ///<summary>
  /// Collection of data<seealso cref="DataEntry"/>
  ///</summary>
  public class DataEntryCollection : ObservableCollection<DataEntry>
  {
    
  }

  internal class PriceStyleValue
  {
    public DateTime TimeStamp;
    public double Value;
    public PriceStyleValue(DateTime timeStamp, double value)
    {
      TimeStamp = timeStamp;
      Value = value;
    }
  }

  internal class PriceStyleValuesCollection : List<PriceStyleValue>
  {
    
  }
}

