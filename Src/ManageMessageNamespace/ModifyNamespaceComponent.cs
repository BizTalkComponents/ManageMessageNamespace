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
    [System.Runtime.InteropServices.Guid("773A16F2-DA80-4562-BDF6-AC7C0CFF8C08")]

    public class ModifyNamespaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        public string NamespaceToModify { get; set; }
        public string NewNamespace { get; set; }

        public bool ShouldUpdateMessageTypeContext { get; set; }

        #region IBaseComponent members

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "Modify Namespace Component";
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
                return @"Modifies a message namespace from one namespace to another.";
            }
        }

        #endregion

        #region IPersistPropertyBag members

        public void GetClassID(out Guid classid)
        {
            classid = new Guid("D4123F35-ED40-46BA-85AF-01B8D5CBBFED");
        }

        public void InitNew() { }

        public virtual void Load(IPropertyBag pb, int errlog)
        {
            var val = ReadPropertyBag(pb, "NamespaceToRemove");

            if ((val != null))
            {
                NamespaceToModify = ((string)(val));
            }

            val = ReadPropertyBag(pb, "NewNamespace");
            if ((val != null))
            {
                NewNamespace = ((string)(val));
            }

            val = ReadPropertyBag(pb, "ShouldUpdateMessageTypeContext");
            if ((val != null))
            {
                ShouldUpdateMessageTypeContext = ((bool)(val));
            }
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            WritePropertyBag(pb, "NamespaceToRemove", NamespaceToModify);
            WritePropertyBag(pb, "NewNamespace", NewNamespace);
            WritePropertyBag(pb, "ShouldUpdateMessageTypeContext", ShouldUpdateMessageTypeContext);
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
