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
        public Parser Parser { get; set; }

        public ParserWorker(Parser strategy)
        {
            this.Parser = strategy;
        }

        public ParserWorker() { }

        /// <summary>
        /// Set current parser
        /// </summary>
        /// <param name="parser">Parser</param>
        public void SetParser(Parser parser)
        {
            this.Parser = parser;
        }

        /// <summary>
        /// Start links parse process
        /// </summary>
        public async Task Start()
        {
            if (this.Parser == null)
            {
                Console.WriteLine("Parser is not chosen!");
                return;
            }

            await Parser.Start();

            IResultWriter fileWriter = new ResultFileWriter();
            await fileWriter.Write(Parser.GetLinks());

            return;
        }
    }

}

