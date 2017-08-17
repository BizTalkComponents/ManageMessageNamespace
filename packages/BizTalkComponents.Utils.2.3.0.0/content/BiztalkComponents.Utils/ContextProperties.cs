using Microsoft.XLANGs.BaseTypes;

namespace BizTalkComponents.Utils
{
    public class FileProperties
    {
        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/file-properties#ReceivedFileName
        /// </summary>
        public static readonly string ReceivedFileName = SystemProperties.GetFullyQualifiedName(new FILE.ReceivedFileName());
    }

    public class SSOTicketProperties
    {
        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#SSOTicket
        /// </summary>
        public static readonly string SSOTicket = SystemProperties.GetFullyQualifiedName(new BTS.SSOTicket());
    }

    public class SystemProperties
    {
        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#MessageType
        /// </summary>
        public static readonly string MessageType = GetFullyQualifiedName(new BTS.MessageType());

        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#SchemaStrongName
        /// </summary>
        public static readonly string SchemaStrongName = GetFullyQualifiedName(new BTS.SchemaStrongName());

        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#RouteDirectToTP
        /// </summary>
        public static readonly string RouteDirectToTP = GetFullyQualifiedName(new BTS.RouteDirectToTP());

        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#EpmRRCorrelationToken
        /// </summary>
        public static readonly string EpmRRCorrelationToken = GetFullyQualifiedName(new BTS.EpmRRCorrelationToken());

        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#CorrelationToken
        /// </summary>
        public static readonly string CorrelationToken = GetFullyQualifiedName(new BTS.CorrelationToken());

        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#IsRequestResponse
        /// </summary>
        public static readonly string IsRequestResponse = GetFullyQualifiedName(new BTS.IsRequestResponse());

        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2003/system-properties#ReqRespTransmitPipelineID
        /// </summary>
        public static readonly string ReqRespTransmitPipelineID = GetFullyQualifiedName(new BTS.ReqRespTransmitPipelineID());

        public static string GetFullyQualifiedName(PropertyBase propertyBase)
        {
            var qName = propertyBase.QName;
            return qName.Namespace + "#" + qName.Name;
        }
    }

    public class WCFProperties
    {
        /// <summary>
        /// http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties#OutboundHttpStatusCode
        /// </summary>
        public static readonly string OutboundHttpStatusCode = SystemProperties.GetFullyQualifiedName(new WCF.OutboundHttpStatusCode());
    }
}