using FlashCommon;
using System;
using System.Windows;

namespace FlashWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Code for setting up the database
            //var dbug = new AzureAdmin();
            //dbug.Init().Subscribe(_ => Dispatcher.Invoke(() => {
            //    Main.NavigationService.Navigate(new Uri("Playback.xaml", UriKind.Relative));
            //}));

            Main.NavigationService.Navigate(new Uri("Playback.xaml", UriKind.Relative));
        }
    }
}
