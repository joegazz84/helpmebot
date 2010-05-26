﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    class Uptime:GenericCommand
    {
        public Uptime( )
        {

        }

        protected override CommandResponseHandler execute( User source , string channel , string[ ] args )
        {
            Logger.Instance( ).addToLog( "Method:" + System.Reflection.MethodInfo.GetCurrentMethod( ).DeclaringType.Name + System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Logger.LogTypes.DNWB );

            string[ ] messageParams = { Helpmebot6.startupTime.DayOfWeek.ToString( ) , Helpmebot6.startupTime.ToLongDateString( ) , Helpmebot6.startupTime.ToLongTimeString( ) };
            string message = Configuration.Singleton( ).GetMessage( "cmdUptimeUpSince" , messageParams );
            return new CommandResponseHandler( message );

        }
    }
}