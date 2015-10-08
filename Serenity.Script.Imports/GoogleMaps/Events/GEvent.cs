using System;
using System.Runtime.CompilerServices;
using GMapsSharp.Lib;

namespace google.maps
{
    [ScriptName("event")]
    [Imported]
    public static class GEvent
    {
        /// <summary>
        /// Adds the given listener function to the given event name for the given object instance. Returns an identifier for this listener that can be used with removeListener().
        /// </summary>
        public static MapsEventListener AddListener(object instance, string eventName, MapsEventHandler handler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Like addListener, but the handler removes itself after handling the first event.
        /// </summary>
        public static MapsEventListener AddListenerOnce(object instance, string eventName, MapsEventHandler handler)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Removes the given listener, which should have been returned by addListener above.
        /// </summary>
        public static void RemoveListener(MapsEventListener listener)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Removes all listeners for the given event for the given instance.
        /// </summary>
        public static void ClearListeners(object instance, string eventName)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Removes all listeners for all events for the given instance.
        /// </summary>
        public static void ClearInstanceListeners(object instance)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Cross browser event handler registration. This listener is removed by calling removeListener(handle) for the handle that is returned by this function.
        /// </summary>
        public static MapsEventListener AddDomListener(object instance, string eventName, MapsEventHandler handler)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Cross browser event handler registration. This listener is removed by calling removeListener(handle) for the handle that is returned by this function.
        /// </summary>
        public static MapsEventListener AddDomListener(object instance, string eventName, MapsEventHandler handler,
                                                       bool capture)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Wrapper around addDomListener that removes the listener after the first event.
        /// </summary>
        public static MapsEventListener AddDomListenerOnce(object instance, string eventName, MapsEventHandler handler)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Wrapper around addDomListener that removes the listener after the first event.
        /// </summary>
        public static MapsEventListener AddDomListenerOnce(object instance, string eventName, MapsEventHandler handler,
                                                           bool capture)
        {
            throw new System.Exception("Not Implemented - Imported");
        }

        /// <summary>
        /// Triggers the given event. All arguments after eventName are passed as arguments to the listeners.
        /// </summary>
        public static void Trigger(object instance, string eventName, params object[] args)
        {
            throw new System.Exception("Not Implemented - Imported");
        }
    }
}