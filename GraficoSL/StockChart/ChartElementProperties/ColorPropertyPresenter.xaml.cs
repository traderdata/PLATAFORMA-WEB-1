using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Controls;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties
{
  ///<summary>
  ///</summary>
  public partial class ColorPropertyPresenter: IValuePresenter
  {
    private SolidColorBrush _value;
    ///<summary>
    /// ctor
    ///</summary>
    public ColorPropertyPresenter()
    {
      InitializeComponent();

      Loaded += (sender, args) =>
                  {
                    if (_value != null)
                      canvas.Background = _value;
                  };
    }

    internal Panel AppRoot;

    private void btnChooseColor_Click(object sender, RoutedEventArgs e)
    {
      ColorDialog dlg = new ColorDialog();
#if SILVERLIGHT
      dlg.AppRoot = AppRoot;
#endif
      dlg.OnOK += (o, args) => canvas.Background = new SolidColorBrush(dlg.CurrentColor);
      dlg.CurrentColor = ((SolidColorBrush)canvas.Background).Color;
#if SILVERLIGHT
      dlg.Show(Dialog.DialogStyle.ModalDimmed);
#endif
#if WPF
      dlg.ShowDialog();
#endif
    }

    #region Implementation of IValuePresenter

    ///<summary>
    /// 
    ///</summary>
    public object Value
    {
      get { return canvas.Background; }
      set
      {
        if (canvas != null)
          canvas.Background = (SolidColorBrush)value;
        else
          _value = (SolidColorBrush)value;
      }
    }

    ///<summary>
    ///</summary>
    public FrameworkElement Control
    {
      get { return this; }
    }

    #endregion
  }
}
