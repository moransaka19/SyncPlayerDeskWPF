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

        private readonly string[] _formats = new string[] { "*.mp3", "*.wav", "*.ogg", "*.avi", "*.flv", "*.mkv", "*.mp4" };
        private FolderBrowserDialog DirectoryDialog;
        private List<Media> Playlist;

        #endregion Private Fields

        #region Private Methods

        private void ChooseDirectoryBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DirectoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FolderPathTB.Text = DirectoryDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Sync Player");
            }
        }

        private void ConnectBTN_Click(object sender, RoutedEventArgs e)
        {
            FillPlayList();
            if (!string.IsNullOrEmpty(FolderPathTB.Text) &&
                File.Exists(FolderPathTB.Text))
            {
                if (RoomNameTB.Text.Length > 5 && RoomPasswordTB.Password.Length > 5)
                {
                    var room = new Room { UniqName = RoomNameTB.Text, Name = RoomNameTB.Text, Password = RoomPasswordTB.Password, Medias = Playlist };
                    if (room.ConntectToRoom())
                    {
                        room.Medias = Playlist;
                        room.PlaylistPath = FolderPathTB.Text;
                        PlayerForm playerForm = new PlayerForm(room, false);
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
                System.Windows.MessageBox.Show("You did not select playlist folder, or folder does not exist", "Filmst");
            }
        }

        private void FillPlayList()
        {
            try
            {
                Playlist.Clear();
                var mediaService = new MediaService();
                foreach (string format in _formats)
                {
                    foreach (var filePath in Directory.GetFiles(DirectoryDialog.SelectedPath, format, SearchOption.AllDirectories))
                    {
                        Playlist.Add(mediaService.GetMedia(filePath));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Filmst");
            }
        }

        #endregion Private Methods

        #region Public Constructors

        public ConnectToRoom()
        {
            InitializeComponent();
            DirectoryDialog = new FolderBrowserDialog();
            Playlist = new List<Media>();
        }

        #endregion Public Constructors

        private void CreateRoomBTN_Click(object sender, RoutedEventArgs e)
        {
            FillPlayList();
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
                        room.Medias = Playlist;
                        room.PlaylistPath = FolderPathTB.Text;
                        PlayerForm playerForm = new PlayerForm(room, true);
                        playerForm.Show();
                        this.Close();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Something went wrong....", "Filmst");
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
                System.Windows.MessageBox.Show("You did not select playlist folder, or folder does not contain files with available formats", "Filmst");
            }
        }
    }
}