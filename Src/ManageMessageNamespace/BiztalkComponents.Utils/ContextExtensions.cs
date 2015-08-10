using System;
using Microsoft.BizTalk.Message.Interop;

namespace BizTalkComponents.Utils
{
    public static class ContextExtensions
    {
        public static bool TryRead<T>(this IBaseMessageContext ctx, ContextProperty property, out T val)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            object content = ctx.Read(property.PropertyName, property.PropertyNamespace);
            if (content is T)
            {
                val = (T)content;
                return true;
            }
            else
            {
                val = default(T);
                return false;
            }
        }

        public static bool TryRead(this IBaseMessageContext ctx, ContextProperty property, out object val)
        {
            return TryRead<object>(ctx, property, out val);
        }

        public static bool TryRead(this IBaseMessageContext ctx, ContextProperty property, out string val)
        {
            TryRead<string>(ctx, property, out val);

            return !string.IsNullOrWhiteSpace(val);
        }

        public static void Promote(this IBaseMessageContext ctx, ContextProperty property, object val)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (val == null)
            {
                throw new ArgumentNullException("val");
            }

            ctx.Promote(property.PropertyName,property.PropertyNamespace,val);
        }

        public static void Write(this IBaseMessageContext ctx, ContextProperty property, object val)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (val == null)
            {
                throw new ArgumentNullException("val");
            }

            ctx.Write(property.PropertyName, property.PropertyNamespace,val);
        }

        public static void Copy(this IBaseMessageContext ctx, ContextProperty source, ContextProperty destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            object sourceValue;

            if (!ctx.TryRead(source, out sourceValue))
            {
                throw new InvalidOperationException("Could not find the specified source property in BizTalk context.");
            }

            ctx.Promote(destination, sourceValue);
        }

        public static bool IsPromoted(this IBaseMessageContext ctx, ContextProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            
            return ctx.IsPromoted(property.PropertyName, property.PropertyNamespace);
        }

        public static object Read(this IBaseMessageContext ctx, ContextProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            return ctx.Read(property.PropertyName, property.PropertyNamespace);
        }
    }
}
