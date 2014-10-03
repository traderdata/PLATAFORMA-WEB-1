using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Traderdata.Client.Componente.GraficoSL.Util
{
    public partial class ColorDialog : ChildWindow
    {
		#region Campos e Construtores

        // current HSL Value
        private double[] _hsl = { 0, 0, 0.5 };

        // New HSL Value
        private double[] _tempHsl = { 0, 0, 0.5 };

        // Check the Mouse Down
        private bool _isBoardMouseDown = false;
        private bool _isGradientMouseDown = false;
		
		private Color cor;


        public ColorDialog(Color cor)
        {
            InitializeComponent();
			
			this.cor = cor;

            OldColor.Background = new SolidColorBrush(cor);
            NewColor.Background = new SolidColorBrush(cor);

            // Allow to use The RootVisual Object after Loaded
            Loaded += new RoutedEventHandler(ColorPicker_Loaded);
        }
		
		#endregion Campos e Construtores

		#region Propriedades
		
		/// <summary>
		/// Cor selecionada no diálogo.
		/// </summary>
		public Color Cor
		{
			get {return cor;}
		}
		
		/// <summary>
		/// Cor selecionada no diálogo.
		/// </summary>
		public Brush CorBrush
		{
			get {return new SolidColorBrush(cor);}
		}
		
		#endregion Propriedades

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            cor = ((SolidColorBrush)NewColor.Background).Color;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
		
		#region Eventos


		
		#endregion Eventos

        #region Eventos PickerColor

        // Add Handlers
        void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            ColorBoard.MouseLeftButtonDown += new MouseButtonEventHandler(Picker_MouseLeftButtonDown);
            Gradient.MouseLeftButtonDown += new MouseButtonEventHandler(Gradient_MouseLeftButtonDown);

            // update color
            //showColor(_hsl, _tempHsl);
        }



        /////////////////////////////////////////////////////        
        // Handlers 
        /////////////////////////////////////////////////////	

        // Calculate the New Color Value
        void RootVisual_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isBoardMouseDown)  // if change the hue and saturation
            {
                double offsetWidth = ColorBoard.ActualWidth;
                double offsetHeight = ColorBoard.ActualHeight;

                double mouseX = e.GetPosition(ColorBoard).X;
                double mouseY = e.GetPosition(ColorBoard).Y;

                // calculate new hue value
                double hue = mouseX / offsetWidth;
                hue = Math.Min(1, Math.Max(0, hue));

                // calculate new saturation value
                double saturation = 1 - mouseY / offsetHeight;
                saturation = Math.Min(1, Math.Max(0, saturation));

                // update the picker position
                Picker.SetValue(Canvas.LeftProperty, hue * offsetWidth);
                Picker.SetValue(Canvas.TopProperty, (1 - saturation) * offsetHeight);

                _tempHsl[0] = hue * 359;
                _tempHsl[1] = saturation;

                showColor(_hsl, _tempHsl);
            }
            else if (_isGradientMouseDown) // if change the Light
            {
                double offsetHeight = Gradient.ActualHeight;
                double mouseY = e.GetPosition(Gradient).Y;

                // calculate the new light value
                double light = 1 - mouseY / offsetHeight;
                light = Math.Min(1, Math.Max(0, light));

                // update the picker position
                LightPicker.SetValue(Canvas.TopProperty, (1 - light) * (offsetHeight - LightPicker.ActualHeight));

                _tempHsl[2] = light;
                showColor(_hsl, _tempHsl);
            }
        }

        // Update the Color 
        void Picker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isBoardMouseDown = false;
            _isGradientMouseDown = false;

            _tempHsl.CopyTo(_hsl, 0);
            showColor(_hsl, _hsl);
        }

        // When Mouse Down on the Color Board
        void Picker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isBoardMouseDown = true;
            RootVisual_MouseMove(sender, e);
        }

        // When Mouse Down on the Gradient Box
        void Gradient_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isGradientMouseDown = true;
            RootVisual_MouseMove(sender, e);
        }


        // Show the Color Differnt
        private void showColor(double[] hsl1, double[] hsl2)
        {
            double[] rgb1 = hslToRgb(hsl1[0], hsl1[1], hsl1[2]);
            double[] rgb2 = hslToRgb(hsl2[0], hsl2[1], hsl2[2]);
            OldColor.Background = new SolidColorBrush(Color.FromArgb(255, (byte)rgb1[0], (byte)rgb1[1], (byte)rgb1[2]));
            NewColor.Background = new SolidColorBrush(Color.FromArgb(255, (byte)rgb2[0], (byte)rgb2[1], (byte)rgb2[2]));

            double[] rgb3 = hslToRgb(hsl2[0], hsl2[1], 0.5);
            GradientBox.Background = new SolidColorBrush(Color.FromArgb(255, (byte)rgb3[0], (byte)rgb3[1], (byte)rgb3[2]));
        }

        // Convert HSL to RGB
        private double[] hslToRgb(double H, double S, double L)
        {
            double[] rgb = new double[3];
            double p1 = 0;
            double p2 = 0;

            if (L <= 0.5)
            {
                p2 = L * (1 + S);
            }
            else
            {
                p2 = L + S - (L * S);
            }
            p1 = 2 * L - p2;
            if (S == 0)
            {
                rgb[0] = L;
                rgb[1] = L;
                rgb[2] = L;
            }
            else
            {
                rgb[0] = toRgb(p1, p2, H + 120);
                rgb[1] = toRgb(p1, p2, H);
                rgb[2] = toRgb(p1, p2, H - 120);
            }
            rgb[0] *= 255;
            rgb[1] *= 255;
            rgb[2] *= 255;
            return rgb;
        }

        // Calculate the RGB Value
        private double toRgb(double q1, double q2, double hue)
        {
            if (hue > 360)
            {
                hue = hue - 360;
            }
            if (hue < 0)
            {
                hue = hue + 360;
            }
            if (hue < 60)
            {
                return (q1 + (q2 - q1) * hue / 60);
            }
            else if (hue < 180)
            {
                return (q2);
            }
            else if (hue < 240)
            {
                return (q1 + (q2 - q1) * (240 - hue) / 60);
            }
            else
            {
                return (q1);
            }
        }

        #endregion Eventos PickerColor

    }
}

