namespace SyncPlayer.Models
{
    public class ApplicationUser
    {
        #region Public Properties

        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string AccessToken { get; set; }

        #endregion Public Properties
    }
}