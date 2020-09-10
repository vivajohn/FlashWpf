using FlashCommon;
using Google.Cloud.Firestore;
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
    /// Displays a prompt-response pair in a Material Card
    /// </summary>
    public partial class PromptsCard : UserControl
    {
        private ReplaySubject<PromptResponsePair> bs;
        //private FirebaseService fs = new FirebaseService();

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

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (bs == null)
            {
                // Note: Rx.net throttle seems to act like debounce in RxJS
                bs = new ReplaySubject<PromptResponsePair>(0);
                bs.Throttle(new TimeSpan(0, 0, 1)).Subscribe(pair => {
                    Dispatcher.Invoke(() =>
                    {
                        var fs = new FirebaseService();
                        fs.SavePromptPair(pair).Subscribe();
                    });
                });
            }
            bs.OnNext(Pair);
        }
    }

}
