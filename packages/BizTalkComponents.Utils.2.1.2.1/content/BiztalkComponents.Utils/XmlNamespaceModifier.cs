using System.IO;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Streaming;

namespace BizTalkComponents.Utils
{
    public class XmlNamespaceModifier : XmlTranslatorStream
    {
        private readonly string _newNamespace;
        private readonly string _newNamespacePrefix;
        private readonly string _oldNamespace;

        public XmlNamespaceModifier(Stream input, string newNamespace, string newNamespacePrefix, string oldNamespace) : base(new XmlTextReader(input))
        {
            _newNamespace = newNamespace;
            _newNamespacePrefix = newNamespacePrefix;
            _oldNamespace = oldNamespace;
        }

        public XmlNamespaceModifier(Stream input, Encoding encoding, string newNamespace, string newNamespacePrefix, string oldNamespace)
            : base(new XmlTextReader(input), encoding)
        {
            _newNamespace = newNamespace;
            _newNamespacePrefix = newNamespacePrefix;
            _oldNamespace = oldNamespace;
        }

        protected override void TranslateXmlDeclaration(string target, string val)
        {
            base.TranslateXmlDeclaration(target, val);
            m_writer.WriteProcessingInstruction(target, val);
        }

        protected override void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            if (nsURI == _oldNamespace)
            {
                base.TranslateStartElement(prefix, localName, _newNamespace);
            }
            else
            {
                base.TranslateStartElement(prefix, localName, nsURI);
            }
        }

        protected override void TranslateAttributeValue(string prefix, string localName, string nsURI, string val)
        {
            if (localName == "xmlns" && val == _oldNamespace)
            {
                base.TranslateAttributeValue(prefix, localName, nsURI, _newNamespace);
            }
            else if (val == _oldNamespace)
            {
                base.TranslateAttributeValue(prefix, _newNamespace, nsURI, _newNamespace);
            }
            else
            {
                base.TranslateAttributeValue(prefix, localName, nsURI, val);
            }
        }
    }
}