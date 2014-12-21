using System;
using System.ComponentModel;
using System.IO;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

// TODO: Should have better GUI name for exposed parameters

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("950C8198-9AAD-467E-BA9C-16AA080C7D7C")]

    public partial class AddNamespaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        public string NewNamespace { get; set; }
        public bool ShouldUpdateMessageTypeContext { get; set; }
        public NamespaceFormEnum NamespaceForm { get; set; }
        public string XPath { get; set; }

        #region IPersistPropertyBag members
       
        public virtual void Load(IPropertyBag pb, int errlog)
        {
            NewNamespace = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pb, "NewNamespace"), string.Empty);

            var namespaceForm = PropertyBagHelper.ReadPropertyBag(pb, "NamespaceForm");

            if ((namespaceForm != null))
            {
                NamespaceForm = ((NamespaceFormEnum)(namespaceForm));
            }

            XPath = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pb, "XPath"), string.Empty);

            var shouldUpdateMessageTypeContext = PropertyBagHelper.ReadPropertyBag(pb, "ShouldUpdateMessageTypeContext");

            if ((shouldUpdateMessageTypeContext != null))
            {
                ShouldUpdateMessageTypeContext = ((bool)(shouldUpdateMessageTypeContext));
            }
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(pb, "NewNamespace", NewNamespace);
            PropertyBagHelper.WritePropertyBag(pb, "NamespaceForm", NamespaceForm);
            PropertyBagHelper.WritePropertyBag(pb, "XPath", XPath);
            PropertyBagHelper.WritePropertyBag(pb, "ShouldUpdateMessageTypeContext", ShouldUpdateMessageTypeContext);
        }

        #endregion

        #region Utility functionality

        private static void WritePropertyBag(IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private static object ReadPropertyBag(IPropertyBag pb, string propName)
        {
            object val = null;
            try
            {
                pb.Read(propName, out val, 0);
            }

            catch (ArgumentException)
            {
                return val;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return val;
        }

        #endregion

        #region IComponent members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            var contentReader = new ContentReader();

            //Stream virtualStream = new VirtualStream();
            //Stream data = new ReadOnlySeekableStream(pInMsg.BodyPart.GetOriginalDataStream(), virtualStream);
            var data = pInMsg.BodyPart.GetOriginalDataStream();
            if (contentReader.IsXmlContent(data))
            {
                var encoding = contentReader.Encoding(data);
                pInMsg.BodyPart.Data = new ContentWriter().AddNamespace(data, NewNamespace, NamespaceForm, XPath, encoding);

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
