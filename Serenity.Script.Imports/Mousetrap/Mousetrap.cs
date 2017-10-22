using System.Runtime.CompilerServices;

namespace System.Html
{
    [IgnoreNamespace, Imported]
    public static class Mousetrap
    {
        [InlineCode("Mousetrap({el})")]
        public static MousetrapInstance Wrap(Element el)
        {
            return null;
        }

        public static bool StopCallback(KeyboardEvent e, Element element, string combo)
        {
            return false;
        }

        public static void Bind(string keys, Func<KeyboardEvent, string, bool> callback)
        {
        }

        public static void Bind(string keys, Func<KeyboardEvent, string, bool> callback, string action)
        {
        }

        public static void Bind(string[] keyArray, Func<KeyboardEvent, string, bool> callback)
        {
        }


        public static void Bind(string[] keyArray, Func<KeyboardEvent, string, bool> callback, string action)
        {
        }

        public static void Unbind(string keys)
        {
        }

        public static void Unbind(string keys, string action)
        {
        }

        public static void Unbind(string[] keyArray)
        {
        }

        public static void Unbind(string[] keyArray, string action)
        {
        }

        public static void Trigger(string keys)
        {
        }

        public static void Trigger(string keys, string action)
        {
        }

        public static void Reset()
        {
        }
    }

    [IgnoreNamespace, Imported]
    public class MousetrapInstance
    {
        public MousetrapInstance(Element el)
        {
        }

        public void Bind(string keys, Func<KeyboardEvent, string, bool> callback)
        {
        }

        public void Bind(string keys, Func<KeyboardEvent, string, bool> callback, string action)
        {
        }

        public void Bind(string[] keyArray, Func<KeyboardEvent, string, bool> callback)
        {
        }

        public void Bind(string[] keyArray, Func<KeyboardEvent, string, bool> callback, string action)
        {
        }

        public void Unbind(string keys)
        {
        }

        public void Unbind(string keys, string action)
        {
        }

        public void Unbind(string[] keyArray)
        {
        }

        public void Unbind(string[] keyArray, string action)
        {
        }

        public void Trigger(string keys, string action)
        {
        }

        public void Reset()
        {
        }
    }
}