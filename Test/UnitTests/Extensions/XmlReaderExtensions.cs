using System.Xml;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace.Tests.Extensions
{
    public static class XmlReaderExtensions
    {
        public static XmlReader MoveToNextElement(this XmlReader reader)
        {
            do { reader.Read(); }
            while (reader.NodeType != XmlNodeType.Element);

            return reader;
        }
    }
}
