using FlashCommon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlashWpf
{
    /// <summary>
    /// Displays a prompt-response pair in a Material Card
    /// </summary>
    public partial class PromptsCard : UserControl
    {
        
        public PromptsCard()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public PromptResponsePair Pair
        {
            get { return (PromptResponsePair)GetValue(PairProperty); }
            set { SetValue(PairProperty, value); }
        }

        public static readonly DependencyProperty PairProperty
            = DependencyProperty.Register(
                  "Pair",
                  typeof(PromptResponsePair),
                  typeof(PromptsCard),
                  new PropertyMetadata(default(PromptResponsePair))
              );

    }
}
