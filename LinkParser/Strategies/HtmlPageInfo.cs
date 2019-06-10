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
        public HttpResponseMessage Response { get; set; }

        public string Url { get; set; }

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
