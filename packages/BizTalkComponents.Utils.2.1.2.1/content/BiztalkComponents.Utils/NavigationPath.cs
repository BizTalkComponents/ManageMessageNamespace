using System;
using System.Collections.Generic;

namespace BizTalkComponents.Utils
{
    public class NavigationPath
    {
        public string Path { get; private set; }
        private readonly List<NavigationHistoryItem.NavgiationHistoryItem> _navgiationHistoryItems;

        public NavigationPath(string path, List<NavigationHistoryItem.NavgiationHistoryItem> navgiationHistoryItems)
        {
            Path = path;
            _navgiationHistoryItems = navgiationHistoryItems;
        }

        public bool IsMatch(string path)
        {
            return Path.Equals(path, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsChildToMatching(string inPath)
        {
            var nodes = inPath.Split('/');

            if (nodes.Length > _navgiationHistoryItems.Count)
                return false;

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != _navgiationHistoryItems[i].Name())
                    return false;
            }

            return true;
        }
    }
}