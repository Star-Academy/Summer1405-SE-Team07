using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchHistoryApp
{
    public class SearchHistory
    {
        private readonly List<string> _history = new List<string>();
        private int _currentIndex = -1; 
        private readonly Dictionary<string, int> _counts = new Dictionary<string, int>();

        public void Search(string query)
        {
            
            if (_currentIndex < _history.Count - 1)
            {
                int removeStart = _currentIndex + 1;
                int removeCount = _history.Count - removeStart;
                _history.RemoveRange(removeStart, removeCount);
            }

            _history.Add(query);
            _currentIndex = _history.Count - 1;

            if (_counts.ContainsKey(query))
                _counts[query]++;
            else
                _counts[query] = 1;

            PrintCurrent();
        }

        public void Back()
        {
            if (_currentIndex <= 0)
            {
                Console.WriteLine("BACK is not possible.");
                return;
            }

            _currentIndex--;
            PrintCurrent();
        }

        public void Forward()
        {
            if (_currentIndex >= _history.Count - 1)
            {
                Console.WriteLine("FORWARD is not possible.");
                return;
            }

            _currentIndex++;
            PrintCurrent();
        }

        public void PrintCurrent()
        {
            if (_currentIndex < 0 || _history.Count == 0)
            {
                Console.WriteLine("current: (empty)");
                return;
            }

            Console.WriteLine($"current: {_history[_currentIndex]}");
        }

        public void PrintStats()
        {
            if (_counts.Count == 0)
            {
                Console.WriteLine("No searches have been recorded.");
                return;
            }

            var topThree = _counts
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key) // for stable ordering in case of equal counts
                .Take(3);

            foreach (var kv in topThree)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
        }

        public void PrintUnique()
        {
            Console.WriteLine(_counts.Count);
        }
    }
}