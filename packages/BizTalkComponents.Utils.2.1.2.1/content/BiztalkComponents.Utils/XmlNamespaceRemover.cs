using System.IO;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Streaming;

namespace BizTalkComponents.Utils
{
    public class XmlNamespaceRemover : XmlTranslatorStream
    {

        public XmlNamespaceRemover(Stream input) : base(new XmlTextReader(input))
        {

        }

        protected override void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            base.TranslateStartElement(null, localName, null);
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
    }
}