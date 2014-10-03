﻿using System.Windows;
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties
{
  ///<summary>
  ///</summary>
  public partial class TextBoxPropertyPresenter : IValuePresenter
  {
    private readonly TextBox _textBox;

    ///<summary>
    ///</summary>
    public TextBoxPropertyPresenter()
    {
      _textBox = new TextBox();
    }

    #region Implementation of IValuePresenter

    ///<summary>
    ///</summary>
    public object Value
    {
      get { return _textBox.Text; }
      set { _textBox.Text = value.ToString(); }
    }

    ///<summary>
    ///</summary>
    public FrameworkElement Control
    {
      get { return _textBox; }
    }

    #endregion
  }
}
