﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\Principal.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FC49E3CF4E3C85CB0B3B676D6E19F25A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
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
    
    
    public partial class Principal : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Border pnlCarregando;
        
        internal System.Windows.Controls.TextBlock lblCarregando;
        
        internal System.Windows.Controls.Grid stpMenu;
        
        internal C1.Silverlight.C1Menu menu;
        
        internal C1.Silverlight.C1MenuItem mnuConfiguracaoPadrao;
        
        internal C1.Silverlight.C1MenuItem mnuNovoGrafico;
        
        internal C1.Silverlight.C1MenuItem mnuSalvarAT;
        
        internal C1.Silverlight.C1MenuItem mnuSalvarTemplate;
        
        internal C1.Silverlight.C1MenuItem mnuAplicarTemplate;
        
        internal C1.Silverlight.C1MenuItem mnuExcluirTemplate;
        
        internal C1.Silverlight.C1MenuItem mnuFerramentas;
        
        internal C1.Silverlight.C1MenuItem mnuAlertas;
        
        internal C1.Silverlight.C1MenuItem mnuBugReport;
        
        internal C1.Silverlight.C1MenuItem mnuChat;
        
        internal C1.Silverlight.C1MenuItem mnuemailsuporte;
        
        internal C1.Silverlight.C1MenuItem mnuAnaliseCompartilhada;
        
        internal C1.Silverlight.C1MenuItem mnuPublicar;
        
        internal C1.Silverlight.C1MenuItem mnuRegistrarInteresse;
        
        internal C1.Silverlight.C1MenuItem mnuCentralAnalise;
        
        internal C1.Silverlight.C1MenuItem mnuAdmin;
        
        internal C1.Silverlight.C1MenuItem mnuUsuarios;
        
        internal C1.Silverlight.C1MenuItem mnuStatisticas;
        
        internal C1.Silverlight.C1MenuItem mnuAjuda;
        
        internal C1.Silverlight.C1MenuItem mnuManual;
        
        internal C1.Silverlight.C1MenuItem mnuSobre;
        
        internal System.Windows.Controls.TextBlock txtStatus;
        
        internal System.Windows.Controls.Image imgLogoMenu;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Principal.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.pnlCarregando = ((System.Windows.Controls.Border)(this.FindName("pnlCarregando")));
            this.lblCarregando = ((System.Windows.Controls.TextBlock)(this.FindName("lblCarregando")));
            this.stpMenu = ((System.Windows.Controls.Grid)(this.FindName("stpMenu")));
            this.menu = ((C1.Silverlight.C1Menu)(this.FindName("menu")));
            this.mnuConfiguracaoPadrao = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuConfiguracaoPadrao")));
            this.mnuNovoGrafico = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuNovoGrafico")));
            this.mnuSalvarAT = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuSalvarAT")));
            this.mnuSalvarTemplate = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuSalvarTemplate")));
            this.mnuAplicarTemplate = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuAplicarTemplate")));
            this.mnuExcluirTemplate = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuExcluirTemplate")));
            this.mnuFerramentas = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuFerramentas")));
            this.mnuAlertas = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuAlertas")));
            this.mnuBugReport = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuBugReport")));
            this.mnuChat = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuChat")));
            this.mnuemailsuporte = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuemailsuporte")));
            this.mnuAnaliseCompartilhada = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuAnaliseCompartilhada")));
            this.mnuPublicar = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuPublicar")));
            this.mnuRegistrarInteresse = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuRegistrarInteresse")));
            this.mnuCentralAnalise = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuCentralAnalise")));
            this.mnuAdmin = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuAdmin")));
            this.mnuUsuarios = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuUsuarios")));
            this.mnuStatisticas = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuStatisticas")));
            this.mnuAjuda = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuAjuda")));
            this.mnuManual = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuManual")));
            this.mnuSobre = ((C1.Silverlight.C1MenuItem)(this.FindName("mnuSobre")));
            this.txtStatus = ((System.Windows.Controls.TextBlock)(this.FindName("txtStatus")));
            this.imgLogoMenu = ((System.Windows.Controls.Image)(this.FindName("imgLogoMenu")));
            this.tabControl = ((System.Windows.Controls.TabControl)(this.FindName("tabControl")));
            this.bkcImage = ((System.Windows.Controls.Grid)(this.FindName("bkcImage")));
            this.imgLogoTD = ((System.Windows.Controls.Image)(this.FindName("imgLogoTD")));
            this.graficoAvulso = ((System.Windows.Controls.Grid)(this.FindName("graficoAvulso")));
        }
    }
}

