using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Controls;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties
{
  public partial class PropertiesDialogContent
  {
    internal PropertiesDialog ParentDialog;
    internal string Title;
    internal List<IChartElementProperty> Properties;

    public PropertiesDialogContent()
    {
      InitializeComponent();

      btnOK.Click += BtnOkOnClick;
      btnCancel.Click += (sender, args) => ParentDialog.Close();
      Loaded += OnLoaded;
    }

    private void BtnOkOnClick(object sender, RoutedEventArgs args)
    {
      StringBuilder sbErrors = new StringBuilder();
      foreach (IChartElementProperty property in Properties)
      {
        property.Validate(sbErrors);
      }

      if (sbErrors.Length > 0)
      {
        MessageBox.Show(sbErrors.ToString());
        return;
      }

      //validation ok, invoke property changed
      foreach (IChartElementProperty property in Properties)
      {
        property.InvokeSetChatElementPropertyValue();        
      }

      ParentDialog.OkClose();
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
      Loaded -= OnLoaded;

      LayoutRoot.Background = ParentDialog.Background;

      txtProps.Text = Title;

      int row = 0;
      foreach (IChartElementProperty property in Properties)
      {
        gridProps.RowDefinitions.Add(new RowDefinition());

        TextBlock txt = new TextBlock
                          {
                            Text = property.Title,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(1)
                          };
        Grid.SetRow(txt, row);

        FrameworkElement content = property.ValuePresenter.Control;
        Grid.SetColumn(content, 1);
        Grid.SetRow(content, row);

        content.Margin = new Thickness(1);

        gridProps.Children.Add(txt);
        gridProps.Children.Add(content);

        row++;
      }
      Height = row * 23 + 21 + 28;
    }
  }

  internal class PropertiesDialog : Dialog
  {
    private readonly PropertiesDialogContent _content;

    public event EventHandler OnOK = delegate { };

    public PropertiesDialog(string title, IEnumerable<IChartElementProperty> properties)
    {
      _content = new PropertiesDialogContent
                   {
                     ParentDialog = this, 
                     Title = title,
                     Properties = new List<IChartElementProperty>(properties)
                   };

      Background = new SolidColorBrush(Colors.White);
    }

    public Brush Background { get; set; }

    #region Overrides of Dialog

    protected override FrameworkElement GetContent()
    {
      return _content;
    }

    #endregion

    internal void OkClose()
    {
      OnOK(this, EventArgs.Empty);
      Close();
    }
  }
}
