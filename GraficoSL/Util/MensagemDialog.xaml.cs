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
using System.Windows.Media.Imaging;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.Util
{
    public partial class MensagemDialog : ChildWindow
    {


        public MensagemDialog(string msg)
        {
            InitializeComponent();

           lblMsg.Text = msg;

            /*		switch(icone)
                    {
                        case Icones.Informacao:
                            Uri uri1 = new Uri("img\\Info.png", UriKind.Relative);
                            ImageSource imsSource1 = new BitmapImage(uri1);
                            imgIcone.Source = imsSource1;
                            break;
					
                        case Icones.Erro:
                            Uri uri2 = new Uri("img\\Erro.png", UriKind.Relative);
                            ImageSource imsSource2 = new BitmapImage(uri2);
                            imgIcone.Source = imsSource2;
                            break;
					
                        case Icones.Atencao:
                            Uri uri3 =new Uri("img\\Atencao.png", UriKind.Relative);
                            ImageSource imsSource3 = new BitmapImage(uri3);
                            imgIcone.Source = imsSource3;	
					
                            break;
					
                        case Icones.Interrogacao:
                            Uri uri4 =new Uri("img\\Interrogacao.png", UriKind.Relative);
                            ImageSource imsSource4 = new BitmapImage(uri4);
                            imgIcone.Source = imsSource4;
                            OKButton.Content=(string)"Sim";
                            CancelButton.Opacity=0;
                            break;
					
                        case Icones.Sucesso:
                            Uri uri5 =new Uri("img\\Sucesso.png", UriKind.Relative);
                            ImageSource imsSource5 = new BitmapImage(uri5);
                            imgIcone.Source = imsSource5;					
                            break;
                    }*/

        }




        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = false;
        }

    }
}

