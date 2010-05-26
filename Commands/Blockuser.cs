﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    class Blockuser: GenericCommand
    {
        public Blockuser( )
        {

        }

        protected override CommandResponseHandler execute( User source, string channel, string[ ] args )
        {
            Logger.Instance( ).addToLog( "Method:" + System.Reflection.MethodInfo.GetCurrentMethod( ).DeclaringType.Name + System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Logger.LogTypes.DNWB );

            string name = string.Join( " ", args );

            string url = Configuration.Singleton( ).retrieveLocalStringOption( "wikiUrl", channel );

            return new CommandResponseHandler( url + "Special:BlockIP/" + name );
        }
    }
}