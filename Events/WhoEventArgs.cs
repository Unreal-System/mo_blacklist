﻿using System;

namespace BlackList.Events
{
    /// <summary>
    /// Describes the response to a WHO (WHOX protocol) query. Note that CnCNET may generate WHO
    /// queries that the user did not ask for.
    /// </summary>
    public class WhoxReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The WHOIS response from the server.
        /// </summary>
        public ExtendedWho[] WhoxResponse { get; set; }

        internal WhoxReceivedEventArgs(ExtendedWho[] whoxResponse)
        {
            WhoxResponse = whoxResponse;
        }
    }
}
