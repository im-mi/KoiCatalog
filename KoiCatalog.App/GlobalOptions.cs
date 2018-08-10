using System;
using System.Collections.Generic;
using System.Linq;

namespace KoiCatalog.App
{
    public static class GlobalOptions
    {
        static GlobalOptions()
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            foreach (var option in SplitOptions(args))
            {
                switch (option.Name)
                {
                    case null:
                    case "Directory":
                        StartupDirectories.AddRange(option.Arguments);
                        break;
                    case "NoReport":
                        ErrorReportingEnabled = false;
                        break;
                    default:
                        Console.WriteLine($"Unknown command-line option '{option.Name}'");
                        break;
                }
            }
        }

        private static List<Option> SplitOptions(IEnumerable<string> args)
        {
            var options = new List<Option>();

            Option currentOption = null;

            foreach (var arg in args)
            {
                if (arg.StartsWith("/"))
                {
                    currentOption = new Option
                    {
                        Name = arg.Substring(1)
                    };
                    options.Add(currentOption);
                }
                else
                {
                    if (currentOption == null)
                    {
                        currentOption = new Option();
                        options.Add(currentOption);
                    }
                    currentOption.Arguments.Add(arg);
                }
            }

            return options;
        }

        private class Option
        {
            public string Name { get; set; }
            public List<string> Arguments { get; } = new List<string>();
        }

        public static bool ErrorReportingEnabled { get; } = true;
        public static List<string> StartupDirectories { get; } = new List<string>();
    }
}
