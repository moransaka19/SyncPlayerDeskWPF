using SignalRChatClient.WorkerForms;
using SyncPlayer.Helpers;
using SyncPlayer.Models;
using System.Windows;
using System.Windows.Input;

namespace SignalRChatClient.SignForms
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        #region Public Constructors

        public SignIn()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        private void Login()
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

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
            this.Close();
        }

        #endregion Private Methods
    }
}