using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Streaming;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace.Streams
{
    public class XmlNamespaceAdder : XmlTranslatorStream
    {
        private readonly string _xPath;
        private readonly NamespaceFormEnum _namespaceForm;
        private readonly string _namespaceToAdd;
        private static NavigationHistoryManager _navigationHistoryManager;

        public XmlNamespaceAdder(Stream input, string xPath, NamespaceFormEnum namespaceForm, string namespaceToAdd, Encoding encoding) : base(new XmlTextReader(input), encoding)
        {
            _xPath = xPath;
            _namespaceForm = namespaceForm;
            _namespaceToAdd = namespaceToAdd;
            _navigationHistoryManager = new NavigationHistoryManager();
        }

        public XmlNamespaceAdder(Stream input, Encoding encoding) : base(new XmlTextReader(input), encoding)
        {
            _navigationHistoryManager = new NavigationHistoryManager();
        }

        protected override void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            const string nsPrefix = "ns0";
            var path =
                _navigationHistoryManager.Add(new NavigationHistoryItem(m_reader.Depth, m_reader.LocalName,
                    m_reader.NamespaceURI));

            if (string.IsNullOrEmpty(_xPath) && m_reader.NamespaceURI != string.Empty && m_reader.Depth == 0)
                throw new InvalidOperationException(
                    string.Format("Can not add an new root namespace as a namespace already exists ({0})",
                        m_reader.NamespaceURI));

            if ((!string.IsNullOrEmpty(_xPath)) && path.IsMatch(_xPath) && m_reader.NamespaceURI != string.Empty)
                throw new InvalidOperationException(
                    string.Format("Can not add an new namespace to node '{0}' as a namespace already exists ({1})",
                        m_reader.NamespaceURI, m_reader.Name));

            // Add namespace depending on the form of namespace
            if ((!string.IsNullOrEmpty(_xPath) && path.IsMatch(_xPath) ||
                 (string.IsNullOrEmpty(_xPath) && m_reader.Depth == 0)) && _namespaceForm == NamespaceFormEnum.Default)
            {
                base.TranslateStartElement(null,m_reader.LocalName, _namespaceToAdd);
            }
            else if ((!string.IsNullOrEmpty(_xPath) && path.IsMatch(_xPath) ||
                      (string.IsNullOrEmpty(_xPath) && m_reader.Depth == 0)) &&
                     _namespaceForm == NamespaceFormEnum.Unqualified)
            {
                base.TranslateStartElement(nsPrefix, m_reader.LocalName, _namespaceToAdd);
            }
            else if ((!string.IsNullOrEmpty(_xPath) && (path.IsMatch(_xPath) || path.IsChildToMatching(_xPath)) ||
                      string.IsNullOrEmpty(_xPath)) && _namespaceForm == NamespaceFormEnum.Qualified)
            {
                base.TranslateStartElement(nsPrefix, m_reader.LocalName, _namespaceToAdd);
            }
            else
            {
                var ns = "";

                if (!string.IsNullOrEmpty(m_reader.NamespaceURI))
                    ns = m_reader.NamespaceURI;

                if (m_reader.Prefix == string.Empty)
                    base.TranslateStartElement(null, m_reader.LocalName, ns);
                else
                {
                    base.TranslateStartElement(m_reader.Prefix, m_reader.LocalName, ns);
                }
            }
        }

        protected override void TranslateXmlDeclaration(string target, string val)
        {
            base.TranslateXmlDeclaration(target, val);
            m_writer.WriteProcessingInstruction(target, val);
        }
    }
}