using System;

namespace SyncPlayer.Models
{
    public class Media
    {
        #region Public Properties

        public string Album { get; set; }
        public double BitRate { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal EndPostiotion { get; set; }
        public string FileName { get; set; }
        public string Genre { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public long PlayListId { get; set; }
        public double Rate { get; set; }
        public string Singler { get; set; }
        public decimal StartPosition { get; set; }
        public string Type { get; set; }

        #endregion Public Properties
    }
}