using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SilverlightContrib.Xaml;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public partial class StockChartX
    {
        internal const int Version = 100;

        /// <summary>
        /// Serialization type
        /// </summary>
        public enum SerializationTypeEnum
        {
            /// <summary>
            /// Entire chart is saved
            /// </summary>
            All,
            /// <summary>
            /// Chart's styles and indicators are saved
            /// </summary>
            General,
            /// <summary>
            /// All lines studies are saved
            /// </summary>
            Objects
        }

        /// <summary>
        /// Save's the chart into a binary data
        /// </summary>
        /// <returns>true - if file was succesfully saved. false  - otherwise</returns>
        public byte[] SaveAsImage(UIElement elemento1, UIElement elemento2)
        {
            return GetBytesComplete(elemento1, elemento2, ImageExportType.Png);
        }

        /// <summary>
        /// Save's the chart into a binary data
        /// </summary>
        /// <returns>true - if file was succesfully saved. false  - otherwise</returns>
        public byte[] Save()
        {
            return SaveFile(SerializationTypeEnum.All);
        }

        ///<summary>
        /// Save's the chart into a binary data
        ///</summary>
        ///<param name="serializationType">Type of serialization to use.</param>
        /// <returns>true - if file was succesfully saved. false  - otherwise</returns>
        public byte[] SaveFile(SerializationTypeEnum serializationType)
        {
            try
            {
                XElement xRoot;
                XDocument xDocument = new XDocument(xRoot = new XElement("StockChartX", new XAttribute("SaveType", serializationType)));

                ConvertToXmlEx("Version", xRoot, Version);

                switch (serializationType)
                {
                    case SerializationTypeEnum.All:
                        SaveWorkSpace(xRoot);
                        SaveChartPanels(xRoot);
                        SaveSeries(xRoot);
                        SaveSeriesData(xRoot);
                        SaveIndicators(xRoot);
                        SaveLineStudies(xRoot);
                        break;
                    case SerializationTypeEnum.General:
                        SaveWorkSpace(xRoot);
                        SaveIndicators(xRoot);
                        break;
                    case SerializationTypeEnum.Objects:
                        SaveLineStudies(xRoot);
                        break;
                }


                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipOutputStream zipStream = new ZipOutputStream(ms))
                    {
                        zipStream.SetLevel(9);
                        ZipEntry entry = new ZipEntry("root");
                        zipStream.PutNextEntry(entry);

                        MemoryStream xStream = new MemoryStream();
                        xDocument.Save(xStream);
                        xStream.Seek(0, SeekOrigin.Begin);
                        byte[] buffer = new byte[4096];
                        StreamUtils.Copy(xStream, zipStream, buffer);
                    }
                    return ms.GetBuffer();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Deserialize a chart from binary data
        /// </summary>
        /// <param name="data"></param>
        /// <returns>
        /// 0 - no errors
        /// -1 - root element is null
        /// -2 - wrong serialization type
        /// -3 - un-suported serialization type
        /// -4 - un-suported version of file
        /// -5 - unknown exception
        /// </returns>
        public int LoadFile(byte[] data)
        {
            return LoadFile(data, SerializationTypeEnum.All);
        }

        /// <summary>
        /// Deserialize a chart from binary data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serializationType">Serialization type</param>
        /// <returns>
        /// 0 - no errors
        /// -1 - root element is null
        /// -2 - wrong serialization type
        /// -3 - un-suported serialization type
        /// -4 - un-suported version of file
        /// -5 - unknown exception
        /// </returns>
        public int LoadFile(byte[] data, SerializationTypeEnum serializationType)
        {
            try
            {
                Status = StatusGrafico.Construindo;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (ZipInputStream zipStream = new ZipInputStream(ms))
                    {
                        ZipEntry entry = zipStream.GetNextEntry();
                        if (entry == null)
                            return -1;

                        using (MemoryStream xStream = new MemoryStream())
                        {
                            int size = 4096;
                            byte[] buffer = new byte[size];
                            while ((size = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                xStream.Write(buffer, 0, size);
                            }
                            xStream.Seek(0, SeekOrigin.Begin);
                            XDocument xDocument = XDocument.Load(xStream);
                            XElement xRoot = xDocument.Root;
                            if (xRoot == null)
                                return -1;
                            int version = ConvertFromXmlEx("Version", xRoot, 0);
                            SerializationTypeEnum serializationTypeEnum = (SerializationTypeEnum)System.Enum.Parse(typeof(SerializationTypeEnum),
                              xRoot.Attributes("SaveType").First().Value, true);
                            if (serializationTypeEnum != serializationType) return -2;

                            if (version == 100)
                            {
                                switch (serializationType)
                                {
                                    case SerializationTypeEnum.All:
                                        ReadWorkSpace(xRoot);
                                        ReadChartPanels(xRoot);
                                        ReadSeries(xRoot);
                                        ReadSeriesData(xRoot);
                                        ReadIndicators(xRoot);
                                        ReadLineStudies(xRoot);
                                        break;
                                    case SerializationTypeEnum.General:
                                        ReadWorkSpace(xRoot);
                                        ReadIndicators(xRoot);
                                        break;
                                    case SerializationTypeEnum.Objects:
                                        ReadLineStudies(xRoot);
                                        break;
                                    default:
                                        return -3;
                                }
                            }
                            else
                            {
                                return -4;
                            }
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not desirialize. Error: " + ex.Message);
                return -5;
            }
            finally
            {
                Status = StatusGrafico.Preparado;
            }
        }

        private void SaveWorkSpace(XContainer xRoot)
        {
            XElement xmlWorkspace = new XElement("WorkSpace");
            xRoot.Add(xmlWorkspace);

            ConvertToXmlEx("ShowAnimations", xmlWorkspace, MostrarAnimacoes);
            ConvertToXmlEx("YAxes", xmlWorkspace, EscalaAlinhamento);
            ConvertToXmlEx("ScalingType", xmlWorkspace, EscalaTipo);
            ConvertToXmlEx("ShowYGrid", xmlWorkspace, GradeY);
            ConvertToXmlEx("ShowXGrid", xmlWorkspace, GradeX);
            ConvertToXml("GridStroke", xmlWorkspace, GradeCor);
            ConvertToXmlEx("ThreeDStyle", xmlWorkspace, Estilo3D);
            ConvertToXmlEx("UpColor", xmlWorkspace, CandleCorAlta);
            ConvertToXmlEx("DownColor", xmlWorkspace, CandleCorBaixa);
            ConvertToXmlEx("CrossHairs", xmlWorkspace, CrossHairs);
            ConvertToXmlEx("LeftChartSpace", xmlWorkspace, EspacoEsquerdaGrafico);
            ConvertToXmlEx("RightChartSpace", xmlWorkspace, EspacoDireitaGrafico);
            ConvertToXmlEx("RealTimeData", xmlWorkspace, LabelsEixoXRealTime);
            ConvertToXmlEx("FontFace", xmlWorkspace, FontFace);
            ConvertToXmlEx("FontSize", xmlWorkspace, FontSize);
            ConvertToXml("FontForeground", xmlWorkspace, FontForeground);
            ConvertToXmlEx("PriceStyle", xmlWorkspace, PriceStyle);
            ConvertToXmlEx("DisplayTitles", xmlWorkspace, MostrarTitulosPaineis);
            ConvertToXmlEx("DarvasBoxes", xmlWorkspace, DarvasBoxes);
            ConvertToXmlEx("ChartType", xmlWorkspace, ChartType);
            ConvertToXmlEx("TickPeriodicity", xmlWorkspace, TickPeriodicity);
            ConvertToXmlEx("TickCompressionType", xmlWorkspace, TickCompressionType);
            ConvertToXmlEx("InfoPanelFontSize", xmlWorkspace, InfoPanelTamanhoFonte);
            ConvertToXmlEx("InfoPanelFontFamily", xmlWorkspace, InfoPanelFonte);
            ConvertToXml("InfoPanelLabelsForeground", xmlWorkspace, InfoPanelCorFonteLabels);
            ConvertToXml("InfoPanelLabelsBackground", xmlWorkspace, InfoPanelCorFundoLabels);
            ConvertToXml("InfoPanelValuesForeground", xmlWorkspace, InfoPanelCorFonteValores);
            ConvertToXml("InfoPanelValuesBackground", xmlWorkspace, InfoPanelCorFundoValores);
            ConvertToXmlEx("InfoPanelPosition", xmlWorkspace, InfoPanelPosicao);
            ConvertToXml("IndicatorDialogBackground", xmlWorkspace, BackGroundDialogoIndicador);
            ConvertToXmlEx("InfoPanelPositionCoords", xmlWorkspace,
                           paineisContainers.infoPanel != null ? paineisContainers.infoPanel.Position : new Point(0, 0));

            for (int i = 0; i < parametrosEstiloPreco.Length; i++)
                ConvertToXmlEx("PriceStyleParam_" + i, xmlWorkspace, parametrosEstiloPreco[i]);

            return;
        }

        private void ReadWorkSpace(XContainer xRoot)
        {
            XElement xmlWorkspace = xRoot.Element("WorkSpace");
            if (xmlWorkspace == null) return;

            try
            {
                MostrarAnimacoes = ConvertFromXmlEx("ShowAnimations", xmlWorkspace, false);
                EscalaAlinhamento = (EnumGeral.TipoAlinhamentoEscalaEnum)System.Enum.Parse(typeof(EnumGeral.TipoAlinhamentoEscalaEnum), ConvertFromXmlEx("YAxes", xmlWorkspace, String.Empty), true);
                EscalaTipo = (EnumGeral.TipoEscala)System.Enum.Parse(typeof(EnumGeral.TipoEscala), ConvertFromXmlEx("ScalingType", xmlWorkspace, String.Empty), true);
                GradeY = ConvertFromXmlEx("ShowYGrid", xmlWorkspace, true);
                GradeX = ConvertFromXmlEx("ShowXGrid", xmlWorkspace, true);
                GradeCor = (Brush)ConvertFromXml("GridStroke", xmlWorkspace);
                Estilo3D = ConvertFromXmlEx("ThreeDStyle", xmlWorkspace, true);
                CandleCorAlta = Utils.StringToColor(ConvertFromXmlEx("UpColor", xmlWorkspace, String.Empty));
                CandleCorBaixa = Utils.StringToColor(ConvertFromXmlEx("DownColor", xmlWorkspace, String.Empty));
                CrossHairs = ConvertFromXmlEx("CrossHairs", xmlWorkspace, false);
                EspacoEsquerdaGrafico = ConvertFromXmlEx("LeftChartSpace", xmlWorkspace, 5);
                EspacoDireitaGrafico = ConvertFromXmlEx("RightChartSpace", xmlWorkspace, 10);
                LabelsEixoXRealTime = ConvertFromXmlEx("RealTimeData", xmlWorkspace, true);
                FontFace = ConvertFromXmlEx("FontFace", xmlWorkspace, String.Empty);
                FontForeground = (Brush)ConvertFromXml("FontForeground", xmlWorkspace);
                PriceStyle = (EnumGeral.EstiloPrecoEnum)System.Enum.Parse(typeof(EnumGeral.EstiloPrecoEnum), ConvertFromXmlEx("PriceStyle", xmlWorkspace, String.Empty), true);
                MostrarTitulosPaineis = ConvertFromXmlEx("DisplayTitles", xmlWorkspace, true);
                DarvasBoxes = ConvertFromXmlEx("DarvasBoxes", xmlWorkspace, false);
                ChartType = (EnumGeral.TipoGraficoEnum)System.Enum.Parse(typeof(EnumGeral.TipoGraficoEnum), ConvertFromXmlEx("ChartType", xmlWorkspace, String.Empty), true);
                TickPeriodicity = ConvertFromXmlEx("TickPeriodicity", xmlWorkspace, 5);
                TickCompressionType = (EnumGeral.CompressaoTickEnum)System.Enum.Parse(typeof(EnumGeral.CompressaoTickEnum), ConvertFromXmlEx("TickCompressionType", xmlWorkspace, String.Empty), true);
                InfoPanelTamanhoFonte = ConvertFromXmlEx("InfoPanelFontSize", xmlWorkspace, 10);
                InfoPanelFonte = new FontFamily(ConvertFromXmlEx("InfoPanelFontFamily", xmlWorkspace, "Arial"));
                InfoPanelCorFonteLabels = (Brush)ConvertFromXml("InfoPanelLabelsForeground", xmlWorkspace);
                InfoPanelCorFundoLabels = (Brush)ConvertFromXml("InfoPanelLabelsBackground", xmlWorkspace);
                InfoPanelCorFonteValores = (Brush)ConvertFromXml("InfoPanelValuesForeground", xmlWorkspace);
                InfoPanelCorFundoValores = (Brush)ConvertFromXml("InfoPanelValuesBackground", xmlWorkspace);
                InfoPanelPosicao = (EnumGeral.InfoPanelPosicaoEnum)System.Enum.Parse(typeof(EnumGeral.InfoPanelPosicaoEnum), ConvertFromXmlEx("InfoPanelPosition", xmlWorkspace, String.Empty), true);
                BackGroundDialogoIndicador = (Brush)ConvertFromXml("IndicatorDialogBackground", xmlWorkspace);
                if (paineisContainers.infoPanel != null)
                    paineisContainers.infoPanel.Position = PointEx.Parse(ConvertFromXmlEx("InfoPanelPositionCoords", xmlWorkspace, "0,0"));

                for (int i = 0; i < parametrosEstiloPreco.Length; i++)
                    parametrosEstiloPreco[i] = ConvertFromXmlEx("PriceStyleParam_" + i, xmlWorkspace, 0.0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void SaveChartPanels(XContainer xRoot)
        {
            XElement xmlChartPanels = new XElement("ChartPanels");
            xRoot.Add(xmlChartPanels);
            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                XElement xmlChartPanel = new XElement("ChartPanel");
                xmlChartPanels.Add(xmlChartPanel);

                ConvertToXmlEx("Position", xmlChartPanel, chartPanel._position);
                ConvertToXml("Background", xmlChartPanel, chartPanel.Background);
                ConvertToXmlEx("IsHeatMap", xmlChartPanel, chartPanel.IsHeatMap);
                ConvertToXmlEx("Visible", xmlChartPanel, chartPanel.Visible);
                ConvertToXmlEx("Height", xmlChartPanel, chartPanel.Height);
                ConvertToXmlEx("Index", xmlChartPanel, chartPanel.Index);
            }
        }

        private void ReadChartPanels(XContainer xRoot)
        {
            XElement xmlChartPanels = xRoot.Element("ChartPanels");
            if (xmlChartPanels == null) return;

            foreach (XElement node in xmlChartPanels.Elements())
            {
                if (node.Name != "ChartPanel") continue;

                ChartPanel.PositionType positionType =
                  (ChartPanel.PositionType)System.Enum.Parse(typeof(ChartPanel.PositionType), ConvertFromXmlEx("Position", node, string.Empty), true);
                bool isHeatMap = ConvertFromXmlEx("IsHeatMap", node, false);
                ChartPanel chartPanel;
                if (isHeatMap)
                {
                    chartPanel = AddHeatMapPanel();
                }
                else
                {
                    chartPanel = AddChartPanel(positionType);
                    chartPanel.Background = (Brush)ConvertFromXml("Background", node);
                }
                chartPanel.Visible = ConvertFromXmlEx("Visible", node, true);
            }
        }

        private void SaveSeries(XContainer xRoot)
        {
            XElement xmlSeries = new XElement("SeriesCollection");
            xRoot.Add(xmlSeries);
            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                foreach (Series series in chartPanel.SeriesCollection)
                {
                    XElement xmlS = new XElement("Series");
                    xmlSeries.Add(xmlS);

                    ConvertToXmlEx("Name", xmlS, series.Name);
                    ConvertToXmlEx("OHLCType", xmlS, series.OHLCType);
                    ConvertToXmlEx("PanelIndex", xmlS, series._chartPanel.Index);
                    ConvertToXmlEx("UpColor", xmlS, series.UpColor.HasValue ? series.UpColor.ToString() : string.Empty);
                    ConvertToXmlEx("DownColor", xmlS, series.DownColor.HasValue ? series.DownColor.ToString() : string.Empty);
                    ConvertToXmlEx("StrokeThicknes", xmlS, series.StrokeThickness);
                    ConvertToXmlEx("StrokeColor", xmlS, series.StrokeColor);
                    ConvertToXmlEx("Selectable", xmlS, series.Selectable);
                    ConvertToXmlEx("Visible", xmlS, series.Visible);
                    ConvertToXmlEx("StrokePattern", xmlS, series.StrokePattern);
                    ConvertToXmlEx("SeriesType", xmlS, series.SeriesType);
                    ConvertToXmlEx("SeriesIndex", xmlS, series._seriesIndex);
                }
            }
        }

        private void ReadSeries(XContainer xRoot)
        {
            XElement xmlSeries = xRoot.Element("SeriesCollection");
            if (xmlSeries == null) return;
            int useablePanelsCount = UseablePanelsCount;

            foreach (XElement node in xmlSeries.Elements())
            {
                if (node.Name != "Series") continue;

                int panelIndex = ConvertFromXmlEx("PanelIndex", node, -1);
                if (panelIndex == -1) continue;
                if (panelIndex >= useablePanelsCount) panelIndex = 0;

                ChartPanel chartPanel = GetPanelByIndex(panelIndex);
                string seriesName = ConvertFromXmlEx("Name", node, string.Empty);
                if (seriesName.Length == 0) continue;
                EnumGeral.TipoSerieOHLC ohlc = (EnumGeral.TipoSerieOHLC)System.Enum.Parse(typeof(EnumGeral.TipoSerieOHLC), ConvertFromXmlEx("OHLCType", node, string.Empty), true);
                EnumGeral.TipoSeriesEnum seriesType = (EnumGeral.TipoSeriesEnum)System.Enum.Parse(typeof(EnumGeral.TipoSeriesEnum), ConvertFromXmlEx("SeriesType", node, string.Empty), true);
                Series series = chartPanel.CreateSeries(seriesName, ohlc, seriesType);
                if (series == null) continue;

                string upColor = ConvertFromXmlEx("UpColor", node, string.Empty);
                string downColor = ConvertFromXmlEx("DownColor", node, string.Empty);
                series.StrokeThickness = ConvertFromXmlEx("StrokeThicknes", node, 1.0);
                series.StrokeColor = Utils.StringToColor(ConvertFromXmlEx("StrokeColor", node, string.Empty));
                series._selectable = ConvertFromXmlEx("Selectable", node, false);
                series.Visible = ConvertFromXmlEx("Visible", node, true);
                series.StrokePattern = (EnumGeral.TipoLinha)System.Enum.Parse(typeof(EnumGeral.TipoLinha), ConvertFromXmlEx("StrokePattern", node, "Solido"), true);
                series.UpColor = upColor.Length > 0 ? Utils.StringToColor(upColor) : (Color?)null;
                series.DownColor = downColor.Length > 0 ? Utils.StringToColor(downColor) : (Color?)null;
                series._seriesIndex = ConvertFromXmlEx("SeriesIndex", node, 0);

                dataManager.AddSeries(series.Name, series.OHLCType);
                dataManager.BindSeries(series);
            }
        }

        private void SaveSeriesData(XElement xRoot)
        {
            dataManager.SaveToXml(xRoot);
        }

        private void ReadSeriesData(XElement xRoot)
        {
            dataManager.LoadFromXml(xRoot);
        }

        private void SaveIndicators(XContainer xRoot)
        {
            XElement xmlIndicators = new XElement("Indicators");
            xRoot.Add(xmlIndicators);
            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                foreach (Indicator indicator in chartPanel.IndicatorsCollection)
                {
                    if (indicator.IsTwin) continue;

                    XElement xmlIndicator = new XElement("Indicator");
                    xmlIndicators.Add(xmlIndicator);

                    ConvertToXmlEx("Name", xmlIndicator, indicator.Name);
                    ConvertToXmlEx("IndicatorType", xmlIndicator, indicator.IndicatorType);
                    ConvertToXmlEx("PanelIndex", xmlIndicator, indicator._chartPanel.Index);
                    ConvertToXmlEx("UpColor", xmlIndicator, indicator.UpColor.HasValue ? indicator.UpColor.ToString() : string.Empty);
                    ConvertToXmlEx("DownColor", xmlIndicator, indicator.DownColor.HasValue ? indicator.DownColor.ToString() : string.Empty);
                    ConvertToXmlEx("StrokeThicknes", xmlIndicator, indicator.StrokeThickness);
                    ConvertToXmlEx("StrokeColor", xmlIndicator, indicator.StrokeColor);
                    ConvertToXmlEx("Selectable", xmlIndicator, indicator.Selectable);
                    ConvertToXmlEx("Visible", xmlIndicator, indicator.Visible);
                    ConvertToXmlEx("StrokePattern", xmlIndicator, indicator.StrokePattern);
                    ConvertToXmlEx("UserParams", xmlIndicator, indicator.UserParams);
                    List<StockChartX_IndicatorsParameters.IndicatorParameter> indicatorParams =
                      StockChartX_IndicatorsParameters.GetIndicatorParameters(indicator.IndicatorType);
                    for (int i = 0; i < indicatorParams.Count; i++)
                    {
                        ConvertToXmlEx("Parameter_" + i, xmlIndicator, indicator.GetParameterValue(i));
                    }
                }
            }
        }

        private void ReadIndicators(XContainer xRoot)
        {
            XElement xmlIndicators = xRoot.Element("Indicators");
            if (xmlIndicators == null) return;

            int useablePanelsCount = UseablePanelsCount;
            foreach (XElement node in xmlIndicators.Elements())
            {
                if (node.Name != "Indicator") continue;

                int panelIndex = ConvertFromXmlEx("PanelIndex", node, -1);
                if (panelIndex == -1) continue;
                if (panelIndex >= useablePanelsCount) panelIndex = 0;

                ChartPanel chartPanel = GetPanelByIndex(panelIndex);
                string indicatorName = ConvertFromXmlEx("Name", node, string.Empty);
                if (indicatorName == "") continue;
                EnumGeral.IndicatorType indicatorType = (EnumGeral.IndicatorType)System.Enum.Parse(typeof(EnumGeral.IndicatorType), ConvertFromXmlEx("IndicatorType", node, string.Empty), true);
                int indicatorCount = GetIndicatorCountByType(indicatorType);
                if (indicatorCount > 0)
                    indicatorName += indicatorCount;

                Indicator indicator = AddIndicator(indicatorType, indicatorName, chartPanel, true);
                string upColor = ConvertFromXmlEx("UpColor", node, string.Empty);
                string downColor = ConvertFromXmlEx("DownColor", node, string.Empty);
                if (upColor.Length > 0)
                    indicator.UpColor = Utils.StringToColor(upColor);
                if (downColor.Length > 0)
                    indicator.DownColor = Utils.StringToColor(downColor);
                indicator.StrokeThickness = ConvertFromXmlEx("StrokeThicknes", node, 1.0);
                indicator.StrokeColor = Utils.StringToColor(ConvertFromXmlEx("StrokeColor", node, string.Empty));
                indicator.Selectable = ConvertFromXmlEx("Selectable", node, true);
                indicator.Visible = ConvertFromXmlEx("Visible", node, true);
                indicator.StrokePattern =
                  (EnumGeral.TipoLinha)System.Enum.Parse(typeof(EnumGeral.TipoLinha), ConvertFromXmlEx("StrokePattern", node, "Solido"), true);
                indicator.UserParams = ConvertFromXmlEx("UserParams", node, false);
                var indicatorParams = StockChartX_IndicatorsParameters.GetIndicatorParameters(indicator.IndicatorType);
                for (int i = 0; i < indicatorParams.Count; i++)
                {
                    object oValue = ConvertFromXmlEx("Parameter_" + i, node, (object)null);
                    if (i > 0)
                    {
                        var type = indicatorParams[i].ValueType;
                        if (type.IsEnum)
                            oValue = System.Enum.Parse(type, oValue.ToString(), true);
                        else
                            oValue = oValue == null ? 0 : Convert.ChangeType(oValue, indicatorParams[i].ValueType, CultureInfo.InvariantCulture);

                        indicator.SetParameterValue(i, oValue);
                    }
                    else
                    {
                        Debug.Assert(indicatorParams[i].ValueType == typeof(string));
                        string sValue = oValue.ToString();
                        string[] values = sValue.Split('.');
                        if (values.Length == 2 && GetSeriesByName(sValue) == null) //pointing to a non-existing series
                        {
                            indicator.SetParameterValue(i, Symbol + "." + values[1]);
                        }
                        else
                        {
                            indicator.SetParameterValue(i, sValue);
                        }
                    }
                }
            }
            recalc = true;
        }

        private void SaveLineStudies(XContainer xRoot)
        {
            XElement xmlLineStudies = new XElement("LineStudies");
            xRoot.Add(xmlLineStudies);

            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                foreach (LineStudy study in chartPanel._lineStudies)
                {
                    XElement xmlLineStudy = new XElement("LineStudy");
                    xmlLineStudies.Add(xmlLineStudy);

                    ConvertToXmlEx("Key", xmlLineStudy, study.Key);
                    ConvertToXmlEx("StudyType", xmlLineStudy, study.StudyType);
                    ConvertToXmlEx("PanelIndex", xmlLineStudy, study._chartPanel.Index);
                    ConvertToXmlEx("X1Value", xmlLineStudy, study.X1Value);
                    ConvertToXmlEx("X2Value", xmlLineStudy, study.X2Value);
                    ConvertToXmlEx("Y1Value", xmlLineStudy, study.Y1Value);
                    ConvertToXmlEx("Y2Value", xmlLineStudy, study.Y2Value);
                    ConvertToXmlEx("Selectable", xmlLineStudy, study.Selectable);
                    ConvertToXml("StrokeColor", xmlLineStudy, study.Stroke);
                    ConvertToXmlEx("StrokeThickness", xmlLineStudy, study.StrokeThickness);
                    ConvertToXmlEx("StrokeType", xmlLineStudy, study.StrokeType);
                    ConvertToXmlEx("ExtraArgs", xmlLineStudy, ConvertToBinary(study.ExtraArgs));
                    ConvertToXmlEx("Opacity", xmlLineStudy, study.Opacity);

                    if (study is IShapeAble)
                    {
                        IShapeAble shapeAble = study as IShapeAble;
                        ConvertToXml("Fill", xmlLineStudy, shapeAble.Fill);
                    }
                }
            }
        }

        private void ReadLineStudies(XContainer xRoot)
        {
            XElement xmlLineStudies = xRoot.Element("LineStudies");
            if (xmlLineStudies == null) return;
            int useablePanelsCount = UseablePanelsCount;

            foreach (XElement node in xmlLineStudies.Elements())
            {
                if (node.Name != "LineStudy") return;

                int panelIndex = ConvertFromXmlEx("PanelIndex", node, -1);

                if (panelIndex == -1) 
                    continue;

                if (panelIndex >= useablePanelsCount) 
                    panelIndex = 0;

                string key = ConvertFromXmlEx("Key", node, String.Empty);
                string studyTypeS = ConvertFromXmlEx("StudyType", node, String.Empty);

                LineStudy.StudyTypeEnum studyType =
                  (LineStudy.StudyTypeEnum)System.Enum.Parse(typeof(LineStudy.StudyTypeEnum), studyTypeS, true);

                Brush stroke = (Brush)ConvertFromXml("StrokeColor", node);
                object[] extrArgs = (object[])ConvertFromBinary(ConvertFromXmlEx("ExtraArgs", node, string.Empty), typeof(object[]));

                LineStudy lineStudy = CreateLineStudy(studyType, key, stroke, panelIndex);
                lineStudy.SetArgs(extrArgs);

                double x1Value = ConvertFromXmlEx("X1Value", node, 0.0);
                double x2Value = ConvertFromXmlEx("X2Value", node, 0.0);
                double y1Value = ConvertFromXmlEx("Y1Value", node, 0.0);
                double y2Value = ConvertFromXmlEx("Y2Value", node, 0.0);

                lineStudy.SetXYValues(x1Value, y1Value, x2Value, y2Value);
                lineStudy._selectable = ConvertFromXmlEx("Selectable", node, false);
                lineStudy.StrokeThickness = ConvertFromXmlEx("StrokeThickness", node, 1.0);
                lineStudy.StrokeType =
                  (EnumGeral.TipoLinha)System.Enum.Parse(typeof(EnumGeral.TipoLinha), ConvertFromXmlEx("StrokeType", node, EnumGeral.TipoLinha.Solido.ToString()), true);
                lineStudy.Opacity = ConvertFromXmlEx("Opacity", node, 1.0);

                if (lineStudy is IShapeAble)
                {
                    IShapeAble shapeAble = (IShapeAble)lineStudy;
                    shapeAble.Fill = (Brush)ConvertFromXml("Fill", node);
                }
            }
        }


        private static void ConvertToXml<T>(string name, XContainer xRoot, T value)
        {
            xRoot.Add(new XElement(name, XamlWriter.Save(value)));
        }

        private static void ConvertToXmlEx<T>(string name, XContainer xRoot, T value)
        {
            xRoot.Add(new XElement(name, Convert.ToString(value, CultureInfo.InvariantCulture)));
        }

        private static T ConvertFromXmlEx<T>(string name, XContainer xRoot, T defValue) /*where T : IConvertible*/
        {
            XElement xmlElement = xRoot.Element(name);
            if (xmlElement == null) return defValue;

            try
            {
                return (T)Convert.ChangeType(xmlElement.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return defValue;
            }
        }

        private static object ConvertFromXml(string name, XContainer xRoot)
        {
            XElement xmlElement = xRoot.Element(name);
            if (xmlElement == null) return null;

            try
            {
                StringReader stringReader = new StringReader(xmlElement.Value);
                return XamlReader.Load(stringReader.ReadToEnd());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }


        private static string ConvertToBinary(object obj)
        {
            if (obj == null)
                return string.Empty;
            DataContractSerializer dcs = new DataContractSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            dcs.WriteObject(stream, obj);
            return Convert.ToBase64String(stream.GetBuffer());
        }

        private static object ConvertFromBinary(string base64Text, Type type)
        {
            if (string.IsNullOrEmpty(base64Text))
                return null;
            DataContractSerializer dcs = new DataContractSerializer(type);
            MemoryStream stream = new MemoryStream();
            byte[] data = Convert.FromBase64String(base64Text);
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return dcs.ReadObject(stream);
        }
    }
}
