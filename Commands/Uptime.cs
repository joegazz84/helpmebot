using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    class Uptime:GenericCommand
    {
        protected override CommandResponseHandler execute( User source , string channel , string[ ] args )
        {
            string message = "Up since " + Helpmebot6.startupTime.DayOfWeek.ToString( ) + " " + Helpmebot6.startupTime.ToLongDateString( ) + " " + Helpmebot6.startupTime.ToLongTimeString( );
            return new CommandResponseHandler( message );
        }
    }
}
