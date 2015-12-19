using System.IO;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Streaming;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace.Streams
{
    public class XmlNamespaceRemover : XmlTranslatorStream
    {

        public XmlNamespaceRemover(Stream input) : base(new XmlTextReader(input))
        {

        }

        public XmlNamespaceRemover(Stream input, Encoding encoding) : base(new XmlTextReader(input), encoding)
        {
            
        }

        protected override void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            base.TranslateStartElement(null, localName, null);
        }


        protected override void TranslateAttribute()
        {
            if (this.m_reader.Prefix != "xmlns")
                base.TranslateAttribute();
        }


        protected override void TranslateAttributeValue(string prefix, string localName, string nsURI, string val)
        {
            if (localName == "xmlns")
            {
                base.TranslateAttributeValue(prefix, localName, nsURI, null);
            }
            else
            {
                base.TranslateAttributeValue(prefix, localName, nsURI, val);
            }
        }


        protected override void TranslateXmlDeclaration(string target, string val)
        {
            base.TranslateXmlDeclaration(target, val);
            m_writer.WriteProcessingInstruction(target, val);
        }
    }
}