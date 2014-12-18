namespace BizTalkComponents.Utils
{
    public class FileProperties
    {
        public const string ReceivedFileName = "http://schemas.microsoft.com/BizTalk/2003/file-properties#ReceivedFileName";
    }

    public class SSOTicketProperties
    {
        public const string SSOTicket = "http://schemas.microsoft.com/BizTalk/2003/system-properties#SSOTicket";
    }

    public class SystemProperties
    {
        public const string MessageType = "http://schemas.microsoft.com/BizTalk/2003/system-properties#MessageType";
        public const string SchemaStrongName = "http://schemas.microsoft.com/BizTalk/2003/system-properties#SchemaStrongName";

        public const string RouteDirectToTP =
            "http://schemas.microsoft.com/BizTalk/2003/system-properties#RouteDirectToTP";

        public const string EpmRRCorrelationToken =
            "http://schemas.microsoft.com/BizTalk/2003/system-properties#EpmRRCorrelationToken";

        public const string CorrelationToken =
           "http://schemas.microsoft.com/BizTalk/2003/system-properties#CorrelationToken";

        public const string IsRequestResponse =
           "http://schemas.microsoft.com/BizTalk/2003/system-properties#IsRequestResponse";

        public const string ReqRespTransmitPipelineID =
           "http://schemas.microsoft.com/BizTalk/2003/system-properties#ReqRespTransmitPipelineID";
    }

    public class WCFProperties
    {
        public const string OutboundHttpStatusCode =
            "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties#OutboundHttpStatusCode";
    }
}