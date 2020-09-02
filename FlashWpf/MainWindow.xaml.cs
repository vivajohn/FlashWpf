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
            Main.NavigationService.Navigate(new Uri("Playback.xaml", UriKind.Relative));
        }
    }
}
