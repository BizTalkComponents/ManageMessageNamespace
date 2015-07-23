using System.Collections.Generic;
using System.Linq;

namespace BizTalkComponents.Utils
{
    public class NavigationHistoryManager
    {
        private int _lastDepth;

        private readonly List<NavigationHistoryItem.NavgiationHistoryItem> _navigationHistoryItems;

        public NavigationHistoryManager()
        {
            _navigationHistoryItems = new List<NavigationHistoryItem.NavgiationHistoryItem>();
        }

        public NavigationPath Add(NavigationHistoryItem.NavgiationHistoryItem item)
        {
            if (item.Depth < _lastDepth)
                _navigationHistoryItems.RemoveAll(i => i.Depth >= item.Depth);

            _navigationHistoryItems.Add(item);

            _lastDepth = item.Depth;

            var path = ToPath(_navigationHistoryItems);

            return new NavigationPath(path, _navigationHistoryItems);
        }

        public string ToPath(List<NavigationHistoryItem.NavgiationHistoryItem> items)
        {
            var path = "";

            var lastItem = items.Last();

            foreach (var item in items)
            {
                path += item.Name();

                if (item != lastItem)
                    path += "/";
            }

            return path;
        }
    }
}