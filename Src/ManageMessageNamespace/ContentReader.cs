using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    class ContentReader
    {
        public Encoding Encoding(Stream stream)
        {
            var reader = new XmlTextReader(stream);
            
            reader.MoveToContent();

            var encoding = reader.Encoding;

            stream.Seek(0, SeekOrigin.Begin);

            // Handle situation where we've removed the BOM
            if (encoding is UTF8Encoding && !HasUtf8Bom(stream))
                encoding = new UTF8Encoding(false);

            return encoding;
        }

        public bool IsXmlContent(Stream stream)
        {
            var buffer = new byte[1024];

            stream.Read(buffer, 0, 1024);

            stream.Seek(0, SeekOrigin.Begin);

            // Have to assume UTF8 as we can know encoding of text files and binary content
            return System.Text.Encoding.UTF8.GetString(buffer).StartsWith("<");
        }

        public bool NamespacExists(Stream stream, string nsToModify)
        {
            var reader = XmlReader.Create(stream);

            reader.MoveToContent();

            var ns = reader.NamespaceURI;

            stream.Seek(0, SeekOrigin.Begin);

            return ns == nsToModify;
        }

        public string GetRootNode(Stream stream)
        {
            var reader = XmlReader.Create(stream);

            reader.MoveToContent();

            var name = reader.LocalName;

            stream.Seek(0, SeekOrigin.Begin);

            return name;
        }

        private static bool HasUtf8Bom(Stream stream)
        {
            var buffer = new byte[3];

            stream.Read(buffer, 0, 3);

            stream.Seek(0, SeekOrigin.Begin);

            return System.Text.Encoding.UTF8.GetPreamble().SequenceEqual(buffer);
        }
    }

}
