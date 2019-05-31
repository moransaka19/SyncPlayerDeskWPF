using SignalRChatClient.WorkerForms;
using SyncPlayer.Helpers;
using SyncPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SignalRChatClient.SignForms
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
        {
            InitializeComponent();

        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTB.Text.Length >= 5)
            {
                string email = LoginTB.Text;
                string password = PasswordTB.Password;
                //httpPost validation
                if (SessionHelper.SetActiveUserSession(new ApplicationUser { UserName = email, Password = password }))
                {
                    ConnectToRoom connectToRoomForm = new ConnectToRoom();
                    connectToRoomForm.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid login or password", "Sync Player");
                }
            }
            else
            {
                LoginTB.Focus();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
            this.Close();
        }
    }
}
