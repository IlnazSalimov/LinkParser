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
    /// <summary>
    /// Provides base parsing methods
    /// </summary>
    public abstract class Parser
    {
        /// <summary>
        /// Parser configuration
        /// </summary>
        protected readonly ParsingSettings Settings;

        protected readonly HttpClient HttpClient;

        /// <summary>
        /// Temparary collection to provide adding unique pages
        /// </summary>
        private List<string> _tempLinks { get; set; } = new List<string>();

        /// <summary>
        /// Unique pages of site
        /// </summary>
        public List<HtmlPageInfo> Pages { get; set; } = new List<HtmlPageInfo>();

        /// <summary>
        /// Invoked when new page is parsing
        /// </summary>
        public event Action<object, HtmlPageInfo> OnNewPage;

        /// <summary>
        /// Invoked when parse has been completed
        /// </summary>
        public event Action<object> OnCompleted;

        protected Parser(ParsingSettings settings)
        {
            Settings = settings;
            HttpClient = new HttpClient();
        }

        protected virtual async Task<IHtmlDocument> Parse(HttpContent httpContent)
        {
            string html = await httpContent.ReadAsStringAsync();
            return await new HtmlParser().ParseDocumentAsync(html);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Specified Uri</param>
        /// <returns>Http response message if response is successful or null</returns>
        protected virtual async Task<HttpResponseMessage> GetResponseAsync(string url)
        {
            var response = await HttpClient.GetAsync(url);

            if (response != null &&
                response.StatusCode == HttpStatusCode.OK &&
                response.Content.Headers.ContentType.MediaType == "text/html")
            {
                return response;
            }

            return null;
        }

        /// <summary>
        /// Serve url information as HtmlPageInfo object
        /// </summary>
        protected async Task<HtmlPageInfo> GetPageInfo(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }

                HttpResponseMessage response = await GetResponseAsync(url);

                // Returning page with url even If we did not get response
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
                // Returning page with url even If we get http exception
                return new HtmlPageInfo(url);
            }
        }

        /// <summary>
        /// Start parse process
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            HtmlPageInfo rootPage = await GetPageInfo(Settings?.Url?.AbsoluteUri);
            if(rootPage == null)
            {
                OnCompleted?.Invoke(this);
                return;
            }

            _tempLinks.Add(rootPage.Url);
            Pages.Add(rootPage);

            await FillChildPages(rootPage);
            OnCompleted?.Invoke(this);
        }

        /// <summary>
        /// Recursive parsing of link tree
        /// </summary>
        /// <param name="parrentPage"></param>
        /// <returns></returns>
        protected async Task FillChildPages(HtmlPageInfo parrentPage)
        {
            List<string> links = GetUniqueLinks(parrentPage);
            _tempLinks.AddRange(links);

            foreach (string link in links)
            {
                HtmlPageInfo childPage = await GetPageInfo(link);
                if (childPage == null)
                {
                    continue;
                }

                Pages.Add(childPage);
                OnNewPage?.Invoke(this, childPage);
                await FillChildPages(childPage);
            }
        }

        /// <summary>
        /// Getting all unique links on the page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        protected List<string> GetUniqueLinks(HtmlPageInfo page)
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
                .Where(href => !string.IsNullOrEmpty(href) && (href.StartsWith(Settings?.UrlSchemeAndHost) || href.StartsWith("/")))
                // Select links that have not been added before and remove the repetition
                .Where(href => !_tempLinks.Contains(href)).Distinct().ToList();
        }

        /// <summary>
        /// Getting all links of the site
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetLinks();

        /// <summary>
        /// Change relative URl to absolute
        /// </summary>
        protected string ToAbsoluteUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            if (url.StartsWith("//"))
            {
                return $"{Settings?.Url?.Scheme}: + {url}";
            }

            if (url.StartsWith("/"))
            {
                return Settings?.UrlSchemeAndHost + url;
            }

            return url;
        }
    }
}
