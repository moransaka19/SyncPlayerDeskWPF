using SyncPlayer.Models;

namespace SignalRChatClient.Models
{
    public class RoomUserList
    {
        #region Public Properties

        public string Name { get; set; }
        public ApplicationUser[] Users { get; set; }

        #endregion Public Properties
    }
}