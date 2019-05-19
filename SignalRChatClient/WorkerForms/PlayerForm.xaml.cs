using Microsoft.AspNetCore.SignalR.Client;
using SignalRChatClient.WorkerForms;
using SyncPlayer.Helpers;
using SyncPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SyncPlayer
{
    public partial class PlayerForm : Window
    {
        private HubConnection _connection;
        private List<string> _roomUsers;
        private bool IsPlaying = false;
        private ConnectToRoom _connectToRoom;
        private List<Media> _playlist;

        public PlayerForm(Room room, IEnumerable<Media> playlist)
        {
            InitializeComponent();
            _roomUsers = new List<string>();
            _playlist = playlist.ToList();

            foreach (var media in _playlist)
            {
                PlayListLB.Items.Add(media.Name);
            }

            this.Title = room.Name;
            this.Closing += PlayerForm_FormClosing;

            mePlayer.Source = new Uri(playlist.FirstOrDefault().FileName);
            mePlayer.Volume = PlayerVolume.Value;
            mePlayer.MediaEnded += MediaEnd;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


            _connection = new HubConnectionBuilder()
                .WithUrl("https://sync4u.azurewebsites.net/room", options =>
                {
                    options.AccessTokenProvider = async () => SessionHelper.ActiveUser.AccessToken;
                })
                .Build();

            #region snippet_ClosedRestart

            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            #endregion snippet_ClosedRestart

            _connection.On<string>("UserDisconect", (username) => { _roomUsers.Remove(username); UpdateUserList(); });
            _connection.On<string>("UserConnect", (username) => { _roomUsers.Add(username); UpdateUserList(); });
            _connection.On("RoomClosed", async () =>
            {
                await _connection.StopAsync();
                if (_connectToRoom == null)
                {
                    _connectToRoom = new ConnectToRoom();
                    _connectToRoom.Show();
                    this.Close();
                }
            });

            _connection.On<string, string>("Receive", AddTextToChat);

            try
            {
                Task.Run(async () => await _connection.StartAsync());
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private void UpdateUserList()
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (var user in _roomUsers)
                {
                    messagesList.Items.Add(user);
                }
            });
        }

        private void AddTextToChat(string userName, string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"{userName}: {message}";
                messagesList.Items.Add(newMessage);
            });
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            #region snippet_ErrorHandling

            try
            {
                #region snippet_InvokeAsync

                await _connection.InvokeAsync("Message",
                     messageTextBox.Text);

                #endregion snippet_InvokeAsync
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }

            #endregion snippet_ErrorHandling
        }

        private async void PlayerForm_FormClosing(object sender, EventArgs e)
        {
            await _connection.StopAsync();
        }

        private async void DisconnectBTN_Click(object sender, RoutedEventArgs e)
        {
            await _connection.StopAsync();
            if (_connectToRoom == null)
            {
                _connectToRoom = new ConnectToRoom();
                _connectToRoom.Show();
                this.Close();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }

        private void PlayerVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            mePlayer.Volume = PlayerVolume.Value;
        }

        private void MediaEnd(object sender, EventArgs e)
        {
            if (_playlist.Remove(_playlist.FirstOrDefault(media => mePlayer.Source.LocalPath == media.FileName)) && _playlist.Count > 0)
            {
                mePlayer.Source = new Uri(_playlist.FirstOrDefault().FileName);
                mePlayer.Play();

                PlayListLB.Items.Clear();
                foreach (var media in _playlist)
                {
                    PlayListLB.Items.Add(media.Name);
                }
            }
        }
    }
}