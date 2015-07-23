using System;

namespace BizTalkComponents.Utils
{
    public class ContextProperty
    {
        public ContextProperty(string propertyName, string propertyNamespace)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (string.IsNullOrEmpty(propertyNamespace))
            {
                throw new ArgumentNullException("propertyNamespace");
            }

            PropertyName = propertyName;
            PropertyNamespace = propertyNamespace;
        }

        public ContextProperty(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException("property");
            }

            if (!property.Contains("#"))
            {
                throw new ArgumentException("The property path {0} is not valid", property);
            }

            PropertyNamespace = property.Split('#')[0];
            PropertyName = property.Split('#')[1];
        }

        public string PropertyName { get; private set; }
        public string PropertyNamespace { get; private set; }

        public string ToPropertyString()
        {
            return string.Format("{0}#{1}", PropertyNamespace, PropertyName);
        }
    }
}