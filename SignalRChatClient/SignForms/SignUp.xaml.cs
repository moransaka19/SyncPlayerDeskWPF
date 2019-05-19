using SignalRChatClient.WorkerForms;
using SyncPlayer.Helpers;
using SyncPlayer.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace SignalRChatClient.SignForms
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void RegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            if (EmailTB.Text.Length >= 5 &&
                LoginTB.Text.Length >= 5 &&
                PasswordTB.Password.Length >= 5 &&
                RepeatPasswordTB.Password.Length >= 5)
            {
                if (PasswordTB.Password == RepeatPasswordTB.Password)
                {
                    string email = EmailTB.Text;
                    string password = PasswordTB.Password;
                    string username = LoginTB.Text;
                    var user = new ApplicationUser() { Email = email, Username = username, Password = password };
                    if (new HttpHelper().Request(user, new AppSettingsReader().GetValue("ServerHost", typeof(string)) + "api/Auth/Register"))
                    {
                        SessionHelper.SetActiveUserSession(user);
                        ConnectToRoom connectToRoomForm = new ConnectToRoom();
                        connectToRoomForm.Show();
                        this.Close();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    RepeatPasswordTB.Password = string.Empty;
                    RepeatPasswordTB.Focus();
                }
            }
            else
            {
                EmailTB.Focus();
            }
        }

        private bool ValidateForm()
        {
            return EmailTB.Text.Contains("@")
                   && EmailTB.Text.Split('@').Last().Contains(".");
        }
    }
}