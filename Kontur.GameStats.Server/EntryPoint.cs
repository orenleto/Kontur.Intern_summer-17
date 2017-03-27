using System;
using Fclp;
using Microsoft.Owin.Hosting;
using NLog;

namespace Kontur.GameStats.Server
{
    public class EntryPoint
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledExceptionEventHandler;

            var commandLineParser = new FluentCommandLineParser<Options>();

            commandLineParser
                .Setup(options => options.Prefix)
                .As("prefix")
                .SetDefault("http://+:8080/")
                .WithDescription("HTTP prefix to listen on");

            commandLineParser
                .SetupHelp("h", "help")
                .WithHeader(string.Format("{0} [--prefix <prefix>]", AppDomain.CurrentDomain.FriendlyName))
                .Callback(text => Console.WriteLine(text));

            if (commandLineParser.Parse(args).HelpCalled)
                return;

            RunServer(commandLineParser.Object);
        }

        private static void RunServer(Options options)
        {
            WebApp.Start<Startup>(options.Prefix);
            Console.ReadKey(true);
        }

        private static void CurrentDomain_UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            logger.Fatal(exception.Message);
            logger.Fatal(exception.StackTrace);
        }

        private class Options
        {
            public string Prefix { get; set; }
        }
    }
}
