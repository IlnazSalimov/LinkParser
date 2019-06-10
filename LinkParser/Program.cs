using System;
using System.Collections.Generic;
using LinkParser.Core;
using LinkParser.Strategies;
using LinkParser.Strategies.Factory;

namespace LinkParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string entryUrl = args.Length >= 1 ? args[0] : String.Empty;
            string parserType = args.Length >= 2 ? args[1] : String.Empty;

            if (string.IsNullOrEmpty(entryUrl))
            {
                Console.WriteLine("Entry URL was not pass");
                return;
            }

            ParsingSettings settings = new ParsingSettings(entryUrl, parserType);

            Parser parser = new ParserCreator().GetParser(settings);
            parser.OnNewPage += OnNewPage;
            parser.OnCompleted += OnCompleted;

            try
            {
                ParserWorker parserWorker = new ParserWorker(parser);
                parserWorker.Start().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        private static void OnNewPage(object e, HtmlPageInfo page)
        {
            Console.Clear();
            Console.WriteLine($"Founded links count: {((Parser)e).Pages.Count}");
            Console.WriteLine($"Current page: {page.Url}");
            Console.SetCursorPosition(0, 0);
        }

        private static void OnCompleted(object e)
        {
            Console.Clear();
            Console.WriteLine($"Founded links count: {((Parser)e).Pages.Count}");
            Console.WriteLine("Parse completed");
        }
    }
}
