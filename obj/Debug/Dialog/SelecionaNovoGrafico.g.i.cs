﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\GRAFICOWEB\Dialog\SelecionaNovoGrafico.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4B5A9CB20AACA7740C291C7909393F12"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace TDGraficoSL {
    
    
    public partial class GraficoLoadDialog : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.TextBox txtAtivo;
        
        internal System.Windows.Controls.Button btnPesquisaAtivo;
        
        internal System.Windows.Controls.ComboBox cmbPeriodo;
        
        internal System.Windows.Controls.ComboBox cmbPeriodicidade;
        
        internal System.Windows.Controls.Button OKButton;
        
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.GRAFICOWEB;component/Dialog/SelecionaNovoGrafico.xaml", System.UriKind.Relative));
            this.txtAtivo = ((System.Windows.Controls.TextBox)(this.FindName("txtAtivo")));
            this.btnPesquisaAtivo = ((System.Windows.Controls.Button)(this.FindName("btnPesquisaAtivo")));
            this.cmbPeriodo = ((System.Windows.Controls.ComboBox)(this.FindName("cmbPeriodo")));
            this.cmbPeriodicidade = ((System.Windows.Controls.ComboBox)(this.FindName("cmbPeriodicidade")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}
