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
        private IDatabase fs;

        public PromptsCard()
        {
            InitializeComponent();
            fs = ServiceLocator.GetInstance<IDatabase>();
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
                        fs.SavePromptPair(pair).Subscribe();
                    });
                });
            }
            bs.OnNext(Pair);
        }


        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            Dialog.CurrentSession.Close();
            RaiseEvent(new RoutedEventArgs(DeleteEvent));
        }

        public static readonly RoutedEvent DeleteEvent = EventManager.RegisterRoutedEvent(
            "Delete",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PromptsCard));

        public event RoutedEventHandler Delete
        {
            add { AddHandler(DeleteEvent, value); }
            remove { RemoveHandler(DeleteEvent, value); }
        }


    }

}
