using System;
using System.Collections.Generic;
using System.Text;
using LinkParser.Core;

namespace LinkParser.Strategies.Factory
{
    public interface IParserCreator
    {
        Parser GetParser(ParsingSettings settings);
    }
}
