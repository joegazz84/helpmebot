using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helpmebot6.UdpListener.Command
{
    /// <summary>
    /// Represents a standard UDP message
    /// </summary>
    internal class Message : IUdpCommand
    {
        /// <summary>
        /// Runs the UDP command
        /// </summary>
        /// <param name="message">message parameters</param>
        void IUdpCommand.execute(string message)
        {
            Helpmebot6.irc.sendRawLine("PRIVMSG " + message);
        }
    }
}
