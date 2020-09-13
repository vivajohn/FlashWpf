using FlashCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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


        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
            "TextChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PromptBox));

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var t = sender as TextBox;
            if (t.IsLoaded) {
                Prompt.text = t.Text;
                RaiseEvent(new RoutedEventArgs(TextChangedEvent));
            }
        }

    }
}
