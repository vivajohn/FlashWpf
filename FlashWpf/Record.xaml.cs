using FlashCommon;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FlashWpf
{
    /// <summary>
    /// Page for creating and recording prompts and responses
    /// </summary>
    public partial class Record : Page
    {
        // Used for data binding in the ListView in the xaml code
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

        private Dictionary<long, IList<PromptResponsePair>> cache;
        private IDatabase db;
        private string uid;
        private Topic currentTopic;
        public ObservableCollection<string> DBName { get; } = new ObservableCollection<string>();

        public Record()
        {
            InitializeComponent();
            DataContext = this;

            var ddb = ServiceLocator.GetInstance<IDynamicDB>();
            ddb.CurrentDB.SelectMany(db =>
            {
                this.db = db;
                return db.CurrentUserId;
            }).Subscribe(uid => {
                // Get the top-level data
                this.uid = uid;
                cache = new Dictionary<long, IList<PromptResponsePair>>();
                DBName.Clear();
                DBName.Add(db.Name.ToString());
                db.GetTopics(uid).Subscribe(topics =>
                {
                    if (topics.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            // TODO: Assuming current topic is first in list
                            currentTopic = topics[0];
                            var decks = currentTopic.decks;
                            var source = new List<DeckSource>(decks.Count);
                            decks.ForEach(deck => {
                                source.Add(new DeckSource(deck));
                            });
                            DecksList.ItemsSource = source;
                        });
                    }
                });
            });

        }

        public void Navigate_Click(object sender, RoutedEventArgs e) 
        {
            NavigationService.Navigate(new Uri("Playback.xaml", UriKind.Relative));
        }

        // The user has clicked on the top-level card
        // If opening, get the data from the database or from the dictionary
        // if it has already been read.
        private void OnCardOpen(object sender, RoutedEventArgs e)
        {
            var card = sender as Expander;
            var ds = card.DataContext as DeckSource;

            if (!cache.ContainsKey(ds.deck.id))
            {
                db.GetPairs(uid, ds.deck).Subscribe(pairs =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        cache[ds.deck.id] = pairs.groups;
                        foreach (var pair in pairs.groups) ds.pairs.Add(pair);
                    });
                });
                return;
            }

            if (ds.pairs.Count == 0)
            {
                foreach (var pair in cache[ds.deck.id]) ds.pairs.Add(pair);
                return;
            }
        }

        // Delete a prompt-reponse pair. The confirmation question has already been answered.
        private void OnDelete(object sender, RoutedEventArgs e)
        {
            var pair = (sender as PromptsCard).Pair;
            var list = DecksList.ItemsSource as List<DeckSource>;
            var source = list.Find(s => s.deck.id == pair.deckId);
            var deck = source.deck;
            deck.groups.Remove(pair);
            deck.numPairs -= 1;

            source.pairs.Remove(pair);
            db.DeletePair(pair).Subscribe();
            db.SaveTopic(currentTopic);
        }

        // Add a new prompt-reponse pair.
        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            var ds = (sender as Button).DataContext as DeckSource;
            var pair = currentTopic.CreatePromptResponsePair(ds.deck);
            ds.pairs.Insert(0, pair);
            db.SaveTopic(currentTopic).Subscribe();
            db.SavePromptPair(pair).Subscribe();
        }

        // Toggle the database between Azure and Firebase
        private void OnToggleDB(object sender, RoutedEventArgs e)
        {
            DBChange.To(db.Name == DBNames.Azure ? DBNames.Firebase : DBNames.Azure);
        }
    }
}
