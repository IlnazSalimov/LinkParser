using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using LinkParser.Core;

namespace LinkParser.Strategies
{
    public abstract class Parser
    {
        protected readonly ParsingSettings Settings;
        protected readonly HttpClient Client;

        public List<HtmlPageInfo> Pages { get; set; } = new List<HtmlPageInfo>();

        public event Action<object, HtmlPageInfo> OnNewPage;
        public event Action<object> OnCompleted;

        protected Parser(ParsingSettings settings)
        {
            Settings = settings;
            Client = new HttpClient();
        }

        protected virtual async Task<IHtmlDocument> Parse(HttpContent httpContent)
        {
            string html = await httpContent.ReadAsStringAsync();
            return await new HtmlParser().ParseDocumentAsync(html);
        }

        protected virtual async Task<HttpResponseMessage> GetResponseAsync(string url)
        {
            var response = await Client.GetAsync(url);

            if (response != null &&
                response.StatusCode == HttpStatusCode.OK &&
                response.Content.Headers.ContentType.MediaType == "text/html")
            {
                return response;
            }

            return null;
        }

        protected async Task<HtmlPageInfo> GetPageInfo(string url)
        {
            try
            {
                HttpResponseMessage response = await GetResponseAsync(url);
                if (response == null)
                {
                    return new HtmlPageInfo(url);
                }

                IHtmlDocument document = await Parse(response.Content);

                return new HtmlPageInfo(response, url, document);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }

            return new HtmlPageInfo(url);
        }

        public async Task Start()
        {
            HtmlPageInfo rootPage = await GetPageInfo(Settings.Url.AbsoluteUri);
            Pages.Add(rootPage);

            await FillChildPages(rootPage);
            OnCompleted?.Invoke(this);
        }

        protected async Task FillChildPages(HtmlPageInfo parrentPage)
        {
            Pages.AddRange(GetUniqLinks(parrentPage).Select(p => GetPageInfo(p).Result));

            foreach (string link in GetUniqLinks(parrentPage))
            {
                if (string.IsNullOrEmpty(link))
                {
                    continue;
                }

                HtmlPageInfo childPage = await GetPageInfo(link);
                OnNewPage?.Invoke(this, childPage);
                await FillChildPages(childPage);
            }
        }

        protected virtual List<string> GetUniqLinks(HtmlPageInfo page)
        {
            if (page.Document == null)
            {
                return new List<string>();
            }

            return page.Document.All
                // Select only the tag <a> (link)
                .Where(tag => tag.LocalName == "a")
                // Project the value of the href attribute (link)
                .Select(tag => ToAbsoluteUrl(tag.GetAttribute("href")))
                // Select only links of this domain (including relative links)
                .Where(href => !string.IsNullOrEmpty(href) && (href.StartsWith(Settings.UrlSchemeAndHost) || href.StartsWith("/")))
                // Select links that have not been added before and remove the repetition
                .Where(href => !Pages.Select(p => p.Url).Contains(href)).Distinct().ToList();
        }

        public abstract List<string> GetLinks();

        /// <summary>
        /// Change relative URl to absolute
        /// </summary>
        protected string ToAbsoluteUrl(string url)
        {
            if (!string.IsNullOrEmpty(url) && url.StartsWith("/"))
            {
                return Settings.UrlSchemeAndHost + url;
            }
            return url;
        }
    }
}
