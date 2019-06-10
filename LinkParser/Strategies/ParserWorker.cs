using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using LinkParser.Core;
using LinkParser.FileWriter;

namespace LinkParser.Strategies
{
    /// <summary>
    /// Parser actions wrapping class
    /// </summary>
    public class ParserWorker
    {
        /// <summary>
        /// Specific parser according to parser settings
        /// </summary>
        public Parser Parser { get; set; }

        private readonly IResultWriter _fileResultWriter;

        public ParserWorker(Parser strategy, IResultWriter resultWriter)
        {
            Parser = strategy;
            _fileResultWriter = resultWriter;
        }

        /// <summary>
        /// Set current parser
        /// </summary>
        /// <param name="parser">Parser</param>
        public void SetParser(Parser parser)
        {
            this.Parser = parser;
        }

        /// <summary>
        /// Start pages parse process and write links to the file
        /// </summary>
        public async Task Start()
        {
            if (this.Parser == null)
            {
                Console.WriteLine("Parser is not chosen!");
                return;
            }

            await Parser.Start();

            List<string> links = Parser.GetLinks();
            await _fileResultWriter.WriteAsync(links);

            return;
        }
    }

}

