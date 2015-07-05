using Microsoft.BizTalk.Streaming;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace
{
    internal class ContentWriter
    {
        private static NavigationHistoryManager _navigationHistoryManager;

        public ContentWriter()
        {
            _navigationHistoryManager = new NavigationHistoryManager();
        }

        public Stream ModifyNamespace(Stream stream, string namespaceToModify, string newNamespace, Encoding encoding)
        {
            return MessageWriter(WriteModifyNamespaceElements, stream, encoding, namespaceToModify, newNamespace);
        }

        public Stream AddNamespace(Stream stream, string newNamespace, NamespaceFormEnum namespaceForm, string xPath,
            Encoding encoding)
        {
            return MessageWriter(WriteAddNamespaceElements, stream, encoding, newNamespace: newNamespace,
                namespaceForm: namespaceForm, xPath: xPath);
        }

        public Stream RemoveNamespace(Stream stream, Encoding encoding)
        {
            return MessageWriter(WriteRemoveNamespaceElements, stream, encoding);
        }

        private static Stream MessageWriter(
            Action<XmlWriter, XmlReader, string, string, NamespaceFormEnum, string> writeElement, Stream stream,
            Encoding encoding, string nsToModify = null, string newNamespace = null,
            NamespaceFormEnum namespaceForm = NamespaceFormEnum.Unqualified, string xPath = "")
        {
            var outStream = new VirtualStream();

            using (var reader = XmlReader.Create(stream))
            {
                using (var writer = XmlWriter.Create(outStream, new XmlWriterSettings {Encoding = encoding}))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.CDATA:
                                writer.WriteCData(reader.Value);
                                break;

                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;

                            case XmlNodeType.DocumentType:
                                writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"),
                                    reader.GetAttribute("SYSTEM"), reader.Value);
                                break;

                            case XmlNodeType.Element:
                                var isEmpty = reader.IsEmptyElement;

                                // Will call the injected action depending if it's add, modify or remove
                                writeElement(writer, reader, nsToModify, newNamespace, namespaceForm, xPath);

                                while (reader.MoveToNextAttribute())
                                {
                                    // Copy all attributed that aren't namespaces
                                    if (reader.Value != nsToModify &&
                                        reader.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI,
                                            reader.Value);
                                }

                                if (isEmpty)
                                    writer.WriteEndElement();

                                break;

                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;

                            case XmlNodeType.EntityReference:
                                writer.WriteEntityRef(reader.Name);
                                break;

                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;

                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;

                            case XmlNodeType.SignificantWhitespace:
                            case XmlNodeType.Whitespace:
                                writer.WriteWhitespace(reader.Value);
                                break;
                        }
                    }
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }

        private void WriteRemoveNamespaceElements(XmlWriter writer, XmlReader reader, string namespaceToModify,
            string namespaceToAdd, NamespaceFormEnum namespaceForm, string xPath)
        {
            writer.WriteStartElement(reader.LocalName);
        }

        // TODO: possible imporvment is to have parameter for naming of namespace
        // Possible issue: What happens if there already is a ns0, is it ok to redefine?
        private void WriteAddNamespaceElements(XmlWriter writer, XmlReader reader, string namespaceToModify,
            string namespaceToAdd, NamespaceFormEnum namespaceForm, string xPath)
        {
            const string prefix = "ns0";
            var path =
                _navigationHistoryManager.Add(new NavgiationHistoryItem(reader.Depth, reader.LocalName,
                    reader.NamespaceURI));

            if (string.IsNullOrEmpty(xPath) && reader.NamespaceURI != string.Empty && reader.Depth == 0)
                throw new InvalidOperationException(
                    string.Format("Can not add an new root namespace as a namespace already exists ({0})",
                        reader.NamespaceURI));

            if ((!string.IsNullOrEmpty(xPath)) && path.IsMatch(xPath) && reader.NamespaceURI != string.Empty)
                throw new InvalidOperationException(
                    string.Format("Can not add an new namespace to node '{0}' as a namespace already exists ({1})",
                        reader.NamespaceURI, reader.Name));

            // Add namespace depending on the form of namespace
            if ((!string.IsNullOrEmpty(xPath) && path.IsMatch(xPath) ||
                 (string.IsNullOrEmpty(xPath) && reader.Depth == 0)) && namespaceForm == NamespaceFormEnum.Default)
            {
                writer.WriteStartElement(reader.LocalName, namespaceToAdd);
            }
            else if ((!string.IsNullOrEmpty(xPath) && path.IsMatch(xPath) ||
                      (string.IsNullOrEmpty(xPath) && reader.Depth == 0)) &&
                     namespaceForm == NamespaceFormEnum.Unqualified)
            {
                writer.WriteStartElement(prefix, reader.LocalName, namespaceToAdd);
            }
            else if ((!string.IsNullOrEmpty(xPath) && (path.IsMatch(xPath) || path.IsChildToMatching(xPath)) ||
                      string.IsNullOrEmpty(xPath)) && namespaceForm == NamespaceFormEnum.Qualified)
            {
                writer.WriteStartElement(prefix, reader.LocalName, namespaceToAdd);
            }
            else
            {
                var ns = "";

                if (!string.IsNullOrEmpty(reader.NamespaceURI))
                    ns = reader.NamespaceURI;

                if (reader.Prefix == string.Empty)
                    writer.WriteStartElement(reader.LocalName, ns);
                else
                {
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, ns);
                }
            }
        }

        private void WriteModifyNamespaceElements(XmlWriter writer, XmlReader reader, string namespaceToModify,
            string namespaceToAdd, NamespaceFormEnum namespaceForm, string xPath)
        {
            var ns = "";
            if (reader.NamespaceURI == namespaceToModify)
                ns = namespaceToAdd;

            if (reader.Prefix == string.Empty)
                writer.WriteStartElement(reader.LocalName, ns);
            else
            {
                writer.WriteStartElement(reader.Prefix, reader.LocalName, ns);
            }
        }
    }
}