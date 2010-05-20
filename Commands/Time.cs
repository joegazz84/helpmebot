using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    /// <summary>
    /// Returns the current date/time
    /// </summary>
    class Time : GenericCommand
    {
        public Time( )
        {
 
        }

        protected override CommandResponseHandler execute( User source , string channel , string[ ] args )
        {
            return new CommandResponseHandler( DateTime.Now.ToLongDateString( ) + " " + DateTime.Now.ToLongTimeString( ) );
        }
    }

    /// <summary>
    /// Returns the current date/time. Alias for Time.
    /// </summary>
    class Date : Time
    {

    }
}
