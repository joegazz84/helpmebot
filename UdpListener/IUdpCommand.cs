using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helpmebot6.UdpListener
{
    /// <summary>
    /// UDP command interface
    /// </summary>
    interface IUdpCommand
    {
        /// <summary>
        /// Runs the UDP command
        /// </summary>
        /// <param name="message">message parameters</param>
        void execute(string message);
    }
}
