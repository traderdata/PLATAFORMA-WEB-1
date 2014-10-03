﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
#endif
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    ///<summary>
    ///</summary>
    [TemplatePart(Name = PART_Thumb, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Track, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_LeftTrackButton, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_RightTrackButton, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ThumbButton, Type = typeof(FrameworkElement))]
    public partial class ChartScroller : Control
    {
        /// <summary>Delaget para duplo clique no scroll</summary>
        public delegate void DuploCliqueScrollDelegate();
        /// <summary>Evento de duplo clique no scroll.</summary>
        public event DuploCliqueScrollDelegate DuploCliqueScroll;
        

        internal const string PART_Thumb = "PART_Thumb";
        internal const string PART_Track = "PART_Track";
        internal const string PART_LeftTrackButton = "PART_LeftTrackButton";
        internal const string PART_ThumbButton = "PART_ThumbButton";
        internal const string PART_RightTrackButton = "PART_RightTrackButton";

        private const int GripSize = 5;

        private FrameworkElement _thumb;
        private Panel _leftTrackButton;
        private Panel _rightTrackButton;
        private Panel _thumbButton;
        private Canvas _track;
        private MouseDownPositionType _mouseDownPositionType = MouseDownPositionType.None;
        private int _startX;
        private bool _reCalcValues;

        ///<summary>
        ///</summary>
        ///<param name="sender"></param>
        ///<param name="newLeft"></param>
        ///<param name="newRight"></param>
        ///<param name="cancel"></param>
        public delegate void OnPositionChangedHandler(object sender, int newLeft, int newRight, ref bool cancel);
        /// <summary>
        /// 
        /// </summary>
        public event OnPositionChangedHandler OnPositionChanged;

        private enum MouseDownPositionType
        {
            Left,
            Right,
            All,
            None
        }

#if WPF
    static ChartScroller()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartScroller), new FrameworkPropertyMetadata(typeof(ChartScroller)));
    }
#endif
        ///<summary>
        ///</summary>
        public ChartScroller()
        {
            _reCalcValues = true;
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartScroller);
#endif
        }

        #region Dependency Properties

        #region MinValueProperty (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                if (value >= MaxValue)
                    throw new ArgumentException("MinValue can't be greater or equal then MaxValue");
                SetValue(MinValueProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
          DependencyProperty.Register("MinValue", typeof(int), typeof(ChartScroller),
                                      new PropertyMetadata(-100, OnMinValueChanged));

        private static void OnMinValueChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnMinValueChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMinValueChanged(DependencyPropertyChangedEventArgs e)
        {
            CalculateThumbPosition();
        }

        #endregion

        #region MaxValueProperty (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                if (MinValue >= value)
                    throw new ArgumentException("MaxValue can't be less or equal then MinValue");
                SetValue(MaxValueProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
          DependencyProperty.Register("MaxValue", typeof(int), typeof(ChartScroller),
                                      new PropertyMetadata(100, OnMaxValueChanged));

        private static void OnMaxValueChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnMaxValueChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMaxValueChanged(DependencyPropertyChangedEventArgs e)
        {
            CalculateThumbPosition();
        }

        #endregion

        #region LeftValueProperty (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public int LeftValue
        {
            get { return (int)GetValue(LeftValueProperty); }
            set { SetValue(LeftValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LeftValueProperty =
          DependencyProperty.Register("LeftValue", typeof(int), typeof(ChartScroller), new PropertyMetadata(-50, OnLeftValueChanged));

        private static void OnLeftValueChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnLeftValueChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLeftValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_mouseDownPositionType != MouseDownPositionType.None || e.NewValue == e.OldValue)
                return;
            _reCalcValues = false;
            CalculateThumbPosition();
            _reCalcValues = true;
        }
        #endregion

        #region RightValueProperty (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public int RightValue
        {
            get { return (int)GetValue(RightValueProperty); }
            set { SetValue(RightValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RightValueProperty =
          DependencyProperty.Register("RightValue", typeof(int), typeof(ChartScroller),
                                      new PropertyMetadata(50, OnRightValueChanged));

        private static void OnRightValueChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnRightValueChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRightValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_mouseDownPositionType != MouseDownPositionType.None || e.NewValue == e.OldValue)
                return;
            _reCalcValues = false;
            CalculateThumbPosition();
            _reCalcValues = true;
        }

        #endregion

        #region TrackBackgroundProperty (DependencyProperty)

        /// <summary>
        /// A description of the TrackBackground.
        /// </summary>
        public Brush TrackBackground
        {
            get { return (Brush)GetValue(TrackBackgroundProperty); }
            set { SetValue(TrackBackgroundProperty, value); }
        }

        /// <summary>
        /// TrackBackground
        /// </summary>
        public static readonly DependencyProperty TrackBackgroundProperty =
          DependencyProperty.Register("TrackBackground", typeof(Brush), typeof(ChartScroller),
                                      new PropertyMetadata(Brushes.Silver, OnTrackBackgroundChanged));

        private static void OnTrackBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnTrackBackgroundChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTrackBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_track != null)
                _track.Background = (Brush)e.NewValue;
        }

        #endregion

        #region TrackButtonsBackgroundProperty (DependencyProperty)

        /// <summary>
        /// A description of the TrackButtonsBackground.
        /// </summary>
        public Brush TrackButtonsBackground
        {
            get { return (Brush)GetValue(TrackButtonsBackgroundProperty); }
            set { SetValue(TrackButtonsBackgroundProperty, value); }
        }

        /// <summary>
        /// TrackButtonsBackground
        /// </summary>
        public static readonly DependencyProperty TrackButtonsBackgroundProperty =
          DependencyProperty.Register("TrackButtonsBackground", typeof(Brush), typeof(ChartScroller),
                                      new PropertyMetadata(Brushes.Green, OnTrackButtonsBackgroundChanged));

        private static void OnTrackButtonsBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnTrackButtonsBackgroundChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTrackButtonsBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_leftTrackButton != null)
                _leftTrackButton.Background = (Brush)e.NewValue;
            if (_rightTrackButton != null)
                _rightTrackButton.Background = (Brush)e.NewValue;
        }

        #endregion

        #region ThumbButtonBackgroundProperty (DependencyProperty)

        /// <summary>
        /// A description of the ThumbButtonBackground.
        /// </summary>
        public Brush ThumbButtonBackground
        {
            get { return (Brush)GetValue(ThumbButtonBackgroundProperty); }
            set { SetValue(ThumbButtonBackgroundProperty, value); }
        }

        /// <summary>
        /// ThumbButtonBackground
        /// </summary>
        public static readonly DependencyProperty ThumbButtonBackgroundProperty =
          DependencyProperty.Register("ThumbButtonBackground", typeof(Brush), typeof(ChartScroller),
                                      new PropertyMetadata(new LinearGradientBrush
                                                             {
                                                                 StartPoint = new Point(0.486, 0),
                                                                 EndPoint = new Point(0.486, 0.986),
                                                                 GradientStops = new GradientStopCollection
                                                                             {
#if SILVERLIGHT
                                                                               new GradientStop
                                                                                 {
                                                                                   Color = ColorsEx.Gray,
                                                                                   Offset = 0
                                                                                 },
                                                                               new GradientStop
                                                                                 {
                                                                                   Color = ColorsEx.MidnightBlue,
                                                                                   Offset = 0.5
                                                                                 },
                                                                               new GradientStop
                                                                                 {
                                                                                   Color = ColorsEx.Gray,
                                                                                   Offset = 1
                                                                                 }
#endif
#if WPF
                                                                               new GradientStop(Colors.Gray, 0),
                                                                               new GradientStop(Colors.MidnightBlue, 0.5),
                                                                               new GradientStop(Colors.Gray, 1)
#endif
                                                                             }
                                                             }, OnThumbButtonBackgroundChanged));

        private static void OnThumbButtonBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScroller)d).OnThumbButtonBackgroundChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbButtonBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_thumbButton != null)
                _thumbButton.Background = (Brush)e.NewValue;
        }

        #endregion

        #endregion

        private double Normalize(double value)
        {
            return (value - MinValue) / (MaxValue - MinValue);
        }

        private int GetX(double value)
        {
            return (int)Math.Round(_track.ActualWidth * (Normalize(value)));
        }

        private void CalculateThumbPosition()
        {
            if (_thumb == null || _frozen)
                return;

            int left = GetX(LeftValue);
            Canvas.SetLeft(_thumb, left);
            int right = GetX(RightValue);
            _thumb.Width = right - left;
        }

        private bool _frozen;
        internal void Freeze()
        {
            _frozen = true;
        }
        internal void Melt()
        {
            _frozen = false;
            CalculateThumbPosition();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _track = (Canvas)GetTemplateChild(PART_Track);
            if (_track == null)
                throw new NullReferenceException("Track part must be present in template.");
            _thumb = (FrameworkElement)GetTemplateChild(PART_Thumb);
            if (_thumb == null)
                throw new NullReferenceException("Thumb part must be present in template.");

            _leftTrackButton = (Panel)GetTemplateChild(PART_LeftTrackButton);
            _thumbButton = (Panel)GetTemplateChild(PART_ThumbButton);
            _rightTrackButton = (Panel)GetTemplateChild(PART_RightTrackButton);

            _track.SizeChanged += RootCanvasOnSizeChanged;

            _track.MouseLeftButtonDown += ThumbOnMouseDown;
            _track.MouseLeftButtonUp += ThumbOnMouseUp;
            _track.MouseMove += ThumbOnMouseMove;
        }

        private MouseDownPositionType GetMousePositionType(double x)
        {
            double thumbLeft = Canvas.GetLeft(_thumb);
            if (x >= thumbLeft && x <= thumbLeft + GripSize)
                return MouseDownPositionType.Left;
            if (x >= thumbLeft + _thumb.ActualWidth - GripSize && x <= thumbLeft + _thumb.ActualWidth)
                return MouseDownPositionType.Right;
            return MouseDownPositionType.All;
        }

        private void ReCalcValues()
        {
            if (!_reCalcValues)
                return;

            double k = Canvas.GetLeft(_thumb) / _track.ActualWidth;
            int leftValue = (int)Math.Round(MinValue + (MaxValue - MinValue) * k);
            if (leftValue < MinValue)
                leftValue = MinValue;

            k = (Canvas.GetLeft(_thumb) + _thumb.Width) / _track.ActualWidth;
            int rightValue = (int)Math.Round(MinValue + (MaxValue - MinValue) * k);
            if (rightValue > MaxValue)
                rightValue = MaxValue;

            LeftValue = leftValue;
            RightValue = rightValue;

            if (OnPositionChanged != null)
            {
                bool cancel = false;
                OnPositionChanged(this, LeftValue, RightValue, ref cancel);
            }
        }

        private void ThumbOnMouseMove(object sender, MouseEventArgs args)
        {
            Point p = args.GetPosition(_track);
            int x = (int)p.X;
            if (x < 0)
                x = 0;
            if (x > _track.ActualWidth)
                x = (int)_track.ActualWidth;

            if (_mouseDownPositionType == MouseDownPositionType.None)
            {
                switch (GetMousePositionType(x))
                {
                    case MouseDownPositionType.Left:
                    case MouseDownPositionType.Right:
                        _thumb.Cursor = Cursors.SizeWE;
                        break;
                    case MouseDownPositionType.All:
                        _thumb.Cursor = Cursors.Hand;
                        break;
                }
                return;
            }

            int newLeft;
            switch (_mouseDownPositionType)
            {
                case MouseDownPositionType.All:
                    newLeft = (int)(Canvas.GetLeft(_thumb) + x - _startX);
                    if (newLeft < 0)
                        newLeft = 0;
                    if (newLeft + (int)_thumb.Width > (int)_track.ActualWidth)
                        newLeft = (int)(_track.ActualWidth - _thumb.Width);
                    if (newLeft + (int)_thumb.Width <= (int)_track.ActualWidth && newLeft >= 0)
                    {
                        Canvas.SetLeft(_thumb, newLeft);
                        _startX = x;

                        ReCalcValues();
                    }
                    break;
                case MouseDownPositionType.Left:
                    newLeft = (int)(Canvas.GetLeft(_thumb) + x - _startX);
                    if (newLeft < 0)
                        newLeft = 0;
                    if (newLeft >= 0 && newLeft < (int)(Canvas.GetLeft(_thumb) + _thumb.ActualWidth - GripSize * 2))
                    {
                        Canvas.SetLeft(_thumb, newLeft);
                        _thumb.Width -= (x - _startX);
                        _startX = x;

                        ReCalcValues();
                    }
                    break;
                case MouseDownPositionType.Right:
                    int left = (int)Canvas.GetLeft(_thumb);
                    int newRight = (int)(left + _thumb.Width + x - _startX);
                    if (newRight > (int)_track.ActualWidth)
                        newRight = (int)_track.ActualWidth;
                    if (newRight <= (int)_track.ActualWidth && newRight >= left + GripSize * 2)
                    {
                        _thumb.Width += (x - _startX);
                        _startX = x;

                        ReCalcValues();
                    }
                    break;
            }
        }

        private void ThumbOnMouseUp(object sender, MouseButtonEventArgs args)
        {
            _track.ReleaseMouseCapture();
            _mouseDownPositionType = MouseDownPositionType.None;
            _track.Cursor = null;
        }

        //Variaveis para duplo clique
        private static DateTime horaClick = new DateTime();
        private static int click = 0;

        /// <summary>
        /// Click na Scroll de Zoom/Avanco e Retrocesso
        /// Este evento tb será usado para obter o duplo clique.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ThumbOnMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (DateTime.Now.Subtract(horaClick).TotalSeconds > 0.3)
                click = 0;

            click++;

            //Resetando a escala com duplo click
            if ((DateTime.Now.Subtract(horaClick).TotalSeconds < 0.3) && (click == 2))
                if (DuploCliqueScroll != null)
                    DuploCliqueScroll();

            _mouseDownPositionType = GetMousePositionType(_startX = (int)args.GetPosition(_track).X);
            switch (_mouseDownPositionType)
            {
                case MouseDownPositionType.Left:
                case MouseDownPositionType.Right:
                    _track.Cursor = Cursors.SizeWE;
                    break;
                case MouseDownPositionType.All:
                    _track.Cursor = Cursors.Hand;
                    break;
            }
            _track.CaptureMouse();

            horaClick = DateTime.Now;

            if (click >= 2)
                click = 0;
        }

        private void RootCanvasOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            _thumb.Height = ActualHeight;

            CalculateThumbPosition();
        }
    }
}
