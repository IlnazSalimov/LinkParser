using System;
using System.Collections.Generic;
using System.Text;

namespace LinkParser.Core
{
    /// <summary>
    /// Class representing parser configuration
    /// </summary>
    public class ParsingSettings
    {
        public ParsingSettings(string uri, string strategy)
        {
            try
            {
                Url = new Uri(uri);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("URI string object is a null reference: {0}", e);
            }
            catch (UriFormatException e)
            {
                Console.WriteLine("URI formatting error: {0}", e);
            }

            ParserType = strategy;
        }

        /// <summary>
        /// Entry address of the site
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Domain of the site along with the protocol
        /// </summary>
        public string UrlSchemeAndHost => $"{Url?.Scheme}://{Url?.Host}";

        /// <summary>
        /// HTML parsing strategy
        /// </summary>
        public string ParserType { get; set; }
    }
}
