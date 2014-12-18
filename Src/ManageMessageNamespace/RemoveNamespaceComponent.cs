using System;
using System.ComponentModel;
using System.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

// TODO: Should have better GUI name for exposed parameters

namespace BizTalkComponents.ManageMessageNamespace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("C318B1D9-6FA5-40AF-98BD-BAE397A7A6B1")]

    public class RemoveNamespaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        #region IBaseComponent members

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "Remove Namespace Component";
            }
        }

        [Browsable(false)]
        public string Version
        {
            get
            {
                return "2.2";
            }
        }

        [Browsable(false)]
        public string Description
        {
            get
            {
                return @"Removes namespace from a message.";
            }
        }

        #endregion

        #region IPersistPropertyBag members

        public void GetClassID(out Guid classid)
        {
            classid = new Guid("36637873-F2E5-4D3F-B0BA-4B24BFEDD73C");
        }

        public void InitNew() { }

        public virtual void Load(IPropertyBag pb, int errlog){}

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties){}

        #endregion

        #region IComponentUI members

        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public System.Collections.IEnumerator Validate(object obj)
        {
            return null;
        }

        #endregion

        #region IComponent members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            var contentReader = new ContentReader();

            const int bufferSize = 0x280;
            const int thresholdSize = 0x100000;
            Stream virtualStream = new VirtualStream(bufferSize, thresholdSize);
            Stream data = new ReadOnlySeekableStream(pInMsg.BodyPart.GetOriginalDataStream(), virtualStream, bufferSize);

            if (contentReader.IsXmlContent(data))
            {
                var encoding = contentReader.Encoding(data);
                pInMsg.BodyPart.Data = new ContentWriter().RemoveNamespace(data, encoding);
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
