﻿#pragma checksum "C:\Backup\Projetos2012\CLIENT\TERMINAL-WEB\Site.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3D13B057DC2F734DAA5FE33832B76D8F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using C1.Silverlight;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Traderdata.Client.TerminalWEB {
    
    
    public partial class Site : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Border pnlCarregando;
        
        internal System.Windows.Controls.TextBlock lblCarregando;
        
        internal System.Windows.Controls.Grid stpSimpletrader;
        
        internal System.Windows.Controls.Button btnSalvar;
        
        internal System.Windows.Controls.CheckBox chkSalvarAutomatico;
        
        internal System.Windows.Controls.Button btnAplicarTemplate;
        
        internal System.Windows.Controls.Button btnExcluirTemplate;
        
        internal System.Windows.Controls.Button btnSalvarTemplate;
        
        internal System.Windows.Controls.Button btnConfiguracao;
        
        internal System.Windows.Controls.Grid stpMenu33;
        
        internal C1.Silverlight.C1Menu menu;
        
        internal System.Windows.Controls.TabControl tabControl;
        
        internal System.Windows.Controls.Grid bkcImage;
        
        internal System.Windows.Controls.Image imgLogoTD;
        
        internal System.Windows.Controls.Grid graficoAvulso;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Site.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.pnlCarregando = ((System.Windows.Controls.Border)(this.FindName("pnlCarregando")));
            this.lblCarregando = ((System.Windows.Controls.TextBlock)(this.FindName("lblCarregando")));
            this.stpSimpletrader = ((System.Windows.Controls.Grid)(this.FindName("stpSimpletrader")));
            this.btnSalvar = ((System.Windows.Controls.Button)(this.FindName("btnSalvar")));
            this.chkSalvarAutomatico = ((System.Windows.Controls.CheckBox)(this.FindName("chkSalvarAutomatico")));
            this.btnAplicarTemplate = ((System.Windows.Controls.Button)(this.FindName("btnAplicarTemplate")));
            this.btnExcluirTemplate = ((System.Windows.Controls.Button)(this.FindName("btnExcluirTemplate")));
            this.btnSalvarTemplate = ((System.Windows.Controls.Button)(this.FindName("btnSalvarTemplate")));
            this.btnConfiguracao = ((System.Windows.Controls.Button)(this.FindName("btnConfiguracao")));
            this.stpMenu33 = ((System.Windows.Controls.Grid)(this.FindName("stpMenu33")));
            this.menu = ((C1.Silverlight.C1Menu)(this.FindName("menu")));
            this.tabControl = ((System.Windows.Controls.TabControl)(this.FindName("tabControl")));
            this.bkcImage = ((System.Windows.Controls.Grid)(this.FindName("bkcImage")));
            this.imgLogoTD = ((System.Windows.Controls.Image)(this.FindName("imgLogoTD")));
            this.graficoAvulso = ((System.Windows.Controls.Grid)(this.FindName("graficoAvulso")));
        }
    }
}

