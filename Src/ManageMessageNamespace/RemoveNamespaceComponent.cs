using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System.IO;
using System.Runtime.InteropServices;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [Guid("C318B1D9-6FA5-40AF-98BD-BAE397A7A6B1")]
    public partial class RemoveNamespaceComponent : IBaseComponent,
        IComponent,
        IComponentUI
    {
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            var contentReader = new ContentReader();

            var data = pInMsg.BodyPart.GetOriginalDataStream();

            if (!data.CanSeek || !data.CanRead)
            {
                const int bufferSize = 0x280;
                const int thresholdSize = 0x100000;
                data = new ReadOnlySeekableStream(data, new VirtualStream(bufferSize, thresholdSize), bufferSize);
                pContext.ResourceTracker.AddResource(data);
            }

            if (contentReader.IsXmlContent(data))
            {
                var encoding = contentReader.Encoding(data);
                data = new ContentWriter().RemoveNamespace(data, encoding);
                pContext.ResourceTracker.AddResource(data);
                pInMsg.BodyPart.Data = data;
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