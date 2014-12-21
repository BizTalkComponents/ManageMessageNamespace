using System;
using System.ComponentModel;
using System.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("C318B1D9-6FA5-40AF-98BD-BAE397A7A6B1")]

    public partial class RemoveNamespaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI
    {
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
    }
}
