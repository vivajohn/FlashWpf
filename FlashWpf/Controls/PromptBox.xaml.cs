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
    /// Displays a prompt or response and its associated buttons
    /// </summary>
    public partial class PromptBox : UserControl
    {
        public PromptBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public Prompt Prompt
        {
            get { return (Prompt)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        public static readonly DependencyProperty PromptProperty
            = DependencyProperty.Register(
                  "Prompt",
                  typeof(Prompt),
                  typeof(PromptBox)
              );

        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty HintProperty
            = DependencyProperty.Register(
                  "Hint",
                  typeof(string),
                  typeof(PromptBox)
              );

    }
}
