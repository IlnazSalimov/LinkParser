using System;
using System.Collections.Generic;
using System.Text;
using LinkParser.Core;

namespace LinkParser.Strategies.Factory
{
    public class ParserCreator : IParserCreator
    {
        /// <summary>
        /// Create specific parser along with settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Parser GetParser(ParsingSettings settings)
        {
            switch (settings.ParserType)
            {
                case "default": return new DefaultParser(settings);
                case "content-length": return new ContentLengthConditionParser(settings);
                case "images": return new ImageLinkParser(settings);
                default: return new DefaultParser(settings);
            }
        }
    }
}
