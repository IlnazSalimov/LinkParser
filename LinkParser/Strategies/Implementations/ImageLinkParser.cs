using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkParser.Core;

namespace LinkParser.Strategies
{
    /// <summary>
    /// Specific parser to parse img tag as a link
    /// </summary>
    public class ImageLinkParser : Parser
    {
        public ImageLinkParser(ParsingSettings settings) : base(settings) { }

        public override List<string> GetLinks()
        {
            List<string> images = GetUniqImagesSources();
            Console.WriteLine($"The number of images: {images.Count}");
            return images;
        }

        /// <summary>
        /// Get uniq all image links
        /// </summary>
        /// <returns></returns>
        private List<string> GetUniqImagesSources()
        {
            return Pages.Where(p => p.Document != null).SelectMany(p => p.Document.All)
                // Select only the tag <img> (link)
                .Where(tag => tag.LocalName == "img")
                // Project the value of the src attribute (link)
                .Select(tag => ToAbsoluteUrl(tag.GetAttribute("src")))
                // Select only links of this domain (including relative links)
                .Where(href => !string.IsNullOrEmpty(href) && (href.StartsWith(Settings.UrlSchemeAndHost) || href.StartsWith("/")))
                // Remove the repetition
                .Distinct().ToList();
        }
    }
}
