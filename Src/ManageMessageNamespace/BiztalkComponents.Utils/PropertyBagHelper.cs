using System;
using Microsoft.BizTalk.Component.Interop;

namespace BizTalkComponents.Utils
{
    public class PropertyBagHelper
    {
        public static string ToStringOrDefault(object property, string defaultValue)
        {
            if (property != null)
            {
                return property.ToString();
            }

            return defaultValue;
        }

        /// <summary>
        /// Reads property value from property bag
        /// </summary>
        /// <param name="pb">Property bag</param>
        /// <param name="propName">Name of property</param>
        /// <returns>Value of the property</returns>
        public static object ReadPropertyBag(IPropertyBag pb, string propName)
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
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return val;
        }

        /// <summary>
        /// Writes property values into a property bag.
        /// </summary>
        /// <param name="pb">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <param name="val">Value of property.</param>
        public static void WritePropertyBag(IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
    }
}