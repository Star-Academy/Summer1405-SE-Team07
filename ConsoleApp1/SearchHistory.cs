using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchHistoryApp
{
    public class SearchHistory
    {
        private readonly List<string> _search_history_list = new List<string>();
        private int _currentIndex = -1;
        private readonly Dictionary<string, int> _search_counts = new Dictionary<string, int>();

        public void Search(string query)
        {

            if (_currentIndex < _search_history_list.Count - 1)
            {
                var removeStart = _currentIndex + 1;
                var removeCount = _search_history_list.Count - removeStart;
             _search_history_list.RemoveRange(removeStart, removeCount);
            }

         _search_history_list.Add(query);
            _currentIndex = _search_history_list.Count - 1;

            if (_search_counts.ContainsKey(query))
                _search_counts[query]++;
            else
                _search_counts[query] = 1;

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
            if (_currentIndex >= _search_history_list.Count - 1)
            {
                Console.WriteLine("FORWARD is not possible.");
                return;
            }

            _currentIndex++;
            PrintCurrent();
        }

        public void PrintCurrent()
        {
            if (_currentIndex < 0 || _search_history_list.Count == 0)
            {
                Console.WriteLine("current: (empty)");
                return;
            }

            Console.WriteLine($"current:  _search_history_list[_currentIndex]}");
        }

        public void PrintStats()
        {
            if (_search_counts.Count == 0)
            {
                Console.WriteLine("No searches have been recorded.");
                return;
            }

            var topThree = _search_counts
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Take(3);

            foreach (var kv in topThree)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
        }

        public void PrintUnique()
        {
            Console.WriteLine(_search_counts.Count);
        }
    }
}