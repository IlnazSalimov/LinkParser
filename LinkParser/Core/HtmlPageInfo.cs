using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AngleSharp.Html.Dom;

namespace LinkParser.Strategies
{
    /// <summary>
    /// Class representing web page
    /// </summary>
    public class HtmlPageInfo
    {
        /// <summary>
        /// Represents a HTTP response message 
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Entry link of site to start parsing
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Parsed document of this page
        /// </summary>
        public IHtmlDocument Document { get; set; }

        public HtmlPageInfo() { }

        public HtmlPageInfo(HttpResponseMessage response, string url, IHtmlDocument document)
        {
            Response = response;
            Url = url;
            Document = document;
        }

        public HtmlPageInfo(string url)
        {
            Url = url;
        }
    }
}
