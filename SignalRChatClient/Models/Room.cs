using SignalRChatClient.Models;
using SyncPlayer.Helpers;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SyncPlayer.Models
{
    public class Room
    {
        #region Public Constructors

        public Room()
        {
            UsersIn = new List<ApplicationUser>();
            Medias = Enumerable.Empty<Media>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<Media> Medias { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string UniqName { get; set; }
        public ICollection<ApplicationUser> UsersIn { get; set; }
        public string PlaylistPath { get; set; }

        #endregion Public Properties

        #region Public Methods

        public bool ConntectToRoom()
        {
            return new HttpHelper().Request(this, new AppSettingsReader().GetValue("ServerHost", typeof(string)) + "api/Rooms/SignIn", SessionHelper.ActiveUser.AccessToken);
        }

        public bool CreateRoom()
        {
            return new HttpHelper().Request(this, new AppSettingsReader().GetValue("ServerHost", typeof(string)) + "api/Rooms", SessionHelper.ActiveUser.AccessToken);
        }

        public RoomUserList GetUsersInRoom()
        {
            return new HttpHelper().Request<RoomUserList, Room>(null, new AppSettingsReader().GetValue("ServerHost", typeof(string)) + $"api/Rooms/{this.UniqName}", SessionHelper.ActiveUser.AccessToken, "GET");
        }

        #endregion Public Methods
    }
}