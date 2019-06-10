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
    public class DefaultParser : Parser
    {
        public DefaultParser(ParsingSettings settings) : base(settings) { }

        public override List<string> GetLinks()
        {
            return Pages.Select(p => p.Url).ToList();
        }
    }
}
