using System;
using Microsoft.BizTalk.Message.Interop;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    class ContextReader
    {
        const string MessageTypeContextName = "MessageType";
        const string MessageTypeContextNs = "http://schemas.microsoft.com/BizTalk/2003/system-properties";

        public void UpdateMessageTypeContext(IBaseMessageContext context, string newNamespace, string name)
        {
            context.Promote(MessageTypeContextName, MessageTypeContextNs, string.Concat(newNamespace, "#", name));
        }
    }
}
