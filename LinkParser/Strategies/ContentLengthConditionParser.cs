using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkParser.Core;

namespace LinkParser.Strategies
{
    public class ContentLengthConditionParser : Parser
    {
        private const int MinContentLength = 200000;

        public ContentLengthConditionParser(ParsingSettings settings) : base(settings) { }

        public override List<string> GetLinks()
        {
            return Pages.Where(p => p.Response?.Content?.Headers?.ContentLength >= MinContentLength).Select(p => p.Url).ToList();
        }
    }
}
