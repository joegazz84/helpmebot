﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    /// <summary>
    /// Returns the maximum replication lag on the wiki
    /// </summary>
    class Maxlag : GenericCommand
    {
        public Maxlag( )
        {

        }

        protected override CommandResponseHandler execute( User source , string channel , string[ ] args )
        {


            string[ ] messageParameters = { source.Nickname , getMaxLag( channel) };
            string message = Configuration.Singleton( ).GetMessage( "cmdMaxLag" , messageParameters );
            return new CommandResponseHandler( message );
        }

        public string getMaxLag(string channel )
        {
            // look up site id
            string baseWiki = Configuration.Singleton( ).retrieveLocalStringOption( "baseWiki", channel );
            // get api

            DAL.Select q = new DAL.Select( "site_api" );
            q.setFrom( "site" );
            q.addWhere( new DAL.WhereConds( "site_id", baseWiki ) );
            string api = DAL.Singleton( ).executeScalarSelect( q );

            System.Xml.XmlTextReader mlreader = new System.Xml.XmlTextReader( HttpRequest.get( api+ "?action=query&meta=siteinfo&siprop=dbrepllag&format=xml" ));
            do
            {
                mlreader.Read( );
            } while( mlreader.Name != "db" );

            string lag = mlreader.GetAttribute( "lag" );

            return lag;

        }
    }
}
