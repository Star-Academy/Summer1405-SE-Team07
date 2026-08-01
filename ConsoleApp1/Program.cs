using System;

namespace SearchHistoryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var history = new SearchHistory();

            while (true)
            {
                string? line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(' ', 2);
                string command = parts[0].Trim().ToUpperInvariant();
                string? argument = parts.Length > 1 ? parts[1].Trim() : null;

                switch (command)
                {
                    case "SEARCH":
                        if (string.IsNullOrWhiteSpace(argument))
                            Console.WriteLine("Please enter something after SEARCH command.");
                        else
                            history.Search(argument);
                        break;

                    case "BACK":
                        history.Back();
                        break;

                    case "FORWARD":
                        history.Forward();
                        break;

                    case "CURRENT":
                        history.PrintCurrent();
                        break;

                    case "STATS":
                        history.PrintStats();
                        break;

                    case "UNIQUE":
                        history.PrintUnique();
                        break;

                    case "EXIT":
                        return;

                    default:
                        Console.WriteLine($"Invalid command: {command}");
                        break;
                }
            }
        }
    }
}