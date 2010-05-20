using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace helpmebot6
{
    static class HttpRequest
    {
        public static Stream get( string uri )
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create( uri );
            hwr.UserAgent = "Helpmebot/0 (Backup bot; mailto:helpmebot@helpmebot.org.uk)";
            HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse( );

            return resp.GetResponseStream( );
        }
    }
}
