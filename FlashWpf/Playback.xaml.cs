﻿using FlashCommon;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FlashWpf
{
    /// <summary>
    /// Plays pack the prompt and response recordings
    /// </summary>
    public partial class Playback : Page
    {

        // Private class for managing data for the DataContext
        private class PageData : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            // Create the OnPropertyChanged method to raise the event
            // The calling member's name will be used as the parameter.
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            private string _labelText;
            public string labelText
            {
                get { return _labelText; }
                set
                {
                    _labelText = value;
                    OnPropertyChanged();
                }
            }

            private bool _IsNotPlaying = true;
            public bool IsNotPlaying
            {
                get { return _IsNotPlaying; }
                set
                {
                    _IsNotPlaying = value;
                    OnPropertyChanged();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dbug"));
                }
            }

        }


        private PlaybackMgr mgr;
        private MediaPlayer[] players = new MediaPlayer[2];
        private PageData pageData = new PageData();

        public Playback()
        {
            InitializeComponent();
            DataContext = pageData;
            mgr = new PlaybackMgr(Globals.uid);
            mgr.Recordings.Subscribe(recs =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetRecordings(recs);
                });
            });
        }

        public void Navigate_Click(object sender, RoutedEventArgs e)
        {
            Stop(0);
            Stop(1);
            NavigationService.Navigate(new Uri("Record.xaml", UriKind.Relative));
        }

        private void SetRecordings(FirestoreBlob[] recs)
        {
            SetRecording(0, recs[0]);
            SetRecording(1, recs[1]);
            pageData.labelText = mgr.CurrentPair.prompts[0].text;
        }

        private void SetRecording(int i, FirestoreBlob blob)
        {
            Stop(i);

            byte[] ba = blob.data.ToByteArray();

            var tempPath = Path.GetTempPath();
            var mediaPath = $"{tempPath}wpfflash{i}.mp3";
            var f = File.Create(mediaPath);
            f.Write(ba, 0, ba.Length);
            f.Close();

            players[i] = new MediaPlayer();
            players[i].Open(new Uri(mediaPath));
        }

        private void Prompt_Click(object sender, RoutedEventArgs e)
        {
            Play(0);
        }

        private void Response_Click(object sender, RoutedEventArgs e)
        {
            Play(1);
        }

        private void ThumbsUp_Click(object sender, RoutedEventArgs e)
        {
            NextPrompts(true);
        }

        private void ThumbsDown_Click(object sender, RoutedEventArgs e)
        {
            NextPrompts(false);
        }

        public void Play(int i)
        {
            if (players[i] != null)
            {
                pageData.IsNotPlaying = false;
                players[i].Position = TimeSpan.FromSeconds(0);
                players[i].Play();
                players[i].MediaEnded += (s, e) => pageData.IsNotPlaying = true;
            }
        }

        private void NextPrompts(Boolean success)
        {
            Stop(0);
            Stop(1);
            mgr.NextCard(success);
        }

        private void Stop(int i)
        {
            if (players[i] != null)
            {
                var file = players[i].Source.LocalPath;
                players[i].Stop();
                players[i] = null;
                File.Delete(file);
            }
        }
    }
}
