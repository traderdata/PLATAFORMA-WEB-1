using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Controls;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public partial class IndicatorDialogInternal
  {
    internal Indicator _indicator;
    internal IndicatorDialog _owner;
    internal ColorDialog _colorDialog = new ColorDialog();

    public IndicatorDialogInternal()
    {
      InitializeComponent();

      _colorDialog.OnOK += (sender, e) => _owner.SetIndicatorColor(_colorDialog.CurrentColor);
    }

    private void canvasColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _colorDialog.CurrentColor = _indicator.StrokeColor;
      _colorDialog.AppRoot = _indicator._chartPanel._chartX.AppRoot;
      _colorDialog.Show(Dialog.DialogStyle.ModalDimmed);        
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      _owner.OkClick();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      _owner.CancelClick(); 
    }
  }

  public class IndicatorDialog : Dialog, IDisposable
  {
    public enum DialogResultType
    {
      Ok,
      Cancel
    }

    private bool _canClose;
    private readonly IndicatorDialogInternal _dialog;

    public object Tag { get; set; }
    public DialogResultType DialogResult { get; set; }

    public event EventHandler OnOk = delegate { };
    public event EventHandler OnCancel = delegate { };

    public IndicatorDialog()
    {
      _dialog = new IndicatorDialogInternal { _owner = this };
    }

    public Button btnCancel
    {
      get { return _dialog.btnCancel; }
    }

    public Panel stackPanelBackground
    {
      get { return _dialog.stackPanelBackground; }
    }

    public Indicator Indicator
    {
      get { return _dialog._indicator; }
      set { _dialog._indicator = value; }
    }

    protected override FrameworkElement GetContent()
    {
      return _dialog;
    }

    internal ComboBox GetComboBox(int index)
    {
      return _dialog.FindName("cmb" + index) as ComboBox;
    }

    internal TextBlock GetTextBlock(int index)
    {
      return _dialog.FindName("lbl" + index) as TextBlock;
    }

    internal TextBox GetTextBox(int index)
    {
      return _dialog.FindName("txt" + index) as TextBox;
    }

    internal double Height
    {
      get { return _dialog.Height; }
      set { _dialog.Height = value; }
    }

    internal void ShowHidePanel(int index, bool hide, bool showTextBox)
    {
      Panel panel = _dialog.FindName("panel" + index) as Panel;
      Debug.Assert(panel != null);
      panel.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
      if (hide) return;
      if (showTextBox)
        GetComboBox(index).Visibility = Visibility.Collapsed;
      else
        GetTextBox(index).Visibility = Visibility.Collapsed;
    }

    internal string Title
    {
      get { return _dialog.txtTitle.Text; }
      set { _dialog.txtTitle.Text = value; }
    }

    internal bool? ShowDialog()
    {
      _dialog.canvasColor.Background = new SolidColorBrush(Indicator.StrokeColor);
      Show(DialogStyle.ModalDimmed);
      return false;
    }

    public void Dispose()
    {
      
    }

    internal bool _userCanceled;
    internal void OkClick()
    {
        try
        {
            Indicator.SetUserInput();
        }
        catch (InvalidCastException ex)
        {
            MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            return;
        }
        catch (FormatException ex)
        {
            MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            return;
        }

        if (Indicator._inputError)
        {
            if (_userCanceled)
                CancelClick();
            return;
        }

        _canClose = true;
        Indicator._dialogShown = false;
        DialogResult = DialogResultType.Ok;
        Close();

        OnOk(this, EventArgs.Empty); //raise event
    }

    internal void CancelClick()
    {
      _canClose = true;
      Indicator._dialogShown = false;
      Indicator._chartPanel._chartX.locked = false;
      //Indicator.OnCancelDialog();
      DialogResult = DialogResultType.Cancel;
      Close();

      OnCancel(this, EventArgs.Empty); //raise event
    }

    internal void SetIndicatorColor(Color color)
    {
      Indicator.StrokeColor = color;
      _dialog.canvasColor.Background = new SolidColorBrush(color);
    }
  }
}
