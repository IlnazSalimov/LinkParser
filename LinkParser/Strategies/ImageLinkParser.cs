using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkParser.Core;

namespace LinkParser.Strategies
{
    public class ImageLinkParser : Parser
    {
        public ImageLinkParser(ParsingSettings settings) : base(settings) { }

        public override List<string> GetLinks()
        {
            return GetImagesSource();
        }

        private List<string> GetImagesSource()
        {
            return Pages.Where(p => p.Document != null).SelectMany(p => p.Document.All)
                // Select only the tag <a> (link)
                .Where(tag => tag.LocalName == "img")
                // Project the value of the href attribute (link)
                .Select(tag => ToAbsoluteUrl(tag.GetAttribute("src")))
                // Select only links of this domain (including relative links)
                .Where(href => !string.IsNullOrEmpty(href) && (href.StartsWith(Settings.UrlSchemeAndHost) || href.StartsWith("/")))
                // Remove the repetition
                .Distinct().ToList();
        }
    }
}
