using System;
using System.Net;

namespace DailySocial.Utils
{
    public class CustomWebClient : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        private int Timeout { get; set; }

        public CustomWebClient()
            : this(60000)
        {
        }

        public CustomWebClient(int timeout)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
            }
            return request;
        }
    }
}