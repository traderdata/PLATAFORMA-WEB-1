using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
#endif
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
#if SILVERLIGHT
    [TemplatePart(Name = BackgroundElement, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TextElement, Type = typeof(TextBlock))]
#endif
    ///<summary>
    ///</summary>
    public partial class MoveSeriesIndicator : Control
    {
#if WPF
    static MoveSeriesIndicator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveSeriesIndicator), new FrameworkPropertyMetadata(typeof(MoveSeriesIndicator)));

      MoveStatusProperty =
        DependencyProperty.Register("MoveStatus", typeof (MoveStatusEnum),
                                    typeof (MoveSeriesIndicator),
                                    new FrameworkPropertyMetadata(MoveStatusEnum.CantMove, OnMoveStatusChanged));
    }

    private static void OnMoveStatusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
    }
    internal static readonly DependencyProperty MoveStatusProperty;
    internal MoveStatusEnum MoveStatus
    {
      get { return (MoveStatusEnum)GetValue(MoveStatusProperty); }
      set { SetValue(MoveStatusProperty, value); }
    }
#endif
#if SILVERLIGHT
        private const string BackgroundElement = "PART_Background";
        private const string TextElement = "PART_Text";
        public MoveSeriesIndicator()
        {
            DefaultStyleKey = typeof(MoveSeriesIndicator);
        }

        public static readonly DependencyProperty BackgroundExProperty =
          DependencyProperty.Register("BackgroundEx", typeof(Brush), typeof(MoveSeriesIndicator),
                                      new PropertyMetadata(Brushes.Red, (d, e) => ((MoveSeriesIndicator)d).OnBackgroundExChanged(d, e)));

        public Brush BackgroundEx
        {
            get { return (Brush)GetValue(BackgroundExProperty); }
            set { SetValue(BackgroundExProperty, value); }
        }

        protected virtual void OnBackgroundExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty TextExProperty =
          DependencyProperty.Register("TextEx", typeof(string), typeof(MoveSeriesIndicator),
                                      new PropertyMetadata("Não é possível mover", (d, e) => ((MoveSeriesIndicator)d).OnTextExChanged(d, e)));

        public string TextEx
        {
            get { return (string)GetValue(TextExProperty); }
            set { SetValue(TextExProperty, value); }
        }

        protected virtual void OnTextExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty MoveStatusProperty =
          DependencyProperty.Register("MoveStatus", typeof(MoveStatusEnum), typeof(MoveSeriesIndicator),
                                      new PropertyMetadata(MoveStatusEnum.ImpossivelMover, (d, e) => ((MoveSeriesIndicator)d).OnMoveStatusChanged(d, e)));

        public MoveStatusEnum MoveStatus
        {
            get { return (MoveStatusEnum)GetValue(MoveStatusProperty); }
            set { SetValue(MoveStatusProperty, value); }
        }

        protected virtual void OnMoveStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MoveStatusEnum newStatus = (MoveStatusEnum)e.NewValue;
            MoveStatusEnum oldStatus = (MoveStatusEnum)e.OldValue;
            if (newStatus == oldStatus) return;

            MoveSeriesIndicator moveSeriesIndicator = (MoveSeriesIndicator)d;

            switch (newStatus)
            {
                case MoveStatusEnum.ImpossivelMover:
                    moveSeriesIndicator.BackgroundEx = Brushes.Red;
                    moveSeriesIndicator.TextEx = "Impossível mover";
                    break;
                case MoveStatusEnum.MoverPainelExistente:
                    moveSeriesIndicator.BackgroundEx = Brushes.Blue;
                    moveSeriesIndicator.TextEx = "Mover para painel";
                    break;
                case MoveStatusEnum.MoverNovoPainel:
                    moveSeriesIndicator.BackgroundEx = Brushes.Green;
                    moveSeriesIndicator.TextEx = "Novo painel";
                    break;
            }
        }
#endif

        ///<summary>
        ///</summary>
        public enum MoveStatusEnum
        {
            /// <summary>
            /// series can't be moved
            /// 1. cause it is droped on same panel
            /// 2. cause the unique series from panel is used to create a new panel
            /// </summary>
            ImpossivelMover,
            /// <summary>
            /// only a series from a panel with multiple series can be used to create a new panel
            /// this flag also includes that series can be droped on an existing panel.
            /// </summary>
            MoverNovoPainel,
            /// <summary>
            /// any series can be droped on an existing panel
            /// </summary>
            MoverPainelExistente
        }

        internal double X
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        internal double Y
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }
    }
}
