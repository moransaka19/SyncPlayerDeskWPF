using SyncPlayer.Models;
using System;
using System.Configuration;

namespace SyncPlayer.Helpers
{
    public static class SessionHelper
    {
        #region Public Properties

        public static ApplicationUser ActiveUser { get; private set; }
        public static DateTime SessionStart { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public static bool SetActiveUserSession(ApplicationUser user)
        {
            if (user != null)
            {
                ActiveUser = new HttpHelper().Request<ApplicationUser, ApplicationUser>(user, new AppSettingsReader().GetValue("ServerHost", typeof(string)) + "/api/Auth/Login");
                ActiveUser.Password = user.Password;
                SessionStart = DateTime.Now;
            }

            return ActiveUser != null;
        }

        public static void EndSession()
        {
            ActiveUser = null;
            SessionStart = default(DateTime);
        }

        #endregion Public Methods
    }
}