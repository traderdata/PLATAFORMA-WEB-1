using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public partial class StockChartX
    {
        /// <summary>
        /// Occurs when indicator's dialog with parameters is shown
        /// </summary>
        public delegate void OnIndicadorErrorDelegate(string erro, Indicators.Indicator.ErroIndicador tipoErro);
        public event OnIndicadorErrorDelegate OnIndicadorError;
        internal void DisparaErroIndicador(string erro, Indicators.Indicator.ErroIndicador tipoErro)
        {
            if (OnIndicadorError != null)
                OnIndicadorError(erro, tipoErro);
        }

        /// <summary>
        /// Occurs when indicator's dialog with parameters is shown
        /// </summary>
        public event EventHandler DialogShown;
        internal void FireOnDialogShown()
        {
            if (DialogShown != null)
                DialogShown(this, EventArgs.Empty);
        }

        /// <summary>
        /// Provides data for the <see cref="DeleteSeries"/> event.
        /// </summary>
        public class DeleteSeriesEventArgs : EventArgs
        {
            /// <summary>
            /// Name of the series being removed
            /// </summary>
            public string RemovedSeries { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteSeriesEventArgs"/> class.
            /// </summary>
            /// <param name="seriesName">Name of the series being removed</param>
            public DeleteSeriesEventArgs(string seriesName)
            {
                RemovedSeries = seriesName;
            }
        }

        /// <summary>
        /// Occurs when a series or indicator is removed from chart. No user interaction is possibly here
        /// </summary>
        public event EventHandler<DeleteSeriesEventArgs> DeleteSeries;
        internal void FireSeriesRemoved(string seriesName)
        {
            if (DeleteSeries != null)
                DeleteSeries(this, new DeleteSeriesEventArgs(seriesName));
        }

        /// <summary>
        /// Penetration type of a linestudy
        /// </summary>
        public enum TrendLinePenetrationEnum
        {
            /// <summary>
            /// penetration was above LineStudy
            /// </summary>
            Above,
            /// <summary>
            /// penetration was below LineStudy
            /// </summary>
            Below
        }
        /// <summary>
        /// Provides data for the <see cref="TrendLinePenetration"/> event.
        /// </summary>
        public class TrendLinePenetrationArgs : EventArgs
        {
            /// <summary>
            /// reference to the LineStudy that was penetrated
            /// </summary>
            public TrendLine TrendLine { get; private set; }
            /// <summary>
            /// penetration type
            /// </summary>
            public TrendLinePenetrationEnum TrendLinePenetrationType { get; private set; }
            /// <summary>
            /// reference to the Series that penetrated the LineStudy
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="TrendLinePenetrationArgs"/> class.
            /// </summary>
            /// <param name="trendLine">Reference to trendline being penetrated</param>
            /// <param name="trendLinePenetrationEnum">Penetration type</param>
            /// <param name="series">Reference to the Series that penetrated the trendline</param>
            public TrendLinePenetrationArgs(TrendLine trendLine,
              TrendLinePenetrationEnum trendLinePenetrationEnum,
              Series series)
            {
                TrendLine = trendLine;
                TrendLinePenetrationType = trendLinePenetrationEnum;
                Series = series;
            }
        }
        /// <summary>
        /// Occurs when a value from a series crosses a watchable trendline
        /// </summary>
        public event EventHandler<TrendLinePenetrationArgs> TrendLinePenetration;
        internal void FireTrendLinePenetration(TrendLine trendLine,
          TrendLinePenetrationEnum trendLinePenetrationEnum,
          Series series)
        {
            if (TrendLinePenetration != null)
                TrendLinePenetration(this,
                                     new TrendLinePenetrationArgs(trendLine, trendLinePenetrationEnum, series));
        }

        /// <summary>
        /// Occurs when entire chart was recreated
        /// </summary>
        public event EventHandler ChartReseted;
        internal void FireChartReseted()
        {
            if (ChartReseted != null)
                ChartReseted(this, EventArgs.Empty);
        }

        ///<summary>
        /// Provides data for the <see cref="SeriesRightClick"/> event.
        ///</summary>
        public class SeriesRightClickEventArgs : EventArgs
        {
            /// <summary>
            /// reference to series being clicked
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// mouse position relative to ChartPanel where the Series is located
            /// </summary>
            public Point Position { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesRightClickEventArgs"/> class.
            /// </summary>
            /// <param name="series">Reference to series</param>
            /// <param name="position">Mouse position</param>
            public SeriesRightClickEventArgs(Series series, Point position)
            {
                Series = series;
                Position = position;
            }
        }

        /// <summary>
        /// Occurs when a right click occurs on a series
        /// </summary>
        public event EventHandler<SeriesRightClickEventArgs> SeriesRightClick;
        internal void FireSeriesRightClick(Series series, Point p)
        {
            if (SeriesRightClick != null)
                SeriesRightClick(this, new SeriesRightClickEventArgs(series, p));
        }

        /// <summary>
        /// Provides data for the <see cref="LineStudyRightClick"/> event.
        /// </summary>
        public class LineStudiesRightClickEventArgs : EventArgs
        {
            /// <summary>
            /// reference to a line study being clicked
            /// </summary>
            public LineStudy LineStudy { get; private set; }
            /// <summary>
            /// position of the mouse relative the ChartPanel where the LineStudy is located
            /// </summary>                            
            public Point Position { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudiesRightClickEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy">Reference to line study</param>
            /// <param name="position">Mouse position</param>
            public LineStudiesRightClickEventArgs(LineStudy lineStudy, Point position)
            {
                LineStudy = lineStudy;
                Position = position;
            }
        }
        /// <summary>
        /// Occurs when a LineStudy was right-clicked
        /// </summary>
        public event EventHandler<LineStudiesRightClickEventArgs> LineStudyRightClick;
        internal void FireLineStudyRightClick(LineStudy lineStudy, Point position)
        {
            if (LineStudyRightClick != null)
                LineStudyRightClick(this, new LineStudiesRightClickEventArgs(lineStudy, position));
        }

        /// <summary>
        /// Provides data for the <see cref="LineStudyDoubleClick"/> event.
        /// </summary>
        public class LineStudyMouseEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the linestudies
            /// </summary>
            public LineStudy LineStudy { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudyMouseEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy">Reference to line study</param>
            public LineStudyMouseEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
            }
        }
        /// <summary>
        /// Occurs when a LineStudy is double clicked
        /// </summary>
        public event EventHandler<LineStudyMouseEventArgs> LineStudyDoubleClick;
        internal void FireLineStudyDoubleClick(LineStudy lineStudy)
        {
            if (LineStudyDoubleClick != null)
                LineStudyDoubleClick(this, new LineStudyMouseEventArgs(lineStudy));
        }

        /// <summary>
        /// Occurs when a mouse click a line study
        /// </summary>
        public event EventHandler<LineStudyMouseEventArgs> LineStudyLeftClick;
        internal void FireLineStudyLeftClick(LineStudy lineStudy)
        {
            if (LineStudyLeftClick != null)
                LineStudyLeftClick(this, new LineStudyMouseEventArgs(lineStudy));
        }

        /// <summary>
        /// Provides data for the <see cref="ChartPanelMouseMoveArgs"/> event.
        /// </summary>
        public class ChartPanelMouseMoveArgs : EventArgs
        {
            /// <summary>
            /// Gets the panel index where mouse is moving. 0-based
            /// </summary>
            public int PanelIndex { get; private set; }
            /// <summary>
            /// Gets the mouse Y coordinate relative to the panel
            /// </summary>
            public double MouseY { get; private set; }
            /// <summary>
            /// Gets the mouse X coordinate relative to the panel
            /// </summary>
            public double MouseX { get; private set; }
            /// <summary>
            /// Gets the Y value from current mouse position
            /// </summary>
            public double Y { get; private set; }
            /// <summary>
            /// Gets the record number from current mouse position
            /// </summary>
            public int Record { get; private set; }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="panelIndex"></param>
            /// <param name="mouseY"></param>
            /// <param name="mouseX"></param>
            /// <param name="y"></param>
            /// <param name="record"></param>
            public ChartPanelMouseMoveArgs(int panelIndex, double mouseY, double mouseX, double y, int record)
            {
                PanelIndex = panelIndex;
                MouseY = mouseY;
                MouseX = mouseX;
                Y = y;
                Record = record;
            }
        }
        /// <summary>
        /// Occurs when mouse is moving above a <see cref="ChartPanel"/>
        /// </summary>
        public event EventHandler<ChartPanelMouseMoveArgs> ChartPanelMouseMove;

        internal void InvokeChartPanelMouseMove(int panelIndex, double mouseY, double mouseX, double y, int record)
        {
            EventHandler<ChartPanelMouseMoveArgs> handler = ChartPanelMouseMove;
            if (handler != null) handler(this, new ChartPanelMouseMoveArgs(panelIndex, mouseY, mouseX, y, record));
        }

        /// <summary>
        /// Provides data for the <see cref="LineStudyBeforeDelete"/> event.
        /// </summary>
        public class LineStudyBeforeDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// reference to LineStudy that is going to be deleted
            /// </summary>
            public LineStudy LineStudy;
            /// <summary>
            /// set to [true] to cancel LineStudies deleting
            /// </summary>
            public bool CancelDelete;
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudyBeforeDeleteEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy">Reference to a line study</param>
            public LineStudyBeforeDeleteEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
                CancelDelete = false;
            }
        }
        /// <summary>
        /// Occurs before a LineStudy is deleted. Here user may cancel LineStudy deletition
        /// </summary>
        public event EventHandler<LineStudyBeforeDeleteEventArgs> LineStudyBeforeDelete;
        internal bool FireLineStudyBeforeDelete(LineStudy lineStudy)
        {
            LineStudyBeforeDeleteEventArgs eventArgs = new LineStudyBeforeDeleteEventArgs(lineStudy);
            if (LineStudyBeforeDelete != null)
                LineStudyBeforeDelete(this, eventArgs);
            return eventArgs.CancelDelete;
        }

        /// <summary>
        /// Provides data for the <see cref="IndicatorDoubleClick"/> event.
        /// </summary>
        public class IndicatorDoubleClickEventArgs : EventArgs
        {
            /// <summary>
            /// a reference to the indicator being clicked
            /// </summary>
            public Indicators.Indicator Indicator { get; private set; }
            /// <summary>
            /// if set to true the double click won't show the indicator properties dialog
            /// </summary>
            public bool CancelPropertiesDialog { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="IndicatorDoubleClickEventArgs"/> class.
            /// </summary>
            /// <param name="indicator">Reference to the indicator being double-clicked</param>
            public IndicatorDoubleClickEventArgs(Indicators.Indicator indicator)
            {
                Indicator = indicator;
                CancelPropertiesDialog = false;
            }
        }

        /// <summary>
        /// Provides data for the <see cref="IndicatorLeftClick"/> event.
        /// </summary>
        public class IndicatorLeftClickEventArgs : EventArgs
        {
            /// <summary>
            /// a reference to the indicator being clicked
            /// </summary>
            public Indicators.Indicator Indicator { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="IndicatorLeftClickEventArgs"/> class.
            /// </summary>
            /// <param name="indicator">Reference to the indicator being left click</param>
            public IndicatorLeftClickEventArgs(Indicators.Indicator indicator)
            {
                Indicator = indicator;
            }
        }

        /// <summary>
        /// Occusr when a series-indicator is double clicked. Here user can cancel indicator's properties dialog.
        /// </summary>
        public event EventHandler<IndicatorDoubleClickEventArgs> IndicatorDoubleClick;
        internal bool FireIndicatorDoubleClick(Indicators.Indicator indicator)
        {
            IndicatorDoubleClickEventArgs args = new IndicatorDoubleClickEventArgs(indicator);
            if (IndicatorDoubleClick != null)
            {
                IndicatorDoubleClick(this, args);
            }
            return args.CancelPropertiesDialog;
        }

        /// <summary>
        /// Occusr when a series-indicator is left clicked.
        /// </summary>
        public event EventHandler<IndicatorDoubleClickEventArgs> IndicatorLeftClick;
        internal bool FireIndicatorLeftClick(Indicators.Indicator indicator)
        {
            IndicatorDoubleClickEventArgs args = new IndicatorDoubleClickEventArgs(indicator);
            if (IndicatorLeftClick != null)
            {
                IndicatorLeftClick(this, args);
            }
            return args.CancelPropertiesDialog;
        }

        /// <summary>
        /// Provides data for the <see cref="SeriesDoubleClick"/> event.
        /// </summary>
        public class SeriesDoubleClickEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the series being double-clicked
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesDoubleClickEventArgs"/> class.
            /// </summary>
            /// <param name="series">Reference to series</param>
            public SeriesDoubleClickEventArgs(Series series)
            {
                Series = series;
            }
        }
        /// <summary>
        /// Occurs when a Series is double clicked
        /// </summary>
        public event EventHandler<SeriesDoubleClickEventArgs> SeriesDoubleClick;
        internal void FireSeriesDoubleClick(Series series)
        {
            if (SeriesDoubleClick != null)
                SeriesDoubleClick(this, new SeriesDoubleClickEventArgs(series));
        }


        /// <summary>
        /// Provides data for the <see cref="SeriesBeforeDelete"/> event.
        /// </summary>
        public class SeriesBeforeDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the series that is going to be deleted
            /// </summary>
            public Series Series;
            /// <summary>
            /// set to [true] to cancel series deleting
            /// </summary>
            public bool CancelDelete;

            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesBeforeDeleteEventArgs"/> class.
            /// </summary>
            /// <param name="series">Reference to series</param>
            public SeriesBeforeDeleteEventArgs(Series series)
            {
                Series = series;
                CancelDelete = false;
            }
        }
        /// <summary>
        /// Occurs before a Series is deleted. Here user can cancel Series deletition
        /// </summary>
        public event EventHandler<SeriesBeforeDeleteEventArgs> SeriesBeforeDelete;
        internal bool FireIndicatorBeforeDelete(Series series)
        {
            if (SeriesBeforeDelete != null)
            {
                SeriesBeforeDeleteEventArgs args = new SeriesBeforeDeleteEventArgs(series);
                SeriesBeforeDelete(this, args);
                return args.CancelDelete;
            }
            return false;
        }

        /// <summary>
        /// Provides data for the <see cref="SeriesMoved"/> event.
        /// </summary>
        public class SeriesMovedEventArgs : EventArgs
        {
            /// <summary>
            /// a reference to a series that was moved
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// panel index where series was located. -1 if panel will be deleted after series move
            /// </summary>
            public int ChartPanelFrom { get; private set; }
            /// <summary>
            /// panel index where series is being moved. 
            /// </summary>
            public int ChartPanelTo { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesMovedEventArgs"/> class.
            /// </summary>
            /// <param name="series">Referemce to series</param>
            /// <param name="chartPanelFrom">Index of panel from where series is moved</param>
            /// <param name="chartPanelTo">Index of panel where series is moved.</param>
            public SeriesMovedEventArgs(Series series, int chartPanelFrom, int chartPanelTo)
            {
                Series = series;
                ChartPanelFrom = chartPanelFrom;
                ChartPanelTo = chartPanelTo;
            }
        }
        /// <summary>
        /// Occurs whenever the user has moved a series or indicator from one panel to another panel. 
        /// </summary>
        public event EventHandler<SeriesMovedEventArgs> SeriesMoved;
        internal void FireSeriesMoved(Series series, int chartPanelFrom, int chartPanelTo)
        {
            if (SeriesMoved != null)
                SeriesMoved(this, new SeriesMovedEventArgs(series, chartPanelFrom, chartPanelTo));
        }

        /// <summary>
        /// Provides data for the <see cref="ChartPanelBeforeClose"/> event.
        /// </summary>
        public class ChartPanelBeforeCloseEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the panel that is going to be closed
            /// </summary>
            public ChartPanel ChartPanel { get; private set; }
            /// <summary>
            /// set to [true] to cancel panel closing
            /// </summary>
            public bool CancelClose;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPanelBeforeCloseEventArgs"/> class.
            /// </summary>
            /// <param name="chartPanel">Reference to chart panel</param>
            public ChartPanelBeforeCloseEventArgs(ChartPanel chartPanel)
            {
                ChartPanel = chartPanel;
                CancelClose = false;
            }
        }
        /// <summary>
        /// Occurs before a panel is closed. Here user can cancel panel closing.
        /// </summary>
        public event EventHandler<ChartPanelBeforeCloseEventArgs> ChartPanelBeforeClose;
        internal bool FireChartPanelBeforeClose(ChartPanel chartPanel)
        {
            ChartPanelBeforeCloseEventArgs args = new ChartPanelBeforeCloseEventArgs(chartPanel);
            if (ChartPanelBeforeClose != null)
            {
                ChartPanelBeforeClose(this, args);
                return args.CancelClose;
            }
            return args.CancelClose;
        }

        /// <summary>
        ///  Provides data for the <see cref="CustomIndicatorNeedsData"/> event.
        /// </summary>
        public class CustomIndicatorNeedsDataEventArgs : EventArgs
        {
            /// <summary>
            /// A reference to the custom indicator that needs data
            /// </summary>
            public Indicators.CustomIndicator Indicator { get; private set; }
            /// <summary>
            /// A reference to an array of values that must be filled by user.
            /// The length of this array can't be bigger than RecordCount, all those values will be ignored 
            /// When the event is fired this array will have the existing values, that can be overwritten
            /// </summary>
            public double?[] Values { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomIndicatorNeedsDataEventArgs"/> class.
            /// </summary>
            /// <param name="indicator">Reference to the custom indicaror that needs data.</param>
            /// <param name="values">Values passed to user</param>
            public CustomIndicatorNeedsDataEventArgs(Indicators.CustomIndicator indicator, double?[] values)
            {
                Indicator = indicator;
                Values = values;
            }
        }

        /// <summary>
        /// Occurs whenever StockChartX updates with a new tick, new bar, or anything changes and the indicator needs to be re-calculated, you will be informed of this via the CustomIndicatorNeedData event. 
        /// </summary>
        public event EventHandler<CustomIndicatorNeedsDataEventArgs> CustomIndicatorNeedsData;

        internal bool CustomIndicatorNeedsDataIsHooked()
        {
            return CustomIndicatorNeedsData != null;
        }

        internal void FireCustomIndicatorNeedsData(CustomIndicatorNeedsDataEventArgs args)
        {
            if (CustomIndicatorNeedsData != null)
                CustomIndicatorNeedsData(this, args);
        }

        /// <summary>
        /// Provides data for the <see cref="ChartPanelPaint"/> event.
        /// </summary>
        public class ChartPanelPaintEventArgs : EventArgs
        {
            /// <summary>
            /// Reference to chart that was painted
            /// </summary>
            public ChartPanel ChartPanel { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPanelPaintEventArgs"/> class.
            /// </summary>
            /// <param name="chartPanel">Reference to chart panel</param>
            public ChartPanelPaintEventArgs(ChartPanel chartPanel)
            {
                ChartPanel = chartPanel;
            }
        }

        /// <summary>
        /// Occurs each time a panel is repainted.
        /// </summary>
        public event EventHandler<ChartPanelPaintEventArgs> ChartPanelPaint;
        internal void FireChartPanelPaint(ChartPanel chartPanel)
        {
            if (ChartPanelPaint != null)
                ChartPanelPaint(this, new ChartPanelPaintEventArgs(chartPanel));
        }

        /// <summary>
        /// Occurs when chart is scrolled with the mouse wheel or programmatically.
        /// </summary>
        public event EventHandler ChartScroll;
        internal void FireChartScroll()
        {
            if (ChartScroll != null)
                ChartScroll(this, EventArgs.Empty);
        }

        /// <summary>
        /// Provides data for the <see cref="UserDrawingComplete"/> event.
        /// </summary>
        public class UserDrawingCompleteEventArgs : EventArgs
        {
            /// <summary>
            /// StudyType of the line study being drawn
            /// </summary>
            public LineStudy.StudyTypeEnum StudyType { get; private set; }
            /// <summary>
            /// The unique key of line study
            /// </summary>
            public string Key { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UserDrawingCompleteEventArgs"/> class.
            /// </summary>
            /// <param name="studyTypeEnum">Study type that was painted</param>
            /// <param name="key">Unique associated with line study</param>
            public UserDrawingCompleteEventArgs(LineStudy.StudyTypeEnum studyTypeEnum, string key)
            {
                StudyType = studyTypeEnum;
                Key = key;
            }
        }
        /// <summary>
        /// Occurs after the user has completed drawing a line study or trend line.
        /// </summary>
        public event EventHandler<UserDrawingCompleteEventArgs> UserDrawingComplete;
        internal void FireUserDrawingComplete(LineStudy.StudyTypeEnum studyTypeEnum, string key)
        {
            if (UserDrawingComplete != null)
                UserDrawingComplete(this, new UserDrawingCompleteEventArgs(studyTypeEnum, key));
        }

        /// <summary>
        /// Provides data for the <see cref="ShowInfoPanel"/> event.
        /// </summary>
        public class ShowInfoPanelEventArgs : EventArgs
        {
            /// <summary>
            /// Has entries shown on info panel
            /// </summary>
            public List<Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>> Entries { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShowInfoPanelEventArgs"/> class.
            /// </summary>
            /// <param name="entries">Reference to a series that are present in info panel.</param>
            public ShowInfoPanelEventArgs(List<Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>> entries)
            {
                Entries = entries;
            }
        }
        /// <summary>
        /// Occurs when the info panel is displayed.
        /// </summary>
        public event EventHandler<ShowInfoPanelEventArgs> ShowInfoPanel;
        internal void FireShowInfoPanel()
        {
            if (ShowInfoPanel != null)
                ShowInfoPanel(this, new ShowInfoPanelEventArgs(paineisContainers.infoPanel.Entries));
        }

        /// <summary>
        /// Occurs anytime the chart is zoomed
        /// </summary>
        public event EventHandler ChartZoom;
        internal void FireZoom()
        {
            if (ChartZoom != null)
                ChartZoom(this, EventArgs.Empty);
        }

        /// <summary>
        /// internal usage
        /// </summary>
        public event EventHandler ChartLoaded = delegate { };

        /// <summary>
        /// Provides data for <see cref="IndicatorAddComplete"/> event.
        /// </summary>
        public class IndicatorAddCompletedEventArgs : EventArgs
        {
            /// <summary>
            /// Panel index where indicator was/is required to be
            /// </summary>
            public int PanelIndex { get; private set; }
            /// <summary>
            /// Indicator name
            /// </summary>
            public string IndicatorName { get; private set; }
            /// <summary>
            /// Gets the value indicating if action was canceled by user or not. 
            /// </summary>
            public bool CanceledByUser { get; private set; }
            ///<summary>
            /// Initializes a new instance of the <see cref="IndicatorAddCompletedEventArgs"/> class.
            ///</summary>
            ///<param name="panelIndex">Panel index</param>
            ///<param name="indicatorName">Indicator name</param>
            ///<param name="canceledByUser">Was operation canceled by user or not</param>
            public IndicatorAddCompletedEventArgs(int panelIndex, string indicatorName, bool canceledByUser)
            {
                PanelIndex = panelIndex;
                IndicatorName = indicatorName;
                CanceledByUser = canceledByUser;
            }
        }

        /// <summary>
        /// Occurs after an attempt to add an indicator.
        /// </summary>
        public event EventHandler<IndicatorAddCompletedEventArgs> IndicatorAddComplete = delegate { };

        internal void FireIndicatorAddCompleted(int panelIndex, string indicatorName, bool userCanceled)
        {
            IndicatorAddComplete(this, new IndicatorAddCompletedEventArgs(panelIndex, indicatorName, userCanceled));
        }

        internal delegate void OnCandleCustomBrushHandler(int barIndex, Brush newBrush);

        internal event OnCandleCustomBrushHandler OnCandleCustomBrush;

        /// <summary>
        /// Provides data for <see cref="StockChartX.LineStudyContextMenu"/> event.
        /// </summary>
        public class LineStudyContextMenuEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the reference who needs a context menu
            /// </summary>
            public LineStudy LineStudy { get; private set; }

            /// <summary>
            /// Gets or sets whether built-in context menu will be shown
            /// </summary>
            public bool Cancel { get; set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudyContextMenuEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy"></param>
            public LineStudyContextMenuEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
            }
        }

        /// <summary>
        /// Occurs when user clicks a <see cref="LineStudy"/> context-menu line
        /// </summary>
        public event EventHandler<LineStudyContextMenuEventArgs> LineStudyContextMenu;

        internal bool InvokeLineStudyContextMenu(LineStudy lineStudy)
        {
            EventHandler<LineStudyContextMenuEventArgs> menu = LineStudyContextMenu;
            var args = new LineStudyContextMenuEventArgs(lineStudy) { Cancel = false };
            if (menu != null) menu(this, args);

            return args.Cancel;
        }
    }
}

