using Newtonsoft.Json;
using SyncPlayer;
using SyncPlayer.Models;
using SyncPlayer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace SignalRChatClient.WorkerForms
{
    /// <summary>
    /// Логика взаимодействия для ConnectToRoom.xaml
    /// </summary>
    public partial class ConnectToRoom : Window
    {
        #region Private Fields

        private List<Media> Playlist;
        private FolderBrowserDialog DirectoryDialog;

        #endregion Private Fields

        #region Private Methods

        private void ChooseDirectoryBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DirectoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Playlist.Clear();
                    string[] formats = { "*.mp3","*.wav", "*.ogg",
                                         "*.avi","*.flv", "*.mkv", "*.mp4" };
                    var mediaService = new MediaService();
                    foreach (string format in formats)
                    {
                        foreach (var filePath in Directory.GetFiles(DirectoryDialog.SelectedPath, format, SearchOption.AllDirectories))
                        {
                            Playlist.Add(mediaService.GetMedia(filePath));
                        }
                    }
                    FolderPathTB.Text = DirectoryDialog.SelectedPath;

                    var text = JsonConvert.SerializeObject(Playlist);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Sync Player");
            }
        }

        private void ConnectBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.Count != 0)
            {
                if (RoomNameTB.Text.Length > 5 && RoomPasswordTB.Password.Length > 5)
                {
                    var room = new Room { UniqName = RoomNameTB.Text, Name = RoomNameTB.Text, Password = RoomPasswordTB.Password };
                    if (room.ConntectToRoom())
                    {
                        PlayerForm playerForm = new PlayerForm(room, Playlist, false);
                        playerForm.Show();
                        this.Close();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Something went wrong....", "Sync Player");
                    }
                }
                else
                {
                    RoomNameTB.Clear();
                    RoomPasswordTB.Clear();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You did not select playlist folder, or folder does not contain files with available formats", "Sync Player");
            }
        }

        #endregion Private Methods

        private void CreateRoomBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.Count != 0)
            {
                if (RoomNameTB.Text.Length > 5 && RoomPasswordTB.Password.Length > 5)
                {
                    var room = new Room
                    {
                        UniqName = RoomNameTB.Text,
                        Name = RoomNameTB.Text,
                        Password = RoomPasswordTB.Password,
                        Medias = Playlist
                    };
                    if (room.CreateRoom())
                    {
                        PlayerForm playerForm = new PlayerForm(room, Playlist, true);
                        playerForm.Show();
                        this.Close();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Something went wrong....", "Sync Player");
                    }
                }
                else
                {
                    RoomNameTB.Clear();
                    RoomPasswordTB.Clear();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You did not select playlist folder, or folder does not contain files with available formats", "Sync Player");
            }
        }

        public ConnectToRoom()
        {
            InitializeComponent();
            DirectoryDialog = new FolderBrowserDialog();
            Playlist = new List<Media>();
        }
    }
}