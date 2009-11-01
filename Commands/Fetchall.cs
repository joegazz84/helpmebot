﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    class Fetchall:GenericCommand
    {
        public Fetchall( )
        {
            accessLevel = GlobalFunctions.commandAccessLevel( "fetchall" );
        }

        protected override CommandResponseHandler execute( User source , string[ ] args )
        {
            CommandResponseHandler crh = new CommandResponseHandler();
            Dictionary<string , Monitoring.CategoryWatcher>.KeyCollection kc = Monitoring.WatcherController.Instance( ).getKeywords( );
            foreach( string key in kc )
            {
                crh.respond( Monitoring.WatcherController.Instance( ).forceUpdate( key ) );
            }
            return crh;
        }
    }
}