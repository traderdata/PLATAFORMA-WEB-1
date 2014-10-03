using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.Controls;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    ///<summary>
    ///</summary>
    public partial class ChartPanelTitleCaptionEntry : Control
    {
        private UIElement _root;
        private bool _isMouseOver;

#if WPF
    static ChartPanelTitleCaptionEntry()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof (ChartPanelTitleCaptionEntry),
                                               new FrameworkPropertyMetadata(typeof (ChartPanelTitleCaptionEntry)));
    }
#endif

        ///<summary>
        ///</summary>
        public ChartPanelTitleCaptionEntry()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartPanelTitleCaptionEntry);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _root = (UIElement)GetTemplateChild("PART_Root");

            if (_root == null)
                throw new NullReferenceException("Root part must exists.");

            _root.MouseLeftButtonUp += RootOnMouseLeftButtonUp;
            _root.MouseEnter += (sender, args) =>
                                  {
                                      _isMouseOver = true;
                                      GoToState(true);
                                  };
            _root.MouseLeave += (sender, args) =>
                                  {
                                      _isMouseOver = false;
                                      GoToState(true);
                                  };
            GoToState(false);
        }

        private void GoToState(bool useTransitions)
        {
            var title = ((SeriesTitleLabel)DataContext);

            if (title.ShowFrame == Visibility.Collapsed) return;

            //  Go to states in NormalStates state group
            if (_isMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
        }


        public void RootOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            //ATENÇÃO: ESTA É A PARTE RESPONSAVEL POR ABRIR O DIALOGO DO INDICADOR QUANDO CLICAMOS EM SEU NOME NA BARRA DE TITULO

            Series series = ((SeriesTitleLabel)DataContext).Series;

            if (!(series is IChartElementPropertyAble)) 
                return;

            series._chartPanel._chartX.DisparaOnTituloIndicador(series);
            VisualStateManager.GoToState(this, "Normal", true);

            //IChartElementPropertyAble propertyAble = (IChartElementPropertyAble)series;
            //List<IChartElementProperty> properties = new List<IChartElementProperty>(propertyAble.Properties);
            //PropertiesDialog dialog = new PropertiesDialog(propertyAble.Title, properties)
            //#if SILVERLIGHT
            // { AppRoot = series._chartPanel._chartX.AppRoot }
            //#endif
            //;
            //dialog.Background = series._chartPanel._chartX.LineStudyPropertyDialogBackground;

            //foreach (IChartElementProperty property in properties)
            //{
            //    if (property is ChartElementColorProperty)
            //        ((ChartElementColorProperty)property).AppRoot
            //        #if SILVERLIGHT
            //         = series._chartPanel._chartX.AppRoot;
            //        #endif
            //        #if WPF
            //                                                = null;
            //        #endif
            //}
            //    #if SILVERLIGHT
            //                dialog.Show(Dialog.DialogStyle.ModalDimmed);
            //    #endif
            //    #if WPF
            //                    dialog.ShowDialog();
            //    #endif
        }
    }
}
