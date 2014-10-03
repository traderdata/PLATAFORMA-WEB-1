using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Line=System.Windows.Shapes.Line;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public partial class PanelsContainer : Panel
    {
        private enum StateEnum { Normal, ResizingPanel, MovingPanel };

        private readonly List<ChartPanel> paineis = new List<ChartPanel>();
        private readonly Storyboard maximizationStoryboard;
        private readonly Storyboard minimizationStoryBoard;
        private readonly Storyboard restoreMinimizedStoryboard;
        private bool _layoutChanged; //used when a manel is maximized and the window changes its size, after restoring back the panel must resize all panels
        private StateEnum estado = StateEnum.Normal;
        private MoveSeriesIndicator moveSeriesIndicator;

        internal readonly Canvas panelsHolder; //this canvas wil be resized automatically, then on it will place the panels
        internal InfoPanel infoPanel;
        internal ChartPanel painelMaximizado;
        internal StockChartX stockChart;
        internal Line verticalCrossHair;
        internal Line horizontalCrossHair;

        /// <summary>
        /// List used to remember panels that need to be repainted. It is used in case
        /// when PriceStyle != psStandard, in this case panel with OHLC series must first be calculated
        /// this will ensure correct values in xMap that will eb used to paint the rest of chart
        /// </summary>
        internal List<ChartPanel> _panelsToBeRepainted = new List<ChartPanel>();

        internal LineStudyContextMenu _lineStudyContextMenu;
        ///<summary>
        ///</summary>
        public PanelsContainer()
        {
            Resources.Add("max_animation", maximizationStoryboard = new Storyboard());
            Resources.Add("min_animation", minimizationStoryBoard = new Storyboard());
            Resources.Add("restoremin_animation", restoreMinimizedStoryboard = new Storyboard());

            panelsHolder = new Canvas
                              {
#if WPF
                          ClipToBounds = true
#else
                                  //Clip = new RectangleGeometry() 
#endif
                              };
            Children.Add(panelsHolder);
            panelsHolder.Background = Background;
            HookPanelsHolderMouseEvents(true);

            _lineStudyContextMenu = new LineStudyContextMenu
                                      {
                                          Visibility = Visibility.Collapsed
                                      };
            panelsHolder.Children.Add(_lineStudyContextMenu);
#if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(this);
            MouseMove += (sender, e) => Mouse.UpdateMousePosition(this, e.GetPosition(this));
#endif
        }

        internal Point ToPanelsHolder(MouseButtonEventArgs eventArgs)
        {
            return eventArgs.GetPosition(panelsHolder);
        }
        internal Point ToPanelsHolder(MouseEventArgs eventArgs)
        {
            return eventArgs.GetPosition(panelsHolder);
        }

        internal ChartPanel AddPanel(ChartPanel.PositionType position)
        {
            return AddPanel(position, false);
        }

        internal ChartPanel AddPanel(ChartPanel.PositionType position, bool heatMap)
        {
            //find the right position in the vector of panels
            int iIndexToInsertAt;
            //
            //
            if (!CanAddNewPanel(position, out iIndexToInsertAt))
                return null;

            ChartPanel panel = !heatMap
                                 ? new ChartPanel(stockChart, position) { _panelsContainer = this }
                                 : new ChartPanel_HeatMap(stockChart) { _panelsContainer = this };

            panel.Visible = false;

            paineis.Insert(iIndexToInsertAt, panel);
            panelsHolder.Children.Add(panel);
#if SILVERLIGHT
            panel.ApplyTemplate();
#endif

            if (painelMaximizado == null)
                panel.State = ChartPanel.StateType.New;
            else
            {
                panel.State = ChartPanel.StateType.Minimized;
                panel._normalHeightPct = 0.1;//when a panel is maximized and we add a new panel, assign 50% of height to it
                stockChart.AddMinimizedPanel(panel);
            }

            //update panels internal index
            int iIndex = 0;
            paineis.ForEach(p =>
                              {
                                  p._index = iIndex++;
                                  //p.Name = "panel_" + p._index;
                              });

            panel.OnMaximizeClick += (sender, e) => MaxMinPanel((ChartPanel)sender);
            panel.OnMinimizeClick += (sender, e) => MinimizePanel((ChartPanel)sender);
            panel.OnCloseClick += (sender, e) =>
                                    {
                                        if (panel.State == ChartPanel.StateType.Maximized)
                                        {
                                            MessageBox.Show("Um painel maximizado não pode ser fechado.");
                                            return;
                                        }
                                        ClosePanel((ChartPanel)sender);
                                    };

            //resize evenly all panels
            ResizePanels(ResizeType.NewPanel, panel);

            if (stockChart.CrossHairs && verticalCrossHair != null)
            {
                Canvas.SetZIndex(verticalCrossHair, ZIndexConstants.CrossHairs);
                Canvas.SetZIndex(horizontalCrossHair, ZIndexConstants.CrossHairs);
            }

            if (paineis.Count > 1)
                panel.Background = paineis[0].Background;

            return panel;
        }

        private int GetIndexToInsertAt(ChartPanel.PositionType position)
        {
            int iIndexToInsertAt = 0;
            switch (position)
            {
                case ChartPanel.PositionType.AlwaysTop: //find next panel that hasn't AlwayTop value
                    foreach (ChartPanel chartPanel in paineis)
                    {
                        if (chartPanel._position != ChartPanel.PositionType.AlwaysTop)
                            break;
                        iIndexToInsertAt++;//move to next panel
                    }
                    break;
                case ChartPanel.PositionType.AlwaysBottom: //insert at the bottom
                    iIndexToInsertAt = paineis.Count;
                    break;
                default: //find first AlwaysBottom and insert before it
                    foreach (ChartPanel chartPanel in paineis)
                    {
                        if (chartPanel._position == ChartPanel.PositionType.AlwaysBottom)
                            break;
                        iIndexToInsertAt++; //next non-bottom panel
                    }
                    break;
            }
            return iIndexToInsertAt;
        }

        internal int VisiblePanelsCount
        {
            get
            {
                int iCount = 0;
                paineis.ForEach(p => { if (p.Visible) iCount++; });
                return iCount;
            }
        }

        internal int PrevVisiblePanelIndex(int currentIndex)
        {
            int iIndex = currentIndex - 1;
            while (iIndex > -1 && !paineis[iIndex].Visible && paineis[iIndex] != painelMaximizado)
            {
                iIndex--;
            }
            return iIndex >= 0 ? iIndex : -1;
        }

        internal ChartPanel PrevVisiblePanel(ChartPanel current)
        {
            int index = PrevVisiblePanelIndex(current._index);
            return index != -1 ? paineis[index] : null;
        }

        internal int NextVisiblePanelIndex(int currentIndex)
        {
            int iIndex = currentIndex + 1;
            while (iIndex < paineis.Count && !paineis[iIndex].Visible && paineis[iIndex] != painelMaximizado)
            {
                iIndex++;
            }
            return iIndex < paineis.Count ? iIndex : -1;
        }

        internal ChartPanel NextVisiblePanel(ChartPanel current)
        {
            int index = NextVisiblePanelIndex(current._index);
            return index == -1 ? null : paineis[index];
        }

        internal ChartPanel PanelByY(double Y)
        {
            //if (painelMaximizado != null) return null;
            foreach (ChartPanel panel in paineis)
            {
                if (panel.Visible && Utils.Between(Y, panel.Top, panel.Top + panel.ActualHeight))
                    return panel;
            }
            return null;
        }

        internal void RecyclePanels()
        {
            for (int i = 0; i < paineis.Count; i++)
            {
                if (paineis[i].SeriesCount != 0 || (paineis[i] is ChartPanel_HeatMap)) continue;
                ClosePanel(paineis[i]);
                i--;
            }
        }


        internal void PostResetPanels()
        {
            ResetHeatMapPanels();

            if (!Utils.GetIsInDesignMode(this))
                RecyclePanels();
        }

        private int _panelsToBePaintedCount;

        /// <summary>
        /// repaints and deletes empty panels
        /// </summary>
        internal void ResetPanels()
        {
            //paint first regular panels. this will make sure all indicators are calculated, 
            //cause later the heat map panel will us them
            _panelsToBePaintedCount = 0;
            paineis.ForEach(p
                            =>
                              {
                                  if (p.IsHeatMap) return;
                                  _panelsToBePaintedCount++;
                                  p._afterPaintAction =
                                    () =>
                                    {
                                        _panelsToBePaintedCount--;
                                        if (_panelsToBePaintedCount == 0)
                                            PostResetPanels();
                                    };
                                  p.Paint();
                              });

            //
            //      ResetHeatMapPanels();
            //
            //      if (!Utils.IsInDesignMode())
            //        RecyclePanels();

        }

        internal void ResetHeatMapPanels()
        {
            paineis.ForEach(p =>
                              {
                                  if (!p.IsHeatMap) return;
                                  p.Paint();
                              });
        }

        public enum ResizeType
        {
            Even,
            Proportional,
            /// <summary>
            /// when minimazing the panel give its height to all other visible panels
            /// </summary>
            PanelMinimized,
            /// <summary>
            /// when adding a new panel it will take the half of above or belowe panel
            /// </summary>
            NewPanel,
            /// <summary>
            /// used when inserting back a minimized panel. it is used its saved percentage of height
            /// </summary>
            InsertExisting,
            /// <summary>
            /// mostly used after rearanging panels. just reposition, not resize
            /// </summary>
            Reposition,
            /// <summary>
            /// Mesma coisa do Even, só que usado no panel que nao apresentao o preço
            /// </summary>
            EvenSemPreco,
            TamanhoIndicadorFixo
        }

        private Size _size;
        public void ResizePanels(ResizeType resizeType, params object[] args)
        {
            ResizePanels(resizeType, false, args);
        }
        private Rect ResizePanels(ResizeType resizeType, bool bOnlyCalculate, params object[] args)
        {
            if (paineis.Count == 0)
                return new Rect();
            Rect rcBounds = new Rect(0, 0, _size.Width, _size.Height);
            double dPanelHeight;
            double dTop;

            if (painelMaximizado != null)
            {
                painelMaximizado.Bounds = rcBounds;
                _layoutChanged = true;
                return new Rect();
            }

            switch (resizeType)
            {
                case ResizeType.Reposition:
                    dTop = 0;
                    foreach (ChartPanel panel in paineis)
                    {
                        if (panel == painelMaximizado || panel.State != ChartPanel.StateType.Normal) continue;
                        panel.Top = dTop;
                        dTop += panel.Height;
                    }
                    break;
                case ResizeType.Proportional:
                    double dOldPanelsHeight = 0;
                    paineis.ForEach(p =>
                                      {
                                          if (!p.Visible) return;
                                          dOldPanelsHeight += p.Bounds.Height;
                                      });

                    double dMultiplier = dOldPanelsHeight == 0 ? 1 : rcBounds.Height / dOldPanelsHeight;
                    paineis.ForEach(p =>
                                      {
                                          if (p == painelMaximizado || p.State != ChartPanel.StateType.Normal) return;
                                          Rect rcPanelBounds = p.Bounds;
                                          Rect rcPanel = new Rect(rcBounds.Left, rcPanelBounds.Top * dMultiplier,
                                                                  rcBounds.Width, rcPanelBounds.Height * dMultiplier);
                                          p.Bounds = rcPanel;
                                      });
                    break;
                case ResizeType.Even:
                    dPanelHeight = rcBounds.Height / paineis.Count;
                    double dPanelTop = rcBounds.Top;

                    paineis.ForEach(p =>
                        {
                            if (p.State == ChartPanel.StateType.Minimized)
                                RestauraPainelMinimizado(p);
                        });


                    paineis.ForEach(p =>
                                      {
                                         if (!p.Visible) return;
                                          Rect rcPanel = new Rect(rcBounds.Left, dPanelTop, rcBounds.Width, dPanelHeight);
                                          p.Bounds = rcPanel;
                                          dPanelTop += dPanelHeight;
                                      });
                    break;
                case ResizeType.EvenSemPreco:
                    dPanelHeight = rcBounds.Height / (paineis.Count-1);
                    double dPanelTopIndicador = rcBounds.Top;

                    paineis.ForEach(p =>
                    {
                        if (p.State == ChartPanel.StateType.Minimized)
                            RestauraPainelMinimizado(p);
                    });


                    paineis.ForEach(p =>
                    {
                        if (!p.Visible) return;
                        Rect rcPanel = new Rect(rcBounds.Left, dPanelTopIndicador, rcBounds.Width, dPanelHeight);
                        p.Bounds = rcPanel;
                        dPanelTopIndicador += dPanelHeight;
                    });
                    break;
                case ResizeType.PanelMinimized:
                    dPanelHeight = (double)args[0];
                    double dExtraHeight = dPanelHeight / VisiblePanelsCount;
                    dTop = 0;
                    foreach (ChartPanel panel in paineis)
                    {
                        if (!panel.Visible) continue;
                        panelsHolder.BringToFront(panel);
                        Rect rcPanelBounds = panel.Bounds;
                        panel.Bounds = new Rect(rcPanelBounds.Left, dTop, rcPanelBounds.Width,
                                                rcPanelBounds.Height + dExtraHeight);
                        dTop += (rcPanelBounds.Height + dExtraHeight);
                    }
                    break;
                case ResizeType.InsertExisting:
                    Debug.Assert(args.Length > 0);
                    ChartPanel panelToRestore = (ChartPanel)args[0];
                    double dPanelHeightPct = panelToRestore._normalHeightPct; //it is in percents 0.xx
                    if (dPanelHeightPct >= 0.99 && VisiblePanelsCount > 1)
                        dPanelHeightPct /= 2;

                    //take from all panels the part of height needed by the restored panel
                    //its state must be changes to Normal and it must be removed from minimized list of panels
                    double dPanelsRemainingPct = 1 - dPanelHeightPct;
                    dTop = 0;
                    foreach (ChartPanel panel in paineis)
                    {
                        if (!panel.Visible && !bOnlyCalculate) continue;
                        double dPanelNewHeight;
                        if (panel._index != panelToRestore._index)
                        {
                            double dPanelCurPct = panel.Height / rcBounds.Height;
                            double dPanelNewPct = dPanelCurPct * dPanelsRemainingPct;
                            dPanelNewHeight = rcBounds.Height * dPanelNewPct;
                        }
                        else
                            dPanelNewHeight = rcBounds.Height * dPanelHeightPct;
                        if (!bOnlyCalculate)
                        {
                            panel.Top = dTop;
                            panel.Height = dPanelNewHeight;
                            panel.Left = rcBounds.Left;
                            panel.Width = rcBounds.Width;
                        }
                        else
                        {
                            if (panel._index == panelToRestore._index)
                                return new Rect(rcBounds.Left, dTop, rcBounds.Width, dPanelNewHeight);
                        }
                        dTop += dPanelNewHeight;
                    }
                    break;
                case ResizeType.NewPanel:
                    Debug.Assert(args.Length > 0);
                    ChartPanel newPanel = (ChartPanel)args[0];
                    ChartPanel abovePanel = PrevVisiblePanel(newPanel);
                    if (abovePanel == null) //no panels
                    {
                        if (VisiblePanelsCount == 0)
                        {
                            newPanel.Bounds = new Rect(rcBounds.Left, 0, rcBounds.Width, rcBounds.Height);
                        }
                        else
                        {
                            ChartPanel nextPanel = NextVisiblePanel(newPanel);
                            double dNewHeight = nextPanel.Height / 2;
                            nextPanel.Top += dNewHeight;
                            nextPanel.Height -= dNewHeight;
                            newPanel.Bounds = new Rect(rcBounds.Left, 0, rcBounds.Width, dNewHeight);
                        }
                    }
                    else
                    {
                        double dNewHeight = abovePanel.Height / 2;
                        abovePanel.Height = dNewHeight;
                        newPanel.Bounds = new Rect(rcBounds.Left, abovePanel.Top + dNewHeight, rcBounds.Width, dNewHeight);
                    }
                    newPanel.Visible = true;
                    newPanel.State = ChartPanel.StateType.Normal;
                    break;
            }
            return new Rect();
        }

        private bool CanAddNewPanel(ChartPanel.PositionType position, out int at)
        {
            at = GetIndexToInsertAt(position);

            int aboveIndex = PrevVisiblePanelIndex(at);
            if (aboveIndex == -1) //no panels
                return true;

            if (paineis[aboveIndex].ActualHeight == 0)
                return true;

            return paineis[aboveIndex].ActualHeight / 2 >= PanelAllowedMinimumHeight;
        }

        #region Ovverides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            //Size availableSize = new Size(constraint.Width, double.PositiveInfinity);
            Size childSize = availableSize;

            panelsHolder.Measure(childSize);
            //return availableSize;
            return panelsHolder.DesiredSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrangeSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            panelsHolder.Arrange(new Rect(new Point(0, 0), arrangeSize));

            _size = arrangeSize;

            if (stockChart.Name.ToUpper().Contains("INDICA"))
                ResizePanels(ResizeType.EvenSemPreco);
            else
                ResizePanels(ResizeType.Proportional);
            //ResizePanels(ResizeType.Proportional);

            return arrangeSize;
        }
        #endregion

        #region Maximize/Restore/Minimize panels
        /// <summary>
        /// maximize and restores a panel
        /// </summary>
        /// <param name="panel"></param>
        public void MaxMinPanel(ChartPanel panel)
        {
            if (paineis.Count - stockChart.paineisBar.PanelCount <= 1)
            {
                //throw new InvalidOperationException("Last visible panel can't be maximized.");
                MessageBox.Show("O último painel visível não pode ser maximizado.", "Error", MessageBoxButton.OK
            #if WPF
                , MessageBoxImage.Error
            #endif
                );
                return;
            }

            Rect rcBounds = new Rect(new Point(), _size);
            //current panel's bounds
            Rect rcPanelBounds = panel.Bounds;
            //remeber the reference
            painelMaximizado = panel;
            if (panel.State == ChartPanel.StateType.Normal) //maximizing
            {
                panel.State = ChartPanel.StateType.Maximized;
                //remember panel's position and size
                panel._normalTopPosition = rcPanelBounds.Top;
                panel._normalHeight = rcPanelBounds.Height;

                if (stockChart.MostrarAnimacoes)
                {
                    CreateAnimationObject(maximizationStoryboard, MaximizationCompletedEvent);
                    CreateMinMaxRect();
                    InitAnimation(maximizationStoryboard, rcPanelBounds, rcBounds);
                    EnsureMinMaxRectVisible();

                    Storyboard.SetTarget(maximizationStoryboard, _rcMinMax);
                    maximizationStoryboard.Begin();
                }
                else
                {
                    MaximizePanel();
                }
            }
            else
            {
                panel.State = ChartPanel.StateType.Normal;
                Rect rcNormalSizePanel = new Rect(rcBounds.Left, panel._normalTopPosition, rcBounds.Width, panel._normalHeight);
                //minimizing
                if (stockChart.MostrarAnimacoes)
                {
                    InitAnimation(maximizationStoryboard, rcBounds, rcNormalSizePanel);
                    EnsureMinMaxRectVisible();

                    //_rcMinMax.BeginStoryboard(_maximizationStoryboard);
                    Storyboard.SetTarget(maximizationStoryboard, _rcMinMax);
                    maximizationStoryboard.Begin();
                }
                else
                {
                    RestoreMaximizedPanel();
                }
            }
        }

        public void MaximizePanel()
        {
            Rect rcBounds = new Rect(new Point(), _size);

            painelMaximizado.Bounds = rcBounds;
            //hide all panels except maximized
            paineis.ForEach(p => { if (p != painelMaximizado) p.Visible = false; });
        }

        public void RestoreMaximizedPanel()
        {
            Rect rcBounds = new Rect(new Point(), _size);
            Rect rcNormalSizePanel = new Rect(rcBounds.Left, painelMaximizado._normalTopPosition, rcBounds.Width,
                                              painelMaximizado._normalHeight);
            //show again all panels non-minimized panels
            foreach (ChartPanel p in paineis)
            {
                if (p.IsEnabled)
                    p.Visible = true;
            }
            //paineis.ForEach(p => p.Visible = true);

            painelMaximizado.Bounds = rcNormalSizePanel;
            painelMaximizado = null;
            if (_layoutChanged)
                ResizePanels(ResizeType.Proportional);
            _layoutChanged = false;
        }

        private ChartPanel _minimizedPanel;
        public void MinimizePanel(ChartPanel panel)
        {
            if (VisiblePanelsCount == 1)
            {
                //throw new ChartException("Last visible panel can't be minimized.");
                MessageBox.Show("O último painel visível não pode ser minimizado.", "Error", MessageBoxButton.OK
#if WPF
          , MessageBoxImage.Error
#endif
);
                return;
            }
            _minimizedPanel = panel;
            panel._normalHeightPct = panel.ActualHeight / panelsHolder.ActualHeight;
            panel._normalHeight = panel.ActualHeight;
            if (stockChart.MostrarAnimacoes)
            {
                CreateMinMaxRect();
                CreateAnimationObject(minimizationStoryBoard, MinimizationCompletedEvent);

                panel._minimizedRect = stockChart.paineisBar.GetNextRectToMinimize;
                panel._minimizedRect.Y += ActualHeight;
                InitAnimation(minimizationStoryBoard, panel.Bounds, panel._minimizedRect);
                EnsureMinMaxRectVisible();

                Storyboard.SetTarget(minimizationStoryBoard, _rcMinMax);
                minimizationStoryBoard.Begin();
                //_minimizationStoryBoard.Begin(_rcMinMax);
            }
            else
            {
                DoMinimizePanel(panel);
            }
        }

        private void DoMinimizePanel(ChartPanel chartPanel)
        {
            chartPanel.Visible = false;
            chartPanel.State = ChartPanel.StateType.Minimized;
            stockChart.AddMinimizedPanel(chartPanel);
            ResizePanels(ResizeType.PanelMinimized, chartPanel._normalHeight);
            HideMinMaxRect();
        }

        private void MinimizationCompletedEvent(object sender, EventArgs e)
        {
            DoMinimizePanel(_minimizedPanel);
            _minimizedPanel = null;
        }

        public void RestauraPainelMinimizado(ChartPanel chartPanel)
        {
            RestorePanel(chartPanel);
        }

        internal void RestorePanel(ChartPanel chartPanel)
        {
            if (painelMaximizado != null)
            {
                //throw new InvalidOperationException("Can't restore a minimized panel while there is a maximized panel.");
                MessageBox.Show("Não é possível restaurar um painel minimizado enquanto houver um painel maximizado..", "Error",
                                MessageBoxButton.OK
#if WPF
                        , MessageBoxImage.Error
#endif
);
                return;
            }
            if (stockChart.MostrarAnimacoes)
            {
                CreateMinMaxRect();
                CreateAnimationObject(restoreMinimizedStoryboard, RestoreMinimizedCompletedEvent);

                Rect rcWhere = ResizePanels(ResizeType.InsertExisting, true, chartPanel);
                InitAnimation(restoreMinimizedStoryboard, chartPanel._minimizedRect, rcWhere);
                EnsureMinMaxRectVisible();

                _minimizedPanel = chartPanel;

                Storyboard.SetTarget(restoreMinimizedStoryboard, _rcMinMax);
                restoreMinimizedStoryboard.Begin();
                //_restoreMinimizedStoryboard.Begin(_rcMinMax);
            }
            else
            {
                DoRestorePanel(chartPanel);
            }
        }

        private void RestoreMinimizedCompletedEvent(object sender, EventArgs args)
        {
            DoRestorePanel(_minimizedPanel);
        }

        private void DoRestorePanel(ChartPanel chartPanel)
        {
            chartPanel.State = ChartPanel.StateType.Normal;
            chartPanel.Visible = true;
            ResizePanels(ResizeType.InsertExisting, chartPanel);
            stockChart.DeleteMinimizedPanel(chartPanel);
            HideMinMaxRect();
        }

        private void ClosePanel(ChartPanel panel)
        {
            /*if (VisiblePanelsCount == 1)
            {
              MessageBox.Show("Last visible panel can't be closed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
              return;
            } */
            if ((panel.SeriesCount > 0 || panel.IsHeatMap) && stockChart.FireChartPanelBeforeClose(panel)) return;
            if (!panel.CanBeDeleted)
            {
                MessageBox.Show(panel.ReasonCantBeDeleted, "Error", MessageBoxButton.OK
#if WPF
          , MessageBoxImage.Error
#endif
);
                return;
            }

            RemovePainel(panel);
        }

        public void RemovePainel(ChartPanel panel)
        {
            double dExtraHeight = panel.Height;
            paineis.Remove(panel);
            panelsHolder.Children.Remove(panel);
            panel.UnRegisterSeriesFromDataManager();
            ResizePanels(ResizeType.PanelMinimized, dExtraHeight);

            //reindex panels
            int iIndex = 0;
            paineis.ForEach(p => p._index = iIndex++);

            stockChart.ReCalc = true; //cause panel was removed we must recalculate the chart. 
        }

        internal void CloseHeatMap()
        {
            ChartPanel heatMap = paineis.FirstOrDefault(p => p.IsHeatMap);
            if (heatMap == null)
                return;

            RemovePainel(heatMap);
        }

        internal void ClearAll()
        {
            foreach (var panel in paineis)
            {
                panelsHolder.Children.Remove(panel);
            }
            paineis.Clear();
            _panelToMove = null;
            _panelToResize = null;
        }

        public void LimpaGraficoSemDeletarPaineis(bool limpaIndicadores, bool limpaObjetos)
        {
            int index = 0;

            //Excluindo séries
            foreach (ChartPanel painel in paineis)
            {
                if (painel != null)
                {
                    foreach (Series serie in painel.Series)
                    {
                        if (serie.FullName.Contains(".Ultimo") || serie.FullName.Contains(".Abertura") || serie.FullName.Contains(".Maximo")
                                                  || serie.FullName.Contains(".Minimo")|| (serie.FullName.Contains(".Volume")))
                        {
                            painel.RemoveSeries(serie, true);
                        }
                        else if (((serie.SeriesType == EnumGeral.TipoSeriesEnum.Indicador) || (serie.IsSerieFilha)) && (limpaIndicadores))
                            painel.RemoveSeries(serie, true);
                    }
                }

                index++;
            }
        }

       
        #endregion

        internal List<ChartPanel> Panels
        {
            get { return paineis; }
        }

        #region Panel Resizing
        private ChartPanelsDivider _panelsDivider;
        private ChartPanel _panelToResize;

        private void HookPanelsHolderMouseEvents(bool bHook)
        {
            if (bHook)
            {
                panelsHolder.MouseMove += PanelsHolder_OnMouseMove;
#if WPF
        _panelsHolder.MouseLeave += (sender, e) => Mouse.OverrideCursor = null;
#endif
                panelsHolder.MouseLeftButtonUp += PanelsHolder_OnMouseUp;
                panelsHolder.MouseLeftButtonDown += PanelsHolder_OnMouseDown;
                panelsHolder.KeyDown += PanelsHolder_OnKeyDown;
            }
            else
            {
                panelsHolder.MouseMove -= PanelsHolder_OnMouseMove;
#if WPF
        _panelsHolder.MouseUp -= PanelsHolder_OnMouseUp;
        _panelsHolder.MouseDown -= PanelsHolder_OnMouseDown; 
#endif
#if SILVERLIGHT
                panelsHolder.MouseLeftButtonUp -= PanelsHolder_OnMouseUp;
                panelsHolder.MouseLeftButtonDown -= PanelsHolder_OnMouseDown;
#endif
            }
        }

        private bool CanResizePanel(double X, double Y)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return false;

            if (_panelToResize != null)
                _panelToResize.Cursor = null;

            _panelToResize = null;
            if (VisiblePanelsCount > 1 && X < panelsHolder.ActualWidth - 100)
            {
                foreach (ChartPanel panel in paineis)
                {
                    if (!panel.Visible || panel._index <= 0 || !Utils.Between(Y, panel.Top, panel.Top + 2)) continue;
                    _panelToResize = panel;
                    break;
                }
            }
#if WPF
      //Mouse.OverrideCursor = _panelToResize != null ? Cursors.SizeNS : null;
#endif
            //#if SILVERLIGHT
            if (_panelToResize != null)
                _panelToResize.Cursor = Cursors.SizeNS;
            //#endif

            return _panelToResize != null;
        }

        private void EnsurePanelsDividerVisible()
        {
            if (_panelsDivider == null)
            {
                _panelsDivider = new ChartPanelsDivider();
                panelsHolder.Children.Add(_panelsDivider);
                _panelsDivider.ApplyTemplate();
            }
            _panelsDivider.Visible = true;
            panelsHolder.BringToFront(_panelsDivider);
            _panelsDivider.Width = panelsHolder.ActualWidth;
        }


        internal void StartResizePanel(double Y)
        {
            _panelToResize.State = ChartPanel.StateType.Resizing;
            panelsHolder.CaptureMouse();
            EnsurePanelsDividerVisible();
            _panelsDivider.Y = Y;
        }
        internal void DoResizePanel(double Y)
        {
            if (Y < _panelToResize.Top) //rezise up
            {
                ChartPanel abovePanel = PrevVisiblePanel(_panelToResize);
                Debug.Assert(abovePanel != null);
                _panelsDivider.IsOK = Y > (abovePanel.Top + abovePanel.TitleBarHeight);
            }
            else if (Y > (_panelToResize.Top + _panelToResize.TitleBarHeight))
            {
                _panelsDivider.IsOK = Y <= (_panelToResize.Top + _panelToResize.ActualHeight);
            }

            _panelsDivider.Y = Y;
        }
        internal void CancelResizePanel()
        {
            panelsHolder.ReleaseMouseCapture();
            _panelToResize = null;
            _panelsDivider.Visible = false;
        }
        internal void EndResizePanel(double Y)
        {
            //resize the current panel, and the above one
            panelsHolder.ReleaseMouseCapture();
            _panelsDivider.Visible = false;

            if (Y <= 0)
                Y = 10;
            ResizePanel(_panelToResize, Y);

            _panelToResize.State = ChartPanel.StateType.Normal;
            _panelToResize = null;
        }

        internal const double PanelAllowedMinimumHeight = Constants.PanelTitleBarHeight + 10;
        internal void ResizePanel(ChartPanel panelToResize, double Y)
        {
            ChartPanel abovePanel = PrevVisiblePanel(panelToResize);
            Debug.Assert(abovePanel != null);

            if (Y <= abovePanel.Top)
                Y = abovePanel.Top + 10;

            //Debug.Assert(Y > abovePanel.Top);

            double newHeight = panelToResize.Height + (panelToResize.Top - Y);


            if (newHeight >= PanelAllowedMinimumHeight)
            {
                abovePanel.Height = Y - abovePanel.Top;

                panelToResize.Height += (panelToResize.Top - Y);
                panelToResize.Top = Y;
            }
            else
            {
                abovePanel.Height += panelToResize.Height - PanelAllowedMinimumHeight;

                panelToResize.Top = (panelToResize.Top + panelToResize.Height - PanelAllowedMinimumHeight);
                panelToResize.Height = PanelAllowedMinimumHeight;
            }
        }

        internal void ResizePanelByHeight(ChartPanel panelToResize, double newHeight)
        {
            ChartPanel belowPanel = NextVisiblePanel(panelToResize);
            ChartPanel abovePanel = PrevVisiblePanel(panelToResize);

            if (belowPanel == null && abovePanel == null) return; //not possible to resize one panel
            if (newHeight <= PanelAllowedMinimumHeight) return; //new size too small

            double heightDiff = panelToResize.Height - newHeight;

            if (belowPanel == null) //the lowest one, usually the panel with volume
            {
                if (newHeight > 0)
                    panelToResize.Height = newHeight;
                else
                    panelToResize.Height = 0;

                if (panelToResize.Top + heightDiff < 0)
                    panelToResize.Top = 0;
                else
                    panelToResize.Top += heightDiff;

                if (abovePanel.Height + heightDiff < 0)
                    abovePanel.Height = 0;
                else
                    abovePanel.Height += heightDiff;

                return;
            }

            double belowPanelNewSize = belowPanel.Height + heightDiff;
            if (belowPanelNewSize < PanelAllowedMinimumHeight)
            {
                double availableMinHeight = belowPanel.Height - PanelAllowedMinimumHeight;
                heightDiff += availableMinHeight;
                newHeight -= availableMinHeight;
            }

            panelToResize.Height = newHeight;
            belowPanel.Top -= heightDiff;

            if (belowPanel.Height + heightDiff < 0)
                belowPanel.Height = 0;
            else
                belowPanel.Height += heightDiff;
        }
        #endregion

        #region Panels Moving
        private ChartPanel _panelToMove;
        private ChartPanel _panelToMoveOver;
        private ChartPanelMoveShadow _chartPanelMoveShadow;
        private ChartPanelMovePlaceholder _chartPanelMovePlaceholder;
        private double _oldY;
        /// <summary>
        /// A panel can be moved only along the panels with same style
        /// </summary>
        /// <returns></returns>
        private bool CanMovePanel(double X, double Y)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return false;

            if (_panelToMove != null)
                _panelToMove.Cursor = null;

            _panelToMove = null;
            if (VisiblePanelsCount > 1 && X < panelsHolder.ActualWidth - 100)
            {
                foreach (ChartPanel panel in paineis)
                {
                    if (!panel.Visible) continue;
                    if (!Utils.Between(Y, panel.Top, panel.Top + panel.TitleBarHeight)) continue;
                    _panelToMove = panel;
                    break;
                }
            }
            if (_panelToMove != null)
                _panelToMove.Cursor = Cursors.Hand;

            //Mouse.OverrideCursor = _panelToMove != null ? Cursors.Hand : null;
            return _panelToMove != null;
        }

        private void EnsureChartPanelMoveShadowVisible()
        {
            if (_chartPanelMoveShadow == null)
            {
                _chartPanelMoveShadow = new ChartPanelMoveShadow();
                panelsHolder.Children.Add(_chartPanelMoveShadow);
            }
            _chartPanelMoveShadow.Visible = true;
            _chartPanelMoveShadow.InitFromPanel(_panelToMove);
            panelsHolder.BringToFront(_chartPanelMoveShadow);
        }

        private void EnsureChartPanelMovePlaceholderVisible()
        {
            if (_chartPanelMovePlaceholder != null) return;
            _chartPanelMovePlaceholder = new ChartPanelMovePlaceholder();
            panelsHolder.Children.Add(_chartPanelMovePlaceholder);
        }

        private void StartMovingPanel(double Y)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return;

            _panelToMove.State = ChartPanel.StateType.Moving;
            panelsHolder.CaptureMouse();
            EnsureChartPanelMoveShadowVisible();
            EnsureChartPanelMovePlaceholderVisible();
            _chartPanelMovePlaceholder.Visible = false;
            _oldY = Y;
        }

        private void TryToMovePanel(double Y)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return;
            _chartPanelMovePlaceholder.Visible = false;
            _panelToMoveOver = null;
            ChartPanel chartPanelFromY = PanelByY(Y);
            _chartPanelMoveShadow.Top += (Y - _oldY);
            _oldY = Y;

            //find if we can move the panel
            //same panel
            if (!(_chartPanelMoveShadow.IsOkToMove = (chartPanelFromY != _panelToMove) && (chartPanelFromY != null))) return;

            //different panel, check position style
            if (!(_chartPanelMoveShadow.IsOkToMove = chartPanelFromY._position == _panelToMove._position)) return;

            _panelToMoveOver = chartPanelFromY;
            panelsHolder.BringToFront(_chartPanelMovePlaceholder);
            _chartPanelMovePlaceholder.Visible = true;
            _chartPanelMovePlaceholder.ShowOnPanel(_panelToMoveOver);
        }

        private void EndMovePanel()
        {
            _panelToMove.State = ChartPanel.StateType.Normal;
            _chartPanelMovePlaceholder.Visible = false;
            _chartPanelMoveShadow.Visible = false;

            if (_chartPanelMoveShadow.IsOkToMove)
            {
                paineis[_panelToMoveOver._index] = _panelToMove;
                paineis[_panelToMove._index] = _panelToMoveOver;
                int iIndex = 0;
                paineis.ForEach(p => p._index = iIndex++);
                ResizePanels(ResizeType.Reposition);
            }

            panelsHolder.ReleaseMouseCapture();
            _panelToMove = null;
            _panelToMoveOver = null;
        }

        private void CancelMovePanel()
        {
            _chartPanelMovePlaceholder.Visible = false;
            _panelToMove = null;
            panelsHolder.ReleaseMouseCapture();
            _chartPanelMoveShadow.Visible = false;
        }
        #endregion

        #region Mouse Event for PanelsHolder
        private void PanelsHolder_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return;

            Point p = e.GetPosition(panelsHolder);
            switch (estado)
            {
                case StateEnum.Normal:

                    if (CanResizePanel(p.X, p.Y))
                        break;
                    if (stockChart._ctrlDown && CanMovePanel(p.X, p.Y))
                        break;
                    break;
                case StateEnum.ResizingPanel:
                    DoResizePanel(p.Y);
                    break;
                case StateEnum.MovingPanel:
                    TryToMovePanel(p.Y);
                    break;
            }
        }
        private void PanelsHolder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return;

            switch (estado)
            {
                case StateEnum.Normal:
                    if (_panelToResize != null)
                    {
                        StartResizePanel(e.GetPosition(panelsHolder).Y);
                        estado = StateEnum.ResizingPanel;
                        break;
                    }
                    if (_panelToMove != null)
                    {
                        StartMovingPanel(e.GetPosition(panelsHolder).Y);
                        estado = StateEnum.MovingPanel;
                        break;
                    }
                    break;
            }
        }
        private void PanelsHolder_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (stockChart.Status != StockChartX.StatusGrafico.Preparado) return;

            switch (estado)
            {
                case StateEnum.ResizingPanel:
                    EndResizePanel(e.GetPosition(panelsHolder).Y);
                    estado = StateEnum.Normal;
                    break;
                case StateEnum.MovingPanel:
                    estado = StateEnum.Normal;
                    EndMovePanel();
                    break;
            }
        }

        private void PanelsHolder_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (estado)
            {
                case StateEnum.ResizingPanel:
                    if (e.Key == Key.Escape)
                    {
                        CancelResizePanel();
                        estado = StateEnum.Normal;
                    }
                    break;
                case StateEnum.MovingPanel:
                    if (e.Key == Key.Escape)
                    {
                        CancelMovePanel();
                        estado = StateEnum.Normal;
                    }
                    break;
            }
        }
        #endregion

        #region Move Series Indicator

        #region MostraMensagemMovimentacaoIndicador()
        /// <summary>
        /// Mostra mensagem de movimentacao de indicador.
        /// </summary>
        /// <param name="p">Point: localização.</param>
        /// <param name="moveStatusEnum">Enumerador que representa o tipo de movimentação.</param>
        internal void MostraMensagemMovimentacaoIndicador(Point p, MoveSeriesIndicator.MoveStatusEnum moveStatusEnum)
        {
            if (moveSeriesIndicator == null)
            {
                moveSeriesIndicator = new MoveSeriesIndicator();
                panelsHolder.Children.Add(moveSeriesIndicator);
                moveSeriesIndicator.ApplyTemplate();
                Canvas.SetZIndex(moveSeriesIndicator, ZIndexConstants.MoveSeriesIndicator);
            }
            if (moveSeriesIndicator.Visibility != Visibility.Visible)
                moveSeriesIndicator.Visibility = Visibility.Visible;

            if (p.X + moveSeriesIndicator.ActualWidth + 10 < panelsHolder.ActualWidth)
                moveSeriesIndicator.X = p.X + 10;
            else
                moveSeriesIndicator.X = p.X - moveSeriesIndicator.ActualWidth;

            if (p.Y + moveSeriesIndicator.ActualHeight + 5 < panelsHolder.ActualHeight)
                moveSeriesIndicator.Y = p.Y;
            else
                moveSeriesIndicator.Y = p.Y - moveSeriesIndicator.ActualHeight + 10;

            moveSeriesIndicator.MoveStatus = moveStatusEnum;
        }
        #endregion MostraMensagemMovimentacaoIndicador()

        #region EscondeMensagemMovimentacaoIndicador()
        /// <summary>
        /// Esconde a mensagem de movimentação do info painel.
        /// </summary>
        internal void EscondeMensagemMovimentacaoIndicador()
        {
            if (moveSeriesIndicator != null && moveSeriesIndicator.Visibility == Visibility.Visible)
                moveSeriesIndicator.Visibility = Visibility.Collapsed;
        }
        #endregion EscondeMensagemMovimentacaoIndicador()

        #endregion

        #region InfoPanel

        internal readonly EnumGeral.ObjetoSobCursor[] _goodParts = new[] { EnumGeral.ObjetoSobCursor.PanelLeftNonPaintableArea, EnumGeral.ObjetoSobCursor.PanelRightNonPaintableArea, EnumGeral.ObjetoSobCursor.PanelPaintableArea, };

        #region AssegurarInfoPanelCriado()
        /// <summary>
        /// 
        /// </summary>
        internal void AssegurarInfoPanelCriado()
        {
            if (infoPanel != null)
                return;

            infoPanel = new InfoPanel(stockChart) { PanelsContainer = this };
            panelsHolder.Children.Add(infoPanel);

            Canvas.SetZIndex(infoPanel, ZIndexConstants.InfoPanel);
            infoPanel.Visible = false;
        }
        #endregion AssegurarInfoPanelCriado()

        #region MostraInfoPanelInternal()
        /// <summary>
        /// Mostra o info panel.
        /// </summary>
        internal void MostraInfoPanelInternal(int record)
        {
            if (stockChart.InfoPanelPosicao == EnumGeral.InfoPanelPosicaoEnum.Escondido) 
                return;

            AssegurarInfoPanelCriado();

            Point pos = Mouse.GetPosition(this);

            bool positionChanged = (pos.X != infoPanel.X) || (pos.Y != infoPanel.Y);
            //if (pos.X == _infoPanel.X && pos.Y == _infoPanel.Y) return;

            object o;
            EnumGeral.ObjetoSobCursor objectFromCursor = stockChart.GetObjectFromCursor(out o);
            //ChartPanel chartPanel = new ChartPanel();
            //alterando se for RightYAxis
            //if (o != null)
            //{
            //    if ((o.GetType().ToString().Contains("YAxis")) || (o.GetType().ToString().Contains("PanelsBar")))
            //    {
            //        objectFromCursor = EnumGeral.ObjetoSobCursor.PanelRightNonPaintableArea;
            //        chartPanel = stockChart.GetPanelByIndex(0);
            //    }
            //    else
            //    {
            //        if (!_goodParts.Contains(objectFromCursor)) return;

            //        infoPanel.X = pos.X;
            //        infoPanel.Y = pos.Y;

            //        chartPanel = (ChartPanel)o;
            //    }
            //}
            //else
            //    return;

            infoPanel.Clear();
            infoPanel.AddInfoPanelItems(stockChart.calendario.InfoPanelItems);

            List<InfoPanelItem> lista = new List<InfoPanelItem>();
            List<string> captions = new List<string>();
            for (int i = 0; i <= stockChart.PanelsCount - 1; i++)
            {
                if (stockChart.GetPanelByIndex(i).Visible)
                {                    
                    foreach (InfoPanelItem obj in new ChartPanelInfoPanelAble { ChartPanel = stockChart.GetPanelByIndex(i) }.InfoPanelItems)
                    {
                        if (!captions.Contains(obj.Caption))
                        {
                            lista.Add(obj);
                            captions.Add(obj.Caption);
                        }
                    }
                                        
                }
            }
            infoPanel.AddInfoPanelItems(lista);
            //stockChart.PanelsCollection[0];
            //if (chartPanel.Index != infoPanel.PanelOwnerIndex)
            //{
            //    infoPanel.Clear();
            //    infoPanel.AddInfoPanelItems(stockChart.calendario.InfoPanelItems);
            //    infoPanel.AddInfoPanelItems(new ChartPanelInfoPanelAble { ChartPanel = chartPanel }.InfoPanelItems);

            //    infoPanel.PanelOwnerIndex = chartPanel.Index;
            //}

            if (!infoPanel.Visible)
                infoPanel.Visible = true;
            
            infoPanel.RecalculateLayout(record);

            if (positionChanged && stockChart.InfoPanelPosicao == EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse)
                stockChart.FireShowInfoPanel();

            if (stockChart.InfoPanelPosicao != EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse)
                return;

            pos.Offset(10, 5);
            infoPanel.Position = pos;
        }

        /// <summary>
        /// Mostra o info panel.
        /// </summary>
        internal void MostraInfoPanelInternal()
        {
            if (stockChart.InfoPanelPosicao == EnumGeral.InfoPanelPosicaoEnum.Escondido)
                return;

            AssegurarInfoPanelCriado();

            Point pos = Mouse.GetPosition(this);

            bool positionChanged = (pos.X != infoPanel.X) || (pos.Y != infoPanel.Y);
            //if (pos.X == _infoPanel.X && pos.Y == _infoPanel.Y) return;

            object o;
            EnumGeral.ObjetoSobCursor objectFromCursor = stockChart.GetObjectFromCursor(out o);
            if (!_goodParts.Contains(objectFromCursor)) return;

            infoPanel.X = pos.X;
            infoPanel.Y = pos.Y;

            ChartPanel chartPanel = (ChartPanel)o;

            if (chartPanel.Index != infoPanel.PanelOwnerIndex)
            {
                infoPanel.Clear();
                infoPanel.AddInfoPanelItems(stockChart.calendario.InfoPanelItems);
                infoPanel.AddInfoPanelItems(new ChartPanelInfoPanelAble { ChartPanel = chartPanel }.InfoPanelItems);

                infoPanel.PanelOwnerIndex = chartPanel.Index;
            }

            if (!infoPanel.Visible)
                infoPanel.Visible = true;

            infoPanel.RecalculateLayout();

            if (positionChanged && stockChart.InfoPanelPosicao == EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse)
                stockChart.FireShowInfoPanel();

            if (stockChart.InfoPanelPosicao != EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse)
                return;

            pos.Offset(10, 5);
            infoPanel.Position = pos;
        }
             
        #endregion MostraInfoPanelInternal()

        #region EnforceInfoPanelUpdate()
        /// <summary>
        /// Força info panel update.
        /// </summary>
        internal void EnforceInfoPanelUpdate()
        {
            if (infoPanel == null || stockChart.InfoPanelPosicao != EnumGeral.InfoPanelPosicaoEnum.Fixo) 
                return;

            infoPanel.X = -1;
            MostraInfoPanelInternal();
        }
        #endregion EnforceInfoPanelUpdate()

        #region ResetInfoPanelContent()
        /// <summary>
        /// Reseta a propriedade que guarda o proprietário do info panel.
        /// </summary>
        internal void ResetInfoPanelContent()
        {
            if (infoPanel != null)
                infoPanel.PanelOwnerIndex = -1;
        }
        #endregion ResetInfoPanelContent()

        #region EscondeInfoPanel()
        /// <summary>
        /// Esconde o info panel.
        /// </summary>
        internal void EscondeInfoPanel()
        {
            if (infoPanel != null)
                infoPanel.Visible = false;
        }
        #endregion EscondeInfoPanel()

        #region TornaInfoPanelEstatico()
        /// <summary>
        /// Faz com que o info panel fique estatico.
        /// </summary>
        internal void TornaInfoPanelEstatico()
        {
            AssegurarInfoPanelCriado();

            Point? p = infoPanel._position ?? new Point(10, Constants.PanelTitleBarHeight + 10);

            infoPanel.Position = p.Value;
        }
        #endregion TornaInfoPanelEstatico()

        #endregion InfoPanel

        #region Cross Hairs

        #region ShowCrossHairs()
        /// <summary>
        /// Mostra o Cross Hairs.
        /// </summary>
        internal void ShowCrossHairs()
        {
            if (!stockChart.CrossHairs)
                return;

            if (verticalCrossHair == null)
            {
                verticalCrossHair = new Line { Stroke = stockChart.CorCrossHairs, StrokeThickness = 1, IsHitTestVisible = false };
                panelsHolder.Children.Add(verticalCrossHair);

                horizontalCrossHair = new Line { Stroke = stockChart.CorCrossHairs, StrokeThickness = 1, IsHitTestVisible = false };
                panelsHolder.Children.Add(horizontalCrossHair);
            }

            if (verticalCrossHair.Visibility != Visibility.Visible)
            {
                verticalCrossHair.Visibility = horizontalCrossHair.Visibility = Visibility.Visible;
                Canvas.SetZIndex(verticalCrossHair, ZIndexConstants.CrossHairs);
                Canvas.SetZIndex(horizontalCrossHair, ZIndexConstants.CrossHairs);
            }

            Point pos = Mouse.GetPosition(this);

            verticalCrossHair.X1 = verticalCrossHair.X2 = (int)pos.X;
            verticalCrossHair.Y1 = 0;
            verticalCrossHair.Y2 = (int)ActualHeight;

            horizontalCrossHair.Y1 = horizontalCrossHair.Y2 = (int)pos.Y;
            horizontalCrossHair.X1 = 0;
            horizontalCrossHair.X2 = (int)ActualWidth;
        }

        internal void ShowCrossHairs(double record, double y)
        {
            if (!stockChart.CrossHairs)
                return;

            if (verticalCrossHair == null)
            {
                verticalCrossHair = new Line { Stroke = stockChart.CorCrossHairs, StrokeThickness = 1, IsHitTestVisible = false };
                panelsHolder.Children.Add(verticalCrossHair);

                horizontalCrossHair = new Line { Stroke = stockChart.CorCrossHairs, StrokeThickness = 1, IsHitTestVisible = false };
                panelsHolder.Children.Add(horizontalCrossHair);
            }

            if (verticalCrossHair.Visibility != Visibility.Visible)
            {
                verticalCrossHair.Visibility = horizontalCrossHair.Visibility = Visibility.Visible;
                Canvas.SetZIndex(verticalCrossHair, ZIndexConstants.CrossHairs);
                Canvas.SetZIndex(horizontalCrossHair, ZIndexConstants.CrossHairs);
            }

            Point pos = Mouse.GetPosition(this);

            verticalCrossHair.X1 = verticalCrossHair.X2 = record;
            verticalCrossHair.Y1 = 0;
            verticalCrossHair.Y2 = (int)ActualHeight;

            horizontalCrossHair.Y1 = horizontalCrossHair.Y2 = y;
            horizontalCrossHair.X1 = 0;
            horizontalCrossHair.X2 = (int)ActualWidth;
        }

        #endregion ShowCrossHairs()

        #region EscondeCrossHairs()
        /// <summary>
        /// Esconde o cross hair se estiver visível.
        /// </summary>
        internal void EscondeCrossHairs()
        {
            if (verticalCrossHair == null)
                return;

            verticalCrossHair.Visibility = horizontalCrossHair.Visibility = Visibility.Collapsed;
        }
        #endregion EscondeCrossHairs()

        #region AtualizaCorCrossHairs()
        /// <summary>
        /// Atualiza cores do Cross Hairs com os valores contidos nas propriedades CrossHairsStroke e CrossHairsStroke.
        /// </summary>
        internal void AtualizaCorCrossHairs()
        {
            if (verticalCrossHair != null)
                verticalCrossHair.Stroke = stockChart.CorCrossHairs;

            if (horizontalCrossHair != null)
                horizontalCrossHair.Stroke = stockChart.CorCrossHairs;
        }
        #endregion AtualizaCorCrossHairs()

        #endregion Cross Hairs

     
    }
}
