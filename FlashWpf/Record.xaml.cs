using FlashCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FlashWpf
{
    /// <summary>
    /// Interaction logic for Record.xaml
    /// </summary>
    public partial class Record : Page
    {
        // Used for data binding in the xaml code
        private class DeckSource 
        {
            public string name { get; set; }
            public Deck deck { get; set; }

            public ObservableCollection<PromptResponsePair> pairs { get; } = new ObservableCollection<PromptResponsePair>();

            public DeckSource(Deck deck)
            {
                this.deck = deck;
                name = deck.name;
            }
        }

        private FirebaseService fb = new FirebaseService();
        private Dictionary<long, List<PromptResponsePair>> cache = new Dictionary<long, List<PromptResponsePair>>();

        public Record()
        {
            InitializeComponent();

            // Get the top-level data
            fb.GetTopics(Globals.uid).Subscribe(topics =>
            {
                if (topics.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var decks = topics[0].decks;
                        var source = new List<DeckSource>(decks.Count);
                        decks.ForEach(deck => {
                            source.Add(new DeckSource(deck));
                        });
                        DecksList.ItemsSource = source;
                    });
                }
            });
        }

        public void Navigate_Click(object sender, RoutedEventArgs e) 
        {
            NavigationService.Navigate(new Uri("Playback.xaml", UriKind.Relative));
        }

        // The user has clicked on the top-level card
        // If opening, get the data from the database or from the dictionary
        // if it has already been read.
        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            var card = sender as MaterialDesignThemes.Wpf.Card;
            var ds = card.DataContext as DeckSource;

            if (!cache.ContainsKey(ds.deck.id))
            {
                fb.GetPairs(Globals.uid, ds.deck).Subscribe(pairs =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        cache[ds.deck.id] = pairs.groups;
                        pairs.groups.ForEach(pair => ds.pairs.Add(pair));
                    });
                });
                return;
            }

            if (ds.pairs.Count == 0)
            {
                cache[ds.deck.id].ForEach(pair => ds.pairs.Add(pair));
                return;
            }

            ds.pairs.Clear();
        }
    }
}
