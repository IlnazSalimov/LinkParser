using System;
using System.Collections.Generic;
using LinkParser.Core;
using LinkParser.FileWriter;
using LinkParser.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace LinkParser
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            string entryUrl = args.Length >= 1 ? args[0] : String.Empty;
            string parserType = args.Length >= 2 ? args[1] : String.Empty;

            if (string.IsNullOrEmpty(entryUrl))
            {
                Console.WriteLine("Entry URL was not pass");
                Console.ReadLine();
                return;
            }

            ParsingSettings settings = new ParsingSettings(entryUrl, parserType);
            RegisterServices(settings);

            ParserWorker parserWorker = _serviceProvider.GetService<ParserWorker>();
            parserWorker.Parser.OnNewPage += OnNewPage;
            parserWorker.Parser.OnCompleted += OnCompleted;
            parserWorker.Start().Wait();

            Console.ReadLine();
        }

        private static void OnNewPage(object e, HtmlPageInfo page)
        {
            ClearConsoleLines(2);
            Console.WriteLine($"Parsed page count: {((Parser)e).Pages.Count}");
            Console.WriteLine($"Current page: {page.Url}");
            Console.SetCursorPosition(0, Console.CursorTop - 2);
        }

        private static void OnCompleted(object e)
        {
            ClearConsoleLines(2);
            Console.WriteLine($"Parsed page count: {((Parser)e).Pages.Count}");
            Console.WriteLine("Parse completed");
        }

        private static void RegisterServices(ParsingSettings settings)
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<Parser>(provider =>
            {
                switch (settings.ParserType)
                {
                    case "default": return new DefaultParser(settings);
                    case "content": return new ContentLengthConditionParser(settings);
                    case "images": return new ImageLinkParser(settings);
                    default: return new DefaultParser(settings);
                }
            });
            collection.AddSingleton<ParserWorker>();
            collection.AddScoped<IResultWriter, ResultFileWriter>();

            _serviceProvider = collection.BuildServiceProvider();
        }

        public static void ClearConsoleLines(int lineCountToClear)
        {
            int currentLineCursor = Console.CursorTop;
            for (int i = 0; i <= lineCountToClear; i++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
