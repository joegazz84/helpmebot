using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helpmebot6.UdpListener.Command
{
    /// <summary>
    /// Shutdown command via UDP
    /// </summary>
    class Shutdown : IUdpCommand
    {
        /// <summary>
        /// Runs the UDP command
        /// </summary>
        /// <param name="message">message parameters</param>
        public void execute(string message)
        {
            Helpmebot6.irc.ircQuit(message);
            System.Threading.Thread.Sleep(5000);
            Helpmebot6.stop();
        }
    }
}
