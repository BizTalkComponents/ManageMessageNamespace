namespace BizTalkComponents.ManageMessageNamespace
{
    public class NavgiationHistoryItem
    {
        public int Branch { get; set; }
        public int Depth { get; set; }
        public string Localname { get; set; }
        public string NamespaceName { get; set; }

        public NavgiationHistoryItem(int depth, string localname, string namespaceName)
        {
            Depth = depth;
            Localname = localname;
            NamespaceName = namespaceName;
        }

        public string Name()
        {
            return !string.IsNullOrEmpty(NamespaceName) ? string.Concat(NamespaceName, ":", Localname) : Localname;
        }
    }
}
