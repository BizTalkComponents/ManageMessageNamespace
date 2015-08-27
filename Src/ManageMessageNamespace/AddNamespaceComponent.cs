using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.ComponentModel;
using System.IO;
using BizTalkComponents.PipelineComponents.ManageMessageNamespace.Streams;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("950C8198-9AAD-467E-BA9C-16AA080C7D7C")]
    public partial class AddNamespaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        private const string XPathPropertyName = "XPath";
        private const string NewNamespacePropertyName = "NewNamespace";
        private const string NamespaceFormPropertyName = "NamespaceFormPropertyName";
        private const string ShouldUpdateMessageTypeContextPropertyName = "ShouldUpdateMessageTypeContext";

        [RequiredRuntime]
        [DisplayName("New Namespace")]
        [Description("The new namespace to set.")]
        public string NewNamespace { get; set; }

        [RequiredRuntime]
        [DisplayName("Should update messagetype context")]
        [Description("Specifies wether the message type should be updated with the new namespace.")]
        public bool ShouldUpdateMessageTypeContext { get; set; }

        [RequiredRuntime]
        [DisplayName("Namespace form")]
        [Description("0 = Unqualified, 1 = Qualified, 2 = Default")]
        public NamespaceFormEnum NamespaceForm { get; set; }

        [DisplayName("XPath")]
        [Description("The path to set namespace on. Optional.")]
        public string XPath { get; set; }

        #region IPersistPropertyBag members

        public virtual void Load(IPropertyBag pb, int errlog)
        {
            NewNamespace = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pb, NewNamespacePropertyName), string.Empty);

            var namespaceForm = PropertyBagHelper.ReadPropertyBag(pb, NamespaceFormPropertyName);

            if ((namespaceForm != null))
            {
                NamespaceForm = ((NamespaceFormEnum)(namespaceForm));
            }

            XPath = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pb, XPathPropertyName), string.Empty);

            var shouldUpdateMessageTypeContext = PropertyBagHelper.ReadPropertyBag(pb, ShouldUpdateMessageTypeContextPropertyName);

            if ((shouldUpdateMessageTypeContext != null))
            {
                ShouldUpdateMessageTypeContext = ((bool)(shouldUpdateMessageTypeContext));
            }
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(pb, NewNamespacePropertyName, NewNamespace);
            PropertyBagHelper.WritePropertyBag(pb, NamespaceFormPropertyName, NamespaceForm);
            PropertyBagHelper.WritePropertyBag(pb, XPathPropertyName, XPath);
            PropertyBagHelper.WritePropertyBag(pb, ShouldUpdateMessageTypeContextPropertyName, ShouldUpdateMessageTypeContext);
        }

        #endregion IPersistPropertyBag members

        #region IComponent members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var contentReader = new ContentReader();

            var data = pInMsg.BodyPart.GetOriginalDataStream();
            const int bufferSize = 0x280;
            const int thresholdSize = 0x100000;

            if (!data.CanSeek || !data.CanRead)
            {
               
                data = new ReadOnlySeekableStream(data, new VirtualStream(bufferSize, thresholdSize), bufferSize);
                pContext.ResourceTracker.AddResource(data);
            }

            if (contentReader.IsXmlContent(data))
            {
                var encoding = contentReader.Encoding(data);
                data = new XmlNamespaceAdder(data, XPath, NamespaceForm, NewNamespace, encoding);
                data = new ReadOnlySeekableStream(data, new VirtualStream(bufferSize, thresholdSize), bufferSize);
                pContext.ResourceTracker.AddResource(data);
                pInMsg.BodyPart.Data = data;

                if (ShouldUpdateMessageTypeContext)
                {
                    var rootName = contentReader.GetRootNode(data);

                    var contextReader = new ContextReader();
                    contextReader.UpdateMessageTypeContext(pInMsg.Context, NewNamespace, rootName);
                }
            }
            else
            {
                data.Seek(0, SeekOrigin.Begin);
                pInMsg.BodyPart.Data = data;
            }

            return pInMsg;
        }

        #endregion IComponent members
    }
}
