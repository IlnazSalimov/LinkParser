using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkParser.Core;

namespace LinkParser.Strategies
{
    /// <summary>
    /// Specific parser with content length condition
    /// </summary>
    public class ContentLengthConditionParser : Parser
    {
        private const int MinContentLength = 200000;

        public ContentLengthConditionParser(ParsingSettings settings) : base(settings) { }

        public override List<string> GetLinks()
        {
            List<string> links = Pages.Where(p => p.Response?.Content?.Headers?.ContentLength >= MinContentLength).Select(p => p.Url).ToList();
            Console.WriteLine($"The number of links larger than {MinContentLength} bytes: {links.Count}");
            return links;
        }
    }
}
