using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using LinkParser.Core;

namespace LinkParser.Strategies
{
    /// <summary>
    /// Default specific parser
    /// </summary>
    public class DefaultParser : Parser
    {
        public DefaultParser(ParsingSettings settings) : base(settings) { }

        public override List<string> GetLinks()
        {
            List<string> links = Pages.Select(p => p.Url).ToList();
            Console.WriteLine($"The number of links: {links.Count}");
            return links;
        }
    }
}
