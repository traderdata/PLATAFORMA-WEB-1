using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Line=System.Windows.Shapes.Line;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif
using Traderdata.Client.Componente.GraficoSL.Enum;


namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// Exception type used when a error comes in indicator calculation
    /// </summary>
    public class IndicatorException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="indicator">Reference to indicator with error</param>
        public IndicatorException(string message, Indicator indicator)
            : base(message)
        {
            Indicator = indicator;
        }

        /// <summary>
        /// Reference to indicator that had an error
        /// </summary>
        public Indicator Indicator { get; private set; }
    }

    /// <summary>
    /// Base class for all indicators
    /// </summary>
    [CLSCompliant(true)]
    public partial class Indicator : Series, IChartElementPropertyAble
    {
        internal bool _toBeAdded; //indicates that the indicator is going to be added
        internal bool _calculateResult;
        internal bool _calculated;
        internal bool _dialogShown;
        internal bool _isTwin;
        internal bool _dialogNeeded = true; //used for custom indicators
        internal ToolTip _tooltip;
        internal PaintObjectsManager<PaintObjects.Line> _lines = new PaintObjectsManager<PaintObjects.Line>();

        internal bool _inputError;
        internal bool _calculating;
        internal bool _showDialog;
        internal EnumGeral.IndicatorType _indicatorType;
        internal IndicatorDialog _dialog;
        internal List<object> _params = new List<object>();
        private List<StockChartX_IndicatorsParameters.IndicatorParameter> _parameters;

        private Color corSerieFilha1 = Colors.Red;
        private Color corSerieFilha2 = Colors.Yellow;

        private EnumGeral.TipoLinha tipoLinhaSerieFilha1 = EnumGeral.TipoLinha.Solido;
        private EnumGeral.TipoLinha tipoLinhaSerieFilha2 = EnumGeral.TipoLinha.Solido;

        private double grossuraSerieFilha1 = 1;
        private double grossuraSerieFilha2 = 1;

        private bool painelIndicadores = false;

        public bool PainelIndicadoresLateral { get; set; }
        public bool PainelIndicadoresAbaixo { get; set; }
        public bool PainelPreco { get; set; }
        public bool PainelVolume { get; set; }

        public double GrossuraSerieFilha1
        {
            get { return grossuraSerieFilha1; }
            set 
            {
                grossuraSerieFilha1 = value;

                if ((this.SeriesFilhas != null) && (this.SeriesFilhas.Count > 0))
                    this.SeriesFilhas[0].StrokeThickness = grossuraSerieFilha1;
            }
        }

        public double GrossuraSerieFilha2
        {
            get { return grossuraSerieFilha2; }
            set 
            {
                grossuraSerieFilha2 = value;

                if ((this.SeriesFilhas != null) && (this.SeriesFilhas.Count > 0))
                    this.SeriesFilhas[1].StrokeThickness = grossuraSerieFilha2;
            }
        }


        public EnumGeral.TipoLinha TipoLinhaSerieFilha1
        {
            get { return tipoLinhaSerieFilha1; }
            set 
            {
                tipoLinhaSerieFilha1 = value;

                if ((this.SeriesFilhas != null) && (this.SeriesFilhas.Count > 0))
                    this.SeriesFilhas[0].StrokePattern = tipoLinhaSerieFilha1;
            }
        }

        public EnumGeral.TipoLinha TipoLinhaSerieFilha2
        {
            get { return tipoLinhaSerieFilha2; }
            set
            {
                tipoLinhaSerieFilha2 = value;

                if ((this.SeriesFilhas != null) && (this.SeriesFilhas.Count > 0))
                    this.SeriesFilhas[1].StrokePattern = tipoLinhaSerieFilha2;
            }
        }

        public Color CorSerieFilha1
        {
            get { return corSerieFilha1; }
            set 
            {
                corSerieFilha1 = value;

                if ((this.SeriesFilhas != null) && (this.SeriesFilhas.Count > 0))
                {
                    this.SeriesFilhas[0].StrokeColor = corSerieFilha1;
                    this.SeriesFilhas[0].UpColor = corSerieFilha1;
                    this.SeriesFilhas[0].DownColor = corSerieFilha1;
                    this.SeriesFilhas[0].TitleBrush = new SolidColorBrush(corSerieFilha1);                    
                }
            }
        }

        public Color CorSerieFilha2
        {
            get { return corSerieFilha2; }
            set 
            {
                corSerieFilha2 = value;

                if ((this.SeriesFilhas != null) && (this.SeriesFilhas.Count > 1))
                {
                    this.SeriesFilhas[1].StrokeColor = corSerieFilha2;
                    this.SeriesFilhas[1].UpColor = corSerieFilha2;
                    this.SeriesFilhas[1].DownColor = corSerieFilha2;
                    this.SeriesFilhas[1].TitleBrush = new SolidColorBrush(corSerieFilha2);
                }
            }
        }

        public List<StockChartX_IndicatorsParameters.IndicatorParameter> Parameters
        {
            get { return _parameters; }
        }

        public int QtdParametros
        {
            
            get {return StockChartX_IndicatorsParameters.GetIndicatorParameters(this.IndicatorType).Count;}
        }

        public enum ErroIndicador 
        {
            PeriodosInvalidos,
            TipoMediaMovelInvalida,
            DesvioPadraoInvalido,
            LimitMoveInvalido,
            RateOfChange,
            Serie1IgualSerie2,
            BarraHistoricaInvalida,
            SinalPeriodsIgualShortCicle,
            PorcBandShift,
            MaxAFInvalido,
            MinAFInvalido,
            CicloCurtoInvalido,
            CicloLongoInvalido,
            LevelInvalido,
            PercKPeriodoInvalido,
            PercKSmoothingInvalido,
            PercKDoubleSmoothingInvalido,
            PercKSlowingPeriodosInvalido,
            PercDPeriodsInvalido,
            MinTickValueInvalido,
            Ciclo1Invalido,
            Ciclo2Invalido,
            Ciclo3Invalido,
            EscalaR2Invalida,
            PeriodoTermoCurtoInvalido,
            PeriodoTermoLongoInvalido,
            PontosOuPercentualInvalido,
            Desconhecido
        };


        //Novas variáveis
        private bool contemSeriesFilhas = false;
        private List<Series> seriesFilhas = null;

        private readonly SortedList<string, EnumGeral.IndicatorType> _MATypes =
          new SortedList<string, EnumGeral.IndicatorType>
        {
          {"Simple", EnumGeral.IndicatorType.MediaMovelSimples},
          {"Exponential", EnumGeral.IndicatorType.MediaMovelExponencial},
          {"Tempo Series", EnumGeral.IndicatorType.MediaMovelSerieTempo},
          {"Triangular", EnumGeral.IndicatorType.MediaMovelTriangular},
          {"Variable", EnumGeral.IndicatorType.MediaMovelVariavel},
          {"VIDYA", EnumGeral.IndicatorType.VIDYA},
          {"Weighted", EnumGeral.IndicatorType.MediaMovelPonderada},
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public Indicator(string name, ChartPanel chartPanel)
            : base(name, EnumGeral.TipoSeriesEnum.Indicador, EnumGeral.TipoSerieOHLC.Desconhecido, chartPanel)
        {
            _chartPanel._chartX.dataManager.AddSeries(name, EnumGeral.TipoSerieOHLC.Desconhecido);
            _chartPanel._chartX.dataManager.BindSeries(this);
        }

        /// <summary>
        /// This property is set when adding the indicator. If is true then user will be asked via a dialog for parameters.
        /// if false, then parameters must be set via code.
        /// </summary>
        public bool UserParams { get; internal set; }


        /// <summary>
        /// Indica se o indicador possui séries filhas.
        /// </summary>
        public bool ContemSeriesFilhas
        {
            get { return contemSeriesFilhas; }
            set { contemSeriesFilhas = value; }
        }

        /// <summary>
        /// Séries filhas do indicador. Caso nao possua o valor será null.
        /// </summary>
        public List<Series> SeriesFilhas
        {
            get { return seriesFilhas; }
            set { seriesFilhas = value; }
        }

        /// <summary>
        /// Force the series to be painted as an oscilator (histogram)
        /// </summary>
        public bool ForceOscilatorPaint { get; set; }

        ///<summary>
        /// Get or sets the series to be painted as a linear chart. 
        /// This property is ignored when <see cref="ForceOscilatorPaint"/> = true;
        /// If set to TRUE the negatuve values won't be considered for chart to be painted as a histogram.
        /// If set to FALSE any negative value present in series will make current series to be painted as a historamm.
        ///</summary>
        public bool ForceLinearChart { get; set; }

        /// <summary>
        /// Returns the value of a parameter that indicator uses internal
        /// </summary>
        /// <param name="parameterIndex">Parameter index</param>
        /// <returns>Parameter Value</returns>
        public object GetParameterValue(int parameterIndex)
        {
            Debug.Assert(_parameters != null);
            if (parameterIndex < 0 || parameterIndex >= _parameters.Count)
                throw new ArgumentOutOfRangeException("parameterIndex");
            object r = _params[parameterIndex];
            if (r.GetType() == typeof(EnumGeral.IndicatorType))
                r = (int)(EnumGeral.IndicatorType)r;
            return r;
        }

        internal bool IsTwin
        {
            get { return _isTwin; }
        }

        ///<summary>
        /// When adding indicator by programm use this function to set indicators' parameters
        ///</summary>
        ///<param name="parameterIndex">Parameter Index</param>
        ///<param name="value">Value</param>
        ///<exception cref="ArgumentOutOfRangeException"></exception>
        public void SetParameterValue(int parameterIndex, object value)
        {
            if (parameterIndex < 0 || parameterIndex >= _parameters.Count)
                throw new ArgumentOutOfRangeException("parameterIndex");

            if (value.GetType() == typeof(EnumGeral.IndicatorType))
                value = (EnumGeral.IndicatorType)value;
            _params[parameterIndex] = value;
        }

        /// <summary>
        /// Forces the dialog with indicators' properties to be shown
        /// </summary>
        public void ShowParametersDialog()
        {
            _showDialog = true;
            _dialogNeeded = true; //cause of custom indicators, force dialog to be shown
#if SILVERLIGHT
            _dialog = new IndicatorDialog
                        {
                            AppRoot = _chartPanel._chartX.AppRoot,
                            Indicator = this
                        };
#endif
            GetUserInput(new Func<bool>[] { TrueAction, FalseAction });
        }

        #region VerificaPeriodoValido()
        /// <summary>
        /// Verifica se para este periodo (quantidade de barras/records) o indicador pode ser calculado.
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public virtual bool VerificaPeriodoValido(int records)
        {
            if (records > 0)
                return true;
            else
                return false;
        }
        #endregion VerificaPeriodoValido()

        internal override void Init()
        {
            base.Init();

            _dialogShown = false;
            _inputError = false;
            _calculating = false;
            _calculated = false;
            _shareScale = true;

            if (_indicatorType != EnumGeral.IndicatorType.CustomIndicator)
            {
                _parameters = StockChartX_IndicatorsParameters.GetIndicatorParameters(_indicatorType);
                _params = new List<object>(new object[_parameters.Count]);
            }
            else
            {
                _parameters = new List<StockChartX_IndicatorsParameters.IndicatorParameter>();
                _params = new List<object>();
            }
        }

        /// <summary>
        /// Indicator error types
        /// </summary>
        [Flags]
        protected enum IndicatorErrorType : short
        {
            /// <summary>
            /// Indicator has circular reference
            /// </summary>
            CircularReference = 0x01,
            /// <summary>
            /// Indicator must be removed
            /// </summary>
            RemoveIndicator = 0x02,
            /// <summary>
            /// Throw an exception
            /// </summary>
            ThrowError = 0x04,
            /// <summary>
            /// Show an error message
            /// </summary>
            ShowErrorMessage = 0x08
        }

        /// <summary>
        /// Functions that executes when indicator is canceled
        /// </summary>
        /// <returns></returns>
        protected bool FalseAction()
        {
            return _calculateResult = false;
        }

        /// <summary>
        /// Method that executes after calculation on indicator is done.
        /// </summary>
        /// <returns></returns>
        protected bool PostCalculate()
        {
            _calculated = true;
            _chartPanel._chartX.atualizandoIndicador = false;

            return true;
        }

        /// <summary>
        /// An overradable method used by children classes.
        /// </summary>
        /// <returns></returns>
        protected virtual bool TrueAction() { throw new NotImplementedException(); }

        internal Action _postCalculateAction = () => { };

        internal bool Calculate()
        {
            if (_calculated)
            {
                _postCalculateAction();
                return _calculateResult = true;
            }
            /*
              1. Validate the indicator parameters (if any)
              2. Validate available inputs
              3. Gather the inputs into a TA-SDK recordset
              4. Calculate the indicator
              5. If there is only one output, store the data
                 in the data_master array of this series. 
                 If there are two or more outputs, create new 
                 CSeriesStandard for each additional output
            */

            // Get input from user
            GetUserInput(new Func<bool>[] { TrueAction, FalseAction });

            return true;
        }

        /// <summary>
        /// Mark as true the recycled flag for linked series for current indicator
        /// </summary>
        protected void RecycleLinkedSeries()
        {
            foreach (ChartPanel chartPanel in _chartPanel._panelsContainer.Panels)
            {
                foreach (Series series in chartPanel.AllSeriesCollection)
                {
                    foreach (Series series1 in series._linkedSeries)
                    {
                        series1._recycleFlag = true;
                    }
                }
            }
        }

        /// <summary>
        /// returns an integer value for indicator parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int ParamInt(int index)
        {
            return Convert.ToInt32(_params[index]);
        }

        /// <summary>
        /// returns an double value for indicator parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected double ParamDbl(int index)
        {
            return Convert.ToDouble(_params[index]);
        }

        /// <summary>
        /// returns an string value for indicator parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string ParamStr(int index)
        {
            return Convert.ToString(_params[index]);
        }

        /// <summary>
        /// Functions that takes care of user input dialog
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        protected bool GetUserInput(params Func<bool>[] actions)
        {
            _chartPanel._chartX.atualizandoIndicador = true;

            Func<bool> trueResultAction = actions.Length > 0 ? actions[0] : () => true;
            Func<bool> falseResultAction = actions.Length > 1 ? actions[1] : () => false;

            // If this dialog was originally made programmatically,
            // but the client program is asking to show the dialog,
            // then convert this indicator into a user-param indicator.
            if (_showDialog)
                UserParams = true;
            if (_dialogShown || this is IndicadorSerieFilha)
            {
                if (HasCircularReference(this) && !_inputError)
                {
                    ProcessError(FullName + " has a circular reference to another indicator.", IndicatorErrorType.CircularReference);
                    falseResultAction();
                    return false;
                }
                if (!_inputError)
                    trueResultAction();
                return true;
            }

            /*
        First check to see if we have inputs already.
        If we do (paramStr[0] != ""), then return true.
      
        If we don't have inputs or the dialog is requested
        for some reason anyway, then show the dialog and 
        return false. The dialog will call Calculate()
        again when inputs have been collected.

        If the Calculate() function throws an error,
        ProcessError will call this function.
        */
            if (_parameters.Count == 0)
                throw new ArgumentException("Indicator " + FullName + " must have at least one parameter.");

            bool firstTime = (string.IsNullOrEmpty(ParamStr(0)));
            if (_indicatorType == EnumGeral.IndicatorType.CustomIndicator)
            {
                if (!_dialogNeeded || !UserParams)
                {
                    trueResultAction();
#if SILVERLIGHT
                    if (!_dialogShown)
                        _postCalculateAction();
#endif
                    return true;
                }
                _dialogNeeded = false;
            }
            else
            {
                if (ParamStr(0).Length > 0 && !_inputError && !_showDialog)
                {
                    trueResultAction();
#if SILVERLIGHT
                    if (!_dialogShown)
                        _postCalculateAction();
#endif
                    return true; //No need to get user input
                }
            }

            _chartPanel._chartX.locked = true;

#if WPF
      _dialog = new IndicatorDialog
                  {
                    Owner =
                      Application.Current != null && Application.Current.Windows.Count > 0
                        ? Application.Current.Windows[0]
                        : null,
                    Indicator = this,
                    stackPanelBackground = { Background = _chartPanel._chartX.IndicatorDialogBackground },
                    Tag = actions,
                  };
#endif
#if SILVERLIGHT
            //in SL the dialog is created in ChartPanel_CalcIndicators where we simulate modal behavior
            //in a non-modal enviroment
            _dialog.Tag = actions;
#endif

            #region Set Controls Values
            TextBlock tbl;
            ComboBox cmb = null;
            TextBox tb;

            int n;
            for (n = 0; n < _params.Count; n++)
            {
                switch (_parameters[n].ParameterType)
                {
                    case EnumGeral.TipoParametroIndicador.Ativo:
                        cmb = _dialog.GetComboBox(n);
                        EnumSymbols(cmb);
                        SetComboDefault(cmb, ParamStr(n));
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case EnumGeral.TipoParametroIndicador.Serie:
                    case EnumGeral.TipoParametroIndicador.Serie1:
                    case EnumGeral.TipoParametroIndicador.Serie2:
                    case EnumGeral.TipoParametroIndicador.Serie3:
                    case EnumGeral.TipoParametroIndicador.Volume:
                        cmb = _dialog.GetComboBox(n);
                        EnumSeries(cmb);
                        SetComboDefault(cmb, ParamStr(n));
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case EnumGeral.TipoParametroIndicador.PontosOuPercent:
                        cmb = _dialog.GetComboBox(n);
                        cmb.Items.Clear();
                        cmb.Items.Add("Pontos");
                        cmb.Items.Add("Percentual");
                        cmb.SelectedIndex = 0;
                        if (ParamInt(n) == 1)
                            SetComboDefault(cmb, "Pontos");
                        else
                            SetComboDefault(cmb, "Percentual");
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case EnumGeral.TipoParametroIndicador.TipoMediaMovel:
                    case EnumGeral.TipoParametroIndicador.PercentDTipoMediaMovel:
                        cmb = _dialog.GetComboBox(n);
                        EnumMATypes(cmb);
                        SetComboDefault(cmb, MATypeToStr((EnumGeral.IndicatorType)ParamInt(n)));
                        SetMAComboSel(cmb, (EnumGeral.IndicatorType)_parameters[n].DefaultValue);
                        _dialog.ShowHidePanel(n, false, false);
                        break;
                    case EnumGeral.TipoParametroIndicador.BarraHistorica:
                    case EnumGeral.TipoParametroIndicador.Periodos:
                    case EnumGeral.TipoParametroIndicador.Levels:
                    case EnumGeral.TipoParametroIndicador.Ciclo1:
                    case EnumGeral.TipoParametroIndicador.Ciclo2:
                    case EnumGeral.TipoParametroIndicador.Ciclo3:
                    case EnumGeral.TipoParametroIndicador.CurtoPrazo:
                    case EnumGeral.TipoParametroIndicador.LongoPrazo:
                    case EnumGeral.TipoParametroIndicador.PercentKPeriodos:
                    case EnumGeral.TipoParametroIndicador.PercentDPeriodos:
                    case EnumGeral.TipoParametroIndicador.PercentKRetardo:
                    case EnumGeral.TipoParametroIndicador.PercentKSuave:
                    case EnumGeral.TipoParametroIndicador.PercentDSuave:
                    case EnumGeral.TipoParametroIndicador.PercentDDoubleSuave:
                    case EnumGeral.TipoParametroIndicador.PercentKDoubleSuave:
                    case EnumGeral.TipoParametroIndicador.CicloCurto:
                    case EnumGeral.TipoParametroIndicador.CicloLongo:
                    case EnumGeral.TipoParametroIndicador.DesvioPadrao:
                    case EnumGeral.TipoParametroIndicador.TaxaVariacao:
                    case EnumGeral.TipoParametroIndicador.PeriodosSinal:
                        tb = _dialog.GetTextBox(n);
                        tb.Text = ParamInt(n) == 0 ? Convert.ToString(_parameters[n].DefaultValue) : ParamInt(n).ToString();
                        _dialog.ShowHidePanel(n, false, true);
                        break;
                    case EnumGeral.TipoParametroIndicador.ValorMinimoTick:
                    case EnumGeral.TipoParametroIndicador.EscalaR2:
                    case EnumGeral.TipoParametroIndicador.AFMinimo:
                    case EnumGeral.TipoParametroIndicador.AFMaximo:
                    case EnumGeral.TipoParametroIndicador.Shift:
                    case EnumGeral.TipoParametroIndicador.Fator:
                    case EnumGeral.TipoParametroIndicador.ValorMovelLimite:
                        tb = _dialog.GetTextBox(n);
                        tb.Text = ParamDbl(n) == 0.0 ? Convert.ToString(_parameters[n].DefaultValue) : ParamDbl(n).ToString();
                        _dialog.ShowHidePanel(n, false, true);
                        break;
                }

                if (firstTime)
                {
                    if (_parameters[n].ParameterType == EnumGeral.TipoParametroIndicador.Volume && cmb != null)
                    {
                        foreach (var item in cmb.Items)
                        {
                            string s = item.ToString();
                            if (s.IndexOf("vol", StringComparison.CurrentCultureIgnoreCase) == -1) continue;

                            cmb.SelectedItem = item;
                            break;
                        }
                    }
                }

                //Description
                tbl = _dialog.GetTextBlock(n);
                tbl.Text = _parameters[n].Name;
            }
            _dialog.Height = n * 25 + 85;

            //hide other panels
            for (; n < Constants.MaxIndicatorParamCount; n++)
                _dialog.ShowHidePanel(n, true, true);

            _inputError = false;
            _dialog.Title = FullName;

            _dialog.OnOk += Dialog_OnOk_GetUserInput;
            _dialog.OnCancel += Dialog_OnCancel_GetUserInput;

            #endregion

            bool bResult = _dialog.ShowDialog().Value;
            return bResult;
        }

        /// <summary>
        /// Function that process the error
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="errorType"></param>
        protected void ProcessError(string Description, IndicatorErrorType errorType)
        {
            if (_chartPanel._chartX.mostraDialogoErro) return;
            _chartPanel._chartX.mostraDialogoErro = true;
            _chartPanel._chartX.FireOnDialogShown();

            string error = Description + Environment.NewLine +
                           "Click OK to fix the problem or click" + Environment.NewLine +
                           "Cancel to remove " + _name;

            if ((errorType & IndicatorErrorType.CircularReference) == IndicatorErrorType.CircularReference)
                if (_dialog != null)
                    _dialog.btnCancel.IsEnabled = false;

            if ((errorType & IndicatorErrorType.RemoveIndicator) == IndicatorErrorType.RemoveIndicator)
            {
                _recycleFlag = true;
                RecycleLinkedSeries();
                _chartPanel._chartX.mostraDialogoErro = false;
                return;
            }

            if (UserParams)
            {
                MessageBoxResult mr =
                  _dialog == null
                    ? MessageBoxResult.Cancel
                    : MessageBox.Show(error, "Error:", MessageBoxButton.OKCancel
#if WPF
                              , MessageBoxImage.Warning
#endif
);

                _inputError = true;
                if (mr == MessageBoxResult.OK)
                {
                    GetUserInput();
                }
                else
                {

                    if (_dialog != null)
                    {
                        _dialog._userCanceled = true;
                    }
                    _recycleFlag = true;
                    RecycleLinkedSeries();
                }
            }
            else
            {
                throw new IndicatorException(Description, this);
            }
            
            _chartPanel._chartX.mostraDialogoErro = false;
        }


        protected void DisparaErroIndicador(string erro, ErroIndicador tipoErro)
        {
            _chartPanel._chartX.DisparaErroIndicador(erro, tipoErro);
        }

        internal event EventHandler DialogClosed = delegate { };
        private void Dialog_OnCancel_GetUserInput(object sender, EventArgs e)
        {
            _showDialog = false;

            Func<bool>[] actions = (Func<bool>[])((IndicatorDialog)sender).Tag;
            if (actions.Length > 1)
                actions[1]();

            DialogClosed(this, EventArgs.Empty);
        }

        private void Dialog_OnOk_GetUserInput(object sender, EventArgs e)
        {
            if (_showDialog)
            {
                _chartPanel._recalc = true;
                _calculated = false;
                _dialogShown = true;
                Calculate();
                _dialogShown = false;
                Paint();
                HideSelection();
            }
            _showDialog = false;

            Func<bool>[] actions = (Func<bool>[])((IndicatorDialog)sender).Tag;
            if (actions.Length > 0)
                actions[0]();

            DialogClosed(this, EventArgs.Empty);
        }


        internal bool HasCircularReference(Indicator indicator)
        {
            int p1;

            for (p1 = 0; p1 < indicator._params.Count; p1++)
            {
                if (indicator._indicatorType == EnumGeral.IndicatorType.Unknown) continue; //usually TwinIndicator, ignore

                if (indicator._params[p1].GetType() != Constants.TypeString) continue;
                string value1 = indicator._params[p1].ToString();
                if (value1.Length == 0) continue;
                Series nextIndicator = _chartPanel._chartX.GetSeriesByName(value1);
                if (nextIndicator == null || nextIndicator == this) continue;
                if (_linkedSeries.Contains(nextIndicator))
                {
                    _params[p1] = "";
                    return true;
                }
                int p2;
                Indicator ind = nextIndicator as Indicator;
                if (ind == null) continue;
                for (p2 = 0; p2 < ind._params.Count; p2++)
                {
                    if (ind._params[p2].GetType() != Constants.TypeString) continue;
                    string value2 = ind._params[p2].ToString();
                    if (!Utils.StrICmp(FullName, value2)) continue;
                    _params[p1] = "";
                    return true;
                }
                return HasCircularReference(ind);
            }
            return false;
        }

        internal bool EnsureField(Field field, string name)
        {
            if (field == null)
            {
                ProcessError("Missing source field " + name + " for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update linked indicators
        /// </summary>
        /// <param name="indicator"></param>
        internal void UpdateIndicator(Indicator indicator)
        {
            foreach (ChartPanel chartPanel in _chartPanel._panelsContainer.Panels)
            {
                foreach (Indicator link in chartPanel.IndicatorsCollection)
                {
                    foreach (object param in link._params)
                    {
                        if (param.GetType() != Constants.TypeString) continue;
                        if (param.ToString() != _name) continue;
                        link.Calculate();
                        link.UpdateIndicator(link);
                    }
                }
            }
        }

        internal void SetMAComboSel(ComboBox comboBox, EnumGeral.IndicatorType paramDef)
        {
            int index = _MATypes.IndexOfValue(paramDef);
            if (index < comboBox.Items.Count)
                comboBox.SelectedIndex = index;
        }

        internal void EnumMATypes(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (string key in _MATypes.Keys)
            {
                comboBox.Items.Add(key);
            }
            comboBox.SelectedIndex = 0;
        }

        internal EnumGeral.IndicatorType GetMAType(ComboBox comboBox)
        {
            Debug.Assert(comboBox.Items.Count > 0);
            return _MATypes.Values[comboBox.SelectedIndex];
        }

        internal string MATypeToStr(EnumGeral.IndicatorType maType)
        {
            int index = _MATypes.IndexOfValue(maType);
            Debug.Assert(index < _MATypes.Count);
            return _MATypes.Keys[index];
        }

        internal static void SetComboDefault(ComboBox comboBox, string item)
        {
            int index = -1;
            int i = 0;
            foreach (var o in comboBox.Items)
            {
                if (o.ToString() == item)
                {
                    index = i;
                    break;
                }
                i++;
            }
            if (index != -1)
                comboBox.SelectedIndex = index;
        }

        internal void EnumSeries(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (ChartPanel chartPanel in _chartPanel._panelsContainer.Panels)
            {
                foreach (Series series in chartPanel.AllSeriesCollection)
                {
                    if (!Utils.StrICmp(FullName, series.FullName) &&
                      series.RecordCount > 0)
                    {
                        comboBox.Items.Add(series.FullName.ToUpper());
                    }
                }
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        internal Indicator EnsureSeries(string name)
        {
            Indicator series = (Indicator)_chartPanel._chartX.GetSeriesByName(name);

            if (series == null)
            {
                series = new IndicadorSerieFilha(name, _chartPanel)
                           {
                               _seriesType = _seriesType,
                               _indicatorParent = this,
                               _selectable = true,
                           };
                _chartPanel.AddSeries(series);
                _linkedSeries.Add(series);
            }
            DM.ClearValues(series._seriesIndex);
            return series;
        }

        internal void EnumSymbols(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            Dictionary<string, bool> symbols = new Dictionary<string, bool>();
            foreach (ChartPanel chartPanel in _chartPanel._panelsContainer.Panels)
            {
                foreach (Series series in chartPanel.SeriesCollection)
                {
                    if (symbols.ContainsKey(series.Name)) continue;
                    comboBox.Items.Add(series.Name.ToUpper());
                    symbols[series.Name] = true;
                }
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        internal string GetParamDescription(int index)
        {
            switch (_parameters[index].ParameterType)
            {
                case EnumGeral.TipoParametroIndicador.Ativo:
                    return "A Symbol is a group of high, low and close series that are displayed as a candle or bar chart";
                case EnumGeral.TipoParametroIndicador.Serie:
                    return "Calculations are based upon the source field. A source field can be the open, high, low, close, volume or any other available series";
                case EnumGeral.TipoParametroIndicador.Volume:
                    return "This indicator requires a volume field for calculation";
                case EnumGeral.TipoParametroIndicador.Serie1:
                    return "Calculations are based upon the source field. A source field can be the open, high, low, close, volume or any other available series";
                case EnumGeral.TipoParametroIndicador.Serie2:
                    return "The second source field. A source field can be the open, high, low, close, volume or any other available series";
                case EnumGeral.TipoParametroIndicador.Serie3:
                    return "The third source field. A source field can be the open, high, low, close, volume or any other available series";
                case EnumGeral.TipoParametroIndicador.Ciclo1:
                    return "The first cycle for the multi-step indicator calculations";
                case EnumGeral.TipoParametroIndicador.Ciclo2:
                    return "The second cycle for the multi-step indicator calculations";
                case EnumGeral.TipoParametroIndicador.Ciclo3:
                    return "The third cycle for the multi-step indicator calculations";
                case EnumGeral.TipoParametroIndicador.LongoPrazo:
                    return "The long term smoothing parameter";
                case EnumGeral.TipoParametroIndicador.CurtoPrazo:
                    return "The short term smoothing parameter";
                case EnumGeral.TipoParametroIndicador.CicloLongo:
                    return "The long cycle smoothing parameter";
                case EnumGeral.TipoParametroIndicador.CicloCurto:
                    return "The short cycle smoothing parameter";
                case EnumGeral.TipoParametroIndicador.Levels:
                    return "The level of smoothing periods to use in this calculation";
                case EnumGeral.TipoParametroIndicador.Periodos:
                    return "The number of bars to use for calculating the indicator";
                case EnumGeral.TipoParametroIndicador.TaxaVariacao:
                    return "Rate of change is expressed as momentum / close(t-n) * 100";
                case EnumGeral.TipoParametroIndicador.PercentKRetardo:
                    return "Controls smoothing of %K, where 1 is a fast stochastic and 3 is a slow stochastic";
                case EnumGeral.TipoParametroIndicador.PercentKPeriodos:
                    return "Number of bars used in the stochastic calculation";
                case EnumGeral.TipoParametroIndicador.PercentKSuave:
                    return "Number of bars used in the stochastic smoothing";
                case EnumGeral.TipoParametroIndicador.PercentDSuave:
                    return "Number of bars used in the stochastic double smoothing";
                case EnumGeral.TipoParametroIndicador.PercentDDoubleSuave:
                    return "Controls the smoothing of %D";
                case EnumGeral.TipoParametroIndicador.PercentKDoubleSuave:
                    return "Controls the smoothing of %K";
                case EnumGeral.TipoParametroIndicador.PercentDPeriodos:
                    return "Number of bars used for calculating the average of %D";
                case EnumGeral.TipoParametroIndicador.DesvioPadrao:
                    return "A statistic used as a measure of the dispersion or variation in a distribution";
                case EnumGeral.TipoParametroIndicador.ValorMinimoTick:
                    return "The dollar value of the move of the smallest tick";
                case EnumGeral.TipoParametroIndicador.AFMinimo:
                    return "Minimum acceleration factor";
                case EnumGeral.TipoParametroIndicador.AFMaximo:
                    return "Maximum acceleration factor";
                case EnumGeral.TipoParametroIndicador.Shift:
                    return "The percent of shift to move a series above or below another indicator";
                case EnumGeral.TipoParametroIndicador.PontosOuPercent:
                    return "Determines the indicator output scale in points or percent";
                case EnumGeral.TipoParametroIndicador.TipoMediaMovel:
                    return "The moving average type used for smoothing the indicator";
                case EnumGeral.TipoParametroIndicador.PercentDTipoMediaMovel:
                    return "The %D moving average type used for smoothing the indicator";
                case EnumGeral.TipoParametroIndicador.EscalaR2:
                    return "The r-squared (coefficient of determination) scale";
                case EnumGeral.TipoParametroIndicador.PeriodosSinal:
                    return "The number of bars used for the MACD signal series";
                case EnumGeral.TipoParametroIndicador.ValorMovelLimite:
                    return "The point value of a limit move (futures only)";
                case EnumGeral.TipoParametroIndicador.BarraHistorica:
                    return "The number of bars to use in the historical calculation (e.g. 365)";
                case EnumGeral.TipoParametroIndicador.Fator:
                    return "Keltner Factor";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Returns parameter name by parameter type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Parameter Name</returns>
        public static string GetParamName(EnumGeral.TipoParametroIndicador type)
        {
            switch (type)
            {
                case EnumGeral.TipoParametroIndicador.TipoMediaMovel:
                    return "MA Type";
                case EnumGeral.TipoParametroIndicador.PercentDTipoMediaMovel:
                    return "%D MA Type";
                case EnumGeral.TipoParametroIndicador.Ativo:
                    return "Symbol";
                case EnumGeral.TipoParametroIndicador.Volume:
                    return "Volume";
                case EnumGeral.TipoParametroIndicador.Serie:
                    return "Source";
                case EnumGeral.TipoParametroIndicador.Serie1:
                    return "Source 1";
                case EnumGeral.TipoParametroIndicador.Serie2:
                    return "Source 2";
                case EnumGeral.TipoParametroIndicador.Serie3:
                    return "Source 3";
                case EnumGeral.TipoParametroIndicador.PontosOuPercent:
                    return "Pontos or Percentual";
                case EnumGeral.TipoParametroIndicador.Levels:
                    return "Levels";
                case EnumGeral.TipoParametroIndicador.Periodos:
                    return "Periods";
                case EnumGeral.TipoParametroIndicador.Ciclo1:
                    return "Cycle 1";
                case EnumGeral.TipoParametroIndicador.Ciclo2:
                    return "Cycle 2";
                case EnumGeral.TipoParametroIndicador.Ciclo3:
                    return "Cycle 3";
                case EnumGeral.TipoParametroIndicador.CurtoPrazo:
                    return "Short Term";
                case EnumGeral.TipoParametroIndicador.LongoPrazo:
                    return "Long Term";
                case EnumGeral.TipoParametroIndicador.TaxaVariacao:
                    return "Rate of Chg";
                case EnumGeral.TipoParametroIndicador.PercentKPeriodos:
                    return "%K Periods";
                case EnumGeral.TipoParametroIndicador.PercentKRetardo:
                    return "%K Slowing";
                case EnumGeral.TipoParametroIndicador.PercentKSuave:
                    return "%K Smooth";
                case EnumGeral.TipoParametroIndicador.PercentDSuave:
                    return "%D Smooth";
                case EnumGeral.TipoParametroIndicador.PercentDDoubleSuave:
                    return "%D Dbl Smooth";
                case EnumGeral.TipoParametroIndicador.PercentKDoubleSuave:
                    return "%K Dbl Smooth";
                case EnumGeral.TipoParametroIndicador.PercentDPeriodos:
                    return "%D Periods";
                case EnumGeral.TipoParametroIndicador.CicloCurto:
                    return "Short Cycle";
                case EnumGeral.TipoParametroIndicador.CicloLongo:
                    return "Long Cycle";
                case EnumGeral.TipoParametroIndicador.DesvioPadrao:
                    return "Standard Dev";
                case EnumGeral.TipoParametroIndicador.EscalaR2:
                    return "R2 Scale";
                case EnumGeral.TipoParametroIndicador.ValorMinimoTick:
                    return "Minimum Tick Value";
                case EnumGeral.TipoParametroIndicador.AFMinimo:
                    return "Min AF";
                case EnumGeral.TipoParametroIndicador.AFMaximo:
                    return "Max AF";
                case EnumGeral.TipoParametroIndicador.Shift:
                    return "Shift";
                case EnumGeral.TipoParametroIndicador.Fator:
                    return "Factor";
                case EnumGeral.TipoParametroIndicador.PeriodosSinal:
                    return "Sinal Periods";
                case EnumGeral.TipoParametroIndicador.ValorMovelLimite:
                    return "Limit Move Value";
                case EnumGeral.TipoParametroIndicador.BarraHistorica:
                    return "Bar History";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the indicator type
        /// </summary>
        public EnumGeral.IndicatorType IndicatorType
        {
            get { return _indicatorType; }
        }

        internal Field SeriesToField(string name, string seriesName, int length)
        {
            Series series = _chartPanel._chartX.GetSeriesByName(seriesName);

            if (series == null)
            {
                if (!UserParams)
                    throw new IndicatorException("Invalid source field " + name + " for indicator " + _name, this);
                return null;
            }

            // Ensure the indicator has been calculated
            Indicator indicator = series as Indicator;
            if (indicator != null && !_calculating)
            {
                _calculating = true;
                if (!indicator._calculated)
                    indicator.Calculate();
                _calculating = false;
            }

            double firstVal = 0; // Get first value that isn't a null

            for (int i = 0; i < series.RecordCount; i++)
                if (series[i].Value.HasValue && series[i].Value.Value != 0)
                {
                    firstVal = series[i].Value.Value;
                    break;
                }

            Field ret = new Field(length, name);

            for (int i = 0; i < length; i++)
            {
                if (!series[i].Value.HasValue)
                    ret.Value(i + 1, firstVal);
                else
                    ret.Value(i + 1, series[i].Value);
            }

            return ret;
        }

        internal void SetUserInput()
        {
            ComboBox cmb;
            TextBox txt;

            //assume we have no input error
            _inputError = false;

            for (int i = 0; i < _parameters.Count; i++)
            {
                switch (_parameters[i].ParameterType)
                {
                    case EnumGeral.TipoParametroIndicador.Ativo:
                    case EnumGeral.TipoParametroIndicador.Serie:
                    case EnumGeral.TipoParametroIndicador.Serie1:
                    case EnumGeral.TipoParametroIndicador.Serie2:
                    case EnumGeral.TipoParametroIndicador.Serie3:
                    case EnumGeral.TipoParametroIndicador.Volume:
                        cmb = _dialog.GetComboBox(i);
                        Debug.Assert(cmb != null);
                        Debug.Assert(cmb.SelectedItem != null);
                        _params[i] = cmb.SelectedItem.ToString();
                        break;
                    case EnumGeral.TipoParametroIndicador.PontosOuPercent:
                        cmb = _dialog.GetComboBox(i);
                        Debug.Assert(cmb != null);
                        Debug.Assert(cmb.SelectedItem != null);
                        _params[i] = Utils.StrICmp(cmb.SelectedItem.ToString(), "Pontos") ? 1 : 2;
                        break;
                    case EnumGeral.TipoParametroIndicador.TipoMediaMovel:
                    case EnumGeral.TipoParametroIndicador.PercentDTipoMediaMovel:
                        cmb = _dialog.GetComboBox(i);
                        Debug.Assert(cmb != null);
                        Debug.Assert(cmb.SelectedItem != null);
                        _params[i] = GetMAType(cmb);
                        break;
                    case EnumGeral.TipoParametroIndicador.BarraHistorica:
                    case EnumGeral.TipoParametroIndicador.Periodos:
                    case EnumGeral.TipoParametroIndicador.Levels:
                    case EnumGeral.TipoParametroIndicador.Ciclo1:
                    case EnumGeral.TipoParametroIndicador.Ciclo2:
                    case EnumGeral.TipoParametroIndicador.Ciclo3:
                    case EnumGeral.TipoParametroIndicador.CurtoPrazo:
                    case EnumGeral.TipoParametroIndicador.LongoPrazo:
                    case EnumGeral.TipoParametroIndicador.PercentKPeriodos:
                    case EnumGeral.TipoParametroIndicador.PercentDPeriodos:
                    case EnumGeral.TipoParametroIndicador.PercentKRetardo:
                    case EnumGeral.TipoParametroIndicador.PercentKSuave:
                    case EnumGeral.TipoParametroIndicador.PercentDSuave:
                    case EnumGeral.TipoParametroIndicador.PercentDDoubleSuave:
                    case EnumGeral.TipoParametroIndicador.PercentKDoubleSuave:
                    case EnumGeral.TipoParametroIndicador.CicloCurto:
                    case EnumGeral.TipoParametroIndicador.CicloLongo:
                    case EnumGeral.TipoParametroIndicador.DesvioPadrao:
                    case EnumGeral.TipoParametroIndicador.TaxaVariacao:
                    case EnumGeral.TipoParametroIndicador.PeriodosSinal:
                        txt = _dialog.GetTextBox(i);
                        Debug.Assert(txt != null);
                        _params[i] = Convert.ToInt32(txt.Text);
                        break;
                    case EnumGeral.TipoParametroIndicador.ValorMinimoTick:
                    case EnumGeral.TipoParametroIndicador.EscalaR2:
                    case EnumGeral.TipoParametroIndicador.AFMinimo:
                    case EnumGeral.TipoParametroIndicador.AFMaximo:
                    case EnumGeral.TipoParametroIndicador.Shift:
                    case EnumGeral.TipoParametroIndicador.Fator:
                    case EnumGeral.TipoParametroIndicador.ValorMovelLimite:
                        txt = _dialog.GetTextBox(i);
                        Debug.Assert(txt != null);
                        _params[i] = Convert.ToDouble(txt.Text);
                        break;
                }
            }
            _chartPanel._chartX.InvalidateIndicators();

            _dialogShown = true;
            _chartPanel._chartX.locked = true;

            //Calculate();
            TrueAction();

            if (!_inputError)
            {
                _chartPanel._chartX.locked = false;
                _chartPanel._chartX.InvalidateIndicators();
                _calculated = true;
            }

            //_inputError = false;
            _dialogShown = false;

        }

        internal void OnCancelDialog()
        {
            if (!_calculated && RecordCount == 0)
                ProcessError("", IndicatorErrorType.RemoveIndicator);
        }

        internal override void SetStrokeThickness()
        {
            _lines.Do(line => line.StrokeThickness = _strokeThickness);
        }

        internal override void SetStrokeColor()
        {
            SolidColorBrush brush = new SolidColorBrush(_strokeColor);
            _lines.Do(line => line.Stroke = brush);
        }

        internal override void SetStrokeType()
        {
            _lines.Do(line => Types.SetLinePattern(line._line, _strokePattern));
        }

        internal override void SetOpacity()
        {
            _lines.Do(line => line._line.Opacity = _opacity);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush strokeBrush)
        {
            if (x1 != x2 && Math.Abs(x2 - x1) < 1) return;

            PaintObjects.Line linePO = _lines.GetPaintObject();
            Line line = linePO._line;

            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.Stroke = strokeBrush;
            line.StrokeThickness = _strokeThickness;
            if (_indicatorType == EnumGeral.IndicatorType.SARParabólico)
            {
                line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeEndLineCap = PenLineCap.Round;
                line.StrokeDashCap = PenLineCap.Round;
            }
            Types.SetLinePattern(line, _strokePattern);
        }

        internal override void MoveToPanel(ChartPanel chartPanel)
        {
            _lines.RemoveAll();
            _chartPanel.DeleteSeries(this);
            _chartPanel = chartPanel;
            _chartPanel.AddSeries(this);

            base.MoveToPanel(chartPanel);
        }

        internal override void ShowHide()
        {
            _lines.Do(line =>
                        {
                            line._line.Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
                        });
        }

        internal void Clear()
        {
            DM.ClearValues(_seriesIndex);
        }

        internal void AppendValue(DateTime timeStamp, double? value)
        {
            DM.AppendValue(_seriesIndex, timeStamp, value);
        }

        #region Implementation of IChartElementPropertyAble


        private ChartElementColorProperty propertyStroke;
        private ChartElementStrokeThicknessProperty propertyStrokeThickness;
        private ChartElementStrokeTypeProperty propertyStrokeType;
        private ChartElementOpacityProperty propertyOpacity;

        ///<summary>
        ///</summary>
        public IEnumerable<IChartElementProperty> Properties
        {
            get
            {
                propertyStroke = new ChartElementColorProperty("Stroke Color");
                propertyStroke.ValuePresenter.Value = new SolidColorBrush(_strokeColor);
                propertyStroke.SetChartElementPropertyValue
                  += presenter =>
                       {
                           StrokeColor = ((SolidColorBrush)presenter.Value).Color;
                       };
                yield return propertyStroke;

                propertyStrokeThickness = new ChartElementStrokeThicknessProperty("Stroke Thickness");
                propertyStrokeThickness.ValuePresenter.Value = StrokeThickness;
                propertyStrokeThickness.SetChartElementPropertyValue
                  += presenter =>
                       {
                           StrokeThickness = Convert.ToDouble(presenter.Value);
                       };
                yield return propertyStrokeThickness;

                propertyStrokeType = new ChartElementStrokeTypeProperty("Stroke Type");
                propertyStrokeType.ValuePresenter.Value = _strokePattern.ToString();
                propertyStrokeType.SetChartElementPropertyValue
                  += presenter =>
                       {
                           _strokePattern = (EnumGeral.TipoLinha)System.Enum.Parse(typeof(EnumGeral.TipoLinha), presenter.Value.ToString()
#if SILVERLIGHT
, true
#endif
);
                           SetStrokeType();
                       };
                yield return propertyStrokeType;

                propertyOpacity = new ChartElementOpacityProperty("Opacity");
                propertyOpacity.ValuePresenter.Value = _opacity;
                propertyOpacity.SetChartElementPropertyValue
                  += presenter =>
                       {
                           _opacity = Convert.ToDouble(presenter.Value);
                           SetOpacity();
                       };
                yield return propertyOpacity;
            }
        }

        #endregion
    }
}
