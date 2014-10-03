using System;
using System.Windows;
using System.Windows.Media;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Controls
{
  public partial class ColorDialogInternal
  {
    internal ColorDialog ParentDialog { get; set; }

    public ColorDialogInternal()
    {
      InitializeComponent();

      colorPicker.ColorSelected += colorPicker_ColorSelected;
      btnOK.Click += (sender,e) =>  ParentDialog.OkClose();
      btnCancel.Click += (sender, e) => ParentDialog.Close();
    }

    public Color SelectedColor
    {
      get { return colorPicker.SelectedColor; }
      set
      {
        colorPicker.SelectedColor = value;
        canvasOldColor.Background = new SolidColorBrush(value);
        canvasnewColor.Background = new SolidColorBrush(value);
      }
    }

    void colorPicker_ColorSelected(Color c)
    {
      canvasnewColor.Background = new SolidColorBrush(c);
    }
  }

  public class ColorDialog : Dialog
  {
    private readonly ColorDialogInternal _dialog;

    public event EventHandler OnOK = delegate { };

    public ColorDialog()
    {
      _dialog = new ColorDialogInternal { ParentDialog = this };
    }

    protected override FrameworkElement GetContent()
    {
      return _dialog;
    }

    public Color CurrentColor
    {
      get { return _dialog.SelectedColor; }
      set { _dialog.SelectedColor = value; }
    }

    internal void OkClose()
    {
      OnOK(this, EventArgs.Empty);
      Close();
    }
  }
}
