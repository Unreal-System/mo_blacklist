using System;

namespace BlackList.Events
{
    /// <summary>
    /// Describes the features the server supports.
    /// </summary>
    public class SupportsEventArgs : EventArgs
    {
        /// <summary>
        /// The server's supported featureset.
        /// </summary>
        public ServerInfo ServerInfo { get; set; }

        internal SupportsEventArgs(ServerInfo serverInfo)
        {
            ServerInfo = serverInfo;
        }
    }
}
