using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    ///<summary>
    ///</summary>
    public partial class ChartPanelTitleBar : Control
    {
        private Button _btnMinimize;
        private Button _btnMaximize;
        private Button _btnClose;
        private ItemsControl _titleLabels;

        internal event EventHandler MinimizeClick;
        internal event EventHandler MaximizeClick;
        internal event EventHandler CloseClick;

        internal ChartPanel _chartPanel;

#if WPF
    static ChartPanelTitleBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelTitleBar), new FrameworkPropertyMetadata(typeof(ChartPanelTitleBar)));
    }
#else
        public ChartPanelTitleBar()
        {
            DefaultStyleKey = typeof(ChartPanelTitleBar);
        }
#endif

        private IEnumerable _labelsDataSource;
        internal IEnumerable LabelsDataSource
        {
            get { return _titleLabels != null ? _titleLabels.ItemsSource : null; }
            set
            {
                if (_titleLabels != null)
                    _titleLabels.ItemsSource = value;
                else
                    _labelsDataSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _btnMinimize = GetTemplateChild("btnMinimize") as Button;
            if (_btnMinimize == null) throw new NullReferenceException();
            _btnMaximize = GetTemplateChild("btnMaximize") as Button;
            if (_btnMaximize == null) throw new NullReferenceException();
            _btnClose = GetTemplateChild("btnClose") as Button;
            if (_btnClose == null) throw new NullReferenceException();
            _titleLabels = GetTemplateChild("PART_TitleLabels") as ItemsControl;
            if (_titleLabels == null) throw new NullReferenceException();
            _titleLabels.ItemsSource = _labelsDataSource;

            _btnMinimize.Click += (sender, e) => { if (MinimizeClick != null) MinimizeClick(this, EventArgs.Empty); };
            _btnMaximize.Click += (sender, e) => { if (MaximizeClick != null) MaximizeClick(this, EventArgs.Empty); };
            _btnClose.Click += (sender, e) => { if (CloseClick != null) CloseClick(this, EventArgs.Empty); };

            _btnMaximize.Visibility = MaximizeBox ? Visibility.Visible : Visibility.Collapsed;
            _btnMinimize.Visibility = MinimizeBox ? Visibility.Visible : Visibility.Collapsed;
            _btnClose.Visibility = CloseBox ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void PanelGotNewState(ChartPanel chartPanel)
        {
            switch (chartPanel.State)
            {
                case ChartPanel.StateType.Maximized:
                    if (_btnMaximize != null)
                        _btnMaximize.Content = "2";
                    break;
                case ChartPanel.StateType.Normal:
                    if (_btnMaximize != null)
                        _btnMaximize.Content = "1";
                    break;
            }
        }

        ///<summary>
        /// Gets or sets a value indicating whether the Maximize button is displayed in the caption bar of the panel.
        ///</summary>
        public static DependencyProperty MaximizeBoxProperty =
          DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(ChartPanelTitleBar),
                                      new PropertyMetadata(true, MaximizeBoxPropertyChangedCallback));
        ///<summary>
        /// Gets or sets a value indicating whether the Maximize button is displayed in the caption bar of the panel.
        ///</summary>
        public bool MaximizeBox
        {
            get { return (bool)GetValue(MaximizeBoxProperty); }
            set { SetValue(MaximizeBoxProperty, value); }
        }

        private static void MaximizeBoxPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ChartPanelTitleBar chartPanel = (ChartPanelTitleBar)o;
            bool value = (bool)args.NewValue;
            if (chartPanel._btnMaximize != null)
                chartPanel._btnMaximize.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        ///<summary>
        /// Gets or sets a value indicating whether the Minimize button is displayed in the caption bar of the panel.
        ///</summary>
        public static DependencyProperty MinimizeBoxProperty =
          DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(ChartPanelTitleBar),
                                      new PropertyMetadata(true, MinimizeBoxPropertyChangedCallback));
        ///<summary>
        /// Gets or sets a value indicating whether the Minimize button is displayed in the caption bar of the panel.
        ///</summary>
        public bool MinimizeBox
        {
            get { return (bool)GetValue(MinimizeBoxProperty); }
            set { SetValue(MinimizeBoxProperty, value); }
        }

        private static void MinimizeBoxPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ChartPanelTitleBar chartPanel = (ChartPanelTitleBar)o;
            bool value = (bool)args.NewValue;
            if (chartPanel._btnMinimize != null)
                chartPanel._btnMinimize.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        ///<summary>
        /// Gets or sets a value indicating whether the Close button is displayed in the caption bar of the panel.
        ///</summary>
        public static DependencyProperty CloseBoxProperty =
          DependencyProperty.Register("CloseBox", typeof(bool), typeof(ChartPanelTitleBar),
                                      new PropertyMetadata(true, CloseBoxPropertyChangedCallback));
        ///<summary>
        /// Gets or sets a value indicating whether the Close button is displayed in the caption bar of the panel.
        ///</summary>
        public bool CloseBox
        {
            get { return (bool)GetValue(CloseBoxProperty); }
            set { SetValue(CloseBoxProperty, value); }
        }

        private static void CloseBoxPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ChartPanelTitleBar chartPanel = (ChartPanelTitleBar)o;
            bool value = (bool)args.NewValue;
            if (chartPanel._btnClose != null)
                chartPanel._btnClose.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
