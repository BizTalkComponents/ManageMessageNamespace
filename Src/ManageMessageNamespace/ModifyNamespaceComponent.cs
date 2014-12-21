using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("773A16F2-DA80-4562-BDF6-AC7C0CFF8C08")]

    public partial class ModifyNamespaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        private const string NamespaceToModifyPropertyName = "NamespaceToModify";
        private const string NewNamespacePropertyName = "NewNamespace";
        private const string ShouldUpdateMessagewTypeContextPropertyName = "ShouldUpdateMessageTypeContext";

        [RequiredRuntime]
        [DisplayName("Namespace to modify")]
        [Description("The namespace that should be changed.")]
        public string NamespaceToModify { get; set; }
        [RequiredRuntime]
        [DisplayName("New Namespace")]
        [Description("The new namespace..")]
        public string NewNamespace { get; set; }

        public bool ShouldUpdateMessageTypeContext { get; set; }

        public virtual void Load(IPropertyBag pb, int errlog)
        {
            NamespaceToModify =
                PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pb, NamespaceToModifyPropertyName),
                    string.Empty);

            NewNamespace = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pb, NewNamespacePropertyName),string.Empty);

            var shouldUpdateMessageTypeContext = PropertyBagHelper.ReadPropertyBag(pb, ShouldUpdateMessagewTypeContextPropertyName);
            
            if ((shouldUpdateMessageTypeContext != null))
            {
                ShouldUpdateMessageTypeContext = ((bool)(shouldUpdateMessageTypeContext));
            }
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(pb, NamespaceToModifyPropertyName, NamespaceToModify);
            PropertyBagHelper.WritePropertyBag(pb, NewNamespacePropertyName, NewNamespace);
            PropertyBagHelper.WritePropertyBag(pb, ShouldUpdateMessagewTypeContextPropertyName, ShouldUpdateMessageTypeContext);
        }

        #region IComponent members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var contentReader = new ContentReader();

            const int bufferSize = 0x280;
            const int thresholdSize = 0x100000;
            Stream virtualStream = new VirtualStream(bufferSize, thresholdSize);
            Stream data = new ReadOnlySeekableStream(pInMsg.BodyPart.GetOriginalDataStream(), virtualStream, bufferSize);

            if (contentReader.IsXmlContent(data) && contentReader.NamespacExists(data, NamespaceToModify))
            {
                var encoding = contentReader.Encoding(data);

                pInMsg.BodyPart.Data = new ContentWriter().ModifyNamespace(data, NamespaceToModify, NewNamespace, encoding);

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

        #endregion
    }
}
